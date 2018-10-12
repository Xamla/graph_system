using System;
using System.Collections.Generic;
using System.Text;
using Uml.Robotics.Ros;
using trajectory_msgs = Messages.trajectory_msgs;
using std_srvs = Messages.std_srvs;
using xamlamoveit = Messages.xamlamoveit_msgs;
using System.Reactive.Subjects;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class JoggingClient
        : IJoggingClient
        , IDisposable
    {
        NodeHandle nodeHandle;

        const string TOPIC_JOGGING_COMMAND = "xamlaJointJogging/jogging_command";
        const string TOPIC_SETPOINT_JOGGING_COMMAND = "xamlaJointJogging/jogging_setpoint";
        const string TOPIC_TWIST_JOGGING_COMMAND = "xamlaJointJogging/jogging_twist";
        const string TOPIC_JOGGING_FEEDBACK = "/xamlaJointJogging/feedback";
        const string SERVICE_TOGGLE_TRACKING = "xamlaJointJogging/start_stop_tracking";
        const string SERVICE_GET_MOVEGROUP_NAME = "xamlaJointJogging/get_movegroup_name";
        const string SERVICE_SET_MOVEGROUP_NAME = "xamlaJointJogging/set_movegroup_name";
        const string SERVICE_GET_ENDEFFECTOR_NAME = "xamlaJointJogging/get_endeffector_name";
        const string SERVICE_SET_ENDEFFECTOR_NAME = "xamlaJointJogging/set_endeffector_name";
        const string SERVICE_STATUS = "xamlaJointJogging/status";
        const string SERVICE_GET_VELOCITY_SCALING = "xamlaJointJogging/get_velocity_scaling";
        const string SERVICE_SET_VELOCITY_SCALING = "xamlaJointJogging/set_velocity_scaling";
        const string SERVICE_GET_FLAG = "xamlaJointJogging/get_flag";
        const string SERVICE_SET_FLAG = "xamlaJointJogging/set_flag";

        Publisher<trajectory_msgs.JointTrajectory> publisherJoggingCommand;
        Publisher<Messages.geometry_msgs.PoseStamped> publisherSetpointJoggingCommand;
        Publisher<Messages.geometry_msgs.TwistStamped> publisherTwistJoggingCommand;
        ServiceClient<std_srvs.SetBool> toggleTracking;
        ServiceClient<xamlamoveit.GetSelected> getMovegroupName;
        ServiceClient<xamlamoveit.SetString> setMovegroupName;
        ServiceClient<xamlamoveit.GetSelected> getEndeffectorName;
        ServiceClient<xamlamoveit.SetString> setEndeffectorName;
        ServiceClient<xamlamoveit.StatusController> status;
        ServiceClient<xamlamoveit.GetFloat> getVelocityScaling;
        ServiceClient<xamlamoveit.SetFloat> setVelocityScaling;
        ServiceClient<xamlamoveit.GetFlag> getFlag;
        ServiceClient<xamlamoveit.SetFlag> setFlag;

        Subscriber joggingFeedback;
        Subject<JoggingControllerStatusModel> whenJoggingFeedback;

        public JoggingClient(NodeHandle nodeHandle)
        {
            this.nodeHandle = nodeHandle;
            whenJoggingFeedback = new Subject<JoggingControllerStatusModel>();
        }

        public void Initialize()
        {
            publisherJoggingCommand = nodeHandle.Advertise<trajectory_msgs.JointTrajectory>(TOPIC_JOGGING_COMMAND, 5, false);
            publisherSetpointJoggingCommand = nodeHandle.Advertise<Messages.geometry_msgs.PoseStamped>(TOPIC_SETPOINT_JOGGING_COMMAND, 5, false);
            publisherTwistJoggingCommand = nodeHandle.Advertise<Messages.geometry_msgs.TwistStamped>(TOPIC_TWIST_JOGGING_COMMAND, 5, false);
            toggleTracking = nodeHandle.ServiceClient<std_srvs.SetBool>(SERVICE_TOGGLE_TRACKING);
            getMovegroupName = nodeHandle.ServiceClient<xamlamoveit.GetSelected>(SERVICE_GET_MOVEGROUP_NAME);
            setMovegroupName = nodeHandle.ServiceClient<xamlamoveit.SetString>(SERVICE_SET_MOVEGROUP_NAME);
            getEndeffectorName = nodeHandle.ServiceClient<xamlamoveit.GetSelected>(SERVICE_GET_ENDEFFECTOR_NAME);
            setEndeffectorName = nodeHandle.ServiceClient<xamlamoveit.SetString>(SERVICE_SET_ENDEFFECTOR_NAME);
            status = nodeHandle.ServiceClient<xamlamoveit.StatusController>(SERVICE_STATUS, true);
            getVelocityScaling = nodeHandle.ServiceClient<xamlamoveit.GetFloat>(SERVICE_GET_VELOCITY_SCALING, true);
            setVelocityScaling = nodeHandle.ServiceClient<xamlamoveit.SetFloat>(SERVICE_SET_VELOCITY_SCALING, true);
            getFlag = nodeHandle.ServiceClient<xamlamoveit.GetFlag>(SERVICE_GET_FLAG, true);
            setFlag = nodeHandle.ServiceClient<xamlamoveit.SetFlag>(SERVICE_SET_FLAG, true);
            joggingFeedback = nodeHandle.Subscribe<xamlamoveit.ControllerState>(TOPIC_JOGGING_FEEDBACK, 1, HandleJoggingFeedback, false);
        }

        public void Dispose()
        {
            whenJoggingFeedback.Dispose();
            publisherJoggingCommand?.Dispose();
            publisherSetpointJoggingCommand?.Dispose();
            publisherTwistJoggingCommand?.Dispose();
            toggleTracking?.Dispose();
            getMovegroupName?.Dispose();
            setMovegroupName?.Dispose();
            getEndeffectorName?.Dispose();
            setEndeffectorName?.Dispose();
            status?.Dispose();
            getVelocityScaling?.Dispose();
            setVelocityScaling?.Dispose();
            getFlag?.Dispose();
            setFlag?.Dispose();
            joggingFeedback?.Dispose();
        }

        public IObservable<JoggingControllerStatusModel> WhenJoggingFeedback =>
            whenJoggingFeedback;

        public void SendVelocities(JointValues velocities)
        {
            var point = new trajectory_msgs.JointTrajectoryPoint()
            {
                time_from_start = TimeSpan.FromSeconds(0.008).ToDurationMessage(),
                velocities = velocities.Values,
            };

            var trajectory = new trajectory_msgs.JointTrajectory()
            {
                joint_names = velocities.JointSet.ToArray(),
                points = new [] { point }
            };
            publisherJoggingCommand.Publish(trajectory);
        }

        public void SendSetpoint(Pose setpoint)
        {
            var pose = setpoint.ToPoseStampedMessage();
            publisherSetpointJoggingCommand.Publish(pose);
        }

        public string[] GetMoveGroupName()
        {
            var srv = new xamlamoveit.GetSelected();
            if (!getMovegroupName.Call(srv))
                throw new ServiceCallFailedException(SERVICE_GET_MOVEGROUP_NAME);
            return srv.resp.collection;
        }

        public void SetMoveGroupName(string value)
        {
            var srv = new xamlamoveit.SetString();
            srv.req.data = value;
            if (!setMovegroupName.Call(srv))
                throw new ServiceCallFailedException(SERVICE_SET_MOVEGROUP_NAME);
        }

        public string[] GetEndEffectorName()
        {
            var srv = new xamlamoveit.GetSelected();
            if (!getEndeffectorName.Call(srv))
                throw new ServiceCallFailedException(SERVICE_GET_ENDEFFECTOR_NAME);
            return srv.resp.collection;
        }

        public void SetEndEffectorName(string value)
        {
            var srv = new xamlamoveit.SetString();
            srv.req.data = value;
            if (!setEndeffectorName.Call(srv))
                throw new ServiceCallFailedException(SERVICE_SET_ENDEFFECTOR_NAME);
        }

        public ControllerStatusModel GetStatus()
        {
            var srv = new xamlamoveit.StatusController();
            if (!status.Call(srv))
                throw new ServiceCallFailedException(SERVICE_STATUS);
            var response = srv.resp;
            return new ControllerStatusModel
            {
                IsRunning = response.is_running,
                    InTopic = response.in_topic,
                    OutTopic = response.out_topic,
                    MoveGroupName = response.move_group_name,
                    JointNames = response.joint_names,
                    StatusMessage = response.status_message_tracking
            };
        }

        public double GetVelocityScaling()
        {
            var srv = new xamlamoveit.GetFloat();
            if (!getVelocityScaling.Call(srv))
                throw new ServiceCallFailedException(SERVICE_GET_VELOCITY_SCALING);
            return srv.resp.data;
        }

        public void SetVelocityScaling(double value)
        {
            var srv = new xamlamoveit.SetFloat();
            srv.req.data = value;
            if (!setVelocityScaling.Call(srv))
                throw new ServiceCallFailedException(SERVICE_SET_VELOCITY_SCALING);
        }

        public bool GetFlag(string name)
        {
            var srv = new xamlamoveit.GetFlag();
            srv.req.name = name;
            if (!getFlag.Call(srv))
                throw new ServiceCallFailedException(SERVICE_GET_FLAG);
            return srv.resp.value;
        }

        public void SetFlag(string name, bool value)
        {
            var srv = new xamlamoveit.SetFlag();
            srv.req.name = name;
            srv.req.value = value;
            if (!setFlag.Call(srv))
                throw new ServiceCallFailedException(SERVICE_SET_FLAG);
        }

        private void ToggleTracking(bool activate)
        {
            var srv = new std_srvs.SetBool();
            srv.req.data = activate;
            if (!toggleTracking.Call(srv))
                throw new ServiceCallFailedException(SERVICE_TOGGLE_TRACKING);
        }

        public void Start()
        {
            ToggleTracking(true);
        }

        public void Stop()
        {
            ToggleTracking(false);
        }

        public void SendTwist(Twist twist)
        {
            var twistStamped = twist.ToTwistStampedMessage();
            publisherTwistJoggingCommand.Publish(twistStamped);
        }

        void HandleJoggingFeedback(xamlamoveit.ControllerState state)
        {
            var model = new JoggingControllerStatusModel
            {
                JointDistance = state.joint_distance,
                CartesianDistance = state.cartesian_distance,
                ErrorCode = (JoggingErrorCode) (int) state.error_code,
                Converged = state.converged,
                SelfCollisionCheckEnabled = state.self_collision_check_enabled,
                SceneCollisionCheckEnabled = state.scene_collision_check_enabled,
                JointLimitsCheckEnabled = state.joint_limits_check_enabled
            };

            whenJoggingFeedback.OnNext(model);
        }
    }
}