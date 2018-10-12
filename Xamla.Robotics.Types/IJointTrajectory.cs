using System;
using System.Collections.Generic;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Implementations of <c>IJointTrajectory</c> hold a list of joint trajectory points and provide methods to modify the list of points.
    /// </summary>
    /// <seealso cref="JointTrajectoryPoint"/>
    public interface IJointTrajectory
        : IReadOnlyList<JointTrajectoryPoint>
    {
        /// <summary>
        /// Gets the set of joints, that are used in this joint trajectory.
        /// </summary>
        JointSet JointSet { get; }

        /// <summary>
        /// Concatenates two <c>IJointTrajectory</c>.
        /// </summary>
        IJointTrajectory Concat(IJointTrajectory other);

        /// <summary>
        /// Appends the given collection of <c>JointTrajectoryPoint</c>s to the current <c>IJointTrajectory</c>.
        /// </summary>
        IJointTrajectory Append(IEnumerable<JointTrajectoryPoint> source);

        /// <summary>
        /// Returns the sub trajectory defined by the given start index and end index.
        /// </summary>
        IJointTrajectory Sub(int startIndex, int endIndex);

        /// <summary>
        /// Applies the given transform function to all poses in the current trajectory.
        /// </summary>
        IJointTrajectory Transform(Func<JointTrajectoryPoint, int, JointTrajectoryPoint> transform);

        /// <summary>
        /// Gets the flag indicating whether the trajectory is valid. The meaning of this flag depends on the context where the <c>IJointTrajectory</c> object is used.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Tests if the current trajectory contains velocity values.
        /// </summary>
        bool HasVelocity { get; }

        /// <summary>
        /// Tests if the current trajectory contains acceleration values.
        /// </summary>
        bool HasAccelaration { get; }

        /// <summary>
        /// Tests if the current trajectory contains effort values.
        /// </summary>
        bool HasEffort { get; }

        /// <summary>
        /// Gets the collection of joint positions in the current trajectory.
        /// </summary>
        IEnumerable<JointValues> Positions { get; }

        /// <summary>
        /// Gets the collection of joint velocity values in the current trajectory.
        /// </summary>
        IEnumerable<JointValues> Velocities { get; }

        /// <summary>
        /// Gets the collection of joint acceleration values in the current trajectory.
        /// </summary>
        IEnumerable<JointValues> Accelerations { get; }

        /// <summary>
        /// Gets the collection of joint effort values in the current trajectory.
        /// </summary>
        IEnumerable<JointValues> Efforts { get; }

        /// <summary>
        /// Gets the collection which holds the time by when the robot needs to reach the corresponding point of the trajectory.
        /// </summary>
        IEnumerable<TimeSpan> TimesFromStart { get; }

        /// <summary>
        /// Gets the total duration of the trajectory.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets the <c>IJointPath</c> representation of the trajectory, i.e. the time information as well as speed, velocity and a acceleration information is striped from the trajectory, leaving a collection of joint positions.
        /// </summary>
        IJointPath Path { get; }
    }
}
