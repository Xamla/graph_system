using System;
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
        /// <returns></returns>
        JointStates QueryJointStates(JointSet joints);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveGroupName"></param>
        /// <param name="jointPositions"></param>
        /// <param name="endEffectorLink"></param>
        /// <returns></returns>
        Pose QueryPose(string moveGroupName, JointValues jointPositions, string endEffectorLink = "");
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveGroupName"></param>
        /// <param name="waypoints"></param>
        /// <param name="endEffectorLink"></param>
        /// <returns></returns>
        IList<Pose> QueryPoseMany(string moveGroupName, IJointPath waypoints, string endEffectorLink = "");
        IJointPath QueryCollisionFreeJointPath(string moveGroupName, IJointPath waypoints);
        IJointTrajectory QueryJointTrajectory(
            IJointPath waypoints,
            double[] maxVelocity = null,
            double[] maxAcceleration = null,
            double maxDeviation = 0,
            double dt = 0.008
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveGroupName"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        IList<JointValuesCollision> QueryJointPathCollisions(string moveGroupName, IJointPath points);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endEffectorName"></param>
        /// <param name="maxXYZVelocity"></param>
        /// <param name="maxXYZAcceleration"></param>
        /// <param name="maxAngularVelocity"></param>
        /// <param name="maxAngularAcceleration"></param>
        /// <param name="sampleResolution"></param>
        /// <param name="ikJumpThreshold"></param>
        /// <param name="maxDeviation"></param>
        /// <param name="checkCollision"></param>
        /// <param name="velocityScaling"></param>
        /// <returns></returns>
        TaskSpacePlanParameters CreateTaskSpacePlanParameters(string endEffectorName = null, double maxXYZVelocity = 0.01, double maxXYZAcceleration = 0.04, double maxAngularVelocity = 0.017453292519943, double maxAngularAcceleration = 0.069813170079773, double sampleResolution = 0.05, double ikJumpThreshold = 0.1, double maxDeviation = 0.0, bool checkCollision = true, double velocityScaling = 1);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveGroupName"></param>
        /// <param name="joints"></param>
        /// <param name="maxVelocity"></param>
        /// <param name="maxAcceleration"></param>
        /// <param name="sampleResolution"></param>
        /// <param name="checkCollision"></param>
        /// <param name="velocityScaling"></param>
        /// <returns></returns>
        PlanParameters CreatePlanParameters(string moveGroupName = null, JointSet joints = null, double[] maxVelocity = null, double[] maxAcceleration = null, double sampleResolution = 0.05, bool checkCollision = true, double velocityScaling = 1);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IJointPath PlanCollisionFreeJointPath(JointValues start, JointValues goal, PlanParameters parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waypoints"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
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
        /// <param name="endEffectorLink"></param>
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
        /// <param name="endEffectorLink"></param>
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
        /// <param name="waypoints"></param>
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
        /// <param name="endEffectorLink"></param>
        /// <param name="seed"></param>
        /// <param name="parameters"></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns></returns>
        Task MovePose(Pose target, string endEffectorLink, JointValues seed, PlanParameters parameters, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="endEffectorLink"></param>
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
