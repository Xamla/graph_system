using System;
using System.Numerics;
using Xamla.Utilities;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// <c>Twist</c> class contains Linear velocity in m/s and Angular velocity in rad/s described in a specific ROS TF frame.
    /// </summary>
    /// <remarks>
    /// The class also offer a method to compare the equality of two Twist objects.
    /// </remarks>
    public class Twist : IEquatable<Twist>
    {
        /// <summary>
        /// The frame of Linear and Angular velocities.
        /// </summary>
        public string Frame { get; }

        /// <summary>
        /// Linear Velocity in m/s.
        /// </summary>
        public Vector3 Linear { get; }

        /// <summary>
        /// Angular velocity in rad/s.
        /// </summary>
        public Vector3 Angular { get; }

        public static readonly Twist Identity = new Twist(Vector3.Zero, Vector3.Zero);

        public Twist()
            : this(new Vector3(), new Vector3())
        {
        }

        public Twist(Vector3 linear, Vector3 angular, string frame = "")
        {
            this.Frame = frame;
            this.Linear = linear;
            this.Angular = angular;
        }

        /// <summary>
        /// Test if the current Twist equals the given <c>other</c>.
        /// </summary>
        /// <param name="other">Another <c>Twist</c> that should be tested for similarity.</param>
        /// <returns>True if this <c>Twist</c> and the other given <c>Twist</c> are the same object or if their values are equal. False otherwise.</returns>
        public bool Equals(Twist other)
        {
            return object.Equals(this.Linear, other.Linear)
                && object.Equals(this.Angular, other.Angular);
        }

        /// <summary>
        /// Creates a hash code over Frame, Linear and Angular velocities.
        /// </summary>
        public override int GetHashCode() =>
            HashHelper.GetHashCode(this.Frame, this.Linear, this.Angular);

        /// <summary>Creates a human readable text representation of Linear, Angular and Frame.</summary>
        public override string ToString() =>
            $"Linear: {this.Linear}; Angular: {this.Angular}; Frame: '{this.Frame}';";
    }
}
