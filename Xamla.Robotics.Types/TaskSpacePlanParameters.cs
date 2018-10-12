using System;
using System.Linq;

namespace Xamla.Robotics.Types
{
    public class TaskSpacePlanParameters
    {
        public class Builder
        {
            public string EndEffectorName { get; set; }
            public double MaxXYZVelocity { get; set; }
            public double MaxXYZAcceleration { get; set; }
            public double MaxAngularVelocity { get; set; }
            public double MaxAngularAcceleration { get; set; }
            public bool CollisionCheck { get; set; } = true;
            public double SampleResolution { get; set; } = 0.008;       // 125 Hz
            public double MaxDeviation { get; set; } = 0.2;
            public double IkJumpThreshold { get; set; } = 1.2;

            public Builder()
            {
            }

            public Builder(TaskSpacePlanParameters instance)
            {
                this.EndEffectorName = instance.EndEffectorName;
                this.MaxXYZVelocity = instance.MaxXYZVelocity;
                this.MaxXYZAcceleration = instance.MaxXYZAcceleration;
                this.MaxAngularVelocity = instance.MaxAngularVelocity;
                this.MaxAngularAcceleration = instance.MaxAngularAcceleration;
                this.CollisionCheck = instance.CollisionCheck;
                this.SampleResolution = instance.SampleResolution;
                this.MaxDeviation = instance.MaxDeviation;
                this.IkJumpThreshold = instance.IkJumpThreshold;
            }

            public TaskSpacePlanParameters Build() =>
                new TaskSpacePlanParameters(this);

            public Builder ScaleAcceleration(double factor)
            {
                if (!(factor > 0 && factor <= 1))
                    throw new ArgumentOutOfRangeException(nameof(factor), "Argument `factor` must lie in range(0 - 1).");

                this.MaxAngularAcceleration *= factor;
                this.MaxXYZAcceleration *= factor;
                return this;
            }

            public Builder ScaleVelocity(double factor)
            {
                if (!(factor > 0 && factor <= 1))
                    throw new ArgumentOutOfRangeException(nameof(factor), "Argument `factor` must lie in range(0 - 1).");

                    this.MaxAngularVelocity *= factor;
                    this.MaxXYZVelocity *= factor;
                return this;
            }
        }

        public string EndEffectorName { get; }
        public double MaxXYZVelocity { get; }
        public double MaxXYZAcceleration { get; }
        public double MaxAngularVelocity { get; }
        public double MaxAngularAcceleration { get; }
        public bool CollisionCheck { get; }
        public double SampleResolution { get; }
        public double MaxDeviation { get; }
        public double IkJumpThreshold { get; }

        public TaskSpacePlanParameters()
            : this(new Builder())
        {
        }

        public TaskSpacePlanParameters(string endEffectorName, double maxXYZVelocity = 0.2, double maxXYZAcceleration = 0.4, double maxAngularVelocity = 0.026179938779915, double maxAngularAcceleration = 0.10471975511966, double sampleResolution = 0.008, bool collisionCheck = true, double maxDeviation = 0.2, double ikJumpThreshold = 0.2)
            : this(new Builder() {
                EndEffectorName = endEffectorName,
                MaxXYZVelocity = maxXYZVelocity,
                MaxXYZAcceleration = maxXYZAcceleration,
                MaxAngularVelocity = maxAngularVelocity,
                MaxAngularAcceleration = maxAngularAcceleration,
                CollisionCheck = collisionCheck,
                SampleResolution = sampleResolution,
                MaxDeviation = maxDeviation,
                IkJumpThreshold = ikJumpThreshold
            })
        {
        }

        public TaskSpacePlanParameters(Builder builder)
        {
            this.EndEffectorName = builder.EndEffectorName;
            this.MaxXYZVelocity = builder.MaxXYZVelocity;
            this.MaxXYZAcceleration = builder.MaxXYZAcceleration;
            this.MaxAngularVelocity = builder.MaxAngularVelocity;
            this.MaxAngularAcceleration = builder.MaxAngularAcceleration;
            this.CollisionCheck = builder.CollisionCheck;
            this.SampleResolution = builder.SampleResolution;
            this.MaxDeviation = builder.MaxDeviation;
            this.IkJumpThreshold = builder.IkJumpThreshold;
        }

        public TaskSpacePlanParameters WithCollisionCheck(bool value)
        {
            if (this.CollisionCheck == value)
                return this;
            var builder = this.ToBuilder();
            builder.CollisionCheck = value;
            return builder.Build();
        }

        public TaskSpacePlanParameters WithSampleResolution(double value)
        {
            if (this.SampleResolution == value)
                return this;
            var builder = this.ToBuilder();
            builder.SampleResolution = value;
            return builder.Build();
        }

        public TaskSpacePlanParameters WithIkJumpThreshold(double value)
        {
            if (this.IkJumpThreshold == value)
                return this;
            var builder = this.ToBuilder();
            builder.IkJumpThreshold = value;
            return builder.Build();
        }

        public TaskSpacePlanParameters WithMaxDeviation(double value)
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
