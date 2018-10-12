using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public interface IEndEffector
    {
        string Name { get; }
        string LinkName { get; }
        IMoveGroup MoveGroup { get; }
        Pose CurrentPose { get; }
        Pose ComputePose(JointValues jointValues);
        JointValues InverseKinematic(
           Pose pose,
           bool avoidCollision,
           JointValues jointPositionSeed = null,
           TimeSpan? timeout = null,
           int attempts = 1
        );
        IKResult InverseKinematicMany(
            IEnumerable<Pose> poses,
            bool avoidCollision,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        );
        Task MovePoseAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken));
        Task MovePoseAsync(Pose target, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        Task MoveCartesianPathAsync(ICartesianPath waypoints, JointValues seed = null, CancellationToken cancel = default(CancellationToken));
        Task MoveCartesianPathAsync(ICartesianPath waypoints, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        Task MovePoseCollisionFreeAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MovePoseSupervisedAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MovePoseCollisionFreeSupervisedAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveCartesianPathSupervisedAsync(ICartesianPath waypoints, JointValues seed = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveCartesianPathSupervisedAsync(ICartesianPath waypoints, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinear(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null);
        Task MovePoseLinearAsync(Pose target, CancellationToken cancel = default(CancellationToken));
        Task MovePoseLinearAsync(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinearWaypoints(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null);
        (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinearWaypoints(ICartesianPath waypoints, TaskSpacePlanParameters  parameters);
        Task MoveCartesianPathLinearAsync(ICartesianPath waypoints, CancellationToken cancel = default(CancellationToken));
        Task MoveCartesianPathLinearAsync(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MovePoseLinearSupervisedAsync(Pose target, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MovePoseLinearSupervisedAsync(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveCartesianPathLinearSupervisedAsync(ICartesianPath waypoints, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveCartesianPathLinearSupervisedAsync(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
    }
}
