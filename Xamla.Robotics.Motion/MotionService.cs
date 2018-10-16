using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uml.Robotics.Ros;
using Uml.Robotics.Ros.ActionLib;
using Xamla.Utilities;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    using xamlamoveit = Messages.xamlamoveit_msgs;
    using control_msg = Messages.control_msgs;
    using geometry_msgs = Messages.geometry_msgs;
    using wsg_50 = Messages.wsg_50;
    using xmlRpc = Uml.Robotics.XmlRpc;


    /// <summary>
    /// TODO: description
    /// </summary>
    public class MotionService
        : IMotionService
    {
        const string EE_LIMITS_PARAM = "xamlaJointJogging/end_effector_list";
        const string JOINT_LIMITS_PARAM = "robot_description_planning/joint_limits";
        const string QUERY_IK_SERVICE_NAME = "xamlaMoveGroupServices/query_ik";
        const string MOVEJ_ACTION_NAME = "moveJ_action";

        NodeHandle nodeHandle;
        double globalVelocityScaling = 1;
        double globalAccelerationScaling = 1;
        readonly object gate = new object();
        ServiceClient<xamlamoveit.GetIKSolution> queryIKService;
        IActionClient<xamlamoveit.moveJGoal, xamlamoveit.moveJResult, xamlamoveit.moveJFeedback> moveJActionClient;
        Dictionary<string, IDisposable> actionClientPool = new Dictionary<string, IDisposable>();

        /// <summary>
        /// Creates a motion service 
        /// </summary>
        /// <param name="nodeHandle">Handle of a ROS node</param>
        public MotionService(NodeHandle nodeHandle)
        {
            this.nodeHandle = nodeHandle;
            queryIKService = nodeHandle.ServiceClient<xamlamoveit.GetIKSolution>(QUERY_IK_SERVICE_NAME, true);
            moveJActionClient = GetActionClient<xamlamoveit.moveJGoal, xamlamoveit.moveJResult, xamlamoveit.moveJFeedback>(MOVEJ_ACTION_NAME);
        }

        /// <summary>
        /// Clean up
        /// </summary>
        public void Dispose()
        {
            lock (gate)
            {
                if (queryIKService != null)
                {
                    queryIKService.Dispose();
                    queryIKService = null;
                }

                foreach (var client in actionClientPool.Values)
                {
                    client.Dispose();
                }
                actionClientPool = null;
            }
        }

        /// <summary>
        /// Handle of a ROS node
        /// </summary>
        public NodeHandle NodeHandle =>
            nodeHandle;


        /// <summary>
        /// Creates a move group.
        /// </summary>
        /// <param name="moveGroupName">Optional name for the move group</param>
        /// <param name="endEffectorName">Optional name for the end effector</param>
        /// <returns>A reference to an object implementing <c>IMoveGroup</c></returns>
        public IMoveGroup CreateMoveGroup(string moveGroupName = null, string defaultEndEffectorName = null) =>
            new MoveGroup(this, moveGroupName, defaultEndEffectorName);


        /// <summary>
        /// Creates a move group based on a <c>JointSet</c> object.
        /// </summary>
        /// <param name="jointSet">Joint configurations</param>
        /// <returns>A reference to an object implementing <c>IMoveGroup</c></returns>
        public IMoveGroup CreateMoveGroupForJointSet(JointSet jointSet) =>
            new MoveGroup(this, jointSet);

        /// <summary>
        /// Create an endeffector
        /// </summary>
        /// <param name="endEffectorName">The name of the endeffector to be created</param>
        /// <returns>Returns an object implementing <c>IEndEffector</c>.</returns>
        public IEndEffector CreateEndEffector(string endEffectorName)
        {
            var endEffectors = this.QueryAvailableEndEffectors();

            EndEffectorDescription endEffectorDescription = string.IsNullOrEmpty(endEffectorName) ? endEffectors.FirstOrDefault() : endEffectors.FirstOrDefault(x => x.Name == endEffectorName);

            if (endEffectorDescription == null)
                throw new Exception($"EndEffector '{endEffectorName}' not found.");

            var moveGroup = this.CreateMoveGroup(endEffectorDescription.MoveGroupName, endEffectorDescription.Name);
            return moveGroup.DefaultEndEffector;
        }

         /// <summary>
        /// Query all currently available move groups.
        /// </summary>
        /// <returns>Returns an object implementing IList, which contains instances of <c>MoveGroupDescription</c> of all the available move groups.</returns>        
        /// <exception cref="ServiceCallFailedException">Thrown when TODO: description.</exception>
        public IList<MoveGroupDescription> QueryAvailableMoveGroups()
        {
            const string serviceName = "xamlaMoveGroupServices/query_move_group_interface";
            var moveGroupInterface = nodeHandle.ServiceClient<xamlamoveit.QueryMoveGroupInterfaces>(serviceName);
            var srv = new xamlamoveit.QueryMoveGroupInterfaces();
            if (!moveGroupInterface.Call(srv))
                throw new ServiceCallFailedException(serviceName);

            var groups = new List<MoveGroupDescription>();
            foreach (var g in srv.resp.move_group_interfaces)
            {
                var jointSet = g.joint_names != null ? new JointSet(g.joint_names) : JointSet.Empty;
                groups.Add(new MoveGroupDescription(g.name, g.sub_move_group_ids, jointSet, g.end_effector_names, g.end_effector_link_names));
            }

            return groups;
        }

        /// <summary>
        /// Query all currently available end effectors
        /// </summary>
        /// <returns>Returns an object implementing IList, which contains instances of <c>EndEffectorDescription</c> of all the available end effectors.</returns>
        public IList<EndEffectorDescription> QueryAvailableEndEffectors()
        {
            var moveGroups = QueryAvailableMoveGroups();
            var result = new Dictionary<string, EndEffectorDescription>();
            foreach (var group in moveGroups)
            {
                for (int i = 0; i < group.EndEffectorNames.Count(); i++)
                {
                    if (!result.ContainsKey(group.EndEffectorNames[i]))
                    {
                        var description = new EndEffectorDescription(group.EndEffectorNames[i], group.SubMoveGroupIds, group.JointSet, group.Name, group.EndEffectorLinkNames[i]);
                        result.Add(group.EndEffectorNames[i], description);
                    }
                }
            }
            return result.Values.ToList();
        }

        /// <summary>
        /// Query end effector limits
        /// </summary>
        /// <param name="name">Name of the end effector for which the limits are queried</param>
        /// <returns>Return instance of <c>EndEffectorLimits</c>.</returns>
        public EndEffectorLimits QueryEndEffectorLimits(string name)
        {
            double maxXYZVel = 0;
            double maxXYZAcc = 0;
            double maxAngularVel = 0;
            double maxAngularAcc = 0;
            xmlRpc.XmlRpcValue test;
            if (Param.Get($"/{EE_LIMITS_PARAM}", out test))
            {
                for (int i = 0; i < test.Count; i++)
                {
                    if (test[i].HasMember("name"))
                    {
                        string tmp_name = test[i]["name"].GetString();
                        if (tmp_name == name)
                        {
                            maxXYZVel = test[i]["taskspace_xyz_max_vel"].GetDouble();
                            maxXYZAcc = test[i]["taskspace_xyz_max_acc"].GetDouble();
                            maxAngularVel = test[i]["taskspace_angular_max_vel"].GetDouble();
                            maxAngularAcc = test[i]["taskspace_angular_max_acc"].GetDouble();
                            return new EndEffectorLimits(maxXYZVel, maxXYZAcc, maxAngularVel, maxAngularAcc);
                        }
                    }
                }
            }
            return new EndEffectorLimits(maxXYZVel, maxXYZAcc, maxAngularVel, maxAngularAcc);
        }


        /// <summary>
        /// Query end joint limits
        /// </summary>
        /// <param name="joints">Set of joint for which the limits are queried</param>
        /// <returns>Returns an instance of <c>JointLimits</c>.</returns>
        public JointLimits QueryJointLimits(JointSet joints)
        {
            var maxVel = new double?[joints.Count];
            var maxAcc = new double?[joints.Count];
            var minPos = new double?[joints.Count];
            var maxPos = new double?[joints.Count];

            int i = 0;
            foreach (var name in joints)
            {
                var hasVelParam = $"/{JOINT_LIMITS_PARAM}/{name}/has_velocity_limits";
                var velParam = $"/{JOINT_LIMITS_PARAM}/{name}/max_velocity";
                var hasAccParam = $"/{JOINT_LIMITS_PARAM}/{name}/has_acceleration_limits";
                var accParam = $"/{JOINT_LIMITS_PARAM}/{name}/max_acceleration";
                var hasPosParam = $"/{JOINT_LIMITS_PARAM}/{name}/has_position_limits";
                var minPosParam = $"/{JOINT_LIMITS_PARAM}/{name}/min_position";
                var maxPosParam = $"/{JOINT_LIMITS_PARAM}/{name}/max_position";

                if (Param.GetBool(hasVelParam))
                {
                    maxVel[i] = Param.GetDouble(velParam);
                }

                if (Param.GetBool(hasAccParam))
                {
                    maxAcc[i] = Param.GetDouble(accParam);
                }
                else
                {
                    // fallback values: assume max velocity can be reached within 1 seconds
                    maxAcc[i] = maxVel[i];
                }

                if (Param.GetBool(hasPosParam))
                {
                    minPos[i] = Param.GetDouble(minPosParam);
                    maxPos[i] = Param.GetDouble(maxPosParam);
                }

                i += 1;
            }

            return new JointLimits(joints, maxVel, maxAcc, minPos, maxPos);
        }

        /// <summary>
        /// Query joint states 
        /// </summary>
        /// <param name="joints">Set of joint for which the limits are queried</param>
        /// <returns>Returns a instance of <c>JointStates</c> which contains the current positions the joints defined in  <paramref name="joints"/>.</returns>ld be returned.</param>
        /// <exception cref="ServiceCallFailedException">Thrown when TODO: description.</exception>
        public JointStates QueryJointStates(JointSet joints)
        {
            const string serviceName = "xamlaMoveGroupServices/query_move_group_current_position";
            var service = nodeHandle.ServiceClient<xamlamoveit.GetCurrentJointState>(serviceName);
            var srv = new xamlamoveit.GetCurrentJointState();

            var request = srv.req;
            request.joint_names = joints.ToArray();
            if (!service.Call(srv))
                throw new ServiceCallFailedException(serviceName);
            var response = srv.resp;
            return response.current_joint_position.ToJointState();
        }

        /// <summary>
        /// Evaluate the forward kinematic for given joint positions to get the pose of a MoveGroup.
        /// </summary>
        /// <param name="moveGroupName">Name of the move group from which the pose is queried</param>
        /// <param name="jointPositions">Joint values from which the pose is calculated</param>
        /// <param name="endEffectorLink">End effector link is necessary if end effector is not part of the move group but pose should be computed for the end effector.</param>
        /// <returns>Returns the computed <c>Pose</c> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="moveGroupName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jointPositions"/> is null.</exception>
        public Pose QueryPose(string moveGroupName, JointValues jointPositions, string endEffectorLink = "")
        {
            if (string.IsNullOrEmpty(moveGroupName))
                throw new ArgumentNullException(nameof(moveGroupName));
            if (jointPositions == null)
                throw new ArgumentNullException(nameof(jointPositions));
            return QueryPoseMany(moveGroupName, new JointPath(jointPositions), endEffectorLink).First();
        }

        /// <summary>
        /// Query the poses from joint path points by applying forward kinematics
        /// </summary>
        /// <param name="moveGroupName">Name of the move group from which the pose is queried</param>
        /// <param name="waypoints">Joint path from which the poses are calculated</param>
        /// <param name="endEffectorLink">End effector link is necessary if end effector is not part of the move group but pose should be computed for the end effector.</param>
        /// <returns>Returns an object which implements <c>IList</c> and contains the computed poses.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="moveGroupName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waypoints"/> is null.</exception>
        /// <exception cref="ServiceCallFailedException">Thrown when TODO: description.</exception>
        public IList<Pose> QueryPoseMany(string moveGroupName, IJointPath waypoints, string endEffectorLink = "")
        {
            if (string.IsNullOrEmpty(moveGroupName))
                throw new ArgumentNullException(nameof(moveGroupName));
            if (waypoints == null)
                throw new ArgumentNullException(nameof(waypoints));

            var joints = waypoints.JointSet;

            const string serviceName = "xamlaMoveGroupServices/query_fk";
            var service = nodeHandle.ServiceClient<xamlamoveit.GetFKSolution>(serviceName);
            var srv = new xamlamoveit.GetFKSolution();
            var request = srv.req;
            request.group_name = moveGroupName;
            request.end_effector_link = endEffectorLink;
            request.points = waypoints.Select(x => x.ToJointPathPointMessage()).ToArray();
            request.joint_names = joints.ToArray();

            if (!service.Call(srv))
                throw new ServiceCallFailedException(serviceName);

            var response = srv.resp;
            if (response.error_codes == null || response.error_msgs == null || response.error_codes.Length != response.error_msgs.Length)
                throw new ServiceCallFailedException(serviceName, "Invalid response received from service.");

            var (error, index) = response.error_codes.FirstOrDefaultIndex(x => (MoveItErrorCode)x.val != MoveItErrorCode.SUCCESS);
            if (error != null)
            {
                var errorCode = (MoveItErrorCode)error.val;
                var errorMessage = response.error_msgs[index];
                throw new ServiceCallFailedException(serviceName, $"Service call to {serviceName} failed with error code {errorCode}.");
            }

            return response.solutions.Select(x => x?.ToPose()).ToList();
        }

        private xamlamoveit.GetMoveItJointPath QueryMoveItJointPathInternal(string moveGroupName, IJointPath waypoints)
        {
            if (string.IsNullOrEmpty(moveGroupName))
                throw new ArgumentNullException(nameof(moveGroupName));

            var jointSet = waypoints.JointSet;

            const string serviceName = "xamlaPlanningServices/query_joint_path";
            var service = nodeHandle.ServiceClient<xamlamoveit.GetMoveItJointPath>(serviceName);
            var srv = new xamlamoveit.GetMoveItJointPath();
            var request = srv.req;
            request.group_name = moveGroupName;
            request.joint_names = jointSet.ToArray();
            request.waypoints = waypoints.Select(x => x.ToJointPathPointMessage()).ToArray();

            if (!service.Call(srv))
                throw new ServiceCallFailedException(serviceName);

            return srv;
        }

        /// <summary>
        /// Query a collision free joint path from defined joint path
        /// </summary>
        /// <param name="moveGroupName">Name of the move group for which the collision free joint is queried</param>
        /// <param name="waypoints">Joint path which may contain collisions</param>
        /// <returns>New reference to object implementing <c>IJointPath</c>, which is now collision free.</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when TODO: description.</exception>
        public IJointPath QueryCollisionFreeJointPath(string moveGroupName, IJointPath waypoints)
        {
            const string serviceName = "xamlaPlanningServices/query_joint_path";
            var srv = QueryMoveItJointPathInternal(moveGroupName, waypoints);
            var response = srv.resp;
            MoveItErrorCode errorCode = (MoveItErrorCode)response.error_code.val;
            if (errorCode != MoveItErrorCode.SUCCESS)
                throw new ServiceCallFailedException(serviceName, $"Service call to {serviceName} failed with error code {errorCode}.");

            var jointSet = waypoints.JointSet;
            return new JointPath(waypoints.JointSet, response.path.Select(x => x.ToJointValues(jointSet)));
        }

        /// <summary>
        /// Query a cartesian path TODO: overall description might be ogg
        /// </summary>
        /// <param name="moveGroupName">Name of the move group from which the poses are queried</param>
        /// <param name="jointNames">TODO: description</param>
        /// <param name="endEffectorLink">Name of the endeffector link</param>
        /// <param name="waypoints">The path to be queried</param>
        /// <param name="numberOfSteps">TODO: description</param>
        /// <param name="maxDeviation">Defines the maximal deviation of the joints to the defined key points while executing the trajectory TODO: might be false description</param>
        /// <returns>Returns object implementing <c>ICartesianPath</c>.</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when TODO: description.</exception>
        public ICartesianPath QueryCartesianPath(
            string moveGroupName,
            JointSet jointNames,
            string endEffectorLink,
            ICartesianPath waypoints,
            int numberOfSteps = 50,
            double maxDeviation = 1.0
        )
        {
            const string serviceName = "xamlaPlanningServices/query_cartesian_path";
            var service = nodeHandle.ServiceClient<xamlamoveit.GetLinearCartesianPath>(serviceName);
            var srv = new xamlamoveit.GetLinearCartesianPath();

            var request = srv.req;
            request.waypoints = waypoints.Select(x => x.ToPoseStampedMessage()).ToArray();
            request.num_steps = (uint)numberOfSteps;

            if (!service.Call(srv))
                throw new ServiceCallFailedException(serviceName);

            var response = srv.resp;
            return new CartesianPath(response.path.Select(x => x.ToPose()));
        }

        /// <summary>
        /// Query a joint trajectory from joint path
        /// </summary>
        /// <param name="waypoints">Defines the key joint positions the trajectory must reach</param>
        /// <param name="maxVelocity">Defines the maximal velocity for every joint</param>
        /// <param name="maxAcceleration">Defines the maximal acceleration for every joint</param>
        /// <param name="maxDeviation"> Defines the maximal deviation of the joints to the defined key points while executing the trajectory</param>
        /// <param name="dt">Sampling points frequency or time if value is create as 1.0 the value is interpreted as a value in seconds else the value is interpreted as a value in Hz. TODO: Verify this</param>
        /// <returns>An object which implements <c>IJointTrajectory</c>.</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when TODO: description.</exception>
        public IJointTrajectory QueryJointTrajectory(
            IJointPath waypoints,
            double[] maxVelocity,
            double[] maxAcceleration,
            double maxDeviation,
            double dt
        )
        {
            const string serviceName = "xamlaPlanningServices/query_joint_trajectory";
            var service = nodeHandle.ServiceClient<xamlamoveit.GetOptimJointTrajectory>(serviceName);
            var srv = new xamlamoveit.GetOptimJointTrajectory();

            var request = srv.req;

            request.max_deviation = maxDeviation;
            request.joint_names = waypoints.JointSet.ToArray();
            request.dt = dt > 1.0 ? 1 / dt : dt;
            request.max_velocity = maxVelocity;
            request.max_acceleration = maxAcceleration;
            request.waypoints = waypoints.Select(x => new xamlamoveit.JointPathPoint() { positions = x.Values }).ToArray();

            if (!service.Call(srv))
                throw new ServiceCallFailedException(serviceName);

            var response = srv.resp;
            MoveItErrorCode errorCode = (MoveItErrorCode)response.error_code.val;
            if (errorCode != MoveItErrorCode.SUCCESS)
                throw new ServiceCallFailedException(serviceName, $"Service call to {serviceName} failed with error code {errorCode}.");

            var solution = response.solution;

            var resultJointSet = new JointSet(solution.joint_names);
            JointValues getJointValues(double[] v) => v != null && v.Length > 0 ? new JointValues(resultJointSet, v) : null;

            var trajectoryPoints = solution.points.Select(x => new JointTrajectoryPoint(
                    timeFromStart: x.time_from_start.data.ToTimeSpan(),
                    positions: new JointValues(resultJointSet, x.positions),
                    velocities: getJointValues(x.velocities),
                    accelerations: getJointValues(x.accelerations),
                    effort: getJointValues(x.effort)
                )
            );

            return new JointTrajectory(resultJointSet, trajectoryPoints, true);
        }

        /// <summary>
        /// Query a joint trajectory from task space poses
        /// </summary>
        /// <param name="endEffectorName">Name of the end effector for a trajectory should be queried</param>
        /// <param name="waypoints">Define the key poses the trajectory must reach</param>
        /// <param name="seed">Numerical seed to control configuration of the robot</param>
        /// <param name="maxXyzVelocity">Defines the maximal velocity for translation [m/s]</param>
        /// <param name="maxXyzAcceleration">Defines the maximal acceleration for translation [m/s^2]</param>
        /// <param name="maxAngularVelocity">Defines the maximal angular velocity [rad/s]</param>
        /// <param name="maxAngularAcceleration">Defines the maximal angular acceleration [rad/s^2]</param>
        /// <param name="ikJumpThreshold">Maximal inverse kinematic jump</param>
        /// <param name="maxDeviation">Defines the maximal deviation of the joints to the defined key points while executing the trajectory</param>
        /// <param name="collisionCheck">If true check the trajectory is collision free</param>
        /// <param name="dt">Sampling points frequency or time. If value is greater than 1.0 it is interpreted in seconds,  else it is interpreted in Hz. TODO: Verify this</param>
        /// <returns>Returns an object implementing <c>IJointTrajectory</c>.</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when TODO: description.</exception>
        public IJointTrajectory QueryTaskSpaceTrajectory(
            string endEffectorName,
            ICartesianPath waypoints,
            JointValues seed,
            double maxXyzVelocity,
            double maxXyzAcceleration,
            double maxAngularVelocity,
            double maxAngularAcceleration,
            double ikJumpThreshold,
            double maxDeviation,
            bool collisionCheck,
            double dt
        )
        {
            const string serviceName = "xamlaPlanningServices/query_cartesian_trajectory";
            var service = nodeHandle.ServiceClient<xamlamoveit.GetLinearCartesianTrajectory>(serviceName);
            var srv = new xamlamoveit.GetLinearCartesianTrajectory();

            var request = srv.req;

            request.end_effector_name = endEffectorName;
            request.max_deviation = maxDeviation;
            request.joint_names = seed.JointSet.ToArray();
            request.dt = dt > 1.0 ? 1 / dt : dt;
            request.max_angular_velocity = maxAngularVelocity;
            request.max_angular_acceleration = maxAngularAcceleration;
            request.max_xyz_velocity = maxXyzVelocity;
            request.max_xyz_acceleration = maxXyzAcceleration;
            request.waypoints = waypoints.Select(x => new geometry_msgs.PoseStamped() { pose = x.ToPoseMessage() }).ToArray();
            request.seed = new xamlamoveit.JointPathPoint { positions = seed.ToArray() };
            request.ik_jump_threshold = ikJumpThreshold;
            request.collision_check = collisionCheck;

            if (!service.Call(srv))
                throw new ServiceCallFailedException(serviceName);

            var response = srv.resp;
            MoveItErrorCode errorCode = (MoveItErrorCode)response.error_code.val;
            if (errorCode != MoveItErrorCode.SUCCESS)
                throw new ServiceCallFailedException(serviceName, $"Service call to {serviceName} failed with error code {errorCode}.");

            var solution = response.solution;

            var resultJointSet = new JointSet(solution.joint_names);
            JointValues getJointValues(double[] v) => v != null && v.Length > 0 ? new JointValues(resultJointSet, v) : null;

            var trajectoryPoints = solution.points.Select(x => new JointTrajectoryPoint(
                    timeFromStart: x.time_from_start.data.ToTimeSpan(),
                    positions: new JointValues(resultJointSet, x.positions),
                    velocities: getJointValues(x.velocities),
                    accelerations: getJointValues(x.accelerations),
                    effort: getJointValues(x.effort)
                )
            );

            return new JointTrajectory(resultJointSet, trajectoryPoints, true);
        }

        /// <summary>
        /// Query collisions in joint path
        /// </summary>
        /// <param name="moveGroupName">Name of the move group for which a path should be checked for collisions</param>
        /// <param name="points">The path that should be check for collisions</param>
        /// <returns>Returns an object which implements <c>IList</c> and contains all the instances of <c>JointValuesCollision</c> found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="moveGroupName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is null.</exception>
        /// <exception cref="ServiceCallFailedException">Thrown when TODO: description.</exception>
        public IList<JointValuesCollision> QueryJointPathCollisions(string moveGroupName, IJointPath points)
        {
            if (moveGroupName == null)
                throw new ArgumentNullException(nameof(moveGroupName));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            const string serviceName = "xamlaMoveGroupServices/query_joint_position_collision_check";
            var service = nodeHandle.ServiceClient<xamlamoveit.QueryJointStateCollisions>(serviceName);
            var srv = new xamlamoveit.QueryJointStateCollisions();

            var request = srv.req;
            request.move_group_name = moveGroupName;
            request.joint_names = points.JointSet.ToArray();
            request.points = points.Select(x => x.ToJointPathPointMessage()).ToArray();

            if (!service.Call(srv))
                throw new ServiceCallFailedException(serviceName);

            var response = srv.resp;

            if (response.messages.Length != points.Count || response.messages.Length != points.Count || response.error_codes.Length != points.Count)
                throw new Exception($"Unexpected number of results from service '{serviceName}' received.");

            var result = new List<JointValuesCollision>();
            for (int i = 0; i < points.Count; ++i)
            {
                if (response.in_collision[i])
                {
                    result.Add(new JointValuesCollision
                    {
                        Index = i,
                        ErrorCode = response.error_codes[i],
                        Message = response.messages[i]
                    });
                }
            }
            return result;
        }

        /// Create <c>PlanParameters</c> from user defined and/or queried inputs. TODO: Verify this
        /// </summary>
        /// <param name="moveGroupName">Name of the move group for which plan parameters should be created</param>
        /// <param name="joints"><c>JointSet</c> instance for which plan parameters should be created</param>
        /// <param name="maxVelocity">Defines the maximal velocity for every joint</param>
        /// <param name="maxAcceleration">Defines the maximal acceleration for every joint</param>
        /// <param name="sampleResolution">Sample points frequency</param>
        /// <param name="checkCollision">Check for collision if True</param>
        /// <param name="velocityScaling">Scale query or user defined max acceleration. Values between 0.0 and 1.0.</param>
        /// <returns>Instance of <c>PlanParameters</c> with automatically queried and/or user defined values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="sampleResolution"/>  <= 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="velocityScaling"/> < 0 or > 1 .</exception>
        public PlanParameters CreatePlanParameters(string moveGroupName, JointSet joints, double[] maxVelocity, double[] maxAcceleration, double sampleResolution, bool checkCollision, double velocityScaling)
        {
            if (sampleResolution <= 0)
                throw new ArgumentOutOfRangeException("Sample resolution must not be less or equal to zero.");

            if (velocityScaling < 0 || velocityScaling > 1)
                throw new ArgumentOutOfRangeException("Velocity scaling parameter must be in range [0-1].", nameof(velocityScaling));

            if (string.IsNullOrEmpty(moveGroupName))
            {
                IList<MoveGroupDescription> availableMoveGroups = QueryAvailableMoveGroups();
                var firstMoveGroup = availableMoveGroups.First();
                joints = joints ?? firstMoveGroup.JointSet;
                var selectedMoveGroup = availableMoveGroups.First(x => x.JointSet.IsSimilar(joints));
                moveGroupName = selectedMoveGroup.Name;
            }
            else if (joints == null)
            {
                joints = QueryAvailableMoveGroups().First(x => x.Name == moveGroupName).JointSet;
            }

            if (maxAcceleration == null || maxVelocity == null)
            {
                var limits = QueryJointLimits(joints);
                maxAcceleration = limits.MaxAcceleration.Select(x => x ?? Math.PI).ToArray();
                maxVelocity = limits.MaxVelocity.Select(x => x ?? Math.PI).ToArray();
            }

            maxVelocity = maxVelocity.Select(x => x * velocityScaling).ToArray();
            maxAcceleration = maxAcceleration.Select(x => x * velocityScaling).ToArray();

            return new PlanParameters(moveGroupName, joints, maxVelocity, maxAcceleration, sampleResolution, checkCollision);
        }

        /// <summary>
        /// Creates an instance of <c>TakesSpacePlanParameters</c> from user defined or queried inputs
        /// </summary>
        /// <param name="endEffectorName">Name of the end effector</param>
        /// <param name="maxXYZVelocity">Defines the maximal xyz velocity [m/s]</param>
        /// <param name="maxXYZAcceleration">Defines the maximal xyz acceleration [m/s^2]</param>
        /// <param name="maxAngularVelocity">Defines the maximal angular velocity [rad/s]</param>
        /// <param name="maxAngularAcceleration">Defines the maximal angular acceleration [rad/s^2]</param>
        /// <param name="sampleResolution">Sample points frequency</param>
        /// <param name="ikJumpThreshold">Maximal inverse kinematic jump</param>
        /// <param name="maxDeviation">Maximal deviation from fly by points</param>
        /// <param name="checkCollision">Check for collision if True</param>
        /// <param name="velocityScaling">Scale query or user defined max acceleration. Values between 0.0 and 1.0.</param>
        /// <returns>Returns instance of <c>TaskSpacePlanParameters</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="sampleResolution"/>  <= 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="velocityScaling"/> < 0 or > 1 .</exception>
        public TaskSpacePlanParameters CreateTaskSpacePlanParameters(string endEffectorName, double maxXYZVelocity, double maxXYZAcceleration, double maxAngularVelocity, double maxAngularAcceleration, double sampleResolution, double ikJumpThreshold, double maxDeviation, bool checkCollision, double velocityScaling)
        {
            if (sampleResolution <= 0)
                throw new ArgumentOutOfRangeException("Sample resolution must not be less or equal to zero.");

            if (velocityScaling < 0 || velocityScaling > 1)
                throw new ArgumentOutOfRangeException("Velocity scaling parameter must be in range [0-1].", nameof(velocityScaling));

            if (string.IsNullOrEmpty(endEffectorName))
            {
                var firstMoveGroup = QueryAvailableMoveGroups().First();
                endEffectorName = firstMoveGroup.EndEffectorNames.First();
            }

            maxXYZVelocity *= velocityScaling;
            maxXYZAcceleration *= velocityScaling;
            maxAngularVelocity *= velocityScaling;
            maxAngularAcceleration *= velocityScaling;
            return new TaskSpacePlanParameters(endEffectorName, maxXYZVelocity, maxXYZAcceleration, maxAngularVelocity, maxAngularAcceleration, sampleResolution, checkCollision, maxDeviation, ikJumpThreshold);
        }

        /// <summary>
        /// Plans a collision free joint path by querying it
        /// </summary>
        /// <param name="start">Starting joint configurations</param>
        /// <param name="goal">Target joint configuration</param>
        /// <param name="parameters">Plan parameters which defines the limits and move group.</param>
        /// <returns>Returns An object implementing <c>IJointPath</c> defining a collision free joint path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="start"/>  is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="goal"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/>  is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the JointSet of the start position differs from the JointSet of the goal position.</exception>
        public IJointPath PlanCollisionFreeJointPath(JointValues start, JointValues goal, PlanParameters parameters)
        {
            if (start == null)
                throw new ArgumentNullException(nameof(start));
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var jointSet = parameters.JointSet;
            if (!jointSet.Equals(start.JointSet) && jointSet.IsSimilar(start.JointSet))
                start = start.Reorder(jointSet);

            if (!jointSet.Equals(goal.JointSet) && jointSet.IsSimilar(goal.JointSet))
                goal = goal.Reorder(jointSet);

            if (!start.JointSet.Equals(goal.JointSet))
                throw new ArgumentException("The JointSet of the start position differs from the JointSet of the goal position.", nameof(start));

            return PlanCollisionFreeJointPath(new JointPath(start, goal), parameters);
        }

        /// <summary>
        /// Plans a collision free joint path by querying it
        /// </summary>
        /// <param name="waypoints">Joint path which should be replanned to be collision free</param>
        /// <param name="parameters">Defines the limits and move group</param>
        /// <returns>Returns An object implementing <c>IJointPath</c> defining a replanned collision free joint path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waypoints"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/>  is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the JointSet specified in plan parameters differs from JointSet of joint path.</exception>
        public IJointPath PlanCollisionFreeJointPath(IJointPath waypoints, PlanParameters parameters)
        {
            if (waypoints == null)
                throw new ArgumentNullException(nameof(waypoints));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var jointSet = parameters.JointSet;
            if (!jointSet.Equals(waypoints.JointSet))
            {
                if (!jointSet.IsSimilar(waypoints.JointSet))
                    throw new ArgumentException("JointSet specified in plan parameters differs from JointSet of joint path.", nameof(parameters));

                // joint sets have same elements but in different order, convert to order specified by parameters
                waypoints = new JointPath(jointSet, waypoints.Select(x => x.Reorder(jointSet)));
            }

            return this.QueryCollisionFreeJointPath(
                parameters.MoveGroupName,
                waypoints
            );
        }

        /// <summary>
        /// Plan a joint path from a cartesian path and plan parameters TODO: Verify this, could be wrong
        /// </summary>
        /// <param name="path">Poses the planned trajectory must reach</param>
        /// <param name="numSteps">TODO: Define this</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and move group name</param>
        /// <returns>An object implementing <c>ICartesianPath</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
        public ICartesianPath PlanMoveCartesian(ICartesianPath path, int numSteps, PlanParameters parameters)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return this.QueryCartesianPath(parameters.MoveGroupName, parameters.JointSet, null, path, numSteps, 0);
        }

        /// <summary>
        /// Plans trajectory with linear movements from a cartesian path
        /// </summary>
        /// <param name="path">Cartesian path with poses the trajectory must reach</param>
        /// <param name="seed">Numerical seed to control configuration</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and end effector name.</param>
        /// <returns>Returns planned joint trajectory which reach the poses defined in path under the constraints of parameters.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
        public IJointTrajectory PlanMovePoseLinear(ICartesianPath path, JointValues seed, TaskSpacePlanParameters parameters)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return this.QueryTaskSpaceTrajectory(parameters.EndEffectorName, path, seed, parameters.MaxXYZVelocity, parameters.MaxXYZAcceleration, parameters.MaxAngularVelocity, parameters.MaxAngularAcceleration, parameters.IkJumpThreshold, parameters.MaxDeviation, parameters.CollisionCheck, parameters.SampleResolution);
        }

        /// <summary>
        /// Plans trajectory from a joint path
        /// </summary>
        /// <param name="path">Joint path with positions the trajectory must reach</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and move group name</param>
        /// <returns> Planned joint trajectory which reach the positions defined in path under the constraints of parameters</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/>  is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the path and parameter joint sets do not match.</exception>
        public IJointTrajectory PlanMoveJoints(IJointPath path, PlanParameters parameters)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var jointSet = parameters.JointSet;
            if (jointSet != null && !path.JointSet.Equals(jointSet))
            {
                if (!path.JointSet.IsSimilar(jointSet))
                    throw new ArgumentException("Path and parameter joint sets do not match.", nameof(parameters));

                path = new JointPath(jointSet, path.Select(x => x.Reorder(jointSet)));
            }

            if (parameters.MaxAcceleration == null || parameters.MaxVelocity == null)
            {
                var limits = this.QueryJointLimits(path.JointSet);

                var builder = parameters.ToBuilder();
                if (builder.MaxAcceleration == null)
                    builder.MaxAcceleration = limits.MaxAcceleration.Select(x => x.Value).ToArray();

                if (builder.MaxVelocity == null)
                    builder.MaxVelocity = limits.MaxVelocity.Select(x => x.Value).ToArray();

                parameters = builder.Build();
            }

            var maxVelocity = globalVelocityScaling == 1 ? parameters.MaxVelocity : parameters.MaxVelocity.Select(x => x * globalVelocityScaling).ToArray();
            var maxAcceleration = globalAccelerationScaling == 1 ? parameters.MaxAcceleration : parameters.MaxAcceleration.Select(x => x * globalAccelerationScaling).ToArray();

            IJointTrajectory trajectory = this.QueryJointTrajectory(
                path,
                maxVelocity,
                maxAcceleration,
                parameters.MaxDeviation,
                parameters.SampleResolution
            );

            return trajectory;
        }

        /// <summary>
        /// Executes a joint trajectory asynchronously
        /// </summary>
        /// <param name="trajectory">Joint trajectory which should be executed</param>
        /// <param name="checkCollision">Check for collision if True</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a Task instance which returns the result as <c>int</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="trajectory"/> is null.</exception>
        /// <exception cref="Exception">Thrown when unexpected null result received by ActionClient for.</exception>
        public async Task<int> ExecuteJointTrajectory(IJointTrajectory trajectory, bool checkCollision, CancellationToken cancel = default(CancellationToken))
        {
            if (trajectory == null)
                throw new ArgumentNullException(nameof(trajectory));

            var actionClient = moveJActionClient;

            var g = actionClient.CreateGoal();
            g.check_collision = checkCollision;
            g.trajectory.joint_names = trajectory.JointSet.ToArray();
            g.trajectory.points = trajectory.Select(x => x.ToJointTrajectoryPointMessage()).ToArray();

            var result = await actionClient.SendGoalAsync(g, cancel);
            if (result == null)
                throw new Exception($"Unexpected null result received by ActionClient for {actionClient.Name}.");

            return result.result;
        }

        /// <summary>
        /// Executes a joint trajectory  asynchronously
        /// </summary>
        /// <param name="trajectory">Joint trajectory which should be executed</param>
        /// <param name="velocityScaling">Scale query or user defined max acceleration. Values between 0.0 and 1.0.</param>
        /// <param name="checkCollision">Check for collision if True</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns> Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="trajectory"/> is null.</exception>
        public ISteppedMotionClient ExecuteJointTrajectorySupervised(IJointTrajectory trajectory, double velocityScaling = 1.0, bool checkCollision = true, CancellationToken cancel = default(CancellationToken))
        {
            if (trajectory == null)
                throw new ArgumentNullException(nameof(trajectory));

            return new SteppedMotionClient(nodeHandle, trajectory, velocityScaling, checkCollision, cancel);
        }

        /// <summary>
        /// Get the joint configuration based on pose
        /// </summary>
        /// <param name="pose">Pose to transform to joint space</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and move group name</param>
        /// <param name="jointPositionSeed">Optional numerical seed to control joint configuration</param>
        /// <param name="endEffectorLink">Necessary if poses are defined for end effector link</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="attempts">Attempts to find a solution or each pose</param>
        /// <returns>Returns an instance of <c>JointValues</c> with the joint configuration.</returns>
        /// <exception cref="Exception">Thrown when received invalid IK result.</exception>
        /// <exception cref="Exception">Thrown when IK call was not successful.</exception>
        public JointValues InverseKinematic(
            Pose pose,
            PlanParameters parameters,
            JointValues jointPositionSeed = null,
            string endEffectorLink = "",
            TimeSpan? timeout = null,
            int attempts = 1
        )
        {
            var ikResult = InverseKinematicMany(new[] { pose }, parameters, jointPositionSeed, endEffectorLink, timeout, attempts);
            if (ikResult.Path == null || ikResult.Path.Count == 0 || ikResult.ErrorCodes == null || ikResult.ErrorCodes.Count == 0)
            {
                throw new Exception("Received invalid IK result.");
            }
            if (!ikResult.Suceeded)
            {
                throw new Exception("IK call was not successful.");
            }

            return ikResult.Path.FirstOrDefault();
        }

        /// <summary>
        /// Get the joint configuration based on pose
        /// </summary>
        /// <param name="pose">Pose to transform to joint space</param>
        /// <param name="endEffectorName">Name of the end effector</param>
        /// <param name="avoidCollision">Indicates if collision should be avoided</param>
        /// <param name="jointPositionSeed">Optional numerical seed to control joint configuration</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="attempts">Attempts to find a solution or each pose</param>
        /// <returns>Returns an instance of <c>JointValues</c> with the joint configuration.</returns>
        public JointValues InverseKinematic(
            Pose pose,
            string endEffectorName,
            bool avoidCollision,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1
        )
        {
            var endEffector = CreateEndEffector(endEffectorName);
            return endEffector.InverseKinematic(pose, avoidCollision, jointPositionSeed, timeout, attempts);
        }

        /// <summary>
        /// Inverse kinematic solutions for several points
        /// </summary>
        /// <param name="points">Poses to be transformed to joint space</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and move group name</param>
        /// <param name="jointPositionSeed">Optional numerical seed to control joint configuration</param>
        /// <param name="endEffectorLink">Necessary if poses are defined for end effector link</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="attempts">Attempts to find a solution or each pose</param>
        /// <param name="constSeed">TODO: Definition</param>
        /// <returns>Returns the results as an instance of <c>IKResult</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when plan parameters joint set does not match joint seed.</exception>
        public IKResult InverseKinematicMany(
            IEnumerable<Pose> points,
            PlanParameters parameters,
            JointValues jointPositionSeed = null,
            string endEffectorLink = "",
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        )
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (jointPositionSeed != null)
            {
                if (!jointPositionSeed.JointSet.Equals(parameters.JointSet))
                {
                    if (!jointPositionSeed.JointSet.IsSimilar(parameters.JointSet))
                        throw new ArgumentException($"Plan parameters joint set does not match joint seed ({parameters.JointSet} != {jointPositionSeed.JointSet}).", nameof(jointPositionSeed));

                    jointPositionSeed = jointPositionSeed.Reorder(parameters.JointSet);
                }
            }

            var jointSet = parameters.JointSet;
            var srv = new xamlamoveit.GetIKSolution();

            var request = srv.req;

            request.group_name = parameters.MoveGroupName;
            request.joint_names = parameters.JointSet.ToArray();
            request.end_effector_link = endEffectorLink;
            request.seed = jointPositionSeed?.ToJointPathPointMessage();
            request.collision_check = parameters.CollisionCheck;
            request.attemts = attempts;
            request.timeout = (timeout ?? TimeSpan.FromSeconds(0.2)).ToDurationMessage();
            request.const_seed = constSeed;
            request.points = points.Select(pose => pose.ToPoseStampedMessage()).ToArray();

            if (!queryIKService.Call(srv))
                throw new ServiceCallFailedException(QUERY_IK_SERVICE_NAME);

            var response = srv.resp;

            var path = new JointPath(jointSet, response.solutions.Select(x =>
            {
                if (x == null || x.positions == null || x.positions.Length != jointSet.Count)
                    return JointValues.Zero(jointSet);
                return x.ToJointValues(jointSet);
            }));

            var errorCodes = response.error_codes.Select(x =>
            {
                if (x == null || x.val == 0)
                    return MoveItErrorCode.FAILURE;
                return (MoveItErrorCode)x.val;
            }).ToList();

            return new IKResult(path, errorCodes);
        }

        /// <summary>
        /// Inverse kinematic solutions for several points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="endEffectorName">Poses to be transformed to joint space</param>
        /// <param name="avoidCollision">Indicates if collision should be avoided</param>
        /// <param name="jointPositionSeed">Optional numerical seed to control joint configuration</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="attempts">Attempts to find a solution or each pose</param>
        /// <param name="constSeed">TODO: Definition</param>
        /// <returns>Returns the results as an instance of <c>IKResult</c>.</returns>
        public IKResult InverseKinematicMany(
            IEnumerable<Pose> poses,
            string endEffectorName,
            bool avoidCollision,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        )
        {
            var endEffector = this.CreateEndEffector(endEffectorName);
            return endEffector.InverseKinematicMany(poses, avoidCollision, jointPositionSeed, timeout, attempts, constSeed);
        }

        /// <summary>
        /// Plan a joint trajectory from start and end pose
        /// </summary>
        /// <param name="start">Starting pose</param>
        /// <param name="goal">Target pose</param>
        /// <param name="numSteps">TODO: Definition</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and move group name</param>
        /// <returns>Returns joint trajectory as an object implementing <c>IJointPath</c> which reaches defined poses of path under the constrains in parameters</returns>
        public IJointPath PlanCartesianPath(Pose start, Pose goal, int numSteps, PlanParameters parameters)
        {
            if (start == null)
                throw new ArgumentNullException(nameof(start));
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));
            if (numSteps < 0)
                throw new ArgumentOutOfRangeException(nameof(numSteps));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var seed = this.QueryJointStates(parameters.JointSet).Positions;
            var startJointValues = this.InverseKinematic(start, parameters, seed);
            var goalJointValues = this.InverseKinematic(goal, parameters, seed);
            return PlanCollisionFreeJointPath(startJointValues, goalJointValues, parameters);
        }

        /// <summary>
        /// Plan a joint trajectory from a cartesian path and plan parameters
        /// </summary>
        /// <param name="waypoints">Poses the planned trajectory must reach</param>
        /// <param name="numSteps">TODO: Definition</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and move group name</param>
        /// <returns>Returns joint trajectory as an object implementing <c>IJointPath</c> which reaches defined poses of path under the constrains in parameters</returns>
        public IJointPath PlanCartesianPath(ICartesianPath waypoints, int numSteps, PlanParameters parameters)
        {
            var seed = this.QueryJointStates(parameters.JointSet).Positions;
            var jointPath = InverseKinematicMany(waypoints, parameters, seed).Path;
            return PlanCollisionFreeJointPath(jointPath, parameters);
        }

        /// <summary>
        /// Sets and resets emergency stop
        /// </summary>
        /// <param name="enable">Activates emergency stop if true; resets emergency stop if false</param>
        /// <returns>Return a tuple of bool and string. The former value indicates success (true), the second is a respond/error message.</returns>
        public (bool, string) EmergencyStop(bool enable = true)
        {
            const string serviceName = "EmergencySTOP/query_emergency_stop";
            var service = nodeHandle.ServiceClient<Messages.std_srvs.SetBool>(serviceName);
            var srv = new Messages.std_srvs.SetBool();
            srv.req.data = enable;
            if (!service.Call(srv))
                throw new ServiceCallFailedException(serviceName);
            return (srv.resp.success, srv.resp.message);
        }

        /// <summary>
        /// Query the current joint positions of all joints
        /// </summary>
        /// <param name="joints">The joints for which the positions are requested</param>
        /// <returns>Returns a instance of JointValues with the positions of the requested joint set.</returns>
        public JointValues GetCurrentJointValues(JointSet joints)
        {
            return this.QueryJointStates(joints).Positions;
        }

        /// <summary>
        /// Moves joints to target joint positions
        /// </summary>
        /// <param name="target">Target joint positions</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and move group name</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an instance of <c>Task</c>.</returns>
        public async Task MoveJoints(JointValues target, PlanParameters parameters, CancellationToken cancel = default(CancellationToken))
        {
            var source = GetCurrentJointValues(parameters.JointSet);
            var path = new JointPath(source, target);
            var trajectory = PlanMoveJoints(path, parameters);
            await ExecuteJointTrajectory(trajectory, parameters.CollisionCheck, cancel);
        }

        /// <summary>
        /// Moves to target Pose asynchronously
        /// </summary>
        /// <param name="target">Target pose</param>
        /// <param name="endEffectorLink">Specifies for which link the pose is defined.</param>
        /// <param name="seed">Numerical seed to control configuration</param>
        /// <param name="parameters">Plan parameters which defines the limits, settings and move group name</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an instance of <c>Task</c>.</returns>
        public async Task MovePose(Pose target, string endEffectorLink, JointValues seed, PlanParameters parameters, CancellationToken cancel = default(CancellationToken))
        {
            if (seed == null)
            {
                var moveGroup = QueryAvailableMoveGroups().FirstOrDefault(x => x.Name == parameters.MoveGroupName);
                if (moveGroup == null)
                    throw new Exception($"MoveGroup '{parameters.MoveGroupName}' not available.");

                seed = GetCurrentJointValues(moveGroup.JointSet);
            }

            if (parameters.JointSet == null)
            {
                var builder = parameters.ToBuilder();
                builder.JointSet = seed.JointSet;
                parameters = builder.Build();
            }

            var targetJointValues = InverseKinematic(target, parameters, seed, endEffectorLink);
            await MoveJoints(targetJointValues, parameters, cancel);
        }

        /// <summary>
        /// Move to pose with linear movement asynchronously
        /// </summary>
        /// <param name="target">Target Pose</param>
        /// <param name="endEffectorLink">Specifies for which link the pose is defined</param>
        /// <param name="seed">Numerical seed to control configuration</param>
        /// <param name="parameters">An instance of TaskSpacePlanParameters which defines the limits, settings and end effector name</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an instance of <c>Task</c>.</returns>
        public async Task MovePoseLinear(Pose target, string endEffectorLink, JointValues seed, TaskSpacePlanParameters parameters, CancellationToken cancel = default(CancellationToken))
        {
            var moveGroup = QueryAvailableMoveGroups().FirstOrDefault(x => x.EndEffectorNames.Any(y => y == parameters.EndEffectorName));
            if (moveGroup == null)
                throw new Exception($"MoveGroup '{parameters.EndEffectorName}' not available.");

            if (seed == null)
            {
                seed = GetCurrentJointValues(moveGroup.JointSet);
            }

            var source = QueryPose(moveGroup.Name, seed, endEffectorLink);
            var path = new CartesianPath(source, target);
            var trajectory = PlanMovePoseLinear(path, seed, parameters);
            await ExecuteJointTrajectory(trajectory, parameters.CollisionCheck, cancel);
        }

        /// <summary>
        /// Moves a gripper via the general ros interface <c>GripperCommandAction</c>
        /// </summary>
        /// <param name="actionName">Name of the action to control a specific gripper</param>
        /// <param name="position">Requested position of the gripper [m]</param>
        /// <param name="maxEffort">Force which should be applied [N]</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an instance of <c>Task</c>, which produces the result as a value of type <c>MoveGripperResult</c> .</returns>
        public async Task<MoveGripperResult> MoveGripper(string actionName, double position, double maxEffort, CancellationToken cancel = default(CancellationToken))
        {
            var actionClient = GetActionClient<control_msg.GripperCommandGoal, control_msg.GripperCommandResult, control_msg.GripperCommandFeedback>(actionName);

            var g = actionClient.CreateGoal();
            g.command.position = position;
            g.command.max_effort = maxEffort;

            var result = await actionClient.SendGoalAsync(g, cancel);
            if (result == null)
                throw new Exception($"Unexpected null result received by ActionClient for {actionClient.Name}.");

            return new MoveGripperResult(position: result.position, reachedGoal: result.reached_goal, stalled: result.stalled);
        }


        /// <summary>
        /// Controls a WeissWsg gripper via the the specific action WsgCommandAction
        /// </summary>
        /// <param name="actionName">Name of the action to control a specific gripper</param>
        /// <param name="command">Specifies which kind of action should be performed</param>
        /// <param name="position">Requested position of the gripper [m]</param>
        /// <param name="speed">Requested speed [m/s]</param>
        /// <param name="maxEffort">Maximal force which should be applied [N]</param>
        /// <param name="stopOnBlock">If true stop if maximal force is applied</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        public async Task<WsgResult> WsgGripperCommand(
            string actionName,
            WsgCommand command,
            double width, double speed,
            double maxEffort,
            bool stopOnBlock = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            var actionClient = GetActionClient<wsg_50.CommandGoal, wsg_50.CommandResult, wsg_50.CommandFeedback>(actionName);
            var g = actionClient.CreateGoal();
            switch (command)
            {
                case WsgCommand.Stop:
                    g.command.command_id = wsg_50.Command.SOFT_STOP;
                    break;
                case WsgCommand.Move:
                    g.command.command_id = wsg_50.Command.MOVE;
                    break;
                case WsgCommand.Grasp:
                    g.command.command_id = wsg_50.Command.GRASP;
                    break;
                case WsgCommand.Release:
                    g.command.command_id = wsg_50.Command.RELEASE;
                    break;
                case WsgCommand.Homing:
                    g.command.command_id = wsg_50.Command.HOMING;
                    break;
                case WsgCommand.AcknowledgeError:
                    g.command.command_id = wsg_50.Command.ACKNOWLEDGE_ERROR;
                    break;
                default:
                    throw new ArgumentException($"Unsupported gripper command {command}.", nameof(command));
            }

            g.command.width = width;
            g.command.speed = speed;
            g.command.force = maxEffort;
            g.command.stop_on_block = stopOnBlock;

            var result = await actionClient.SendGoalAsync(g, cancel);
            if (result == null)
                throw new Exception($"Unexpected null result received by ActionClient for {actionClient.Name}.");

            return new WsgResult(result.status.grasping_state_id, result.status.width, result.status.current_force, result.status.grasping_state);
        }

        private IActionClient<TGoal, TResult, TFeedback> GetActionClient<TGoal, TResult, TFeedback>(string actionName)
            where TGoal : InnerActionMessage, new()
            where TResult : InnerActionMessage, new()
            where TFeedback : InnerActionMessage, new()
        {
            if (actionName == null)
                throw new ArgumentNullException(nameof(actionName));

            if (!ROS.OK)
                throw new RosException("ROS is not initialized.");

            lock (gate)
            {
                if (actionClientPool == null)
                    throw new ObjectDisposedException("MotionServer object has been disposed.");

                if (actionClientPool.TryGetValue(actionName, out IDisposable existingClientObject))
                {
                    if (existingClientObject is IActionClient<TGoal, TResult, TFeedback> client)
                        return client;
                }

                var ac = new ActionClient<TGoal, TResult, TFeedback>(actionName, this.NodeHandle);
                if (existingClientObject == null)
                {
                    actionClientPool.Add(actionName, ac);
                }

                return ac;
            }
        }
    }
}
