namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Holds the acceleration and velocity limits of an end effector.
    /// </summary>
    public class EndEffectorLimits
    {
        /// <summary>
        /// Gets the allowed maximal linear acceleration.
        /// </summary>
        public double MaxXYZAcceleration { get; }

        /// <summary>
        /// Gets the allowed maximal linear velocity.
        /// </summary>
        public double MaxXYZVelocity { get; }

        /// <summary>
        /// Gets the allowed maximal angular acceleration.
        /// </summary>
        public double MaxAngularAcceleration { get; }

        /// <summary>
        /// Gets the allowed maximal angular velocity.
        /// </summary>
        public double MaxAngularVelocity { get; }

        /// <summary>
        /// Creates a new instance of <c>EndEffectorLimits</c> from the given limits.
        /// </summary>
        /// <param name="maxXYZVelocity">The allowed maximal linear velocity.</param>
        /// <param name="maxXYZAcceleration">The allowed maximal linear acceleration.</param>
        /// <param name="maxAngularVelocity">The allowed maximal angular velocity.</param>
        /// <param name="maxAngularAcceleration">The allowed maximal angular acceleration.</param>
        public EndEffectorLimits(double maxXYZVelocity, double maxXYZAcceleration, double maxAngularVelocity, double maxAngularAcceleration)
        {
            this.MaxXYZAcceleration = maxXYZAcceleration;
            this.MaxXYZVelocity = maxXYZVelocity;
            this.MaxAngularAcceleration = maxAngularAcceleration;
            this.MaxAngularVelocity = maxAngularVelocity;
        }
    }
}
