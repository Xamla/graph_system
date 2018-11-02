using System;
using System.Collections.Generic;
using System.Linq;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Encapsulates move functionality for a specific move group.
    /// </summary>
    public class MoveGroup
        : IMoveGroup
    {
        IMotionService motionService;
        string name;
        MoveGroupDescription details;
        JointSet jointSet;
        PlanParameters defaultPlanParameters;
        TaskSpacePlanParameters defaultTaskSpacePlanParameters;
        double velocityScaling;
        IReadOnlyDictionary<string, IEndEffector> endEffectors;
        string defaultEndEffectorName;

        private void SetDefaultEndEffector(string endEffectorName = null, IList<MoveGroupDescription> groupDescription = null)
        {
            var groups = groupDescription ?? this.motionService.QueryAvailableMoveGroups();
            var groupDetails = groups.ToDictionary(x => x.Name, x => x);

            if (!groupDetails.TryGetValue(this.name, out this.details))
                throw new Exception($"Move group with name '{this.name}' not found.");

            Dictionary<string, IEndEffector> tmp = new Dictionary<string, IEndEffector>();
            for (int i = 0; i < this.details.EndEffectorNames.Length; i++)
            {
                tmp.Add(this.details.EndEffectorNames[i], (IEndEffector)new EndEffector(this, this.details.EndEffectorNames[i], this.details.EndEffectorLinkNames[i]));
            }
            this.endEffectors = tmp;

            if (!this.endEffectors.ContainsKey(endEffectorName ?? ""))
                endEffectorName = this.details.EndEffectorNames.FirstOrDefault(); // use first end effector as default

            this.defaultEndEffectorName = endEffectorName;
            var limits = this.motionService.QueryEndEffectorLimits(endEffectorName);
            this.defaultTaskSpacePlanParameters = this.motionService.CreateTaskSpacePlanParameters(
                this.defaultEndEffectorName,
                limits.MaxXYZVelocity,
                limits.MaxXYZAcceleration,
                limits.MaxAngularVelocity,
                limits.MaxAngularAcceleration
            );
        }

        /// <summary>
        /// Create a instance of <c>MoveGroup</c>
        /// </summary>
        /// <param name="motionService">An object implementing <c>IMotionService</c> which is used to communicate with the motion server.</param>
        /// <param name="moveGroupName">The name of the move group represented by this instance</param>
        /// <param name="defaultEndEffectorName">Name of the default end effector</param>
        /// <exception cref="Exception">Thrown when current name does not exist.  </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="motionService"/> is null.</exception>
        public MoveGroup(IMotionService motionService, string moveGroupName = null, string defaultEndEffectorName = null)
        {
            this.motionService = motionService ?? throw new ArgumentNullException(nameof(motionService));

            var groups = motionService.QueryAvailableMoveGroups();
            var groupDetails = groups.ToDictionary(x => x.Name, x => x);
            this.name = moveGroupName ?? groups.First().Name;

            if (!groupDetails.TryGetValue(this.name, out this.details))
                throw new Exception($"Move group with name '{this.name}' not found.");

            this.jointSet = this.details.JointSet;
            this.defaultPlanParameters = motionService.CreatePlanParameters(this.name, this.jointSet, null, null, 0.05, false, 1);
            this.velocityScaling = 1;
            this.SetDefaultEndEffector(defaultEndEffectorName, groups);
        }

        /// <summary>
        /// Creates a <c>MoveGroup</c> for manually specified joint set. Since the mapping between ROS move-groups
        /// and the specified joint set is not know nor analysed it is not possible to execute end-effector
        /// related functions (e.g. inverse kinematics).
        /// </summary>
        /// <param name="motionService">An object implementing <c>IMotionService</c> which is used to communicate with the motion server.</param>
        /// <param name="jointSet">A joint set</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="motionService"/> is null.</exception>
        public MoveGroup(IMotionService motionService, JointSet jointSet)
        {
            this.motionService = motionService ?? throw new ArgumentNullException(nameof(motionService));
            this.jointSet = jointSet;
            this.defaultPlanParameters = motionService.CreatePlanParameters(null, this.jointSet, null, null, 0.05, false, 1);
            this.velocityScaling = 1;
            this.name = this.defaultPlanParameters.MoveGroupName;
        }

        /// <summary>
        /// Name of the <c>MoveGroup</c>
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Reference to an object implementing <c>IMotionService</c>
        /// Used to communicate with the motion server
        /// </summary>
        public IMotionService MotionService => motionService;

        /// <summary>
        /// Current velocity scaling
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <c>value</c> not in range (0 - 1).</exception>
        public double VelocityScaling
        {
            get => velocityScaling;
            set
            {
                if (value > 0 && value <= 1)
                    velocityScaling = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(value), $"Argument '{nameof(value)}' must lie in range (0-1).");
            }
        }

        /// <summary>
        /// Sets the collision check
        /// If true, the trajectory planing tries to plan a collision free trajectory.
        /// Before executing a trajectory, a collision check is performed.
        /// </summary>
        public bool CollisionCheck
        {
            get => defaultPlanParameters.CollisionCheck;
            set
            {
                defaultPlanParameters = defaultPlanParameters.WithCollisionCheck(value);
                defaultTaskSpacePlanParameters = defaultTaskSpacePlanParameters?.WithCollisionCheck(value);
            }
        }

        /// <summary>
        /// Trajectory point sampling frequency
        /// </summary>
        public double SampleResolution
        {
            get => defaultPlanParameters.SampleResolution;
            set
            {
                defaultPlanParameters = defaultPlanParameters.WithSampleResolution(value);
                defaultTaskSpacePlanParameters = defaultTaskSpacePlanParameters?.WithSampleResolution(value);
            }
        }

        /// <summary>
        /// Maximal deviation from the trajectory points
        /// </summary>
        public double MaxDeviation
        {
            get => defaultPlanParameters.MaxDeviation;
            set
            {
                defaultPlanParameters = defaultPlanParameters.WithMaxDeviation(value);
                defaultTaskSpacePlanParameters = defaultTaskSpacePlanParameters?.WithMaxDeviation(value);
            }
        }

        /// <summary>
        /// Maximal allowed inverse kinematics jump threshold
        /// </summary>
        public double IkJumpThreshold
        {
            get => defaultTaskSpacePlanParameters.IkJumpThreshold;
            set => defaultTaskSpacePlanParameters = defaultTaskSpacePlanParameters.WithIkJumpThreshold(value);
        }

        /// <summary>
        /// Object implementing <c>IEnumerable<string></c> containing endeffector names of the the move group
        /// </summary>
        public IEnumerable<string> EndEffectorNames => this.details.EndEffectorNames;

        /// <summary>
        /// Default end effector name
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <c>value</c> is null.</exception>
        /// <exception cref="Exception">Thrown when end efector of name <c>value</c> does not exist in move group.</exception>
        public string DefaultEndEffectorName
        {
            get => defaultEndEffectorName;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (!endEffectors.ContainsKey(value))
                    throw new Exception($"Specified end effector '{value}' does not exist in move group '{name}'.");
                defaultEndEffectorName = value;
            }
        }

        /// <summary>
        /// Get an end effector by name
        /// </summary>
        /// <param name="endEffectorName">Name of the end effector. The default end effector is used when null.</param>
        /// <returns>Returns an object implementing <c>IEndEffector</c> associated with  <paramref name="endEffectorName"/>.</returns>
        public IEndEffector GetEndEffector(string endEffectorName = null) =>
            endEffectors[endEffectorName ?? defaultEndEffectorName];

        /// <summary>
        /// Get default end effector
        /// </summary>
        /// <returns>Returns an object implementig <c>IEndEffector</c> representing the default end effector</returns>
        public IEndEffector DefaultEndEffector =>
            this.GetEndEffector(null);

        /// <summary>
        /// Get the current position of an end effector
        /// </summary>
        /// <param name="endEffectorName">Name of the <c>IEndEffector</c> instance. The default end effector is used when null.</param>
        /// <returns>Returns the current <c>Pose</c> instance of the end effector associated with <paramref name="endEffectorName"/>.</returns>
        public Pose GetCurrentPose(string endEffectorName = null) =>
            this.GetEndEffector(endEffectorName).CurrentPose;

        /// <summary>
        /// <c>JointSet</c> of the <c>MoveGroup</c>
        /// </summary>
        public JointSet JointSet =>
            jointSet;

        /// <summary>
        /// Current <c>JointStates</c> of the of the <c>MoveGroup</c> joints
        /// </summary>
        public JointStates CurrentJointStates =>
            motionService.QueryJointStates(jointSet);

        /// <summary>
        /// Current joint positions of the of the <c>MoveGroup</c> joints
        /// </summary>
        public JointValues CurrentJointPositions =>
            this.CurrentJointStates.Positions;

        /// <summary>
        /// Default instance of <c>PlanParameters</c>
        /// </summary>
        public PlanParameters DefaultPlanParameters =>
            defaultPlanParameters;

        /// <summary>
        /// Instance of TaskSpacePlanParameters from which the limits and settings are used when no specific user input is given
        /// </summary>
        public TaskSpacePlanParameters DefaultTaskSpacePlanParameters =>
            defaultTaskSpacePlanParameters;

        /// <summary>
        /// Builds an instance of <c>TaskSpacePlanParameters</c>.
        /// </summary>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities. Must lie in range (0 - 1).</param>
        /// <param name="collisionCheck"> If true the trajectory planing tries to plan a collision free trajectory and before executing a trajectory a collision check is performed.</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <param name="endEffectorName">Name of the end effector</param>
        /// <returns>Returns a new <c>TaskSpacePlanParameters</c> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="velocityScaling"/> is not in range (0 - 1).</exception>
        public TaskSpacePlanParameters BuildTaskSpacePlanParameters(
            double? velocityScaling = null,
            bool? collisionCheck = null,
            double? maxDeviation = null,
            double? accelerationScaling = null,
            double? ikJumpThreshold = null,
            string endEffectorName = null
        )
        {
            // check input or use member values if arguments where not provided
            if (velocityScaling.HasValue && !(velocityScaling > 0 && velocityScaling <= 1))
                throw new ArgumentOutOfRangeException(nameof(velocityScaling), $"Argument '{nameof(velocityScaling)}' must lie in range (0 - 1).");

            // start with default plan parameters
            var builder = defaultTaskSpacePlanParameters.ToBuilder();
            builder.EndEffectorName = endEffectorName ?? this.DefaultEndEffectorName;

            if (collisionCheck.HasValue)
                builder.CollisionCheck = collisionCheck.Value;

            if (maxDeviation.HasValue)
                builder.MaxDeviation = maxDeviation.Value;

            if (ikJumpThreshold.HasValue)
                builder.IkJumpThreshold = ikJumpThreshold.Value;

            builder.ScaleVelocity(velocityScaling ?? this.velocityScaling);
            builder.ScaleAcceleration(accelerationScaling ?? velocityScaling ?? this.velocityScaling);

            return builder.Build();
        }

        /// <summary>
        /// Builds an instance of <c>PlanParameters</c>.
        /// For parameter == null, the default values in <c>DefaultPlanParameters</c> are used.
        /// </summary>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities. Must lie in range (0 - 1).</param>
        /// <param name="collisionCheck">Activates/Deactivates collision check</param>
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space</param>
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations</param>
        /// <returns>Returns a new <c>PlanParameters</c> instance based on <c>DefaultPlanParameters</c> and parameters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="velocityScaling"/> is not in range (0 - 1).</exception>
        public PlanParameters BuildPlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, double? sampleResolution = null)
        {
            // check input or use member values if arguments where not provided
            if (velocityScaling.HasValue && !(velocityScaling > 0 && velocityScaling <= 1))
                throw new ArgumentOutOfRangeException(nameof(velocityScaling), $"Argument {nameof(velocityScaling)} must lie in range (0 - 1).");

            // start with default plan parameters
            var builder = defaultPlanParameters.ToBuilder();
            if (collisionCheck.HasValue)
                builder.CollisionCheck = collisionCheck.Value;

            if (maxDeviation.HasValue)
                builder.MaxDeviation = maxDeviation.Value;

            if (sampleResolution != null)
                builder.SampleResolution = sampleResolution.Value;

            builder.ScaleVelocity(velocityScaling ?? this.velocityScaling);
            builder.ScaleAcceleration(accelerationScaling ?? velocityScaling ?? this.velocityScaling);

            return builder.Build();
        }

        /// <summary>
        /// Creates a <c>MoveJointsOperation<c> to move to the provided <c>JointValues</c> target.
        /// </summary>
        /// <param name="target">Target joint positions</param>
        /// <returns>Returns a <c>MoveJointsOperation</c> instance.</returns>
        public IMoveJointsOperation MoveJoints(JointValues target)
        {
            var args = new MoveJointsArgs
            {
                MoveGroup = this,
                Target = target,
                VelocityScaling = this.VelocityScaling,
                AccelerationScaling = this.VelocityScaling,
                CollisionCheck = this.CollisionCheck,
                SampleResolution = this.SampleResolution,
                MaxDeviation = this.MaxDeviation
            };
            return new MoveJointsOperation(args);
        }

        /// <summary>
        /// Creates an object implementing <c>IMoveJointsOperation<c> that parameterizes a collision free move to the provided <c>JointValues</c> target.
        /// </summary>
        /// <param name="target">Target joint positions</param>
        /// <returns>Returns an <c>IMoveJointsOperation</c> instance.</returns>
        public IMoveJointsOperation MoveJointsCollisionFree(JointValues target)
        {
            var args = new MoveJointsArgs
            {
                MoveGroup = this,
                Target = target,
                VelocityScaling = this.VelocityScaling,
                AccelerationScaling = this.VelocityScaling,
                CollisionCheck = this.CollisionCheck,
                SampleResolution = this.SampleResolution,
                MaxDeviation = this.MaxDeviation
            };
            return new MoveJointsCollisionFreeOperation(args);
        }

        /// <summary>
        /// Creates a move joint path operation object that parameterizes a move along a path defined by waypoints in joint space.
        /// </summary>
        /// <param name="waypoints">Joint path</param>
        /// <returns>Returns an operation object implementing <c>IMoveJointsOperation</c>.</returns>
        public IMoveJointPathOperation MoveJointPath(IJointPath waypoints)
        {
            var args = new MoveJointPathArgs
            {
                MoveGroup = this,
                Waypoints = waypoints,
                VelocityScaling = this.VelocityScaling,
                AccelerationScaling = this.VelocityScaling,
                CollisionCheck = this.CollisionCheck,
                SampleResolution = this.SampleResolution,
                MaxDeviation = this.MaxDeviation
            };
            return new MoveJointPathOperation(args);
        }

        /// <summary>
        /// Clean up
        /// </summary>
        public void Dispose()
        {
            motionService = null;
        }
    }
}
