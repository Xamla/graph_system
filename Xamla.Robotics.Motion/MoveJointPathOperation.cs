using System;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveJointPathOperation
        : IMoveJointPathOperation
    {
        public JointValues Start { get; }
        public IJointPath Waypoints { get; }

        public IMoveGroup MoveGroup { get; }
        public double? VelocityScaling { get; }
        public double? AccelerationScaling { get; }
        public PlanParameters Parameters { get; }

        public MoveJointPathOperation(MoveJointPathArgs args)
        {
            this.Start = args.Start;
            this.Waypoints = args.Waypoints;
            this.MoveGroup = args.MoveGroup;
            this.VelocityScaling = args.VelocityScaling;
            this.AccelerationScaling = args.AccelerationScaling;
            this.Parameters = this.MoveGroup.BuildPlanParameters(args.VelocityScaling, args.CollisionCheck, args.MaxDeviation, args.AccelerationScaling, args.SampleResolution);
        }

        public IPlan Plan()
        {
            // get start joint values
            var start = this.Start ?? this.MoveGroup.CurrentJointPositions;

            // generate joint path
            var jointPath = this.Waypoints.Prepend(start);

            // plan trajectory
            var trajectory = this.MoveGroup.MotionService.PlanMoveJoints(jointPath, this.Parameters);

            return new Plan(this.MoveGroup, trajectory, this.Parameters);
        }

        public IMoveJointPathOperation WithStart(JointValues value) =>
          object.Equals(this.Start, value) ? this : With(a => a.Start = value);

        public IMoveJointPathOperation WithCollisionCheck(bool value = true) =>
            this.Parameters.CollisionCheck == value ? this : With(a => a.CollisionCheck = value);

        public IMoveJointPathOperation WithVelocityScaling(double? value) =>
            this.VelocityScaling == value ? this : With(a => a.VelocityScaling = value);

        public IMoveJointPathOperation WithAccelerationScaling(double? value) =>
            this.AccelerationScaling == value ? this : With(a => a.AccelerationScaling = value);

        public IMoveJointPathOperation WithArgs(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? sampleResolution = null, double? accelerationScaling = null) =>
            this.With(a =>
            {
                a.VelocityScaling = velocityScaling ?? a.VelocityScaling;
                a.CollisionCheck = collisionCheck ?? a.CollisionCheck;
                a.MaxDeviation = maxDeviation ?? a.MaxDeviation;
                a.SampleResolution = sampleResolution ?? a.SampleResolution;
                a.AccelerationScaling = accelerationScaling ?? a.AccelerationScaling;
            });

        public IMoveJointPathOperation With(Func<MoveJointPathArgs, MoveJointPathArgs> mutator) =>
            new MoveJointPathOperation(mutator(this.ToArgs()));

        public IMoveJointPathOperation With(Action<MoveJointPathArgs> mutator)
        {
            var args = this.ToArgs();
            mutator(args);
            return new MoveJointPathOperation(args);
        }

        public MoveJointPathArgs ToArgs()
            => new MoveJointPathArgs
            {
                MoveGroup = this.MoveGroup,
                Start = this.Start,
                Waypoints = this.Waypoints,
                VelocityScaling = this.VelocityScaling,
                AccelerationScaling = this.AccelerationScaling,
                CollisionCheck = this.Parameters.CollisionCheck,
                SampleResolution = this.Parameters.SampleResolution,
                MaxDeviation = this.Parameters.MaxDeviation
            };
    }
}
