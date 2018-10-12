﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uml.Robotics.Ros;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Implementations of <c>IMotionService</c> expose a range of functionality to be used by TODO:
    /// </summary>  
    public interface IMotionService
        : IDisposable
    {
        /// <summary>
        /// Handle of a ROS node
        /// </summary>  
        NodeHandle NodeHandle { get; }
     

        /// <summary>
        /// Creates a move group.
        /// </summary>
        /// <param name="moveGroupName">Optional name for the move group</param>
        /// <param name="endEffectorName">Optional name for the end effector</param>
        /// <returns>A reference to an object implementing <c>IMoveGroup</c></returns>
        IMoveGroup CreateMoveGroup(string moveGroupName = null, string endEffectorName = null);
      
        /// <summary>
        /// Creates a move group based on a <c>JointSet</c> object.
        /// </summary>
        /// <param name="jointSet">Joint configurations</param>
        /// <returns>A reference to an object implementing <c>IMoveGroup</c></returns>
        IMoveGroup CreateMoveGroupForJointSet(JointSet jointSet);
 
        /// <summary>
        /// Query all currently available move groups.
        /// </summary>
        /// <returns>Returns an object implementing IList, which contains instances of <c>MoveGroupDescription</c> of all the available move groups.</returns>
        IList<MoveGroupDescription> QueryAvailableMoveGroups();
      
        /// <summary>
        /// Query all currently available end effectors
        /// </summary>
        /// <returns>Returns an object implementing IList, which contains instances of <c>EndEffectorDescription</c> of all the available end effectors.</returns>
        IList<EndEffectorDescription> QueryAvailableEndEffectors();
      
        /// <summary>
        /// Query end joint limits
        /// </summary>
        /// <param name="joints">Set of joint for which the limits are queried</param>
        /// <returns>Returns an instance of <c>JointLimits</c>.</returns>
        JointLimits QueryJointLimits(JointSet joints);

        /// <summary>
        /// Query end effector limits
        /// </summary>
        /// <param name="name">Name of the end effector for which the limits are queried</param>
        /// <returns>Return instance of <c>EndEffectorLimits</c>.</returns>
        EndEffectorLimits QueryEndEffectorLimits(string name);

        /// <summary>
        /// Query joint states 
        /// </summary>
        /// <param name="joints">Set of joint for which the limits are queried</param>
        /// <returns>Returns a instance of <c>JointStates</c> which contains the joint states of all joints defined in <c>joints</c>.</returns>
        JointStates QueryJointStates(JointSet joints);

        /// <summary>
        /// Computes the pose by applying forward kinematics
        /// </summary>
        /// <param name="moveGroupName">Name of the move group from which the pose is queried</param>
        /// <param name="jointPositions">Joint values from which the pose is calculated</param>
        /// <param name="endEffectorLink">End effector link is necessary if end effector is not part of the move group but pose should be computed for the end effector.</param>
        /// <returns>Returns the computed <c>Pose</c> object.</returns>
        Pose QueryPose(string moveGroupName, JointValues jointPositions, string endEffectorLink = "");
        
        /// <summary>
        /// Query the poses from joint path points by applying forward kinematics
        /// </summary>
        /// <param name="moveGroupName">Name of the move group from which the pose is queried</param>
        /// <param name="waypoints">Joint path from which the poses are calculated</param>
        /// <param name="endEffectorLink">End effector link is necessary if end effector is not part of the move group but pose should be computed for the end effector.</param>
        /// <returns>Returns an object which implements <c>IList</c> and contains the computed poses.</returns>
        IList<Pose> QueryPoseMany(string moveGroupName, IJointPath waypoints, string endEffectorLink = "");


        /// <summary>
        /// Query a collision free joint path from defined joint path
        /// </summary>
        /// <param name="moveGroupName">Name of the move group for which the collision free joint is queried</param>
        /// <param name="waypoints">Joint path which may contain collisions</param>
        /// <returns>New reference to object implementing <c>IJointPath</c>, which is now collision free.</returns>
        IJointPath QueryCollisionFreeJointPath(string moveGroupName, IJointPath waypoints);

        /// <summary>
        /// Query a joint trajectory from joint path
        /// </summary>
        /// <param name="waypoints">Defines the key joint positions the trajectory must reach</param>
        /// <param name="maxVelocity">Defines the maximal velocity for every joint</param>
        /// <param name="maxAcceleration">Defines the maximal acceleration for every joint</param>
        /// <param name="maxDeviation"> Defines the maximal deviation of the joints to the defined key points while executing the trajectory</param>
        /// <param name="dt">Sampling points frequency or time if value is create as 1.0 the value is interpreted as a value in seconds else the value is interpreted as a value in Hz. TODO: Verify this</param>
        /// <returns>An object which implements <c>IJointTrajectory</c>.</returns>
        IJointTrajectory QueryJointTrajectory(
            IJointPath waypoints,
            double[] maxVelocity = null,
            double[] maxAcceleration = null,
            double maxDeviation = 0,
            double dt = 0.008
        );

        /// <summary>
        /// Query collisions in joint path
        /// </summary>
        /// <param name="moveGroupName">Name of the move group for which a path should be checked for collisions</param>
        /// <param name="points">The path that should be check for collisions</param>
        /// <returns>Returns an object which implements <c>IList</c> and contains all the instances of <c>JointValuesCollision</c> found.</returns>
        IList<JointValuesCollision> QueryJointPathCollisions(string moveGroupName, IJointPath points);

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
        TaskSpacePlanParameters CreateTaskSpacePlanParameters(string endEffectorName = null, double maxXYZVelocity = 0.01, double maxXYZAcceleration = 0.04, double maxAngularVelocity = 0.017453292519943, double maxAngularAcceleration = 0.069813170079773, double sampleResolution = 0.05, double ikJumpThreshold = 0.1, double maxDeviation = 0.0, bool checkCollision = true, double velocityScaling = 1);
        
        /// <summary>
        /// Create <c>PlanParameters</c> from user defined and/or quried inputs. TODO: Verify this
        /// </summary>
        /// <param name="moveGroupName">Name of the move group for which plan parameters should be created</param>
        /// <param name="joints"><c>JointSet</c> instance for wjich plan parameters should be created</param>
        /// <param name="maxVelocity">Defines the maximal velocity for every joint</param>
        /// <param name="maxAcceleration">Defines the maximal acceleration for every joint</param>
        /// <param name="sampleResolution">Sample points frequency</param>
        /// <param name="checkCollision">Check for collision if True</param>
        /// <param name="velocityScaling">Scale query or user defined max acceleration. Values between 0.0 and 1.0.</param>
        /// <returns>Instance of <c>PlanParameters</c> with automatically queried and/or user defined values.</returns>
        PlanParameters CreatePlanParameters(string moveGroupName = null, JointSet joints = null, double[] maxVelocity = null, double[] maxAcceleration = null, double sampleResolution = 0.05, bool checkCollision = true, double velocityScaling = 1);
        
        /// <summary>
        /// Plans a collision free joint path by querying it
        /// </summary>
        /// <param name="start">Starting joint configurations</param>
        /// <param name="goal">Target joint configuration</param>
        /// <param name="parameters">Plan parameters which defines the limits and move group.</param>
        /// <returns>Returns An object implementing <c>IJointPath</c> defining a collision free joint path.</returns>
        IJointPath PlanCollisionFreeJointPath(JointValues start, JointValues goal, PlanParameters parameters);

        /// <summary>
        /// Plans a collision free joint path by querying it
        /// </summary>
        /// <param name="waypoints">Joint path which should be replanned to be collision free</param>
        /// <param name="parameters">Defines the limits and move group</param>
        /// <returns>Returns An object implementing <c>IJointPath</c> defining a replanned collision free joint path.</returns>
        IJointPath PlanCollisionFreeJointPath(IJointPath waypoints,  PlanParameters parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="numSteps"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ICartesianPath PlanMoveCartesian(ICartesianPath path, int numSteps, PlanParameters parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="seed"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IJointTrajectory PlanMovePoseLinear(ICartesianPath path, JointValues seed, TaskSpacePlanParameters parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IJointTrajectory PlanMoveJoints(IJointPath path, PlanParameters parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trajectory"></param>
        /// <param name="velocityScaling"></param>
        /// <param name="checkCollision"></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        ISteppedMotionClient ExecuteJointTrajectorySupervised(IJointTrajectory trajectory, double velocityScaling = 1.0, bool checkCollision = true, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trajectory"></param>
        /// <param name="checkCollision"></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        Task<int> ExecuteJointTrajectory(IJointTrajectory trajectory, bool checkCollision, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="parameters"></param>
        /// <param name="jointPositionSeed"></param>
        /// <param name="endEffectorLink">End effector link is necessary if end effector is not part of the move group but pose should be computed for the end effector.</param>
        /// <param name="timeout"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        JointValues InverseKinematic(
            Pose pose,
            PlanParameters parameters,
            JointValues jointPositionSeed = null,
            string endEffectorLink = "",
            TimeSpan? timeout = null,
            int attempts = 1
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="endEffectorName"></param>
        /// <param name="avoidCollision"></param>
        /// <param name="jointPositionSeed"></param>
        /// <param name="timeout"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        JointValues InverseKinematic(
            Pose pose,
            string endEffectorName,
            bool avoidCollision = true,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="parameters"></param>
        /// <param name="jointPositionSeed"></param>
        /// <param name="endEffectorLink">End effector link is necessary if end effector is not part of the move group but pose should be computed for the end effector.</param>
        /// <param name="timeout"></param>
        /// <param name="attempts"></param>
        /// <param name="constSeed"></param>
        /// <returns></returns>
        IKResult InverseKinematicMany(
            IEnumerable<Pose> points,
            PlanParameters parameters,
            JointValues jointPositionSeed = null,
            string endEffectorLink = "",
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="endEffectorName"></param>
        /// <param name="avoidCollision"></param>
        /// <param name="jointPositionSeed"></param>
        /// <param name="timeout"></param>
        /// <param name="attempts"></param>
        /// <param name="constSeed"></param>
        /// <returns></returns>
        IKResult InverseKinematicMany(
            IEnumerable<Pose> points,
            string endEffectorName,
            bool avoidCollision,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <param name="numSteps"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IJointPath PlanCartesianPath(Pose start, Pose goal, int numSteps, PlanParameters parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waypoints">Joint path from which the poses are calculated</param>
        /// <param name="numSteps"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IJointPath PlanCartesianPath(ICartesianPath waypoints, int numSteps, PlanParameters parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        (bool, string) EmergencyStop(bool enable = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joints"></param>
        /// <returns></returns>
        JointValues GetCurrentJointValues(JointSet joints);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parameters"></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        Task MoveJoints(JointValues target, PlanParameters parameters, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="endEffectorLink">End effector link is necessary if end effector is not part of the move group but pose should be computed for the end effector.</param>
        /// <param name="seed"></param>
        /// <param name="parameters"></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        Task MovePose(Pose target, string endEffectorLink, JointValues seed, PlanParameters parameters, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="endEffectorLink">End effector link is necessary if end effector is not part of the move group but pose should be computed for the end effector.</param>
        /// <param name="seed"></param>
        /// <param name="parameters"></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        Task MovePoseLinear(Pose target, string endEffectorLink, JointValues seed, TaskSpacePlanParameters parameters, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="position"></param>
        /// <param name="maxEffort"></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        Task<MoveGripperResult> MoveGripper(
            string actionName,
            double position,
            double maxEffort,
            CancellationToken cancel = default(CancellationToken)
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="command"></param>
        /// <param name="position"></param>
        /// <param name="speed"></param>
        /// <param name="maxEffort"></param>
        /// <param name="stopOnBlock"></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        Task<WsgResult> WsgGripperCommand(
            string actionName,
            WsgCommand command,
            double position,
            double speed,
            double maxEffort,
            bool stopOnBlock = true,
            CancellationToken cancel = default(CancellationToken)
        );
    }
}
