using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Used to store meta data about a trajectory.
    /// </summary>
    [Flags]
    public enum JointTrajectoryFlags
    {
        /// <summary>
        /// The trajectory is marked as valid.
        /// </summary>
        IsValid = 1 << 0,
        /// <summary>
        /// The trajectory contains velocity values.
        /// </summary>
        HasVelocity = 1 << 1,
        /// <summary>
        /// The trajectory contains acceleration values.
        /// </summary>
        HasAcceleration = 1 << 2,
        /// <summary>
        /// The trajectory contains effort values.
        /// </summary>
        HasEffort = 1 << 3,
    }

    /// <summary>
    /// A <c>JointTrajectory</c> holds a list of joint trajectory points and provide methods to modify the list of points.
    /// </summary>
    /// <seealso cref="JointTrajectoryPoint"/>
    public class JointTrajectory
        : IJointTrajectory
    {
        /// <summary>
        /// Creates a new empty instance of <c>JointTrajectory</c>.
        /// </summary>
        public static readonly JointTrajectory Empty = new JointTrajectory(JointSet.Empty, Enumerable.Empty<JointTrajectoryPoint>());

        readonly JointSet joints;
        readonly List<JointTrajectoryPoint> points;
        readonly JointTrajectoryFlags flags;

        /// <summary>
        /// Creates a new instance of <c>JointTrajectory</c> from the given <c>JointSet</c> and the collection of <c>JointTrajectoryPoint</c>
        /// </summary>
        /// <param name="joints">The set of joints that is used for the new trajectory.</param>
        /// <param name="points">The collection of points which should form the trajectory. The points need to meed the following requirements:
        /// <list type="number">
        /// <item><para>The time from start values need to be ascending.</para></item>
        /// <item><para>The position value needs to be set.</para></item>
        /// <item><para>The joint sets used in the joint values, like positions, velocities etc, need to match the given <c>JointSet</c> <paramref name="joints"/>.</para></item>
        /// </list>
        /// </param>
        /// <param name="valid">Indicates if the new trajectory is valid.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="joints"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the criteria for the points defined in <paramref name="points"/> are not fulfilled.</exception>
        public JointTrajectory(JointSet joints, IEnumerable<JointTrajectoryPoint> points, bool valid = true)
        {
            this.joints = joints ?? throw new ArgumentNullException(nameof(joints));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            this.points = points.ToList();

            JointTrajectoryFlags flags = JointTrajectoryFlags.HasEffort | JointTrajectoryFlags.HasVelocity | JointTrajectoryFlags.HasAcceleration;

            if (valid)
            {
                flags |= JointTrajectoryFlags.IsValid;
            }

            // consistency checks for provided trajectory points
            TimeSpan lastTime = TimeSpan.Zero;
            foreach (var point in this.points)
            {
                if (point.TimeFromStart < lastTime)
                    throw new ArgumentException("TimeFromStart values of trajectory points must be ascending.", nameof(points));

                lastTime = point.TimeFromStart;

                if (point.Positions == null)
                    throw new ArgumentException("Position of trajectory point is null.", nameof(points));

                if (!point.Positions.JointSet.Equals(joints))
                    throw new ArgumentException("Provided points have position values for different joint sets.", nameof(points));

                if (point.Velocities != null && point.Velocities.Count > 0)
                {
                    if (!point.Velocities.JointSet.Equals(joints))
                        throw new ArgumentException("Provided points have velocity values for different joint sets.", nameof(points));
                }
                else
                {
                    flags &= ~JointTrajectoryFlags.HasVelocity;
                }

                if (point.Accelerations != null && point.Accelerations.Count > 0)
                {
                    if (!point.Velocities.JointSet.Equals(joints))
                        throw new ArgumentException("Provided points have acceleration values for different joint sets.", nameof(points));
                }
                else
                {
                    flags &= ~JointTrajectoryFlags.HasAcceleration;
                }

                if (point.Efforts != null && point.Efforts.Count > 0)
                {
                    if (!point.Velocities.JointSet.Equals(joints))
                        throw new ArgumentException("Provided points have effort values for different joint sets.", nameof(points));
                }
                else
                {
                    flags &= ~JointTrajectoryFlags.HasEffort;
                }
            }

            this.flags = flags;
        }

        /// <summary>
        /// Accesses the <c>JointTrajectoryPoint</c> at the given index.
        /// </summary>
        public JointTrajectoryPoint this[int index] =>
            points[index];

        /// <summary>
        /// Gets the flag indicating whether the trajectory is valid. The meaning of this flag depends on the context where the <c>IJointTrajectory</c> object is used.
        /// </summary>
        public bool IsValid =>
            flags.HasFlag(JointTrajectoryFlags.IsValid);

        /// <summary>
        /// Tests if the current trajectory contains velocity values.
        /// </summary>
        public bool HasVelocity =>
            flags.HasFlag(JointTrajectoryFlags.HasVelocity);

        /// <summary>
        /// Tests if the current trajectory contains acceleration values.
        /// </summary>
        public bool HasAccelaration =>
            flags.HasFlag(JointTrajectoryFlags.HasAcceleration);

        /// <summary>
        /// Tests if the current trajectory contains effort values.
        /// </summary>
        public bool HasEffort =>
            flags.HasFlag(JointTrajectoryFlags.HasEffort);

        /// <summary>
        /// Gets the collection of joint positions in the current trajectory.
        /// </summary>
        public IEnumerable<JointValues> Positions =>
            points.Select(x => x.Positions);

        /// <summary>
        /// Gets the collection of joint velocity values in the current trajectory.
        /// </summary>
        public IEnumerable<JointValues> Velocities =>
            points.Select(x => x.Velocities);

        /// <summary>
        /// Gets the collection of joint acceleration values in the current trajectory.
        /// </summary>
        public IEnumerable<JointValues> Accelerations =>
            points.Select(x => x.Accelerations);

        /// <summary>
        /// Gets the collection of joint effort values in the current trajectory.
        /// </summary>
        public IEnumerable<JointValues> Efforts =>
            points.Select(x => x.Efforts);

        /// <summary>
        /// Gets the collection which holds the time by when the robot needs to reach the corresponding point of the trajectory.
        /// </summary>
        public IEnumerable<TimeSpan> TimesFromStart =>
            points.Select(x => x.TimeFromStart);

        /// <summary>
        /// Gets the total duration of the trajectory.
        /// </summary>
        public TimeSpan Duration =>
            this.points.Count == 0 ? TimeSpan.Zero : this.points[this.points.Count - 1].TimeFromStart;

        /// <summary>
        /// Gets the number of points included in the current trajectory.
        /// </summary>
        public int Count =>
            points.Count;

        /// <summary>
        /// Gets the <c>IJointPath</c> representation of the trajectory, i.e. the time information as well as speed, velocity and a acceleration information is striped from the trajectory, leaving a collection of joint positions.
        /// </summary>
        public IJointPath Path =>
            new JointPath(joints, this.Positions);

        /// <summary>
        /// Gets the set of joints which is used in the current trajectory.
        /// </summary>
        public JointSet JointSet => joints;

        /// <summary>
        /// Appends the given collection of <c>JointTrajectoryPoint</c>s to the current trajectory.
        /// </summary>
        /// <param name="source">The collection of points that should be appended to the current trajectory. The points need to fulfill the same criteria as described in the constructor of <see cref="JointTrajectory"/>.</param>
        /// <returns>A new instance of <c>JointTrajectory</c>.</returns>
        public IJointTrajectory Append(IEnumerable<JointTrajectoryPoint> source)
        {
            var offset = this.Duration;
            return new JointTrajectory(joints, points.Concat(source.Select(x => x.AddTimeOffset(offset))), this.IsValid);
        }

        /// <summary>
        /// Concatenates the current joint trajectory and the given other joint trajectory.
        /// </summary>
        /// <param name="other">The joint trajectory that the current joint trajectory should be concatenated with.</param>
        /// <returns>A new instance of <c>JointTrajectory</c>.</returns>
        public IJointTrajectory Concat(IJointTrajectory other)
        {
            var offset = this.Duration;
            return new JointTrajectory(joints, points.Concat(other.Select(x => x.AddTimeOffset(offset))), this.IsValid && other.IsValid);
        }

        /// <summary>
        /// Returns an enumerator over the points of the current trajectory.
        /// </summary>
        public IEnumerator<JointTrajectoryPoint> GetEnumerator() =>
            points.GetEnumerator();

        IEnumerable<JointTrajectoryPoint> Slice(int startIndex, int endIndex)
        {
            if (startIndex < 0 || startIndex >= points.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (endIndex < startIndex || endIndex >= points.Count)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            for (int i = startIndex; i < endIndex; ++i)
                yield return points[i];
        }

        /// <summary>
        /// Returns the sub trajectory in the range [startIndex, endIndex].
        /// </summary>
        /// <param name="startIndex">The index where the sub trajectory should begin.</param>
        /// <param name="endIndex">The index where the sub trajectory should end.</param>
        /// <returns>A new instance of <c>JointTrajectory</c></returns>
        /// TODO: ADDED_DOCUMENTATION <exception cref="System.ArgumentOutOfRangeException">Thrown when either the <paramref name="startIndex"/> or <paramref name="endIndex"/> is not within the range of the available points.</exception>
        public IJointTrajectory Sub(int startIndex, int endIndex) =>
            new JointTrajectory(joints, Slice(startIndex, endIndex), this.IsValid);

        /// <summary>
        /// Applies the given transform function to all poses in the current trajectory.
        /// </summary>
        /// <param name="transform">A function that receives each point on the trajectory together with its index and that should return a new <c>JointTrajectoryPoint</c>.</param>
        /// <returns>A new instance of <c>JointTrajectory</c>.</returns>
        public IJointTrajectory Transform(Func<JointTrajectoryPoint, int, JointTrajectoryPoint> transform) =>
            new JointTrajectory(joints, points.Select(transform), this.IsValid);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
