using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamla.Utilities;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// <c>JointValues</c> assign a numeric value to each joint name of a <c>JointSet</c>. The meaning of the value depends on the context where the <c>JointValues</c> are used. It could for example be used for joint positions or joint velocity limits.
    /// </summary>
    /// <seealso cref="JointSet"/>
    public class JointValues
        : IReadOnlyList<double>
        , IEquatable<JointValues>
    {
        /// <summary>
        /// Creates a new instance of <c>JointValues</c> with an empty <c>JointSet</c>.
        /// </summary>
        public static JointValues Empty = new JointValues(JointSet.Empty, 0);

        /// <summary>
        /// Creates a new instance of <c>JointValues</c> with each value set to zero.
        /// </summary>
        /// <param name="jointSet"></param>
        /// <returns></returns>
        public static JointValues Zero(JointSet jointSet) =>
            new JointValues(jointSet, 0);

        /// <summary>
        /// Creates a new instance of <c>JointValues</c> with the given joint set and the collection of values. The first value is assigned the first name in the joint set, the second value is assigned the second name and so on.
        /// </summary>
        /// <param name="jointSet">The names of joint that get values assigned.</param>
        /// <param name="values">The values that should be assigned to the <c>JointSet</c>.</param>
        /// <exception cref="Exception">Thrown when the amount of joint names in <paramref name="jointSet"/> does not match the length of values in <paramref name="values"/>.</exception>
        public JointValues(JointSet jointSet, IEnumerable<double> values)
            : this(jointSet, values.ToArray())
        {
        }

        /// <summary>
        /// Creates a new instance of <c>JointValues</c> with the given joint set and the collection of values. The first value is assigned the first name in the joint set, the second value is assigned the second name and so on.
        /// </summary>
        /// <param name="jointSet">The names of joint that get values assigned.</param>
        /// <param name="values">The values that should be assigned to the <c>JointSet</c>.</param>
        /// <exception cref="Exception">Thrown when the amount of joint names in <paramref name="jointSet"/> does not match the length of values in <paramref name="values"/>.</exception>
        public JointValues(JointSet jointSet, double[] values)
        {
            if (jointSet.Count != values.Length)
                throw new Exception($"The length of the value array needs to match the amount of joint names in the joint set.");

            this.JointSet = jointSet;
            this.Values = values;
        }

        /// <summary>
        /// Creates a new instance of <c>JointValues</c> with the given joint set and fills each value with the given fill value. 
        /// </summary>
        /// <param name="jointSet">The names of joint that get values assigned.</param>
        /// <param name="values">The value that should be assigned to each name in the <c>JointSet</c>.</param>
        public JointValues(JointSet jointSet, double fillValue)
            : this(jointSet, Fill(jointSet.Count, fillValue))
        {
        }

        /// <summary>
        /// Gets the set of joints of the current <c>JointValues</c>.
        /// </summary>
        public JointSet JointSet { get; }

        /// <summary>
        /// Gets the values array of the current <c>JointValues</c>.
        /// </summary>
        public double[] Values { get; }

        /// <summary>
        /// Returns the amount of values
        /// </summary>
        public int Count =>
            Values.Length;

        /// <summary>
        /// Tries to get the numeric value assigned to the given joint name.
        /// </summary>
        /// <param name="jointName">The name of the joint for which the value should be returned.</param>
        /// <param name="jointValue">The variable where the value of the joint name is stored, if it exists.</param>
        /// <returns>True when a value was found for the given joint value, False otherwise.</returns>
        public bool TryGetValue(string jointName, out double jointValue)
        {
            jointValue = 0;
            if (!JointSet.TryGetIndexOf(jointName, out int index))
                return false;
            jointValue = Values[index];
            return true;
        }

        /// <summary>
        /// Gets the numeric value assigned to the given joint name.
        /// </summary>
        /// <param name="jointName">The name of the joint for which the value should be returned.</param>
        /// <returns>The value assigned to the given joint name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jointName"/> is null.</exception>
        /// <exception cref="Exception">Thrown when the given <paramref name="jointName"/> is not part of the <c>JointSet</c>.</exception>
        public double GetValue(string jointName) =>
            Values[JointSet.GetIndexOf(jointName)];

        /// <summary>
        /// Returns the value for the given joint name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jointName"/> is null.</exception>
        /// <exception cref="Exception">Thrown when the given <paramref name="jointName"/> is not part of the <c>JointSet</c>.</exception>
        public double this[string jointName] =>
            this.GetValue(jointName);

        /// <summary>
        /// Returns the value at the given index.
        /// </summary>
        public double this[int index] =>
            Values[index];

        /// <summary>
        /// Reorders the values to match the order of the given <c>JointSet</c>.
        /// </summary>
        /// <param name="newOrder">A <c>JointSet</c> with the same name of joints, but in a different order than the current <c>JointSet</c>.</param>
        /// <returns>A new instance of <c>JointSet</c>.</returns>
        public JointValues Reorder(JointSet newOrder) =>
            new JointValues(JointSet, newOrder.Select(x => this[x]));

        /// <summary>
        /// Applies the given transform function to all joint values in the current <c>JointSet</c>.
        /// </summary>
        /// <param name="transform">A function that receives each joint value with its corresponding index and should return a new joint value.</param>
        /// <returns>A new instance of <c>JointSet</c>.</returns>
        public JointValues Transform(Func<double, int, double> transform) =>
            new JointValues(this.JointSet, this.Values.Select(transform));

        public override bool Equals(object obj) =>
            Equals(obj as JointValues);

        public override int GetHashCode() =>
            HashHelper.CombineHashCode(this.JointSet.GetHashCode(), HashHelper.GetHashCode(this.Values));

        public override string ToString() =>
            string.Join(", ", this.Values.Select(x => $"{x}"));

        public bool Equals(JointValues other) =>
            object.ReferenceEquals(this, other)
            || (other != null
                && other.JointSet.Equals(this.JointSet)
                && this.Values.SequenceEqual(other.Values));

        static double[] Fill(int length, double value)
        {
            var a = new double[length];
            Array.Fill(a, value);
            return a;
        }

        /// <summary>
        /// Creates a new instance of <c>JointSet</c> with random values assigned that lie within the given joint limits.
        /// </summary>
        /// <param name="limits">The limits in which the values of the new <c>JointSet</c> should be. The names of the joints are taken from the <paramref name="limits"/>.</param>
        /// <returns>A new instance of <c>JointSet</c>.</returns>
        /// <seealso cref="JointLimits"/>
        public static JointValues Random(JointLimits limits)
            => Random(limits, ThreadSafeRandom.Generator);

        /// <summary>
        /// Creates a new instance of <c>JointSet</c> with random values assigned that lie within the given joint limits using the given random number generator.
        /// </summary>
        /// <param name="limits">The limits in which the values of the new <c>JointSet</c> should be. The names of the joints are taken from the <paramref name="limits"/>.</param>
        /// <param name="rng">The random number generator that is used.</param>
        /// <returns>A new instance of <c>JointSet</c>.</returns>
        /// <seealso cref="JointLimits"/>
        /// <exception cref="ArgumentException">Thrown when for at least one joint a limit is missing.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="limits"/> or <paramref name="rng"/> is null.</exception>
        public static JointValues Random(JointLimits limits, Random rng)
        {
            if (limits == null)
                throw new ArgumentNullException(nameof(limits));
            if (rng == null)
                throw new ArgumentNullException(nameof(rng));

            if (limits.MinPosition.Concat(limits.MaxPosition).Any(x => !x.HasValue))
                throw new ArgumentException("Position joint limits not set for all joints (at least one is null).", nameof(limits));

            var jointSet = limits.JointSet;
            var min = limits.MinPosition;
            var max = limits.MaxPosition;

            return new JointValues(jointSet, rng.DoubleSequence()
                .Take(jointSet.Count)
                .Select((x, i) => (double)(x * (max[i] - min[i]) + min[i]))
            );
        }

        public IEnumerator<double> GetEnumerator() =>
             Values.OfType<double>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
