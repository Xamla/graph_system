using System;
using System.Linq;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// A Class that contains parameters needed in Task Space to interact with Ros Moveit.
    /// </summary>
    public class TaskSpacePlanParameters
    {
        /// <summary>
        /// Builder for creating a <c>TaskSpacePlanParameters</c> instance from its representation.
        /// </summary>
        public class Builder
        {
            /// <summary>
            /// The name of the End Effector Frame.
            /// </summary>
            public string EndEffectorName { get; set; }
            /// <summary>
            /// Max linear velocity of the end effector in Task Space xyz.
            /// </summary>
            public double MaxXYZVelocity { get; set; }
            /// <summary>
            /// Max linear Acceleration of the end effector in Task Space xyz.
            /// </summary>
            public double MaxXYZAcceleration { get; set; }
            /// <summary>
            /// Maximum angular velocity of the end effector in Task Space xyz.
            /// </summary>
            public double MaxAngularVelocity { get; set; }
            /// <summary>
            /// Maximum angular acceleration of the end effector in Task Space xyz.
            /// </summary>
            public double MaxAngularAcceleration { get; set; }
            /// <summary>
            /// Indicates whether generated trajectories should be tested for collision.
            /// </summary>
            public bool CollisionCheck { get; set; } = true;
            /// <summary>
            /// The time resolution for trajectory generation (dt) [in Second].
            /// </summary>
            public double SampleResolution { get; set; } = 0.008;       // 125 Hz
            /// <summary>
            /// In case waypoints are used, this parameter will allow blending between the segments.
            /// </summary>
            public double MaxDeviation { get; set; } = 0.2;
            /// <summary>
            /// Safty parameter to check if the redundancy resolution switches during the trajectory generation process.
            /// </summary>
            public double IkJumpThreshold { get; set; } = 1.2;

            public Builder()
            {
            }

            /// <summary>
            /// Create <c>Builder</c> object from a <c>PlaTaskSpacePlanParametersnParameters</c> object.
            /// </summary>
            /// <param name="instance">The <c>TaskSpacePlanParameters</c> object to be converted to <c>Builder</c> object.</param>
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

            /// <summary>
            /// Convert this <c>Builder</c> object to <c>TaskSpacePlanParameters</c> object.
            /// </summary>
            /// <returns>A <c>TaskSpacePlanParameters</c> object.</returns>
            public TaskSpacePlanParameters Build() =>
                new TaskSpacePlanParameters(this);

            /// <summary>
            /// Scale the Linear and Angular Accelerations of the End Effector by the factor parameter.
            /// </summary>
            /// <param name="factor">Factor to scale accelerations with, Must lie in range (0,1].</param>
            /// <returns>A <c>Builder</c> Object with same values as this but with the acceleration scaled.</returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when the factor parameter lie outside of the range (0,1].</exception>
            public Builder ScaleAcceleration(double factor)
            {
                if (!(factor > 0 && factor <= 1))
                    throw new ArgumentOutOfRangeException(nameof(factor), "Argument `factor` must lie in range(0 - 1).");

                this.MaxAngularAcceleration *= factor;
                this.MaxXYZAcceleration *= factor;
                return this;
            }

            /// <summary>
            /// Scale the Linear and Angular Velocities of the End Effector by the factor parameter.
            /// </summary>
            /// <param name="factor">Factor to scale velocity with, Must lie in range (0,1].</param>
            /// <returns>A <c>Builder</c> Object with same values as this but with the velocity scaled.</returns>
            /// <exception cref="ArgumentOutOfRangeException">throw when the factor parameter lie outside of the range (0,1].</exception>
            public Builder ScaleVelocity(double factor)
            {
                if (!(factor > 0 && factor <= 1))
                    throw new ArgumentOutOfRangeException(nameof(factor), "Argument `factor` must lie in range(0 - 1).");

                    this.MaxAngularVelocity *= factor;
                    this.MaxXYZVelocity *= factor;
                return this;
            }
        }

        /// <summary>
        /// The name of the End Effector Frame.
        /// </summary>
        public string EndEffectorName { get; }
        /// <summary>
        /// Max linear velocity of the end effector in Task Space xyz.
        /// </summary>
        public double MaxXYZVelocity { get; }
        /// <summary>
        /// Max linear acceleration of the end effector in Task Space xyz.
        /// </summary>
        public double MaxXYZAcceleration { get; }
        /// <summary>
        /// Maximum angular velocity of the end effector in Task Space xyz.
        /// </summary>
        public double MaxAngularVelocity { get; }
        /// <summary>
        /// Maximum angular acceleration of the end effector in Task Space xyz.
        /// </summary>
        public double MaxAngularAcceleration { get; }
        /// <summary>
        /// Indicates whether generated trajectories should be tested for collision.
        /// </summary>
        public bool CollisionCheck { get; }
        /// <summary>
        /// The time resolution for trajectory generation (dt) [in Second].
        /// </summary>
        public double SampleResolution { get; }
        /// <summary>
        /// In case waypoints are used, this parameter will allow blending between the segments.
        /// </summary>
        public double MaxDeviation { get; }
        /// <summary>
        /// Safty parameter to check if the redundancy resolution switches during the trajectory generation process.
        /// </summary>
        public double IkJumpThreshold { get; }

        public TaskSpacePlanParameters()
            : this(new Builder())
        {
        }

        /// <summary>
        /// Creates a new <c>TaskSpacePlanParameters</c> object in Task Space.
        /// </summary>
        /// <param name="endEffectorName">The name of the End Effector Frame.</param>
        /// <param name="maxXYZVelocity">Max linear velocity of the end effector in Task Space xyz. </param>
        /// <param name="maxXYZAcceleration">Max linear acceleration of the end effector in Task Space xyz. </param>
        /// <param name="maxAngularVelocity">Maximum angular velocity of the end effector in Task Space xyz.</param>
        /// <param name="maxAngularAcceleration">Maximum angular acceleration of the end effector in Task Space xyz.</param>
        /// <param name="sampleResolution">The time resolution for trajectory generation (dt) [in Second].</param>
        /// <param name="collisionCheck">Indicates whether generated trajectories should be tested for collision.</param>
        /// <param name="maxDeviation">In case waypoints are used, this parameter will allow blending between the segments.</param>
        /// <param name="ikJumpThreshold">Safty parameter to check if the redundancy resolution switches during the trajectory generation process</param>
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

        /// <summary>
        /// Create a new <c>TaskSpacePlanParameters</c> object from a <c>Builder</c> object.
        /// </summary>
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

        /// <summary>
        /// Returns a <c>TaskSpacePlanParameters</c> object with same values as this but with the specified CollisionCheck value.
        /// </summary>
        public TaskSpacePlanParameters WithCollisionCheck(bool value)
        {
            if (this.CollisionCheck == value)
                return this;
            var builder = this.ToBuilder();
            builder.CollisionCheck = value;
            return builder.Build();
        }

        /// <summary>
        /// Returns a <c>TaskSpacePlanParameters</c> object with same values as this but with the specified SampleResolution value.
        /// </summary>
        public TaskSpacePlanParameters WithSampleResolution(double value)
        {
            if (this.SampleResolution == value)
                return this;
            var builder = this.ToBuilder();
            builder.SampleResolution = value;
            return builder.Build();
        }

        /// <summary>
        /// Returns a <c>TaskSpacePlanParameters</c> object with same values as this but with the specified IkJumpThreshold value.
        /// </summary>
        public TaskSpacePlanParameters WithIkJumpThreshold(double value)
        {
            if (this.IkJumpThreshold == value)
                return this;
            var builder = this.ToBuilder();
            builder.IkJumpThreshold = value;
            return builder.Build();
        }

        /// <summary>
        /// Returns a <c>TaskSpacePlanParameters</c> object with same values as this but with the specified MaxDeviation value.
        /// </summary>
        public TaskSpacePlanParameters WithMaxDeviation(double value)
        {
            if (this.MaxDeviation == value)
                return this;
            var builder = this.ToBuilder();
            builder.MaxDeviation = value;
            return builder.Build();
        }

        /// <summary>
        /// Convert this <c>TaskSpacePlanParameters</c> object to <c>Builder</c> object.
        /// </summary>
        public Builder ToBuilder() =>
            new Builder(this);

    }
}
