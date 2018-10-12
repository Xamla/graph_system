using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public interface IMoveGroup : IDisposable
    {
        string Name { get; }
        IMotionService MotionService { get; }
        double VelocityScaling { get; set; }
        bool CollisionCheck { get; set; }
        double SampleResolution { get; set; }
        double MaxDeviation { get; set; }
        double IkJumpThreshold { get; set; }
        IEnumerable<string> EndEffectorNames { get; }
        string DefaultEndEffectorName { get; set; }
        IEndEffector GetEndEffector(string endEffectorName = null);
        IEndEffector DefaultEndEffector { get; }
        Pose GetCurrentPose(string endEffectorName = null);
        JointSet JointSet { get; }
        JointStates CurrentJointStates { get; }
        JointValues CurrentJointPositions { get; }
        PlanParameters DefaultPlanParameters { get; }
        PlanParameters BuildPlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null);
        TaskSpacePlanParameters BuildTaskSpacePlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, string endEffectorName = null);
        (IJointTrajectory, PlanParameters) PlanMoveJointsCollisionFreeAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken));
        (IJointTrajectory, PlanParameters) PlanMoveJoints(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null);
        Task MoveJointsAsync(JointValues target, CancellationToken cancel = default(CancellationToken));
        Task MoveJointsAsync(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        Task MoveJointsCollisionFreeAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken));
        (IJointTrajectory, PlanParameters) PlanMoveJointPath(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null);
        Task MoveJointPathAsync(IJointPath waypoints, CancellationToken cancel = default(CancellationToken));
        Task MoveJointPathAsync(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveJointPathSupervisedAsync(IJointPath waypoints, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveJointPathSupervisedAsync(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveJointsSupervisedAsync(JointValues target, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveJointsSupervisedAsync(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveJointsCollisionFreeSupervisedAsync(JointValues target, CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient MoveJointsCollisionFreeSupervisedAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken));
    }
}
