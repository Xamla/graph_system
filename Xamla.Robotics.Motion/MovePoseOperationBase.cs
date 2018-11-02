using System;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public abstract class MovePoseOperationBase
        : IMovePoseOperation
    {
        public IEndEffector EndEffector { get; }
        public JointValues Seed { get; }
        public Pose TargetPose { get; }
        public Pose StartPose { get; }
        public JointValues Start { get; }

        public IMoveGroup MoveGroup { get; }
        public double? VelocityScaling { get; }
        public double? AccelerationScaling { get; }
        public PlanParameters Parameters { get; }
        public TaskSpacePlanParameters TaskSpaceParameters { get; }

        public MovePoseOperationBase(MovePoseArgs args)
        {
            this.EndEffector = args.EndEffector;
            this.Seed = args.Seed;
            this.TargetPose = args.TargetPose;
            this.StartPose = args.StartPose;
            this.Start = args.Start;
            this.MoveGroup = this.EndEffector.MoveGroup;
            this.VelocityScaling = args.VelocityScaling;
            this.AccelerationScaling = args.AccelerationScaling;

            this.Parameters = this.MoveGroup.BuildPlanParameters(args.VelocityScaling, args.CollisionCheck, args.MaxDeviation, args.AccelerationScaling, args.SampleResolution);
            this.TaskSpaceParameters = this.EndEffector.BuildTaskSpacePlanParameters(args.VelocityScaling, args.CollisionCheck, args.MaxDeviation, args.AccelerationScaling, args.IkJumpThreshold);
            if (args.SampleResolution.HasValue)
            {
                this.TaskSpaceParameters = this.TaskSpaceParameters.WithSampleResolution(args.SampleResolution.Value);
            }
        }

        protected abstract IMovePoseOperation Build(MovePoseArgs args);

        public abstract IPlan Plan();

        public IMovePoseOperation WithVelocityScaling(double? value) =>
            this.VelocityScaling == value ? this : With(a => a.VelocityScaling = value);

        public IMovePoseOperation WithAccelerationScaling(double? value) =>
            this.AccelerationScaling == value ? this : With(a => a.AccelerationScaling = value);

        public IMovePoseOperation With(Func<MovePoseArgs, MovePoseArgs> mutator) =>
            Build(mutator(this.ToArgs()));

        public IMovePoseOperation With(Action<MovePoseArgs> mutator)
        {
            var args = this.ToArgs();
            mutator(args);
            return Build(args);
        }

        public MovePoseArgs ToArgs() =>
            new MovePoseArgs
            {
                EndEffector = this.EndEffector,
                Seed = this.Seed,
                TargetPose = this.TargetPose,
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
