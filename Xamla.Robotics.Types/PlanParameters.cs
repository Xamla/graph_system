using System;
using System.Linq;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// A Class that contains parameters needed in Joint Space to interact with Ros Moveit.
    /// </summary>
    public class PlanParameters
    {
        /// <summary>
        /// Builder for creating a <c>PlanParameters</c> instance from its representation.
        /// </summary>
        public class Builder
        {
            /// <summary>
            /// The name of the planning group to control and plan for.
            /// </summary>
            public string MoveGroupName { get; set; }

            /// <summary>
            /// Collection of The Move Group Joints set in a certain order.
            /// </summary>
            public JointSet JointSet { get; set; }

            /// <summary>
            /// Array of Maximum Velocities for each joint in the <c>JointSet</c> in the Move Group [in m/s].
            /// </summary>
            public double[] MaxVelocity { get; set; }

            /// <summary>
            /// Array of Maximum Accelerations for each joint in the <c>JointSet</c> in the Move Group [in m/s^2].
            /// </summary>
            public double[] MaxAcceleration { get; set; }

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

            public Builder()
            {
            }

            /// <summary>
            /// Create <c>Builder</c> object from a <c>PlanParameters</c> object.
            /// </summary>
            /// <param name="instance">The <c>PlanParameters</c> object to be converted to <c>Builder</c> object.</param>
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

            /// <summary>
            /// Convert this <c>Builder</c> object to <c>PlanParameters</c> object.
            /// </summary>
            /// <returns>A <c>PlanParameters</c> object.</returns>
            public PlanParameters Build() =>
                new PlanParameters(this);

            /// <summary>
            /// Scale the max acceleration of all joints by the factor parameter.
            /// </summary>
            /// <param name="factor">Factor to scale acceleration with, Must lie in range (0,1].</param>
            /// <returns>A <c>Builder</c> Object with the same values as this but with accelerations scaled.</returns>
            /// <exception cref="ArgumentOutOfRangeException">throw when the factor parameter lie outside of the range (0,1].</exception>
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

            /// <summary>
            /// Scale the max velocity of all joints by the factor parameter.
            /// </summary>
            /// <param name="factor">Factor to scale velocity with, Must lie in range (0,1].</param>
            /// <returns>A <c>Builder</c> Object with the same values as this but with velocity scaled.</returns>
            /// <exception cref="ArgumentOutOfRangeException">throw when the factor parameter lie outside of the range (0,1].</exception>
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

        /// <summary>
        /// The name of the planning group to control and plan for.
        /// </summary>
        public string MoveGroupName { get; }

        /// <summary>
        /// Collection of The Move Group Joint set in a certain order.
        /// </summary>
        public JointSet JointSet { get; }

        /// <summary>
        /// Array of maximum velocities for each joint in the <c>JointSet</c> in the Move Group [in m/s].
        /// </summary>
        public double[] MaxVelocity { get; }

        /// <summary>
        /// Array of maximum accelerations for each joint in the <c>JointSet</c> in the Move Group [in m/s^2].m
        /// </summary>
        public double[] MaxAcceleration { get; }

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

        public PlanParameters()
            : this(new Builder())
        {
        }

        /// <summary>
        /// Creates a new <c>PlanParameters</c> object in Joint Space for a MoveGroup
        /// </summary>
        /// <param name="moveGroupName">The name of the planning group to control and plan for.</param>
        /// <param name="joints">Collection of The Move Group Joints set in a certain order.</param>
        /// <param name="maxVelocity">Array of maximum velocities for each joint in the <c>JointSet</c> in the Move Group [in m/s].</param>
        /// <param name="maxAcceleration">Array of maximum accelerations for each joint in the <c>JointSet</c> in the Move Group [in m/s^2].</param>
        /// <param name="sampleResolution">The time resolution for trajectory generation (dt) [in Second].</param>
        /// <param name="collisionCheck">Indicates whether generated trajectories should be tested for collision.</param>
        /// <param name="maxDeviation">In case waypoints are used, this parameter will allow blending between the segments.</param>
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

        /// <summary>
        /// Create a new <c>PlanParameters</c> object from a <c>Builder</c> object.
        /// </summary>
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

        /// <summary>
        /// Returns a <c>PlanParameters</c> object with the same values as this but with the specified CollisionCheck value.
        /// </summary>
        public PlanParameters WithCollisionCheck(bool value)
        {
            if (this.CollisionCheck == value)
                return this;
            var builder = this.ToBuilder();
            builder.CollisionCheck = value;
            return builder.Build();
        }

        /// <summary>
        /// Returns a <c>PlanParameters</c> object with the same values as this but with the specified SampleResolution value.
        /// </summary>
        public PlanParameters WithSampleResolution(double value)
        {
            if (this.SampleResolution == value)
                return this;
            var builder = this.ToBuilder();
            builder.SampleResolution = value;
            return builder.Build();
        }

        /// <summary>
        /// Returns a <c>PlanParameters</c> object with the same values as this but with the specified MaxDeviation value.
        /// </summary>
        public PlanParameters WithMaxDeviation(double value)
        {
            if (this.MaxDeviation == value)
                return this;
            var builder = this.ToBuilder();
            builder.MaxDeviation = value;
            return builder.Build();
        }

        /// <summary>
        /// Convert this <c>PlanParameters</c> object to <c>Builder</c> object.
        /// </summary>
        public Builder ToBuilder() =>
            new Builder(this);

    }
}
