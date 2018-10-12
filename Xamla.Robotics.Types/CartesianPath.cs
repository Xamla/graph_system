using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// A <c>CartesianPath</c> holds a list of poses (translation and rotation) in the Cartesian space and provides operations for manipulating the list of poses.
    /// </summary>
    public class CartesianPath
        : ICartesianPath
    {
        List<Pose> points;

        /// <summary>
        /// Creates a new <c>CartesianPath</c> from the given <c>Pose</c> instances.
        /// </summary>
        /// <param name="points">The <c>Pose</c> instances the new <c>CartesianPath</c> should hold.</param>
        public CartesianPath(params Pose[] points)
            : this((IEnumerable<Pose>)points)
        {
        }

        /// <summary>
        /// Creates a new <c>CartesianPath</c> from the given <c>Pose</c> instances.
        /// </summary>
        /// <param name="points">The <c>Pose</c> instances the new <c>CartesianPath</c> should hold.</param>
        public CartesianPath(IEnumerable<Pose> points)
        {
            this.points = points?.ToList() ?? throw new ArgumentNullException(nameof(points));
        }

        /// <summary>
        /// Accesses the <c>Pose</c> at the given instance.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The <c>Pose</c> at the given index.</returns>
        public Pose this[int index] =>
            points[index];

        /// <summary>
        /// A collection of the translation values of all the poses on the current path. 
        /// </summary>
        public IEnumerable<Vector3> Positions =>
            points.Select(x => x.Translation);

        /// <summary>
        /// A collection of the rotation values of all the poses on the current path. 
        /// </summary>
        public IEnumerable<Quaternion> Orientations =>
            points.Select(x => x.Rotation);

        /// <summary>
        /// The number of poses in the current path.
        /// </summary>
        public int Count => points.Count;

        /// <summary>
        /// Creates a new <c>CartesianPath</c>, by prepending the given collection of poses to the current path.
        /// </summary>
        /// <param name="source">The collection of <c>Pose</c> instances that should be prepended to the current path.</param>
        /// <returns>A new <c>CartesianPath</c> containing the given poses followed by the present poses.</returns>
        public ICartesianPath Prepend(IEnumerable<Pose> source) =>
            new CartesianPath(source.Concat(points));

        /// <summary>
        /// Creates a new <c>CartesianPath</c>, by appending the given collection of poses to the current path.
        /// </summary>
        /// <param name="source">The collection of <c>Pose</c> instances that should be appended to the current path.</param>
        /// <returns>A new <c>CartesianPath</c> containing the present poses followed by the given poses.</returns>
        public ICartesianPath Append(IEnumerable<Pose> source) =>
            new CartesianPath(points.Concat(source));

        /// <summary>
        /// Creates a new <c>CartesianPath</c> by appending the given Cartesian path, to the current one.
        /// </summary>
        /// <param name="other">A Cartesian path that should be appended to the current one.</param>
        /// <returns>A new <c>CartesianPath</c> containing the current Cartesian path followed by the given one.</returns>
        public ICartesianPath Concat(ICartesianPath other) =>
            Append(other);

        /// <summary>
        /// Returns an enumerator over the poses in the current Cartesian path.
        /// </summary>
        public IEnumerator<Pose> GetEnumerator() =>
            points.GetEnumerator();

        IEnumerable<Pose> Slice(int startIndex, int endIndex)
        {
            if (startIndex < 0 || startIndex >= points.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (endIndex < startIndex || endIndex >= points.Count)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            for (int i = startIndex; i <= endIndex; ++i)
                yield return points[i];
        }

        /// <summary>
        /// Creates a new <c>CartesianPath</c> containing the poses in the range of [<paramref name="startIndex"/>, <paramref name="endIndex"/>].
        /// </summary>
        /// <param name="startIndex">Index from which the sub path should start.</param>
        /// <param name="endIndex">Index where the sub path should end.</param>
        /// <returns>A new <c>CartesianPath</c> containing the poses in the range of [<paramref name="startIndex"/>, <paramref name="endIndex"/>].</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when either the <paramref name="startIndex"/> or <paramref name="endIndex"/> is not within the range of the available points.</exception>
        public ICartesianPath Sub(int startIndex, int endIndex) =>
            new CartesianPath(Slice(startIndex, endIndex));

        /// <summary>
        /// Creates a new <c>CartesianPath</c> by applying the given transform function to the poses in the current path.
        /// </summary>
        /// <param name="transform">A function which receives each <c>Pose</c> of the current path with its index and should return a new <c>Pose</c></param>
        /// <returns>A new <c>CartesianPath</c> which is the result of applying the transform function to the current path.</returns>
        public ICartesianPath Transform(Func<Pose, int, Pose> transform) =>
            new CartesianPath(points.Select(transform));

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
