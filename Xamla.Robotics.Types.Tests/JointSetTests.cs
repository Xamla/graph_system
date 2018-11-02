using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class JointSetTests
    {
        [Fact]
        public void TestAddPrefix()
        {
            var jointsA = new JointSet("a", "b", "c");
            var jointsB = jointsA.AddPrefix("test_");

            Assert.True(jointsB.ToArray().SequenceEqual(new string[] { "test_a", "test_b", "test_c" }));
            Assert.False(jointsB.ToArray().SequenceEqual(new string[] { "Test_a", "Test_b", "Test_c" }));

        }

        [Fact]
        public void TestAppend()
        {
            var jointsA = new JointSet("a", "b", "c");
            // using array
            var jointsB = jointsA.Append(new string[] { "d", "e" });

            Assert.True(jointsB.ToArray().SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));

            // using JointSet
            jointsB = jointsA.Append(new JointSet("d", "e"));
            Assert.True(jointsB.ToArray().SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));

            // Check that exception is thrown when duplicate entry is given
            Assert.Throws<Xamla.Utilities.DuplicateElementException>(() => jointsA.Append(new JointSet("d", "e", "a")));
        }

        [Fact]
        public void TestCombine()
        {
            var jointsA = new JointSet("a", "b", "e");
            // using array
            var jointsB = jointsA.Combine(new string[] { "c", "f", "e" });
            Assert.True(jointsB.ToArray().SequenceEqual(new string[] { "a", "b", "e", "c", "f" }));
            Assert.False(jointsB.ToArray().SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));
            // using JointSet
            jointsB = jointsA.Combine(new JointSet("c", "f", "e"));
            Assert.True(jointsB.ToArray().SequenceEqual(new string[] { "a", "b", "e", "c", "f" }));
            Assert.False(jointsB.ToArray().SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));
        }

        [Fact]
        public void TestIsSubset()
        {
            var jointsA = new JointSet("a", "b", "e");
            var jointsB = new JointSet("b", "a");
            Assert.True(jointsB.IsSubset(jointsA));
            Assert.True(jointsA.IsSubset(jointsA));
            Assert.True(jointsB.IsSubset(jointsB));
            Assert.False(jointsA.IsSubset(jointsB));
        }

        [Fact]
        public void TestIsSimilar()
        {
            var jointsA = new JointSet("a", "b", "e");
            var jointsB = new JointSet("e", "b", "a");
            var jointsC = new JointSet("e", "b", "d");
            Assert.True(jointsB.IsSimilar(jointsA));
            Assert.False(jointsA.IsSimilar(jointsC));
        }

        [Fact]
        public void TestEquals()
        {
            var jointsA = new JointSet("a", "b", "e");
            var jointsB = new JointSet("a", "b", "e");
            var jointsC = new JointSet("e", "b", "a");

            Assert.True(jointsB.Equals(jointsA));
            Assert.True(jointsA.Equals(jointsB));
            Assert.True(jointsA.Equals(jointsA));
            Assert.False(jointsA.Equals(jointsC));
        }

        [Fact]
        public void TestGetIndex()
        {
            var joints = new JointSet("a", "c", "b");
            Assert.Equal(2, joints.GetIndexOf("b"));
            Assert.Equal(0, joints.GetIndexOf("a"));

            Assert.Throws<System.Exception>(() => joints.GetIndexOf("d"));
            // TODO: The following assertion is derived from the documentation, which is not in accordance with the implementation
            Assert.Throws<System.ArgumentNullException>(() => joints.GetIndexOf(null));
        }

        [Fact]
        public void TestTryGetIndex()
        {
            var joints = new JointSet("a", "c", "b");
            var success = joints.TryGetIndexOf("b", out int val1);
            Assert.True(success);
            Assert.Equal(2, val1);
            success = joints.TryGetIndexOf("d", out int val2);
            Assert.False(success);
            // Assert.Equal(-1, val2);
        }

        [Fact]
        public void TestCount()
        {
            var joints = new JointSet("a", "c", "b");
            Assert.Equal(3, joints.Count());
        }

        [Fact]
        public void TestIndexAccess()
        {
            var joints = new JointSet("a", "c", "b");
            Assert.Equal("b", joints[2]);
            Assert.Throws<System.IndexOutOfRangeException>(() => joints[3]);
        }

        [Fact]
        public void TestContains()
        {
            var joints = new JointSet("a", "c", "b");
            Assert.True(joints.Contains("a"));
            Assert.False(joints.Contains("d"));
        }
    }
}
