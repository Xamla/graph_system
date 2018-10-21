using System;
using System.Collections.Generic;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Implementations of <c>IJointPath</c> hold a list of joint values and provide operations for manipulating this list of joint values.
    /// </summary>
    /// <seealso cref="JointValues"/>
    public interface IJointPath
        : IReadOnlyList<JointValues>
    {
        /// <summary>
        /// Gets the set of joints, which holds the names of the joints which are used for this <c>IJointPath</c>.
        /// </summary>
        JointSet JointSet { get; }

        /// <summary>
        /// Concatenates the points of the given joint path and the points of the current joint path.
        /// </summary>
        /// <param name="other">Another joint path that should be concatenated with the current one.</param>
        /// <returns>A new instance of <c>IJointPath</c>.</returns>
        IJointPath Concat(IJointPath other);

        /// <summary>
        /// Prepends the given collection of joint values to the current joint path.
        /// </summary>
        /// <param name="values">A collection of joint values that should be prepended to the current joint path.</param>
        /// <returns>A new instance of <c>IJointPath</c>.</returns>
        IJointPath Prepend(IEnumerable<JointValues> values);

        /// <summary>
        /// Appends the given collection of joint values to the current joint path.
        /// </summary>
        /// <param name="source">A collection of joint values that should be appended to the current joint path.</param>
        /// <returns>A new instance of <c>IJointPath</c>.</returns>
        IJointPath Append(IEnumerable<JointValues> source);

        /// <summary>
        /// Gets the sub joint path defined by the given range [startIndex, endIndex].
        /// </summary>
        /// <param name="startIndex">The index where the sub joint path should start.</param>
        /// <param name="endIndex">The index where the sub joint path should end.</param>
        /// <returns>A new instance of <c>IJointPath</c>.</returns>
        IJointPath Sub(int startIndex, int endIndex);

        /// <summary>
        /// Applies the given transformation function to all joint values of the current joint path.
        /// </summary>
        /// <param name="transform">A function that is called for each joint value with its index in the path and that should return a new joint value.</param>
        /// <returns>A new instance of <c>IJointPath</c></returns>
        IJointPath Transform(Func<JointValues, int, JointValues> transform);
    }

    public static class JointPathExtensions
    {
        /// <summary>
        /// Prepends the given array of joint values to the current joint path.
        /// </summary>
        /// <param name="values">An array of joint values that should be prepended to the current joint path.</param>
        /// <returns>The combination of the given array of joint values and the current joint path.</returns>
        public static IJointPath Prepend(this IJointPath path, params JointValues[] values) =>
            path.Prepend((IEnumerable<JointValues>)values);

        /// <summary>
        /// Appends the given array of joint values to the current joint path.
        /// </summary>
        /// <param name="values">An array of joint values that should be appended to the current joint path.</param>
        /// <returns>The combination of the given array of joint values and the current joint path.</returns>
        public static IJointPath Append(this IJointPath path, params JointValues[] values) =>
            path.Append((IEnumerable<JointValues>)values);
    }
}
