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
        /// Concatenates two <c>ICartesianPath</c>
        /// </summary>
        ICartesianPath Concat(ICartesianPath other);
        /// <summary>
        /// Prepends the given <c>ICartesianPath</c> to the current <c>ICartesianPath</c>.
        /// </summary>
        ICartesianPath Prepend(IEnumerable<Pose> source);
        /// <summary>
        /// Appends the given <c>ICartesianPath</c> to the current <c>ICartesianPath</c>.
        /// </summary>
        ICartesianPath Append(IEnumerable<Pose> source);
        /// <summary>
        /// Returns the sub path defined by the given start index and end index.
        /// </summary>
        ICartesianPath Sub(int startIndex, int endIndex);
        /// <summary>
        /// Applies the given transform function to all poses in the current path.
        /// </summary>
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
