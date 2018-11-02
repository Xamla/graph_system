using System;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveCartesianPathOperation
        : IMoveCartesianPathOperation
    {
        public IEndEffector EndEffector { get; }
        public JointValues Seed { get; }
        public ICartesianPath Waypoints { get; }
        public Pose StartPose { get; }
        public JointValues Start { get; }

        public IMoveGroup MoveGroup { get; }
        public double VelocityScaling { get; }
        public double AccelerationScaling { get; }
        public PlanParameters Parameters { get; }
        public TaskSpacePlanParameters TaskSpaceParameters { get; }

        public MoveCartesianPathOperation(MoveCartesianPathArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (args.EndEffector == null)
                throw new ArgumentException("EndEffector member of arguments must not be null.");

            this.EndEffector = args.EndEffector;
            this.Seed = args.Seed;
            this.Waypoints = args.Waypoints;
            this.StartPose = args.StartPose;
            this.Start = args.Start;
            this.MoveGroup = this.EndEffector.MoveGroup;
            this.VelocityScaling = args.VelocityScaling;
            this.AccelerationScaling = args.AccelerationScaling;

            this.Parameters = this.MoveGroup.BuildPlanParameters(args.VelocityScaling, args.CollisionCheck, args.MaxDeviation, args.AccelerationScaling, args.SampleResolution);
            this.TaskSpaceParameters = this.EndEffector.BuildTaskSpacePlanParameters(args.VelocityScaling, args.CollisionCheck, args.MaxDeviation, args.AccelerationScaling, args.IkJumpThreshold)
                .WithSampleResolution(args.SampleResolution);
        }

        protected virtual IMoveCartesianPathOperation Build(MoveCartesianPathArgs args) =>
            new MoveCartesianPathOperation(args);

        public virtual IPlan Plan()
        {
            if (this.Waypoints.Count == 0)
                return new Plan(this.MoveGroup, JointTrajectory.Empty, this.Parameters);

            JointValues seed = this.Seed ?? this.MoveGroup.CurrentJointPositions;
            var targetJoints = this.EndEffector.InverseKinematicMany(this.Waypoints, this.Parameters.CollisionCheck, seed);
            if (!targetJoints.Succeeded)
                throw new Exception("No inverse kinematic solution found for at least one pose of cartesian path.");

            // TODO: Check IkJoump!

            var path = targetJoints.Path;
            return this.MoveGroup.MoveJointPath(path)
                .With(a => this.ToArgs())
                .Plan();
        }

        public IMoveCartesianPathOperation WithArgs(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? sampleResolution = null, double? accelerationScaling = null) =>
            this.With(a =>
            {
                a.VelocityScaling = velocityScaling ?? a.VelocityScaling;
                a.CollisionCheck = collisionCheck ?? a.CollisionCheck;
                a.MaxDeviation = maxDeviation ?? a.MaxDeviation;
                a.SampleResolution = sampleResolution ?? a.SampleResolution;
                a.AccelerationScaling = accelerationScaling ?? a.AccelerationScaling;
            });

        public IMoveCartesianPathOperation With(Func<MoveCartesianPathArgs, MoveCartesianPathArgs> mutator) =>
            Build(mutator(this.ToArgs()));

        public IMoveCartesianPathOperation With(Action<MoveCartesianPathArgs> mutator)
        {
            var args = this.ToArgs();
            mutator(args);
            return Build(args);
        }

        public MoveCartesianPathArgs ToArgs() =>
            new MoveCartesianPathArgs
            {
                EndEffector = this.EndEffector,
                Seed = this.Seed,
                Waypoints = this.Waypoints,
                StartPose = this.StartPose,
                Start = this.Start,
                IkJumpThreshold = this.TaskSpaceParameters.IkJumpThreshold,
                MoveGroup = this.MoveGroup,
                VelocityScaling = this.VelocityScaling,
                AccelerationScaling = this.AccelerationScaling,
                SampleResolution = this.Parameters.SampleResolution,
                CollisionCheck = this.Parameters.CollisionCheck,
                MaxDeviation = this.Parameters.MaxDeviation
            };
    }
}
