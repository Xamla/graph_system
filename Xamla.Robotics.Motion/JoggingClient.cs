using System;
using System.Collections.Generic;
using System.Text;
using Uml.Robotics.Ros;
using trajectory_msgs = Messages.trajectory_msgs;
using std_srvs = Messages.std_srvs;
using xamlamoveit = Messages.xamlamoveit_msgs;
using System.Reactive.Subjects;
using Xamla.Robotics.Types;
using Xamla.Robotics.Types.Models;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Jogging client offering jogging functionality
    /// </summary>
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

        /// <summary>
        /// Creates a jogging client
        /// </summary>
        /// <param name="nodeHandle">Handle of a ROS node</param>
        public JoggingClient(NodeHandle nodeHandle)
        {
            this.nodeHandle = nodeHandle;
            whenJoggingFeedback = new Subject<JoggingControllerStatusModel>();
        }

        /// <summary>
        /// Initialize the jogging client
        /// </summary>
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

        /// <summary>
        /// Clean up
        /// </summary>
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

        /// <summary>
        /// An object implementing IObservable<JoggingControllerStatusModel>
        /// </summary>
        public IObservable<JoggingControllerStatusModel> WhenJoggingFeedback =>
            whenJoggingFeedback;

        /// <summary>
        /// Send velocities to service
        /// </summary>
        /// <param name="velocities">Velocities to sent [m/s]</param>
        public void SendVelocities(JointValues velocities)
        {
            var point = new trajectory_msgs.JointTrajectoryPoint
            {
                time_from_start = TimeSpan.FromSeconds(0.008).ToDurationMessage(),
                velocities = velocities.ToArray(),
            };

            var trajectory = new trajectory_msgs.JointTrajectory()
            {
                joint_names = velocities.JointSet.ToArray(),
                points = new [] { point }
            };
            publisherJoggingCommand.Publish(trajectory);
        }

        /// <summary>
        /// Send points to service
        /// </summary>
        /// <param name="setpoint">The setpoint to be sent</param>
        public void SendSetpoint(Pose setpoint)
        {
            var pose = setpoint.ToPoseStampedMessage();
            publisherSetpointJoggingCommand.Publish(pose);
        }

        /// <summary>
        /// Get the name of the move group
        /// </summary>
        /// <returns>Returns a list of strings containing the names of move group</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        public string[] GetMoveGroupName()
        {
            var srv = new xamlamoveit.GetSelected();
            if (!getMovegroupName.Call(srv))
                throw new ServiceCallFailedException(SERVICE_GET_MOVEGROUP_NAME);
            return srv.resp.collection;
        }

        /// <summary>
        /// Set the name of the move group
        /// </summary>
        /// <param name="value">The name to be set</param>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        public void SetMoveGroupName(string value)
        {
            var srv = new xamlamoveit.SetString();
            srv.req.data = value;
            if (!setMovegroupName.Call(srv))
                throw new ServiceCallFailedException(SERVICE_SET_MOVEGROUP_NAME);
        }

        /// <summary>
        /// Get the name of the endeffector
        /// </summary>
        /// <returns>Returns a list of strings containing the names of the endeffector</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        public string[] GetEndEffectorName()
        {
            var srv = new xamlamoveit.GetSelected();
            if (!getEndeffectorName.Call(srv))
                throw new ServiceCallFailedException(SERVICE_GET_ENDEFFECTOR_NAME);
            return srv.resp.collection;
        }

        /// <summary>
        /// Set the name of the endeffector
        /// </summary>
        /// <param name="value">The name of the endeffector</param>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        public void SetEndEffectorName(string value)
        {
            var srv = new xamlamoveit.SetString();
            srv.req.data = value;
            if (!setEndeffectorName.Call(srv))
                throw new ServiceCallFailedException(SERVICE_SET_ENDEFFECTOR_NAME);
        }

        /// <summary>
        /// Get the controller status
        /// </summary>
        /// <returns>Returns the controller status as an instance of <c>ControllerStatusModel</c>.</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
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

        /// <summary>
        /// Get the velocity scaling
        /// </summary>
        /// <returns>Returns the velocity scaling</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        public double GetVelocityScaling()
        {
            var srv = new xamlamoveit.GetFloat();
            if (!getVelocityScaling.Call(srv))
                throw new ServiceCallFailedException(SERVICE_GET_VELOCITY_SCALING);
            return srv.resp.data;
        }

        /// <summary>
        /// Set the velocity scaling factor
        /// </summary>
        /// <param name="value">Scaling factor to be set</param>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        public void SetVelocityScaling(double value)
        {
            var srv = new xamlamoveit.SetFloat();
            srv.req.data = value;
            if (!setVelocityScaling.Call(srv))
                throw new ServiceCallFailedException(SERVICE_SET_VELOCITY_SCALING);
        }

        /// <summary>
        /// Get flag by name
        /// </summary>
        /// <param name="name">Name of the flag</param>
        /// <returns>Returns true if flag is set, false otherwise</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        public bool GetFlag(string name)
        {
            var srv = new xamlamoveit.GetFlag();
            srv.req.name = name;
            if (!getFlag.Call(srv))
                throw new ServiceCallFailedException(SERVICE_GET_FLAG);
            return srv.resp.value;
        }

        /// <summary>
        /// Set flag of given name
        /// </summary>
        /// <param name="name">Name of the flag</param>
        /// <param name="value">The value the flag should be set</param>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        public void SetFlag(string name, bool value)
        {
            var srv = new xamlamoveit.SetFlag();
            srv.req.name = name;
            srv.req.value = value;
            if (!setFlag.Call(srv))
                throw new ServiceCallFailedException(SERVICE_SET_FLAG);
        }

        /// <summary>
        /// Toggles tracking
        /// </summary>
        /// <param name="activate">If true, activates tracking; deactivates otherwise</param>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
        private void ToggleTracking(bool activate)
        {
            var srv = new std_srvs.SetBool();
            srv.req.data = activate;
            if (!toggleTracking.Call(srv))
                throw new ServiceCallFailedException(SERVICE_TOGGLE_TRACKING);
        }

        /// <summary>
        /// Starts tracking
        /// </summary>
        public void Start()
        {
            ToggleTracking(true);
        }

        /// <summary>
        /// Stops tracking
        /// </summary>
        public void Stop()
        {
            ToggleTracking(false);
        }

        /// <summary>
        /// Send twist to service
        /// </summary>
        /// <param name="twist">The twist to be sent</param>
        public void SendTwist(Twist twist)
        {
            var twistStamped = twist.ToTwistStampedMessage();
            publisherTwistJoggingCommand.Publish(twistStamped);
        }

        /// <summary>
        /// Handle feedback
        /// </summary>
        /// <param name="state">Controller state: Definition</param>
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