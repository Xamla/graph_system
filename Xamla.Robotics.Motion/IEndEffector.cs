using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Implementations of <c>IEndEffector</c> encapsulate move functionality for a specific end effector.
    /// </summary>
    public interface IEndEffector
    {
        /// <summary>
        /// The name of the end effector
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The link name of the end effector
        /// </summary>
        string LinkName { get; }

        /// <summary>
        /// Instance of an object implementing <c>IMoveGroup</c>, to which the end effector belongs to
        /// </summary>
        IMoveGroup MoveGroup { get; }

        /// <summary>
        /// Current <c>Pose</c> instance of the end effector
        /// </summary>
        Pose CurrentPose { get; }

        /// <summary>
        /// Computes the pose based on an instance of <c>JointValues</c>.
        /// </summary>
        /// <param name="jointValues">Joint configuration of the robot</param>
        /// <returns>Returns a <c>Pose</c> instance.</returns>
        Pose ComputePose(JointValues jointValues);

        /// <summary>
        /// Get inverse kinematic solution for one pose
        /// </summary>
        /// <param name="pose">The pose to be transformmed to joint space</param>
        /// <param name="avoidCollision">If true the trajectory planing tries to plan collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="jointPositionSeed">Optional seed to joint configuration</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="attempts">The amount of attempts</param>
        /// <returns>Returns the joint configuration as a <c>JointValues</c> instance.</returns>
        JointValues InverseKinematic(
           Pose pose,
           bool avoidCollision,
           JointValues jointPositionSeed = null,
           TimeSpan? timeout = null,
           int attempts = 1
        );

        /// <summary>
        /// Get inverse kinematic solutions for several poses
        /// </summary>
        /// <param name="poses">The poses to be transformmed to joint space</param>
        /// <param name="avoidCollision">If true the trajectory planing tries to plan collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="jointPositionSeed">Optional seed joint configuration</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="attempts">The amount of attempts</param>
        /// <param name="constSeed">Determines if for each pose in poses the same seed should be used.</param>
        /// <returns>Returns the results as an instance of <c>IKResult</c>.</returns>
        IKResult InverseKinematicMany(
            IEnumerable<Pose> poses,
            bool avoidCollision,
            JointValues jointPositionSeed = null,
            TimeSpan? timeout = null,
            int attempts = 1,
            bool constSeed = false
        );

        /// <summary>
        /// Move to pose asynchronously.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task MovePoseAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken));

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
        Task MovePoseAsync(Pose target, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task MoveCartesianPathAsync(ICartesianPath waypoints, JointValues seed = null, CancellationToken cancel = default(CancellationToken));

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
        Task MoveCartesianPathAsync(ICartesianPath waypoints, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move to pose asynchronously and collision free
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task MovePoseCollisionFreeAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move to pose asynchronously
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        ISteppedMotionClient MovePoseSupervisedAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move to pose asynchronously and collision free
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        ISteppedMotionClient MovePoseCollisionFreeSupervisedAsync(Pose target, JointValues seed = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        ISteppedMotionClient MoveCartesianPathSupervisedAsync(ICartesianPath waypoints, JointValues seed = null, CancellationToken cancel = default(CancellationToken));

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
        ISteppedMotionClient MoveCartesianPathSupervisedAsync(ICartesianPath waypoints, JointValues seed = null, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Plans a trajectory with linear movements based on a pose
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>
        (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinear(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null);

        /// <summary>
        /// Move to pose with linear movement asynchronously
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task MovePoseLinearAsync(Pose target, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move to pose with linear movement asynchronously
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task MovePoseLinearAsync(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Plans a trajectory with linear movements based on waypoints
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>
        (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinearWaypoints(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null);


        /// <summary>
        /// Plans a trajectory with linear movements based on waypoints and task space plan parameters.
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="parameters">An instance of <c>TaskSpacePlanParameters</c></param>
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>
        (IJointTrajectory, TaskSpacePlanParameters) PlanMovePoseLinearWaypoints(ICartesianPath waypoints, TaskSpacePlanParameters parameters);

        /// <summary>
         /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task MoveCartesianPathLinearAsync(ICartesianPath waypoints, CancellationToken cancel = default(CancellationToken));

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
        Task MoveCartesianPathLinearAsync(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move to pose with linear movement asynchronously
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        ISteppedMotionClient MovePoseLinearSupervisedAsync(Pose target, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move to pose with linear movement asynchronously
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        ISteppedMotionClient MovePoseLinearSupervisedAsync(Pose target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));

        /// <summary>
        /// Move asynchronously using cartesian path
        /// </summary>
        /// <param name="waypoints">The cartesian path as an object implementing <c>ICartesianPath</c></param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>Returns an object implementing <c>ISteppedMotionClient</c>.</returns>
        ISteppedMotionClient MoveCartesianPathLinearSupervisedAsync(ICartesianPath waypoints, CancellationToken cancel = default(CancellationToken));

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
        ISteppedMotionClient MoveCartesianPathLinearSupervisedAsync(ICartesianPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
    }
}
