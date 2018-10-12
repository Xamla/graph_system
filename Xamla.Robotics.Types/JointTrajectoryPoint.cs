using System;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// A <c>JointTrajectoryPoint</c> defines a point on a joint trajectory.
    /// </summary>
    public struct JointTrajectoryPoint
    {
        /// <summary>
        /// Gets the time at which the robot is required to have reached this point.
        /// </summary>
        public TimeSpan TimeFromStart { get; }

        /// <summary>
        /// Gets the joint positions for each joint defined in the <c>JointSet</c> of this point.
        /// </summary>
        public JointValues Positions { get; }

        /// <summary>
        /// Gets the velocities which each joint defined in the <c>JointSet</c> is required to have, when the robot has reached this point.
        /// </summary>
        public JointValues Velocities { get; }

        /// <summary>
        /// Gets the accelerations which each joint defined in the <c>JointSet</c> is required to have, when the robot has reached this point.
        /// </summary>
        public JointValues Accelerations { get; }

        /// <summary>
        /// Gets the effort which each joint defined in the <c>JointSet</c> is required to have, when the robot has reached this point.
        /// </summary>
        public JointValues Effort { get; }

        /// <summary>
        /// Creates a new instance of <c>JointTrajectoryPoint</c>.
        /// </summary>
        /// <param name="timeFromStart">Time at which the robot is required to reach the new point.</param>
        /// <param name="positions">The target joint positions of the new point.</param>
        /// <param name="velocities">Optional: The target velocity that each joint is required to have when the robot reaches the new point. Default: null.</param>
        /// <param name="accelerations">Optional: The target accelerations that each joint is required to have when the robot reaches the new point. Default: null.</param>
        /// <param name="effort">Optional: The target effort that each joint is required to have when the robot reaches the new point. Default: null.</param>
        public JointTrajectoryPoint(TimeSpan timeFromStart, JointValues positions, JointValues velocities = null, JointValues accelerations = null, JointValues effort = null)
        {
            this.TimeFromStart = timeFromStart;
            this.Positions = positions;
            this.Velocities = velocities;
            this.Accelerations = accelerations;
            this.Effort = effort;
        }

        /// <summary>
        /// Creates a new <c>JointTrajectoryPoint</c>, where the given offset is added to time at which the robot is required to reach the point.
        /// </summary>
        /// <param name="offset">The offset that is added to the target time of the current <c>JointTrajectoryPoint</c>.</param>
        /// <returns>A new instance of <c>JointTrajectoryPoint</c>.</returns>
        public JointTrajectoryPoint AddTimeOffset(TimeSpan offset) =>
            new JointTrajectoryPoint(TimeFromStart + offset, Positions, Velocities, Accelerations, Effort);

        public override string ToString() => $"{TimeFromStart} ({Positions})";
    }
}
