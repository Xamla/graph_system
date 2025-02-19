﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamla.Utilities;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// A <c>JointSet</c> is a collection of joint names in a certain order.
    /// </summary>
    /// <remarks>
    /// It provides methods for creating and comparing <c>JointSet</c> instances.
    /// </remarks>
    public class JointSet
        : IEnumerable<string>
        , IReadOnlyList<string>
        , IReadOnlyCollection<string>
        , IEquatable<JointSet>
    {
        /// <value>
        /// A new empty <c>JointSet</c>.
        /// </value>
        public static readonly JointSet Empty = new JointSet();

        readonly string[] jointNames;

        /// <summary>
        /// Creates a new empty <c>JointSet</c>.
        /// </summary>
        public JointSet()
            : this(new string[0])
        {
        }

        /// <summary>
        /// Creates a new <c>JointSet</c> containing the given joint names.
        /// </summary>
        /// <param name="names">A list of joint names, that the new <c>JointSet</c> should hold.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="names"/> is null.</exception>
        public JointSet(params string[] names)
            : this((IEnumerable<string>)names)
        {
        }

        /// <summary>
        /// Creates a new <c>JointSet</c> containing the given joint names.
        /// </summary>
        /// <param name="names">A collection of joint names, that the new <c>JointSet</c> should hold.</param>
        public JointSet(IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            this.jointNames = names.Unique().ToArray();
        }

        /// <summary>
        /// Creates a new <c>JointSet</c> where the given prefix is added to each joint name in the current <c>JointSet</c>.
        /// </summary>
        /// <param name="prefix">The prefix that is added to each joint name.</param>
        /// <returns>A new <c>JointSet</c></returns>
        public JointSet AddPrefix(string prefix) =>
            new JointSet(this.Select(x => prefix + x));

        /// <summary>
        /// Appends the given joint names to the array of joint names already which form the current joint set.
        /// </summary>
        /// <param name="names">The names that should be appended.</param>
        /// <returns>A new <c>JointSet</c></returns>
        public JointSet Append(params string[] names) =>
            Append((IEnumerable<string>)names);

        /// <summary>
        /// Appends the given joint names to the array of joint names already which form the current joint set.
        /// </summary>
        /// <param name="names">The names that should be appended.</param>
        /// <returns>A new <c>JointSet</c></returns>
        public JointSet Append(IEnumerable<string> names) =>
            new JointSet(jointNames.Concat(names));

        /// <summary>
        /// Appends the joint names of the given joint set to the array of joint names already which form the current joint set.
        /// </summary>
        /// <param name="other">The names that should be appended.</param>
        /// <returns>A new <c>JointSet</c></returns>
        public JointSet Append(JointSet other) =>
            Append(other.jointNames);

        /// <summary>
        /// Combines the joint names of the current instance with the provided names and creates a resulting new JointSet. Names that are already included in the current joint set are skipped.
        /// </summary>
        /// <param name="names">The names that should be appended.</param>
        /// <returns>A new <c>JointSet</c></returns>
        public JointSet Combine(string[] names) =>
            new JointSet(Enumerable.Concat(jointNames, names).DistinctInOrder());

        /// <summary>
        /// Appends the joint names of the given joint set to the current joint names, if they are not already in the set.
        /// </summary>
        /// <param name="other">The <c>JointSet</c> containing the names that should be added<param>
        /// <returns>A new <c>JointSet</c></returns>
        public JointSet Combine(JointSet other) =>
            Combine(other.jointNames);

        public static JointSet Combine(JointSet left, JointSet right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;
            return left.Combine(right);
        }

        /// <summary>
        /// Tests whether the all joint names of the current <c>JointSet</c> are included in the given other <c>JointSet</c>.
        /// </summary>
        /// <param name="other">Another <c>JointSet</c> which should be tested for being a subset.</param>
        /// <returns>True when the given other <c>JointSet</c> is a subset of the current one; False otherwise.</returns>
        public bool IsSubset(JointSet other) =>
            jointNames.All(other.Contains);

        /// <summary>
        /// Tests whether the given other <c>JointSet</c> is similar to the current one. Two <c>JointSet</c> instances are similar, when they contain the same number of joint names and are a subset of each other. The order in which they hold the joint names does not matter in this case.
        /// </summary>
        /// <param name="other">Another <c>JointSet</c> which should be tested for similarity.</param>
        /// <returns>True when the other <c>JointSet</c> is similar the current one; False otherwise.</returns>
        public bool IsSimilar(JointSet other) =>
            other.Count == this.Count && this.IsSubset(other);

        /// <summary>
        /// Tries to get the position of the given joint name in the current <c>JointSet</c>.
        /// </summary>
        /// <param name="name">A joint name for which the position should be found.</param>
        /// <param name="index">The position number of the joint name in the <c>JointSet</c>.</param>
        /// <returns>True when the joint name was found in the current <c>JointSet</c>; False otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        public bool TryGetIndexOf(string name, out int index)
        {
            index = Array.IndexOf(this.jointNames, name);
            return index >= 0;
        }

        /// <summary>
        /// Gets the position of the given joint name in the current <c>JointSet</c>.
        /// </summary>
        /// <param name="name">A joint name for which the position should be found.</param>
        /// <returns>The position of the given joint name in the current <c>JointSet</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        /// <exception cref="Exception">Thrown when <paramref name="name"/> is not found in <c>JointSet</c>.</exception>
        public int GetIndexOf(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name), "Name must not be null.");
            if (!this.TryGetIndexOf(name, out int value))
                throw new Exception($"Joint '{name}' is missing in JointSet.");
            return value;
        }

        /// <value>
        /// The number of joint names in the current <c>JointSet</c>.
        /// </value>
        public int Count =>
            jointNames.Length;

        /// <summary>
        /// Accesses the joint name at the given index.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Thrown when index out of bound</exception>
        public string this[int index] =>
            jointNames[index];

        /// <summary>
        /// Tests whether the given joint name is included in the current <c>JointSet</c> or not.
        /// </summary>
        /// <param name="name">A joint name which presents should be tested.</param>
        /// <returns>True if the given joint name is found; False otherwise.</returns>
        public bool Contains(string name) =>
            this.jointNames.Contains(name);

        /// <summary>
        /// Creates a hash value over the current joint names.
        /// </summary>
        public override int GetHashCode() =>
            jointNames.Aggregate(0, (acc, x) => acc ^ x.GetHashCode());

        /// <summary>
        /// Tests whether the given other <c>JointSet</c> equals the current one. Two <c>JointSet</c> instances are equal, when they contain the same joint names in the same order.
        /// </summary>
        /// <param name="other">Another <c>JointSet</c> which should be tested for equality.</param>
        /// <returns>True when the other <c>JointSet</c> equals the current one; False otherwise.</returns>
        public bool Equals(JointSet other) =>
            object.ReferenceEquals(this, other)
                || (other != null
                    && this.Count == other.Count
                    && this.jointNames.SequenceEqual(other.jointNames));

        /// <summary>
        /// Tests whether the given object equals the current <c>JointSet</c>. Two <c>JointSet</c> instances are equal, when they contain the same joint names in the same order.
        /// </summary>
        /// <param name="obj">An object which should be tested for equality.</param>
        /// <returns>True when the given object equals the current <c>JointSet</c>; False otherwise.</returns>
        public override bool Equals(object obj) =>
            this.Equals(obj as JointSet);

        /// <summary>
        /// Returns an enumerator over the current joint names.
        /// </summary>
        public IEnumerator<string> GetEnumerator() =>
            jointNames.OfType<string>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        /// <summary>
        /// Creates a new array holding the current joint names.
        /// </summary>
        public string[] ToArray() =>
            (string[])jointNames.Clone();

        /// <summary>
        /// Creates a new List holding the current joint names.
        /// </summary>
        public List<string> ToList() =>
            jointNames.ToList();

        /// <summary>
        /// Creates a human readable representation of the current <c>JointSet</c>.
        /// </summary>
        public override string ToString() =>
            $"JointSet {{ { string.Join(", ", this.jointNames) } }}";
    }
}
