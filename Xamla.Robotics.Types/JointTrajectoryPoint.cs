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

        public double TimeFromStartSeconds =>
            TimeFromStart.TotalSeconds;

        /// <summary>
        /// The common joint set used by the members Position, Velocities, Accelerations, Efforts.
        /// </summary>
        public JointSet JointSet =>
            Positions?.JointSet;

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
        public JointValues Efforts { get; }

        /// <summary>
        /// Creates a new instance of <c>JointTrajectoryPoint</c>.
        /// </summary>
        /// <param name="timeFromStart">Time at which the robot is required to reach the new point.</param>
        /// <param name="positions">The target joint positions of the new point.</param>
        /// <param name="velocities">Optional: The target velocity that each joint is required to have when the robot reaches the new point. Default: null.</param>
        /// <param name="accelerations">Optional: The target accelerations that each joint is required to have when the robot reaches the new point. Default: null.</param>
        /// <param name="efforts">Optional: The target effort that each joint is required to have when the robot reaches the new point. Default: null.</param>
        /// <exception cref="ArgumentException">Thrown when  <paramref name="positions"/>,  <paramref name="velocities"/>,  <paramref name="accelerations"/> or  <paramref name="efforts"/> have incompatible <c>JointSet</c>.</exception>
        public JointTrajectoryPoint(TimeSpan timeFromStart, JointValues positions, JointValues velocities = null, JointValues accelerations = null, JointValues efforts = null)
        {
            this.TimeFromStart = timeFromStart;
            this.Positions = positions ?? throw new ArgumentNullException(nameof(positions));

            // ensure alle components use the same joint set
            JointSet jointSet = positions.JointSet;
            if (velocities != null && velocities != JointValues.Empty && !velocities.JointSet.Equals(jointSet))
                throw new ArgumentException($"JointValues '{nameof(velocities)}' have incompatible JointSet.", nameof(Velocities));
            if (accelerations != null && accelerations != JointValues.Empty && !accelerations.JointSet.Equals(jointSet))
                throw new ArgumentException($"JointValues '{nameof(accelerations)}' have incompatible JointSet.", nameof(accelerations));
            if (efforts != null && efforts != JointValues.Empty && !efforts.JointSet.Equals(jointSet))
                throw new ArgumentException($"JointValues '{nameof(efforts)}' have incompatible JointSet.", nameof(efforts));

            this.Velocities = velocities;
            this.Accelerations = accelerations;
            this.Efforts = efforts;
        }

        public JointTrajectoryPoint(double timeFromStartSeconds, JointValues positions, JointValues velocities = null, JointValues accelerations = null, JointValues efforts = null)
            : this(TimeSpan.FromSeconds(timeFromStartSeconds), positions, velocities, accelerations, efforts)
        {
        }

        


        public JointTrajectoryPoint WithTimeFromStart(TimeSpan timeFromStart) =>
            new JointTrajectoryPoint(timeFromStart, Positions, Velocities, Accelerations, Efforts);

        /// <summary>
        /// Creates a new <c>JointTrajectoryPoint</c>, where the given offset is added to time at which the robot is required to reach the point.
        /// </summary>
        /// <param name="offset">The offset that is added to the target time of the current <c>JointTrajectoryPoint</c>.</param>
        /// <returns>A new instance of <c>JointTrajectoryPoint</c>.</returns>
        public JointTrajectoryPoint AddTimeOffset(TimeSpan offset) =>
            WithTimeFromStart(TimeFromStart + offset);


        public JointTrajectoryPoint InterpolateCubic(JointTrajectoryPoint point1, double t=0.5) =>
            JointTrajectoryPoint.InterpolateCubic(this, point1, t);


        /// <summary>
        /// Cubic Interpolation between two JointTrajectoryPoints
        /// </summary>
        /// <param name="t"></param>
        /// <param name="point0">The first point to be interpolated.</param>
        /// <param name="point1">The second point to be interpolated.</param>
        /// <returns></returns>
        public static JointTrajectoryPoint InterpolateCubic(JointTrajectoryPoint point0, JointTrajectoryPoint point1, double t=0.5)
        {
            double t0 = point0.TimeFromStart.TotalSeconds;
            double t1 = point1.TimeFromStart.TotalSeconds;
            double dt = t1 - t0;

            JointSet jointSet = point1.Positions.JointSet;
            if (dt < 1e-6)
            {
                return new JointTrajectoryPoint(
                    timeFromStart: TimeSpan.FromSeconds(t0 + dt),
                    positions: point1.Positions,
                    velocities: JointValues.Zero(jointSet)
                );
            }

            double[] pos = point0.Positions.ToArray();
            double[] vel = point0.Velocities.ToArray();
            JointValues p0 = point0.Positions;
            JointValues p1 = point1.Positions;
            JointValues v0 = point0.Velocities;
            JointValues v1 = point1.Velocities;

            t = Math.Max(t - t0, 0);
            for (int i = 0; i < p0.Count; i++)
            {
                double a = p0[i];
                double b = v0[i];
                double c = (-3.0 * p0[i] + 3.0 * p1[i] - 2.0 * dt * v0[i] - dt * v1[i]) / Math.Pow(dt, 2);
                double d = (2.0 * p0[i] - 2.0 * p1[i] + dt * v0[i] + dt * v1[i]) / Math.Pow(dt, 3);
                pos[i] = a + b * t + c * Math.Pow(t, 2) + d * Math.Pow(t, 3);
                vel[i] = b + 2.0 * c * t + 3.0 * d * Math.Pow(t, 2);
            }

            return new JointTrajectoryPoint(
                timeFromStart: TimeSpan.FromSeconds(t),
                positions: new JointValues(jointSet, pos),
                velocities: new JointValues(jointSet, vel)
            );
        }

        /// <summary>
        /// Creates a new instance of JointTrajectory as a result of the merge operation.
        /// </summary>
        /// <param name="other">TrajectoryPoints to merge with the current TrajectoryPoint</param>
        /// <returns>New instance of JointTrajectoryPoint which contains the merged JointTrajectoryPoints</returns>
        /// <exception cref="Exception">Thrown when TimeFromStart of the points do not match.</exception>
        public JointTrajectoryPoint Merge(JointTrajectoryPoint other)
        {
            if (this.TimeFromStart != other.TimeFromStart)
                throw new Exception("Merge conflict: TimeFromStart in other JointTrajectoryPoint is not equal to TimeFromStart of this instance.");

            return new JointTrajectoryPoint(
                this.TimeFromStart,
                this.Positions.Merge(other.Positions),
                this.Velocities?.Merge(other.Velocities),
                this.Accelerations?.Merge(other.Accelerations),
                this.Efforts?.Merge(other.Efforts)
            );
        }

        public override string ToString() => $"{TimeFromStart} ({Positions})";
    }
}
