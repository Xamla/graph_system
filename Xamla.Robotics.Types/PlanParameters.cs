using System;
using System.Linq;

namespace Xamla.Robotics.Types
{
    public class PlanParameters
    {
        public class Builder
        {
            public string MoveGroupName { get; set; }
            public JointSet JointSet { get; set; }
            public double[] MaxVelocity { get; set; }
            public double[] MaxAcceleration { get; set; }
            public bool CollisionCheck { get; set; } = true;
            public double SampleResolution { get; set; } = 0.008;       // 125 Hz
            public double MaxDeviation { get; set; } = 0.2;

            public Builder()
            {
            }

            public Builder(PlanParameters instance)
            {
                this.MoveGroupName = instance.MoveGroupName;
                this.JointSet = instance.JointSet;
                this.MaxVelocity = instance.MaxVelocity;
                this.MaxAcceleration = instance.MaxAcceleration;
                this.CollisionCheck = instance.CollisionCheck;
                this.SampleResolution = instance.SampleResolution;
                this.MaxDeviation = instance.MaxDeviation;
            }

            public PlanParameters Build() =>
                new PlanParameters(this);

            public Builder ScaleAcceleration(double factor)
            {
                if (!(factor > 0 && factor <= 1))
                    throw new ArgumentOutOfRangeException(nameof(factor), "Argument `factor` must lie in range(0 - 1).");

                if (this.MaxAcceleration != null)
                {
                    this.MaxAcceleration = this.MaxAcceleration.Select(x => x * factor).ToArray();
                }
                return this;
            }

            public Builder ScaleVelocity(double factor)
            {
                if (!(factor > 0 && factor <= 1))
                    throw new ArgumentOutOfRangeException(nameof(factor), "Argument `factor` must lie in range(0 - 1).");

                if (this.MaxVelocity != null)
                {
                    this.MaxVelocity = this.MaxVelocity.Select(x => x * factor).ToArray();
                }
                return this;
            }
        }

        public string MoveGroupName { get; }
        public JointSet JointSet { get; }
        public double[] MaxVelocity { get; }
        public double[] MaxAcceleration { get; }
        public bool CollisionCheck { get; }
        public double SampleResolution { get; }
        public double MaxDeviation { get; }

        public PlanParameters()
            : this(new Builder())
        {
        }

        public PlanParameters(string moveGroupName, JointSet joints, double[] maxVelocity, double[] maxAcceleration, double sampleResolution = 0.008, bool collisionCheck = true, double maxDeviation = 0.2)
            : this(new Builder() {
                MoveGroupName = moveGroupName,
                JointSet = joints,
                MaxVelocity = maxVelocity,
                MaxAcceleration = maxAcceleration,
                SampleResolution = sampleResolution,
                CollisionCheck = collisionCheck,
                MaxDeviation = maxDeviation
            })
        {
        }

        public PlanParameters(Builder builder)
        {
            if (builder.JointSet != null)
            {
                int jointCount = builder.JointSet.Count;
                if (builder.MaxVelocity != null && builder.MaxVelocity.Length != jointCount)
                    throw new ArgumentOutOfRangeException("Cannot create PlanParameters with MaxVelocity array size that does not match JointSet size.", nameof(builder.MaxVelocity));
                if (builder.MaxAcceleration != null && builder.MaxAcceleration.Length != jointCount)
                    throw new ArgumentOutOfRangeException("Cannot create PlanParameters with MaxAcceleration array size that does not match JointSet size.", nameof(builder.MaxAcceleration));
            }

            this.MoveGroupName = builder.MoveGroupName;
            this.JointSet = builder.JointSet;
            this.MaxVelocity = builder.MaxVelocity;
            this.MaxAcceleration = builder.MaxAcceleration;
            this.CollisionCheck = builder.CollisionCheck;
            this.SampleResolution = builder.SampleResolution;
            this.MaxDeviation = builder.MaxDeviation;
        }

        public PlanParameters WithCollisionCheck(bool value)
        {
            if (this.CollisionCheck == value)
                return this;
            var builder = this.ToBuilder();
            builder.CollisionCheck = value;
            return builder.Build();
        }

        public PlanParameters WithSampleResolution(double value)
        {
            if (this.SampleResolution == value)
                return this;
            var builder = this.ToBuilder();
            builder.SampleResolution = value;
            return builder.Build();
        }

        public PlanParameters WithMaxDeviation(double value)
        {
            if (this.MaxDeviation == value)
                return this;
            var builder = this.ToBuilder();
            builder.MaxDeviation = value;
            return builder.Build();
        }

        public Builder ToBuilder() =>
            new Builder(this);

    }
}
