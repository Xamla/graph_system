using System;
using System.Linq;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Holds information like joint position, joint velocities and joint efforts.
    /// </summary>
    /// <seealso cref="JointValues"/>
    public struct JointStates
    {
        /// <summary>
        /// Gets the joint positions.
        /// </summary>
        public JointValues Positions { get; }

        /// <summary>
        /// Gets the joint velocities.
        /// </summary>
        public JointValues Velocities { get; }

        /// <summary>
        /// Gets the joint efforts.
        /// </summary>
        public JointValues Efforts { get; }

        /// <summary>
        /// Creates a new instance of <c>JointValues</c> from the given joint values. All joint values are required to have a joint set that matches the other given joint values.
        /// </summary>
        /// <param name="positions">The position values of the joints.</param>
        /// <param name="velocities">The velocity values of the joints.</param>
        /// <param name="efforts">The effort values of the joints.</param>
        public JointStates(JointValues positions, JointValues velocities = null, JointValues efforts = null)
        {
            this.Positions = positions;
            this.Velocities = velocities;
            this.Efforts = efforts;

            var jointSet = this.JointSet;
            var values = new JointValues[] { positions, velocities, efforts };
            if (jointSet != null && !values.All(x => x == null || x.JointSet.Equals(jointSet)))
                throw new Exception("JointSet values do not match.");
        }

        /// <summary>
        /// Gets the set of joint for which the current <c>JointStates</c> provide information.
        /// </summary>
        public JointSet JointSet =>
            Positions?.JointSet ?? Velocities?.JointSet ?? Efforts?.JointSet;

        /// <summary>
        /// Returns a human readable representation of the <c>JointStates</c> values.
        /// </summary>
        public override string ToString() =>
            $"Positions: {Positions?.ToString() ?? "null"}; Velocities: {Velocities?.ToString() ?? "null"}; Efforts: {Efforts?.ToString() ?? "null"};";
    }
}
