using System;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// <c>JointLimits</c> hold the positional limits as well as velocity and accelerations limits for all joints defined in a <c>JointSet</c>.
    /// </summary>
    /// <seealso cref="JointSet"/>
    public class JointLimits
    {
        /// <summary>
        /// Gets the affected set of joints.
        /// </summary>
        public JointSet JointSet { get; }

        /// <summary>
        /// Gets the maximal allowed acceleration for the joints in the specified <c>JointSet</c>.
        /// </summary>
        public double?[] MaxAcceleration { get; }

        /// <summary>
        /// Gets the maximal allowed velocity for the joints in the specified <c>JointSet</c>.
        /// </summary>
        public double?[] MaxVelocity { get; }

        /// <summary>
        /// Gets the minimal allowed position for the joints in the specified <c>JointSet</c>.
        /// </summary>
        public double?[] MinPosition { get; }

        /// <summary>
        /// Gets the maximal allowed position for the joints in the specified <c>JointSet</c>.
        /// </summary>
        public double?[] MaxPosition { get; }

        /// <summary>
        /// Creates a new instance of <c>JointLimits</c>.
        /// </summary>
        /// <param name="jointSet"></param>
        /// <param name="maxVelocity"></param>
        /// <param name="maxAcceleration"></param>
        /// <param name="minPosition"></param>
        /// <param name="maxPosition"></param>
        /// <exception cref="ArgumentNullException">Thrown when any of the given parameters is null</exception>
        /// <exception cref="ArgumentException">Thrown when there is a mismatch in the amount of joints in the joint set and the length of the given limit arrays.</exception>
        public JointLimits(JointSet jointSet, double?[] maxVelocity, double?[] maxAcceleration, double?[] minPosition, double?[] maxPosition)
        {
            if (jointSet == null)
                throw new ArgumentNullException(nameof(jointSet));
            if (maxVelocity == null)
                throw new ArgumentNullException(nameof(maxVelocity));
            if (maxAcceleration == null)
                throw new ArgumentNullException(nameof(maxAcceleration));
            if (minPosition == null)
                throw new ArgumentNullException(nameof(MinPosition));
            if (maxPosition == null)
                throw new ArgumentNullException(nameof(MaxPosition));

            if (jointSet.Count != maxVelocity.Length)
                throw new ArgumentException("Number of joints does not match length of velocity limits array.", nameof(maxVelocity));

            if (jointSet.Count != maxAcceleration.Length)
                throw new ArgumentException("Acceleration limits array length does not match number of joints.", nameof(maxAcceleration));
            if (jointSet.Count != minPosition.Length)
                throw new ArgumentException("Minimum position limits array length does not match number of joints.", nameof(minPosition));
            if (jointSet.Count != maxPosition.Length)
                throw new ArgumentException("Maximum position limits array length does not match number of joints.", nameof(maxPosition));

            this.JointSet = jointSet;
            this.MaxAcceleration = maxAcceleration;
            this.MaxVelocity = maxVelocity;
            this.MinPosition = minPosition;
            this.MaxPosition = maxPosition;
        }
    }
}
