using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Class with encapsulate move functionality for a specific end effector
    /// </summary>
    public class EndEffector
        : IEndEffector
    {
        IMotionService motionService;
        IMoveGroup moveGroup;
        string name;
        string linkName;

        /// <summary>
        /// Creates an <c>EndEffector</c> instance
        /// </summary>
        /// <param name="moveGroup">Instance of move group where the endeffector belongs to</param>
        /// <param name="endEffectorName">Name of the end effector</param>
        /// <param name="endEffectorLinkName">Name of the end effector link</param>
        public EndEffector(IMoveGroup moveGroup, string endEffectorName, string endEffectorLinkName)
        {
            this.moveGroup = moveGroup;
            this.name = endEffectorName;
            this.linkName = endEffectorLinkName;
            this.motionService = moveGroup.MotionService;
        }

        /// <summary>
        /// The name of the end effector
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The link name of the end effector
        /// </summary>
        public string LinkName => linkName;

        /// <summary>
        /// Instance of an object implementing <c>IMoveGroup</c>, to which the end effector belongs to
        /// </summary>
        public IMoveGroup MoveGroup => moveGroup;

        /// <summary>
        /// Current <c>Pose</c> instance of the end effector
        /// </summary>
        public Pose CurrentPose => ComputePose(this.moveGroup.CurrentJointPositions);

        /// <summary>
        /// Computes the pose based on an instance of <c>JointValues</c>.
        /// </summary>
        /// <param name="jointValues">Joint configuration of the robot</param>
        /// <returns>Returns a <c>Pose</c> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jointValues"/> is null.</exception>
        public Pose ComputePose(JointValues jointValues)
        {
            if (jointValues == null)
                throw new ArgumentNullException(nameof(jointValues));
            return motionService.QueryPose(moveGroup.Name, jointValues, this.LinkName);
        }

        /// <summary>
        /// Get inverse kinematic solution for one pose
        /// </summary>
        /// <param name="pose">The pose to be transformmed to joint space</param>
        /// <param name="avoidCollision">If true the trajectory planing tries to plan collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="jointPositionSeed">Optional numerical seed to control joint configuration</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="attempts">Attempts</param>
        /// <returns>Returns the joint configuration as a <c>JointValues</c> instance.</returns>
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

        /// <summary>
        /// Get inverse kinematic solutions for several poses
        /// </summary>
        /// <param name="poses">The poses to be transformmed to joint space</param>
        /// <param name="avoidCollision">If true the trajectory planing tries to plan collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="jointPositionSeed">Optional numerical seed to control joint configuration</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="attempts">The amount of attempts</param>
        /// <param name="constSeed">Determines if for each pose in poses the same seed should be used.</param>
        /// <returns>Returns the results as an instance of <c>IKResult</c>.</returns>
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

        /// <summary>
        /// Move to pose asynchronously.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        public async Task MovePoseAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
         => await MovePoseAsync(target, seed, null, null, null, cancel);

        /// <summary>
        /// Move to pose asynchronously.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        public async Task MovePoseAsync(Pose target, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = moveGroup.BuildPlanParameters(velocityScaling, collisionCheck, null, accelerationScaling);
            await motionService.MovePose(target, this.LinkName, seed, parameters, cancel);
        }

        /// <summary>
        /// Move to pose asynchronously and collision free.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        public async Task MovePoseCollisionFreeAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = moveGroup.BuildPlanParameters();
            var targetJoints = motionService.InverseKinematic(target, parameters, seed, this.LinkName);
            await this.moveGroup.MoveJointsCollisionFreeAsync(targetJoints, null, null, cancel);
        }

        /// <summary>
        /// Move to pose asynchronously and collision free.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        public ISteppedMotionClient MovePoseCollisionFreeSupervisedAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = moveGroup.BuildPlanParameters();
            var targetJoints = motionService.InverseKinematic(target, parameters, seed, this.LinkName);
            return this.moveGroup.MoveJointsCollisionFreeSupervisedAsync(targetJoints, null, null, cancel);
        }

        /// <summary>
        /// Move to pose asynchronously.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        public ISteppedMotionClient MovePoseSupervisedAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = moveGroup.BuildPlanParameters();
            var targetJoints = motionService.InverseKinematic(target, parameters, seed, this.LinkName);
            return this.moveGroup.MoveJointsSupervisedAsync(targetJoints, cancel);
        }

        /// <summary>
        /// Plans a trajectory with linear movements based on a pose.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>
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

        /// <summary>
        /// Move to pose with linear movement asynchronously.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        public Task MovePoseLinearAsync(Pose target, CancellationToken cancel = default(CancellationToken)) =>
            this.MovePoseLinearAsync(target, null, null, null, cancel);

        /// <summary>
        /// Move to pose with linear movement asynchronously
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        public async Task MovePoseLinearAsync(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            // plan trajectory
            var (trajectory, planParameters) = this.PlanMovePoseLinear(target, velocityScaling, collisionCheck, accelerationScaling);

            await motionService.ExecuteJointTrajectory(trajectory, planParameters.CollisionCheck, cancel);
        }

        /// <summary>
        /// Plans a trajectory with linear movements based on waypoints.
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>
        public (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinearWaypoints(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null)
        {
            var parameters = this.moveGroup.BuildTaskSpacePlanParameters(velocityScaling, collisionCheck, maxDeviation, accelerationScaling, this.Name);
            return PlanMovePoseLinearWaypoints(waypoints, parameters);
        }

        /// <summary>
        /// Plans a trajectory with linear movements based on waypoints and task space plan parameters.
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="parameters">An instance of <c>TaskSpacePlanParameters</c></param>
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>
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

        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        public async Task MoveCartesianPathAsync(ICartesianPath waypoints, JointValues seed = null, CancellationToken cancel = default(CancellationToken)) =>
            await this.MoveCartesianPathAsync(waypoints, seed, null, null, null, null, cancel);

        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="maxDeviation">Defines the maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        /// <exception cref="Exception">Thrown when IK solutions failed.</exception>
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

        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        public ISteppedMotionClient MoveCartesianPathSupervisedAsync(ICartesianPath waypoints, JointValues seed = null, CancellationToken cancel = default(CancellationToken))
            => MoveCartesianPathSupervisedAsync(waypoints, seed, null,  null, null, null, cancel);

        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when there are no waypoints in <paramref name="waypoints"/>.</exception>
        /// <exception cref="Exception">Thrown when IK solutions failed.</exception>
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

        /// <summary>
         /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        public async Task MoveCartesianPathLinearAsync(ICartesianPath waypoints, CancellationToken cancel = default(CancellationToken)) =>
            await this.MoveCartesianPathLinearAsync(waypoints, null, null, null, null, cancel);


        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        public async Task MoveCartesianPathLinearAsync(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            if (waypoints.Count == 0)
                return;

            // plan trajectory
            var (trajectory, planParameters) = PlanMovePoseLinearWaypoints(waypoints, velocityScaling, collisionCheck, maxDeviation, accelerationScaling);

            await motionService.ExecuteJointTrajectory(trajectory, planParameters.CollisionCheck, cancel);
        }

        /// <summary>
        /// Move to pose with linear movement asynchronously
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        public ISteppedMotionClient MovePoseLinearSupervisedAsync(Pose target, CancellationToken cancel = default(CancellationToken))
            => MovePoseLinearSupervisedAsync(target, null, null, null, cancel);

        /// <summary>
        /// Move to pose with linear movement asynchronously
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        public ISteppedMotionClient MovePoseLinearSupervisedAsync(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            // plan trajectory
            var (trajectory, planParameters) = this.PlanMovePoseLinear(target, velocityScaling, collisionCheck, accelerationScaling);
            double scaling = 1.0;
            if (velocityScaling.HasValue)
                scaling = velocityScaling.Value;
            return motionService.ExecuteJointTrajectorySupervised(trajectory, scaling, planParameters.CollisionCheck, cancel);
        }

        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        public ISteppedMotionClient MoveCartesianPathLinearSupervisedAsync(ICartesianPath waypoints, CancellationToken cancel = default(CancellationToken)) =>
            this.MoveCartesianPathLinearSupervisedAsync(waypoints, null, null, null, null, cancel);

        /// <summary>
        /// Move with linear movement asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        /// <exception cref="ArgumentException">Thrown when there is no goal point in <paramref name="waypoints"/>.</exception>
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
