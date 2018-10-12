using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uml.Robotics.Ros;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public interface IMotionService
        : IDisposable
    {
        NodeHandle NodeHandle { get; }
        IMoveGroup CreateMoveGroup(string moveGroupName = null, string endEffectorName = null);
        IMoveGroup CreateMoveGroupForJointSet(JointSet jointSet);
        IList<MoveGroupDescription> QueryAvailableMoveGroups();
        IList<EndEffectorDescription> QueryAvailableEndEffectors();
        JointLimits QueryJointLimits(JointSet joints);
        EndEffectorLimits QueryEndEffectorLimits(string name);
        JointStates QueryJointStates(JointSet joints);
        Pose QueryPose(string moveGroupName, JointValues jointPositions, string endEffectorLink = "");
        IList<Pose> QueryPoseMany(string moveGroupName, IJointPath waypoints, string endEffectorLink = "");
        IJointPath QueryCollisionFreeJointPath(string moveGroupName, IJointPath waypoints);
        IJointTrajectory QueryJointTrajectory(
            IJointPath waypoints,
            double[] maxVelocity = null,
            double[] maxAcceleration = null,
            double maxDeviation = 0,
            double dt = 0.008
        );
        IList<JointValuesCollision> QueryJointPathCollisions(string moveGroupName, IJointPath points);
        TaskSpacePlanParameters CreateTaskSpacePlanParameters(string endEffectorName = null, double maxXYZVelocity = 0.01, double maxXYZAcceleration = 0.04, double maxAngularVelocity = 0.017453292519943, double maxAngularAcceleration = 0.069813170079773, double sampleResolution = 0.05, double ikJumpThreshold = 0.1, double maxDeviation = 0.0, bool checkCollision = true, double velocityScaling = 1);
        PlanParameters CreatePlanParameters(string moveGroupName = null, JointSet joints = null, double[] maxVelocity = null, double[] maxAcceleration = null, double sampleResolution = 0.05, bool checkCollision = true, double velocityScaling = 1);
        IJointPath PlanCollisionFreeJointPath(JointValues start, JointValues goal, PlanParameters parameters);
        IJointPath PlanCollisionFreeJointPath(IJointPath waypoints,  PlanParameters parameters);
        ICartesianPath PlanMoveCartesian(ICartesianPath path, int numSteps, PlanParameters parameters);
        IJointTrajectory PlanMovePoseLinear(ICartesianPath path, JointValues seed, TaskSpacePlanParameters parameters);
        IJointTrajectory PlanMoveJoints(IJointPath path, PlanParameters parameters);
        ISteppedMotionClient ExecuteJointTrajectorySupervised(IJointTrajectory trajectory, double velocityScaling = 1.0, bool checkCollision = true, CancellationToken cancel = default(CancellationToken));
        Task<int> ExecuteJointTrajectory(IJointTrajectory trajectory, bool checkCollision, CancellationToken cancel = default(CancellationToken));
        JointValues InverseKinematic(
            Pose pose,
            PlanParameters parameters,
            JointValues jointPositionSeed = null,
            string endEffectorLink = "",
            TimeSpan? timeout = null,
            int attempts = 1
        );
        JointValues InverseKinematic(
            Pose pose,
            string endEffectorName,
            bool avoidCollision = true,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1
        );
        IKResult InverseKinematicMany(
            IEnumerable<Pose> points,
            PlanParameters parameters,
            JointValues jointPositionSeed = null,
            string endEffectorLink = "",
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        );
        IKResult InverseKinematicMany(
            IEnumerable<Pose> points,
            string endEffectorName,
            bool avoidCollision,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        );

        IJointPath PlanCartesianPath(Pose start, Pose goal, int numSteps, PlanParameters parameters);
        IJointPath PlanCartesianPath(ICartesianPath waypoints, int numSteps, PlanParameters parameters);
        (bool, string) EmergencyStop(bool enable = true);

        JointValues GetCurrentJointValues(JointSet joints);
        Task MoveJoints(JointValues target, PlanParameters parameters, CancellationToken cancel = default(CancellationToken));
        Task MovePose(Pose target, string endEffectorLink, JointValues seed, PlanParameters parameters, CancellationToken cancel = default(CancellationToken));
        Task MovePoseLinear(Pose target, string endEffectorLink, JointValues seed, TaskSpacePlanParameters parameters, CancellationToken cancel = default(CancellationToken));

        Task<MoveGripperResult> MoveGripper(
            string actionName,
            double position,
            double maxEffort,
            CancellationToken cancel = default(CancellationToken)
        );
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
