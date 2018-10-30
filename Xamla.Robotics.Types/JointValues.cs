using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamla.Types;
using Xamla.Utilities;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// <c>JointValues</c> assign a numeric value to each joint name of a <c>JointSet</c>. The meaning of the value depends on the context where the <c>JointValues</c> are used. It could for example be used for joint positions or joint velocity limits. The values of <c>JointValues</c> are interpreted as radians.
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

        private readonly ReadOnlyCollection<double> values;

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
        /// <param name="values">The values (in radians) that should be assigned to the <c>JointSet</c>.</param>
        /// <exception cref="Exception">Thrown when the amount of joint names in <paramref name="jointSet"/> does not match the length of values in <paramref name="values"/>.</exception>
        public JointValues(JointSet jointSet, IEnumerable<double> values)
        {
            double[] array = values.ToArray();
            if (jointSet.Count != array.Length)
                throw new Exception($"The length of the values sequence needs to match the amount of joint names in the joint set.");
            this.values = Array.AsReadOnly(array);
            this.JointSet = jointSet;
        }

        /// <summary>
        /// Creates a new instance of <c>JointValues</c> with the given joint set and the collection of values. The first value is assigned the first name in the joint set, the second value is assigned the second name and so on.
        /// </summary>
        /// <param name="jointSet">The names of joint that get values assigned.</param>
        /// <param name="values">The values (in radians) that should be assigned to the <c>JointSet</c>.</param>
        /// <exception cref="Exception">Thrown when the amount of joint names in <paramref name="jointSet"/> does not match the length of values in <paramref name="values"/>.</exception>
        public JointValues(JointSet jointSet, IReadOnlyList<double> values)
        {
            double[] array = new double[values.Count];
            for (int i = 0; i < array.Length; i++)
                array[i] = values[i];
            if (jointSet.Count != array.Length)
                throw new Exception($"The length of the values list needs to match the amount of joint names in the joint set.");
            this.values = Array.AsReadOnly(array);
            this.JointSet = jointSet;
        }

        /// <summary>
        /// Creates a new instance of <c>JointValues</c> with the given joint set and fills each value with the given fill value.
        /// </summary>
        /// <param name="jointSet">The names of joint that get values assigned.</param>
        /// <param name="fillValue">The value that should be assigned to each name in the <c>JointSet</c>.</param>
        public JointValues(JointSet jointSet, double fillValue)
            : this(jointSet, Fill(jointSet.Count, fillValue))
        {
        }

        /// <summary>
        /// Gets the set of joints of the current <c>JointValues</c>.
        /// </summary>
        public JointSet JointSet { get; }

        /// <summary>
        /// Gets the values array of the current <c>JointValues</c>. The values are in radians.
        /// </summary>
        public IReadOnlyList<double> Values =>
            values;

        /// <summary>
        /// Returns the amount of values
        /// </summary>
        public int Count =>
            values.Count;

        /// <summary>
        /// Tries to get the numeric value assigned to the given joint name.
        /// </summary>
        /// <param name="jointName">The name of the joint for which the value should be returned.</param>
        /// <param name="jointValue">The variable where the value (in radians) of the joint name is stored, if it exists.</param>
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
        /// <returns>The value (in radians) assigned to the given joint name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jointName"/> is null.</exception>
        /// <exception cref="Exception">Thrown when the given <paramref name="jointName"/> is not part of the <c>JointSet</c>.</exception>
        public double GetValue(string jointName) =>
            Values[JointSet.GetIndexOf(jointName)];

        /// <summary>
        /// Returns the value (in radians) for the given joint name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="jointName"/> is null.</exception>
        /// <exception cref="Exception">Thrown when the given <paramref name="jointName"/> is not part of the <c>JointSet</c>.</exception>
        public double this[string jointName] =>
            this.GetValue(jointName);

        /// <summary>
        /// Returns the value (in radians) at the given index.
        /// </summary>
        public double this[int index] =>
            Values[index];

        /// <summary>
        /// Reorders the values to match the order of the given <c>JointSet</c>.
        /// </summary>
        /// <param name="newOrder">A <c>JointSet</c> with the same name of joints, but in a different order than the current <c>JointSet</c>.</param>
        /// <returns>A new instance of <c>JointValues</c>.</returns>
        public JointValues Reorder(JointSet newOrder)
        {
            if (!newOrder.IsSimilar(this.JointSet))
                throw new ArgumentException("The given joint set must be similar to the current joint set.", nameof(newOrder));
            return new JointValues(newOrder, newOrder.Select(x => this[x]));
        }

        /// <summary>
        /// Creates a new <c>JointValues</c> instance that contains only values for joints of the given <c>JointSet</c>.
        /// </summary>
        /// <param name="subset">A <c>JointSet</c> with a subset of the joints.</param>
        /// <returns>A new instance of <c>JointValues</c>.</returns>
        public JointValues Select(JointSet subset)
        {
            if (!subset.IsSubset(this.JointSet))
                throw new ArgumentException("The given joint set needs to be a subset of the current joint set.", nameof(subset));
            return new JointValues(subset, subset.Select(x => this[x]));
        }

        /// <summary>
        /// Updates the joint values of the given joint set. Values which are not in the given joint set are not changed.
        /// </summary>
        /// <param name="jointSet">The set of joints whose values should be changed.</param>
        /// <param name="values">The new values for the given joint set.</param>
        /// <returns>A new instance of <c>JointValues</c>.</returns>
        public JointValues SetValues(JointSet jointSet, IReadOnlyList<double> values)
        {
            if (!jointSet.IsSubset(this.JointSet))
                throw new ArgumentException("The given joint set needs to be a subset of the current joint set.", nameof(jointSet));

            double[] newValues = this.ToArray();
            for (var i = 0; i < values.Count; i++)
            {
                string jointName = jointSet[i];
                int index = this.JointSet.GetIndexOf(jointName);
                newValues[index] = values[i];
            }

            return new JointValues(this.JointSet, newValues);
        }

        /// <summary>
        /// Update the value of a single joint specified by name.
        /// </summary>
        /// <param name="name">Name of the joint whose value is to be modified.</param>
        /// <param name="value">New joint value in radians.</param>
        /// <returns>A new instance of <c>JointValues</c>.</returns>
        public JointValues WithValue(string name, double value) =>
            WithValue(JointSet.GetIndexOf(name), value);

        /// <summary>
        /// Update the value of a single joint specified by index.
        /// </summary>
        /// <param name="index">Index of the joint whose value is to be modified.</param>
        /// <param name="value">New joint value in radians.</param>
        /// <returns>A new instance of <c>JointValues</c>.</returns>
        public JointValues WithValue(int index, double value)
        {
            double[] newValues = this.ToArray();
            newValues[index] = value;
            return new JointValues(this.JointSet, newValues);
        }

        /// <summary>
        /// Merge JointValues instance with others <c>JointValues</c>.
        /// </summary>
        /// <param name="other">JointValues which are merge with current instance</param>
        /// <returns>A new instance of <c>JointValues</c> with contains values for
        /// all Joints defined in this and the other JointValues instance.</returns>
        public JointValues Merge(JointValues other)
        {
            var fullJointSet = this.JointSet.Append(other.JointSet);
            var merge = new JointValues(fullJointSet, 0);
            merge = merge.SetValues(this);
            merge = merge.SetValues(other);
            return merge;
        }

        /// <summary>
        /// Updates the joint values of the given joint set. Values which are not in the given joint set are not changed.
        /// </summary>
        /// <param name="source">The new values for the given joint set.</param>
        /// <returns>A new instance of <c>JointValues</c>.</returns>
        public JointValues SetValues(JointValues source) =>
            SetValues(source.JointSet, source.Values);

        /// <summary>
        /// Applies the given transform function to all joint values in the current <c>JointSet</c>.
        /// </summary>
        /// <param name="transform">A function that receives each joint value (in radians) with its corresponding index and should return a new joint value.</param>
        /// <returns>A new instance of <c>JointValues</c>.</returns>
        public JointValues Transform(Func<double, int, double> transform)
        {
            var result = new double[this.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = transform(this.Values[i], i);
            return new JointValues(this.JointSet, result);
        }

        public override bool Equals(object obj) =>
            Equals(obj as JointValues);

        /// <summary>
        /// Returns a hash code for this instance.
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() =>
            HashHelper.CombineHashCode(this.JointSet.GetHashCode(), HashHelper.GetHashCode(this.Values));

        public override string ToString() =>
            string.Join(", ", this.Values.Select(x => $"{x}"));

        public bool Equals(JointValues other) =>
            object.ReferenceEquals(this, other)
            || (other != null
                && other.JointSet.Equals(this.JointSet)
                && this.Values.SequenceEqual(other.Values));

        /// <summary>
        /// Returns a new <c>JointValues</c> object whose values are the sum of the specified
        /// <c>JointValues</c> object and this instance.
        /// </summary>
        /// <param name="jv">The joint values to add.</param>
        /// <returns>A new object that represents the value of this instance plus the value of js.</returns>
        public JointValues Add(JointValues jv)
        {
            if (jv == null)
                throw new ArgumentNullException(nameof(jv));

            if (jv.JointSet == this.JointSet)
            {
                return this.Transform((x, i) => x + jv[i]);
            }
            else
            {
                if (!jv.JointSet.IsSimilar(this.JointSet))
                    throw new ArgumentException("The given joint set must be similar to the current joint set.", nameof(jv));
                return this.Transform((x, i) => x + jv.GetValue(this.JointSet[i]));
            }
        }

        public JointValues Add(double value) =>
            this.Transform((x, i) => x + value);

        /// <summary>
        /// Returns a new <c>JointValues</c> object whose values are the difference of the specified
        /// <c>JointValues</c> object and this instance.
        /// </summary>
        /// <param name="jv">The joint values to subtract.</param>
        /// <returns>A new object that represents the value of this instance minus the value of js.</returns>
        public JointValues Subtract(JointValues jv)
        {
            if (jv == null)
                throw new ArgumentNullException(nameof(jv));
            if (!jv.JointSet.IsSimilar(this.JointSet))
                throw new ArgumentException("The given joint set must be similar to the current joint set.", nameof(jv));

            if (jv.JointSet == this.JointSet)
            {
                return this.Transform((x, i) => x - jv[i]);
            }
            else
            {
                return this.Transform((x, i) => x - jv.GetValue(this.JointSet[i]));
            }
        }

        public JointValues Subtract(double value) =>
            this.Transform((x, i) => x - value);

        public JointValues Divide(double divisor) =>
            this.Transform((x, i) => x / divisor);

        public JointValues Multiply(double factor) =>
            this.Transform((x, i) => x * factor);

        /// <summary>
        /// Returns a new <c>JointValues</c> object whose joint values are the negated values of this instance.
        /// </summary>
        /// <returns>
        /// A new object with the same numeric joint values as this instance, but with the opposite sign.
        /// </returns>
        public JointValues Negate() =>
            this.Transform((x, i) => -x);

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
        /// <returns>A new instance of <c>JointValues</c>.</returns>
        /// <seealso cref="JointLimits"/>
        public static JointValues Random(JointLimits limits)
            => Random(limits, ThreadSafeRandom.Generator);

        /// <summary>
        /// Creates a new instance of <c>JointSet</c> with random values assigned that lie within the given joint limits using the given random number generator.
        /// </summary>
        /// <param name="limits">The limits in which the values of the new <c>JointSet</c> should be. The names of the joints are taken from the <paramref name="limits"/>.</param>
        /// <param name="rng">The random number generator that is used.</param>
        /// <returns>A new instance of <c>JointValues</c>.</returns>
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

        public double[] ToArray()
        {
            var a = new double[values.Count];
            values.CopyTo(a, 0);
            return a;
        }

        public A<double> ToA() =>
            new A<double>(this.ToArray(), this.Count);

        public static JointValues operator +(JointValues a, JointValues b) =>
            a.Add(b);

        public static JointValues operator +(JointValues a, double value) =>
            a.Add(value);

        public static JointValues operator -(JointValues j) =>
            j.Negate();

        public static JointValues operator -(JointValues a, JointValues b) =>
            a.Subtract(b);

        public static JointValues operator -(JointValues a, double value) =>
            a.Subtract(value);

        public static JointValues operator *(JointValues j, double factor) =>
            j.Multiply(factor);

        public static JointValues operator *(double factor, JointValues j) =>
            j.Multiply(factor);

        public static JointValues operator /(JointValues j, double divisor) =>
            j.Divide(divisor);

        public static JointValues Interpolate(JointValues a, JointValues b, double t = 0.5) =>
            (1-t) * a + t * b;
    }
}
