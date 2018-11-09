using System;
using System.Collections.Generic;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Implementations of <c>IMoveGroup</c> encapsulate move functionality.
    /// </summary>
    public interface IMoveGroup : IDisposable
    {
        /// <summary>
        /// Name of the <c>IMoveGroup</c>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Reference to an object implementing <c>IMotionService</c>
        /// Used to communicate with the motion server
        /// </summary>
        IMotionService MotionService { get; }

        /// <summary>
        /// Current velocity scaling
        /// </summary>
        double VelocityScaling { get; set; }

        /// <summary>
        /// Sets the collision check
        /// If true, the trajectory planing tries to plan a collision free trajectory.
        /// Before executing a trajectory, a collision check is performed.
        /// </summary>
        bool CollisionCheck { get; set; }

        /// <summary>
        /// Trajectory point sampling frequency
        /// </summary>
        double SampleResolution { get; set; }

        /// <summary>
        /// Maximal deviation from the trajectory points
        /// </summary>
        double MaxDeviation { get; set; }

        /// <summary>
        /// Maximal allowed inverse kinematics jump threshold
        /// </summary>
        double IkJumpThreshold { get; set; }

        /// <summary>
        /// Object implementing <c>IEnumerable<string></c> containing endeffector names of the the move group
        /// </summary>
        IEnumerable<string> EndEffectorNames { get; }

        /// <summary>
        /// The default end effector name
        /// </summary>
        string DefaultEndEffectorName { get; set; }

        /// <summary>
        /// Get an end effector by name
        /// </summary>
        /// <param name="endEffectorName">Name of the end effector. The default end effector is used when null.</param>
        /// <returns>Returns an object implementing <c>IEndEffector</c> associated with  <paramref name="endEffectorName"/>.</returns>
        IEndEffector GetEndEffector(string endEffectorName = null);

        /// <summary>
        /// Get default end effector
        /// </summary>
        IEndEffector DefaultEndEffector { get; }

        /// <summary>
        /// Get the current position of an end effector
        /// </summary>
        /// <param name="endEffectorName">Name of the <c>IEndEffector</c> instance. The default end effector is used when null.</param>
        /// <returns>Returns the current <c>Pose</c> instance of the end effector associated with <paramref name="endEffectorName"/>.</returns>
        Pose GetCurrentPose(string endEffectorName = null);

        /// <summary>
        /// <c>JointSet</c> of the <c>IMoveGroup</c>
        /// </summary>
        JointSet JointSet { get; }

        /// <summary>
        /// <c>JointLimits</c> of the <c>IMoveGroup</c>
        /// </summary>
        JointLimits JointLimits { get; }

        /// <summary>
        /// Current <c>JointStates</c> of the of the <c>IMoveGroup</c> joints
        /// </summary>
        JointStates CurrentJointStates { get; }

        /// <summary>
        /// Current joint positions of the of the <c>IMoveGroup</c> joints
        /// </summary>
        JointValues CurrentJointPositions { get; }

        /// <summary>
        /// Default instance of <c>PlanParameters</c>
        /// </summary>
        PlanParameters DefaultPlanParameters { get; }

        /// <summary>
        /// Builds an instance of <c>PlanParameters</c>.
        /// For parameter == null, the default values in <c>DefaultPlanParameters</c> are used.
        /// </summary>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck">Activates/Deactivates collision check</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <returns>Returns a new <c>PlanParameters</c> instance based on <c>DefaultPlanParameters</c> and parameters.</returns>
        PlanParameters BuildPlanParameters(
            double? velocityScaling = null,
            bool? collisionCheck = null,
            double? maxDeviation = null,
            double? accelerationScaling = null,
            double? sampleResolution = null
        );

        /// <summary>
        /// Builds an instance of <c>TaskSpacePlanParameters</c>.
        /// </summary>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck"> If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="endEffectorName">Name of the end effector</param>
        /// <returns>Returns a new <c>TaskSpacePlanParameters</c> instance.</returns>
        TaskSpacePlanParameters BuildTaskSpacePlanParameters(
            double? velocityScaling = null,
            bool? collisionCheck = null,
            double? maxDeviation = null,
            double? accelerationScaling = null,
            double? ikJumpThreshold = null,
            string endEffectorName = null
        );

        /// <summary>
        /// Creates a move joints operation object that parameterizes a move to the provided <c>JointValues</c> target.
        /// </summary>
        /// <param name="target">Target joint positions</param>
        /// <returns>Returns an operation object implementing <c>IMoveJointsOperation</c>.</returns>
        IMoveJointsOperation MoveJoints(JointValues target);

        /// <summary>
        /// Creates a collision free move joints operation object that parameterizes a collision free move to the provided <c>JointValues</c> target.
        /// </summary>
        /// <param name="target">Target joint positions</param>
        /// <returns>Returns an operation object implementing <c>IMoveJointsOperation</c>.</returns>
        IMoveJointsOperation MoveJointsCollisionFree(JointValues target);

        /// <summary>
        /// Creates a move joint path operation object that parameterizes a move along a <c>IJointPath</c> in joint space.
        /// </summary>
        /// <param name="waypoints">Joint path</param>
        /// <returns>Returns an operation object implementing <c>IMoveJointsOperation</c>.</returns>
        IMoveJointPathOperation MoveJointPath(IJointPath waypoints);
    }
}
