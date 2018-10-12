using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class EndEffector
        : IEndEffector
    {
        IMotionService motionService;
        IMoveGroup moveGroup;
        string name;
        string linkName;

        public EndEffector(IMoveGroup moveGroup, string endEffectorName, string endEffectorLinkName)
        {
            this.moveGroup = moveGroup;
            this.name = endEffectorName;
            this.linkName = endEffectorLinkName;
            this.motionService = moveGroup.MotionService;
        }

        public string Name => name;
        public string LinkName => linkName;
        public IMoveGroup MoveGroup => moveGroup;
        public Pose CurrentPose => ComputePose(this.moveGroup.CurrentJointPositions);

        public Pose ComputePose(JointValues jointValues)
        {
            if (jointValues == null)
                throw new ArgumentNullException(nameof(jointValues));
            return motionService.QueryPose(moveGroup.Name, jointValues, this.LinkName);
        }

        public JointValues InverseKinematic(
           Pose pose,
           bool avoidCollision,
           JointValues jointPositionSeed = null,
           TimeSpan? timeout = null,
           int attempts = 1
        )
        {
            var parameters = moveGroup.DefaultPlanParameters.WithCollisionCheck(avoidCollision);
            return motionService.InverseKinematic(pose, parameters, jointPositionSeed, linkName, timeout, attempts);
        }

        public IKResult InverseKinematicMany(
            IEnumerable<Pose> poses,
            bool avoidCollision,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        )
        {
            var parameters = moveGroup.DefaultPlanParameters.WithCollisionCheck(avoidCollision);
            return motionService.InverseKinematicMany(poses, parameters, jointPositionSeed, linkName, timeout, attempts, constSeed);
        }

        public async Task MovePoseAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
         => await MovePoseAsync(target, seed, null, null, null, cancel);

        public async Task MovePoseAsync(Pose target, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = moveGroup.BuildPlanParameters(velocityScaling, collisionCheck, null, accelerationScaling);
            await motionService.MovePose(target, this.LinkName, seed, parameters, cancel);
        }

        public async Task MovePoseCollisionFreeAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = moveGroup.BuildPlanParameters();
            var targetJoints = motionService.InverseKinematic(target, parameters, seed, this.LinkName);
            await this.moveGroup.MoveJointsCollisionFreeAsync(targetJoints, null, null, cancel);
        }

        public ISteppedMotionClient MovePoseCollisionFreeSupervisedAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = moveGroup.BuildPlanParameters();
            var targetJoints = motionService.InverseKinematic(target, parameters, seed, this.LinkName);
            return this.moveGroup.MoveJointsCollisionFreeSupervisedAsync(targetJoints, null, null, cancel);
        }

        public ISteppedMotionClient MovePoseSupervisedAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = moveGroup.BuildPlanParameters();
            var targetJoints = motionService.InverseKinematic(target, parameters, seed, this.LinkName);
            return this.moveGroup.MoveJointsSupervisedAsync(targetJoints, cancel);
        }

        public (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinear(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null)
        {
            var parameters = this.moveGroup.BuildTaskSpacePlanParameters(velocityScaling, collisionCheck, null, accelerationScaling, this.Name);

            // get current posture for seed jointvalues
            var seed = this.moveGroup.CurrentJointPositions;
            // get current pose
            var start = this.moveGroup.GetCurrentPose(parameters.EndEffectorName);

            // generate taskspace path
            var posePath = new CartesianPath(start, target);

            // plan trajectory
            var trajectory = motionService.PlanMovePoseLinear(posePath, seed, parameters);

            return (trajectory, parameters);
        }

        public Task MovePoseLinearAsync(Pose target, CancellationToken cancel = default(CancellationToken)) =>
        this.MovePoseLinearAsync(target, null, null, null, cancel);

        public async Task MovePoseLinearAsync(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            // plan trajectory
            var (trajectory, planParameters) = this.PlanMovePoseLinear(target, velocityScaling, collisionCheck, accelerationScaling);

            await motionService.ExecuteJointTrajectory(trajectory, planParameters.CollisionCheck, cancel);
        }
        public (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinearWaypoints(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null)
        {
            var parameters = this.moveGroup.BuildTaskSpacePlanParameters(velocityScaling, collisionCheck, maxDeviation, accelerationScaling, this.Name);
            return PlanMovePoseLinearWaypoints(waypoints, parameters);
        }

        public (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinearWaypoints(ICartesianPath waypoints, TaskSpacePlanParameters parameters)
        {
            // get current posture for seed jointvalues
            var seed = this.moveGroup.CurrentJointPositions;

            // get current pose
            var start = this.moveGroup.GetCurrentPose(parameters.EndEffectorName);

            // generate joint path
            var path = waypoints.Prepend(start);

            // plan trajectory
            var trajectory = motionService.PlanMovePoseLinear(path, seed, parameters);

            return (trajectory, parameters);
        }

        public async Task MoveCartesianPathAsync(ICartesianPath waypoints, JointValues seed = null, CancellationToken cancel = default(CancellationToken)) =>
            await this.MoveCartesianPathAsync(waypoints, seed, null, null, null, null, cancel);

        public async Task MoveCartesianPathAsync(ICartesianPath waypoints, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            if (waypoints.Count == 0)
                return;
            var parameters = moveGroup.BuildPlanParameters();
            var targetJoints = motionService.InverseKinematicMany(waypoints, parameters, seed, this.LinkName);

            if (!targetJoints.Suceeded)
                throw new Exception("IK solutions Failed.");

            var path = targetJoints.Path;
            // plan trajectory
            var (trajectory, planParameters) = moveGroup.PlanMoveJointPath(path, velocityScaling, collisionCheck, maxDeviation, accelerationScaling);

            await motionService.ExecuteJointTrajectory(trajectory, planParameters.CollisionCheck, cancel);
        }

        public ISteppedMotionClient MoveCartesianPathSupervisedAsync(ICartesianPath waypoints, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
            => MoveCartesianPathSupervisedAsync(waypoints, seed, null,  null, null, null, cancel);

        public ISteppedMotionClient MoveCartesianPathSupervisedAsync(ICartesianPath waypoints, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            if (waypoints.Count == 0)
                throw new ArgumentException(nameof(waypoints), "Path needs to contain at least a goal point.");

            var parameters = moveGroup.BuildPlanParameters();
            var targetJoints = motionService.InverseKinematicMany(waypoints, parameters, seed, this.LinkName);

            if (!targetJoints.Suceeded)
                throw new Exception("IK solutions Failed.");

            var path = targetJoints.Path;
            // plan trajectory
            var (trajectory, planParameters) = moveGroup.PlanMoveJointPath(path, velocityScaling, collisionCheck, maxDeviation, accelerationScaling);

            double scaling = 1.0;
            if (velocityScaling.HasValue)
                scaling = velocityScaling.Value;
            return motionService.ExecuteJointTrajectorySupervised(trajectory, scaling, planParameters.CollisionCheck, cancel);
        }

        public async Task MoveCartesianPathLinearAsync(ICartesianPath waypoints, CancellationToken cancel = default(CancellationToken)) =>
            await this.MoveCartesianPathLinearAsync(waypoints, null, null, null, null, cancel);

        public async Task MoveCartesianPathLinearAsync(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            if (waypoints.Count == 0)
                return;

            // plan trajectory
            var (trajectory, planParameters) = PlanMovePoseLinearWaypoints(waypoints, velocityScaling, collisionCheck, maxDeviation, accelerationScaling);

            await motionService.ExecuteJointTrajectory(trajectory, planParameters.CollisionCheck, cancel);
        }

        public ISteppedMotionClient MovePoseLinearSupervisedAsync(Pose target, CancellationToken cancel = default(CancellationToken))
            => MovePoseLinearSupervisedAsync(target, null, null, null, cancel);
        public ISteppedMotionClient MovePoseLinearSupervisedAsync(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            // plan trajectory
            var (trajectory, planParameters) = this.PlanMovePoseLinear(target, velocityScaling, collisionCheck, accelerationScaling);
            double scaling = 1.0;
            if (velocityScaling.HasValue)
                scaling = velocityScaling.Value;
            return motionService.ExecuteJointTrajectorySupervised(trajectory, scaling, planParameters.CollisionCheck, cancel);
        }

        public ISteppedMotionClient MoveCartesianPathLinearSupervisedAsync(ICartesianPath waypoints, CancellationToken cancel = default(CancellationToken)) =>
            this.MoveCartesianPathLinearSupervisedAsync(waypoints, null, null, null, null, cancel);

        public ISteppedMotionClient MoveCartesianPathLinearSupervisedAsync(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            if (waypoints.Count == 0)
                throw new ArgumentException(nameof(waypoints), "Path needs to contain at least a goal point.");

            // plan trajectory
            var (trajectory, planParameters) = PlanMovePoseLinearWaypoints(waypoints, velocityScaling, collisionCheck, maxDeviation, accelerationScaling);

            double scaling = 1.0;
            if (velocityScaling.HasValue)
                scaling = velocityScaling.Value;
            return motionService.ExecuteJointTrajectorySupervised(trajectory, scaling, planParameters.CollisionCheck, cancel);
        }
    }
}
