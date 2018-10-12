using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
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
            this.defaultTaskSpacePlanParameters = this.motionService.CreateTaskSpacePlanParameters(this.defaultEndEffectorName, limits.MaxXYZVelocity, limits.MaxXYZAcceleration, limits.MaxAngularVelocity, limits.MaxAngularAcceleration);
        }

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
        /// Creates a MoveGroup for manually specifued joint set. Since the mapping between ROS move-groups
        /// and the specified joint set is not know nor analysed it is not possible to execute end-effector
        /// related functions (e.g. inverse kinematics).
        /// </summary>
        /// <param name="motionService"></param>
        /// <param name="jointSet"></param>
        public MoveGroup(IMotionService motionService, JointSet jointSet)
        {
            this.motionService = motionService ?? throw new ArgumentNullException(nameof(motionService));
            this.jointSet = jointSet;
            this.defaultPlanParameters = motionService.CreatePlanParameters(null, this.jointSet, null, null, 0.05, false, 1);
            this.velocityScaling = 1;
            this.name = this.defaultPlanParameters.MoveGroupName;
        }

        public string Name => name;
        public IMotionService MotionService => motionService;

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

        public bool CollisionCheck
        {
            get => defaultPlanParameters.CollisionCheck;
            set => defaultPlanParameters = defaultPlanParameters.WithCollisionCheck(value);
        }

        public double SampleResolution
        {
            get => defaultPlanParameters.SampleResolution;
            set
            {
                defaultPlanParameters = defaultPlanParameters.WithSampleResolution(value);
                if (defaultTaskSpacePlanParameters != null)
                {
                    defaultTaskSpacePlanParameters = defaultTaskSpacePlanParameters.WithSampleResolution(value);
                }
            }
        }

        public double MaxDeviation
        {
            get => defaultPlanParameters.MaxDeviation;
            set => defaultPlanParameters = defaultPlanParameters.WithMaxDeviation(value);
        }

        public double IkJumpThreshold
        {
            get => defaultTaskSpacePlanParameters.IkJumpThreshold;
            set => defaultTaskSpacePlanParameters = defaultTaskSpacePlanParameters.WithIkJumpThreshold(value);
        }

        public IEnumerable<string> EndEffectorNames => this.details.EndEffectorNames;

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

        public IEndEffector GetEndEffector(string endEffectorName = null) =>
            endEffectors[endEffectorName ?? defaultEndEffectorName];

        public IEndEffector DefaultEndEffector =>
            this.GetEndEffector(null);

        public Pose GetCurrentPose(string endEffectorName = null) =>
            this.GetEndEffector(endEffectorName).CurrentPose;

        public JointSet JointSet =>
            jointSet;

        public JointStates CurrentJointStates =>
            motionService.QueryJointStates(jointSet);

        public JointValues CurrentJointPositions =>
            this.CurrentJointStates.Positions;

        public PlanParameters DefaultPlanParameters =>
            defaultPlanParameters;

        public TaskSpacePlanParameters DefaultTaskSpacePlanParameters =>
            defaultTaskSpacePlanParameters;

        public TaskSpacePlanParameters BuildTaskSpacePlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, string endEffectorName = null)
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

            builder.ScaleVelocity(velocityScaling ?? this.velocityScaling);
            builder.ScaleAcceleration(accelerationScaling ?? velocityScaling ?? this.velocityScaling);

            return builder.Build();
        }

        public PlanParameters BuildPlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null)
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

            builder.ScaleVelocity(velocityScaling ?? this.velocityScaling);
            builder.ScaleAcceleration(accelerationScaling ?? velocityScaling ?? this.velocityScaling);

            return builder.Build();
        }

        public (IJointTrajectory, PlanParameters) PlanMoveJoints(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null)
        {
            var parameters = this.BuildPlanParameters(velocityScaling, collisionCheck, null, accelerationScaling);

            // get current pose
            var start = this.CurrentJointPositions;

            // generate joint path
            var jointPath = new JointPath(jointSet, start, target);

            // plan trajectory
            var trajectory = motionService.PlanMoveJoints(jointPath, parameters);

            return (trajectory, parameters);
        }

        public (IJointTrajectory, PlanParameters) PlanMoveJointsCollisionFreeAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken))
        {
            var parameters = this.BuildPlanParameters(velocityScaling, false, null, null);

            // get current pose
            var start = this.CurrentJointPositions;

            // plan path
            var jointPath = MotionService.PlanCollisionFreeJointPath(start, target, parameters);

            // plan trajectory
            var trajectory = MotionService.PlanMoveJoints(jointPath, parameters);
            return (trajectory, parameters);
        }

        public async Task MoveJointsCollisionFreeAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken))
        {
            var (trajectory, planParameters) = this.PlanMoveJointsCollisionFreeAsync(target, velocityScaling, sampleResolution);
            await motionService.ExecuteJointTrajectory(trajectory, planParameters.CollisionCheck, cancel);
        }

        public ISteppedMotionClient MoveJointsCollisionFreeSupervisedAsync(JointValues target, CancellationToken cancel = default(CancellationToken)) =>
            this.MoveJointsCollisionFreeSupervisedAsync(target, null, null, cancel);


        public ISteppedMotionClient MoveJointsCollisionFreeSupervisedAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken))
        {
            var (trajectory, planParameters) = this.PlanMoveJointsCollisionFreeAsync(target, velocityScaling, sampleResolution);
            double scaling = 1.0;
            if (velocityScaling.HasValue)
                scaling = velocityScaling.Value;

            return motionService.ExecuteJointTrajectorySupervised(trajectory, scaling, planParameters.CollisionCheck, cancel);
        }

        public Task MoveJointsAsync(JointValues target, CancellationToken cancel = default(CancellationToken)) =>
            this.MoveJointsAsync(target, null, null, null, cancel);

        public async Task MoveJointsAsync(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            // plan trajectory
            var (trajectory, planParameters) = this.PlanMoveJoints(target, velocityScaling, collisionCheck, accelerationScaling);

            await motionService.ExecuteJointTrajectory(trajectory, planParameters.CollisionCheck, cancel);
        }

        public ISteppedMotionClient MoveJointsSupervisedAsync(JointValues target, CancellationToken cancel = default(CancellationToken)) =>
            this.MoveJointsSupervisedAsync(target, null, null, null, cancel);

        public ISteppedMotionClient MoveJointsSupervisedAsync(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            // plan trajectory
            var (trajectory, planParameters) = this.PlanMoveJoints(target, velocityScaling, collisionCheck, accelerationScaling);
            double scaling = 1.0;
            if (velocityScaling.HasValue)
                scaling = velocityScaling.Value;

            return motionService.ExecuteJointTrajectorySupervised(trajectory, scaling, planParameters.CollisionCheck, cancel);
        }

        public (IJointTrajectory, PlanParameters) PlanMoveJointPath(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null)
        {
            var parameters = this.BuildPlanParameters(velocityScaling, collisionCheck, maxDeviation, accelerationScaling);

            // get current pose
            var start = this.CurrentJointPositions;

            // generate joint path
            var jointPath = waypoints.Prepend(start);

            // plan trajectory
            var trajectory = motionService.PlanMoveJoints(jointPath, parameters);

            return (trajectory, parameters);
        }

        public Task MoveJointPathAsync(IJointPath waypoints, CancellationToken cancel = default(CancellationToken)) =>
            this.MoveJointPathAsync(waypoints, null, null, null, null, cancel);

        public async Task MoveJointPathAsync(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            if (waypoints == null || waypoints.Count == 0)
                return;

            // plan trajectory
            var (trajectory, planParameters) = PlanMoveJointPath(waypoints, velocityScaling, collisionCheck, maxDeviation, accelerationScaling);
            await motionService.ExecuteJointTrajectory(trajectory, planParameters.CollisionCheck, cancel);
        }

        public ISteppedMotionClient MoveJointPathSupervisedAsync(IJointPath waypoints, CancellationToken cancel = default(CancellationToken)) =>
            this.MoveJointPathSupervisedAsync(waypoints, null, null, null, null, cancel);

        public ISteppedMotionClient MoveJointPathSupervisedAsync(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken))
        {
            // plan trajectory
            var (trajectory, planParameters) = PlanMoveJointPath(waypoints, velocityScaling, collisionCheck, maxDeviation, accelerationScaling);
            double scaling = 1.0;
            if (velocityScaling.HasValue)
                scaling = velocityScaling.Value;
            return motionService.ExecuteJointTrajectorySupervised(trajectory, scaling, planParameters.CollisionCheck, cancel);
        }

        public void Dispose()
        {
            motionService = null;
        }
    }
}
