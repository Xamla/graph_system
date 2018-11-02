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
        /// Builds an instance of <c>TaskSpacePlanParameters</c>.
        /// </summary>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck"> If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <returns>Returns a new <c>TaskSpacePlanParameters</c> instance.</returns>
        TaskSpacePlanParameters BuildTaskSpacePlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, double? ikJumpThreshold = null);

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
        /// Creates a collision free move pose operation.
        /// </summary>
        /// <param name="target">The target pose</param>
        /// <param name="seed">Numerical seed to control joint configuration</param>
        /// <returns>Returns an operation object implementing <c>IMovePoseOperation</c>.</returns>
        IMovePoseOperation MovePoseCollisionFree(Pose target, JointValues seed = null);

        IMovePoseOperation MovePoseLinear(Pose target, JointValues seed = null);

        IMoveCartesianPathOperation MoveCartesianPathLinear(ICartesianPath waypoints);

        IMoveCartesianPathOperation MoveCartesianPath(ICartesianPath waypoints);
    }
}
