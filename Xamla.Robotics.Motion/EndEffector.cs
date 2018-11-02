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
        /// Builds an instance of <c>TaskSpacePlanParameters</c>.
        /// </summary>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities</param>
        /// <param name="collisionCheck"> If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <returns>Returns a new <c>TaskSpacePlanParameters</c> instance.</returns>
        public TaskSpacePlanParameters BuildTaskSpacePlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, double? ikJumpThreshold = null) =>
           this.MoveGroup.BuildTaskSpacePlanParameters(velocityScaling, collisionCheck, maxDeviation, accelerationScaling, ikJumpThreshold, this.Name);

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
        /// Get inverse kinematic solution for one pose.
        /// </summary>
        /// <param name="pose">The pose to be transformmed to joint space</param>
        /// <param name="avoidCollision">If true the trajectory planing tries to plan collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="jointPositionSeed">Optional seed joint configuration</param>
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
        /// Get inverse kinematic solutions for multiple poses.
        /// </summary>
        /// <param name="poses">The poses to be transformmed to joint space</param>
        /// <param name="avoidCollision">If true the trajectory planing tries to plan collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="jointPositionSeed">Optional seed joint configuration</param>
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

        private MovePoseArgs CreateMovePoseArgs(Pose target, JointValues seed = null) =>
            new MovePoseArgs
            {
                TargetPose = target,
                Seed = seed,
                EndEffector = this,
                MoveGroup = this.MoveGroup,
                IkJumpThreshold = this.MoveGroup.IkJumpThreshold,
                VelocityScaling = this.MoveGroup.VelocityScaling,
                AccelerationScaling = this.MoveGroup.VelocityScaling,
                CollisionCheck = this.MoveGroup.CollisionCheck,
                SampleResolution = this.MoveGroup.SampleResolution,
                MaxDeviation = this.MoveGroup.MaxDeviation
            };

        private MoveCartesianPathArgs CreateMoveCartesianPathArgs(ICartesianPath waypoints) =>
            new MoveCartesianPathArgs
            {
                Waypoints = waypoints,
                EndEffector = this,
                MoveGroup = this.MoveGroup,
                IkJumpThreshold = this.MoveGroup.IkJumpThreshold,
                VelocityScaling = this.MoveGroup.VelocityScaling,
                AccelerationScaling = this.MoveGroup.VelocityScaling,
                CollisionCheck = this.MoveGroup.CollisionCheck,
                SampleResolution = this.MoveGroup.SampleResolution,
                MaxDeviation = this.MoveGroup.MaxDeviation
            };

        public IMovePoseOperation MovePoseCollisionFree(Pose target, JointValues seed = null) =>
            new MovePoseCollisionFreeOperation(CreateMovePoseArgs(target, seed));

        public IMovePoseOperation MovePoseLinear(Pose target, JointValues seed = null) =>
            new MovePoseLinearOperation(CreateMovePoseArgs(target, seed));

        public IMoveCartesianPathOperation MoveCartesianPathLinear(ICartesianPath waypoints) =>
            new MoveCartesianPathLinearOperation(CreateMoveCartesianPathArgs(waypoints));

        public IMoveCartesianPathOperation MoveCartesianPath(ICartesianPath waypoints) =>
            new MoveCartesianPathOperation(CreateMoveCartesianPathArgs(waypoints));
    }
}
