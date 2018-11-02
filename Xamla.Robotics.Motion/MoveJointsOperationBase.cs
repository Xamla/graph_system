using System;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public abstract class MoveJointsOperationBase
        : IMoveJointsOperation
    {
        public JointValues Start { get; }
        public JointValues Target { get; }

        public IMoveGroup MoveGroup { get; }
        public double VelocityScaling { get; }
        public double AccelerationScaling { get; }
        public double SampleResolution { get; }
        public PlanParameters Parameters { get; }

        public MoveJointsOperationBase(MoveJointsArgs args)
        {
            this.Start = args.Start;
            this.Target = args.Target;
            this.MoveGroup = args.MoveGroup;
            this.VelocityScaling = args.VelocityScaling;
            this.AccelerationScaling = args.AccelerationScaling;
            this.Parameters = this.MoveGroup.BuildPlanParameters(VelocityScaling, args.CollisionCheck, args.MaxDeviation, args.AccelerationScaling, args.SampleResolution);
        }

        protected abstract IMoveJointsOperation Build(MoveJointsArgs args);

        public abstract IPlan Plan();

        public IMoveJointsOperation WithStart(JointValues value) =>
            object.Equals(this.Start, value) ? this : With(a => a.Start = value);

        public IMoveJointsOperation WithCollisionCheck(bool value = true) =>
            this.Parameters.CollisionCheck == value ? this : With(a => a.CollisionCheck = value);

        public IMoveJointsOperation WithVelocityScaling(double value) =>
            this.VelocityScaling == value ? this : With(a => a.VelocityScaling = value);

        public IMoveJointsOperation WithArgs(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? sampleResolution = null, double? accelerationScaling = null) =>
            this.With(a =>
            {
                a.VelocityScaling = velocityScaling ?? a.VelocityScaling;
                a.CollisionCheck = collisionCheck ?? a.CollisionCheck;
                a.MaxDeviation = maxDeviation ?? a.MaxDeviation;
                a.SampleResolution = sampleResolution ?? a.SampleResolution;
                a.AccelerationScaling = accelerationScaling ?? a.AccelerationScaling;
            });

        public IMoveJointsOperation With(Func<MoveJointsArgs, MoveJointsArgs> mutator) =>
            Build(mutator(this.ToArgs()));

        public IMoveJointsOperation With(Action<MoveJointsArgs> mutator)
        {
            var args = this.ToArgs();
            mutator(args);
            return Build(args);
        }

        public MoveJointsArgs ToArgs()
            => new MoveJointsArgs
            {
                MoveGroup = this.MoveGroup,
                Start = this.Start,
                Target = this.Target,
                VelocityScaling = this.VelocityScaling,
                AccelerationScaling = this.AccelerationScaling,
                CollisionCheck = this.Parameters.CollisionCheck,
                SampleResolution = this.Parameters.SampleResolution,
                MaxDeviation = this.Parameters.MaxDeviation
            };
    }
}
