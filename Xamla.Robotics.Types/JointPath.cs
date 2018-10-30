using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// A <c>JointPath</c> hold a list of joint values and provide operations for manipulating this list of joint values.
    /// </summary>
    /// <seealso cref="JointValues"/>
    public class JointPath
        : IJointPath
    {
        /// <summary>
        /// Creates a new empty <c>JointPath</c>.
        /// </summary>
        public static readonly JointPath Empty = new JointPath(JointSet.Empty, Enumerable.Empty<JointValues>());

        readonly JointSet joints;
        readonly List<JointValues> points;

        /// <summary>
        /// Creates a new <c>JointPath</c> containing only the given point in Joint space.
        /// </summary>
        /// <param name="point"></param>
        public JointPath(JointValues point)
            : this(point?.JointSet, point)
        {
        }

        /// <summary>
        /// Creates a new <c>JointPath</c> containing the given start and end point in Joint space.
        /// </summary>
        /// <param name="start">Start point of the new <c>JointPath</c>.</param>
        /// <param name="goal">End point of the new <c>JointPath</c>.</param>
        public JointPath(JointValues start, JointValues goal)
            : this(start?.JointSet, start, goal)
        {
        }

        /// <summary>
        /// Creates a new <c>JointPath</c> from the given set of joints and the given array of joint values.
        /// </summary>
        /// <param name="joints">The names of the joints used in the new <c>JointPath</c>.</param>
        /// <param name="points">The joint values that should be added to the new <c>JointPath</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="joints"/> or no <paramref name="points"/> are given.</exception>
        /// <exception cref="ArgumentException">Thrown when there is a mismatch between any <c>JointSet</c> in <paramref name="points"/> and the given <c>JointSet</c> <paramref name="joints"/>.</exception>
        /// <seealso cref="JointSet"/>
        public JointPath(JointSet joints, params JointValues[] points)
            : this(joints, (IEnumerable<JointValues>)points)
        {
        }

        /// <summary>
        /// Creates a new <c>JointPath</c> from the given set of joints and the given collection of joint values.
        /// </summary>
        /// <param name="joints">The names of the joints used in the new <c>JointPath</c>.</param>
        /// <param name="points">The joint values that should be added to the new <c>JointPath</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="joints"/> or no <paramref name="points"/> are given.</exception>
        /// <exception cref="ArgumentException">Thrown when there is a mismatch between any <c>JointSet</c> in <paramref name="points"/> and the given <c>JointSet</c> <paramref name="joints"/>.</exception>
        /// <seealso cref="JointSet"/>
        public JointPath(JointSet joints, IEnumerable<JointValues> points)
        {
            this.joints = joints ?? throw new ArgumentNullException(nameof(joints));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            this.points = points
                .Where(x => x != null)
                .Select(x =>
                {
                    if (!x.JointSet.Equals(joints))
                    {
                        if (!x.JointSet.IsSimilar(joints))
                            throw new ArgumentException("Provided path points have joint values of incompatible joint sets.", nameof(points));
                        x = x.Reorder(joints);
                    }
                    return x;
                })
                .ToList();
        }

        /// <summary>
        /// Gets the set of joints that is used for the current <c>JointPath</c>.
        /// </summary>
        public JointSet JointSet =>
            joints;

        /// <summary>
        /// Accesses the <c>JointValues</c> of the current <c>JointPath</c> at the given index.
        /// </summary>
        /// <param name="index">The index at which the <c>JointValue</c> should be retrieved.</param>
        /// <returns>The <c>JointValue</c> at the given index.</returns>
        public JointValues this[int index] =>
            points[index];

        /// <summary>
        /// Gets the amount of points in the current <c>JointPath</c>.
        /// </summary>
        public int Count =>
            points.Count;

        /// <summary>
        /// Prepends the given collection of joint values to the current joint path.
        /// </summary>
        /// <param name="source">A collection of joint values that should be prepended to the current joint path.</param>
        /// <returns>A new instance of <c>JointPath</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
        public IJointPath Prepend(IEnumerable<JointValues> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new JointPath(joints, source.Concat(points));
        }

        /// <summary>
        /// Appends the given collection of joint values to the current joint path.
        /// </summary>
        /// <param name="source">A collection of joint values that should be appended to the current joint path.</param>
        /// <returns>A new instance of <c>JointPath</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
        public IJointPath Append(IEnumerable<JointValues> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new JointPath(joints, points.Concat(source));
        }

        /// <summary>
        /// Concatenates the points of the given joint path and the points of the current joint path.
        /// </summary>
        /// <param name="other">Another joint path that should be concatenated with the current one.</param>
        /// <returns>A new instance of <c>JointPath</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
        public IJointPath Concat(IJointPath other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            return new JointPath(joints, points.Concat(other));
        }

        /// <summary>
        /// Gets an enumerator over the points in the current <c>JointPath</c>.
        /// </summary>
        public IEnumerator<JointValues> GetEnumerator() =>
            points.GetEnumerator();

        IEnumerable<JointValues> Slice(int startIndex, int endIndex)
        {
            if (startIndex < 0 || startIndex >= points.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (endIndex < startIndex || endIndex >= points.Count)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            for (int i = startIndex; i < endIndex; ++i)
                yield return points[i];
        }

        /// <summary>
        /// Gets the sub joint path defined by the given range [startIndex, endIndex].
        /// </summary>
        /// <param name="startIndex">The index where the sub joint path should start.</param>
        /// <param name="endIndex">The index where the sub joint path should end.</param>
        /// <returns>A new instance of <c>JointPath</c>.</returns>
        public IJointPath Sub(int startIndex, int endIndex) =>
            new JointPath(joints, Slice(startIndex, endIndex));

        /// <summary>
        /// Applies the given transformation function to all joint values of the current joint path.
        /// </summary>
        /// <param name="transform">A function that is called for each joint value with its index in the path and that should return a new joint value.</param>
        /// <returns>A new instance of <c>JointPath</c></returns>
        public IJointPath Transform(Func<JointValues, int, JointValues> transform) =>
            new JointPath(joints, points.Select(transform));

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
