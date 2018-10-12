namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Holds the acceleration and velocity limits of an end effector.
    /// </summary>
    public class EndEffectorLimits
    {
        /// <summary>
        /// Gets the allowed maximal linear acceleration in m/s².
        /// </summary>
        public double MaxXYZAcceleration { get; }

        /// <summary>
        /// Gets the allowed maximal linear velocity in m/s.
        /// </summary>
        public double MaxXYZVelocity { get; }

        /// <summary>
        /// Gets the allowed maximal angular acceleration in rad/s².
        /// </summary>
        public double MaxAngularAcceleration { get; }

        /// <summary>
        /// Gets the allowed maximal angular velocity in rad/s.
        /// </summary>
        public double MaxAngularVelocity { get; }

        /// <summary>
        /// Creates a new instance of <c>EndEffectorLimits</c> from the given limits.
        /// </summary>
        /// <param name="maxXYZVelocity">The allowed maximal linear velocity in m/s.</param>
        /// <param name="maxXYZAcceleration">The allowed maximal linear acceleration in m/s².</param>
        /// <param name="maxAngularVelocity">The allowed maximal angular velocity in rad/s.</param>
        /// <param name="maxAngularAcceleration">The allowed maximal angular acceleration in rad/s².</param>
        public EndEffectorLimits(double maxXYZVelocity, double maxXYZAcceleration, double maxAngularVelocity, double maxAngularAcceleration)
        {
            this.MaxXYZAcceleration = maxXYZAcceleration;
            this.MaxXYZVelocity = maxXYZVelocity;
            this.MaxAngularAcceleration = maxAngularAcceleration;
            this.MaxAngularVelocity = maxAngularVelocity;
        }
    }
}
