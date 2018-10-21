using System;
using System.Collections.Generic;
using System.Numerics;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Implementations of <c>ICartesianPath</c> hold a list of poses (translation and rotation) in the Cartesian space and provide operations for manipulating the list of poses.
    /// </summary>
    public interface ICartesianPath
        : IReadOnlyList<Pose>
    {
        /// <summary>
        /// Concatenates current and the passed other objects to a new object all implementing <c>ICartesianPath</c>
        /// </summary>
        /// <param name="other">An ICartesianPath that should be appended to the current one.</param>
        /// <returns>A new object implementing <c>ICartesianPath</c> containing the current ICartesianPath followed by the given one.</returns>
        ICartesianPath Concat(ICartesianPath other);

        /// <summary>
        /// Creates a new object implementing <c>ICartesianPath</c>, by prepending the given collection of poses to the current path.
        /// </summary>
        /// <param name="source">The collection of <c>Pose</c> instances that should be prepended to the current path.</param>
        /// <returns>A new object implementing <c>ICartesianPath</c> containing the given poses followed by the present poses.</returns>
        ICartesianPath Prepend(IEnumerable<Pose> source);

        /// <summary>
        /// Creates a new object implementing <c>ICartesianPath</c>, by appending the given collection of poses to the current path.
        /// </summary>
        /// <param name="source">The collection of <c>Pose</c> instances that should be appended to the current path.</param>
        /// <returns>A new object implementing <c>ICartesianPath</c> containing the present poses followed by the given poses.</returns>
        ICartesianPath Append(IEnumerable<Pose> source);

        /// <summary>
        /// Creates a new object implementing <c>ICartesianPath</c> containing the poses in the range of [<paramref name="startIndex"/>, <paramref name="endIndex"/>].
        /// </summary>
        /// <param name="startIndex">Index from which the sub path should start.</param>
        /// <param name="endIndex">Index where the sub path should end.</param>
        /// <returns>A new object implementing <c>ICartesianPath</c> containing the poses in the range of [<paramref name="startIndex"/>, <paramref name="endIndex"/>].</returns>
        ICartesianPath Sub(int startIndex, int endIndex);

        /// <summary>
        /// Creates a new object implementing <c>ICartesianPath</c> by applying the given transform function to the poses in the current path.
        /// </summary>
        /// <param name="transform">A function which receives each <c>Pose</c> of the current path with its index and should return a new <c>Pose</c></param>
        /// <returns>A new object implementing <c>ICartesianPath</c> which is the result of applying the transform function to the current path.</returns>
        ICartesianPath Transform(Func<Pose, int, Pose> transform);

        /// <summary>
        /// A collection of the translation values of all the poses on the current path.
        /// </summary>
        IEnumerable<Vector3> Positions { get; }
        /// <summary>
        /// A collection of the rotation values of all the poses on the current path.
        /// </summary>
        IEnumerable<Quaternion> Orientations { get; }
    }

    public static class CartesianPathExtensions
    {
        /// <summary>
        /// Prepends the given <c>Pose</c> values to the current path.
        /// </summary>
        public static ICartesianPath Prepend(this ICartesianPath path, params Pose[] values) =>
            path.Prepend((IEnumerable<Pose>)values);

        /// <summary>
        /// Appends the given <c>Pose</c> values to the current path.
        /// </summary>
        public static ICartesianPath Append(this ICartesianPath path, params Pose[] values) =>
            path.Append((IEnumerable<Pose>)values);
    }
}
