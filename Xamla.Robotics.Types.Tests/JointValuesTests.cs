using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class JointValuesTests
    {
        [Fact]
        public void TestMerge()
        {
            var jointsA = new JointSet("a", "b", "c");
            var jointsB = new JointSet("d", "e", "f");
            var a = new JointValues(jointsA, new double[] { 0, 1, 2 });
            var b = new JointValues(jointsB, new double[] { 3, 4, 5 });

            var result = a.Merge(b);

            Assert.Equal(6, result.Count);
            Assert.True(result.Values.SequenceEqual(new double[] { 0, 1, 2, 3, 4, 5 }));

            var fullJoints = jointsA.Combine(jointsB);
            Assert.True(result.JointSet.IsSimilar(fullJoints));
        }

        [Fact]
        public void TestInit()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 7, 6, 5 };

            // test value clone in ctor
            var a = new JointValues(jointsA, array);
            Assert.True(a.Values.SequenceEqual(array));
            array[1] = 5;
            Assert.False(a.Values.SequenceEqual(array));
            Assert.NotSame(a.Values, array);
            // same amount of names and values
            Assert.Throws<Exception>(() => new JointValues(jointsA, new double[] { 1 }));
            Assert.Throws<Exception>(() => new JointValues(jointsA, new double[] { 1, 2, 3, 4, 5 }));

            var b = new JointValues(jointsA, 3);
            Assert.Equal(3, b.Count);
            Assert.True(b.Values.All(x => x == 3));
        }

        [Fact]
        public void TestUpdateSingle()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 7, 6, 5 };

            var a = new JointValues(jointsA, array);

            Assert.Equal(7, a[0]);
            Assert.Equal(6, a[1]);
            Assert.Equal(5, a[2]);

            var b = a.WithValue("c", -3);
            Assert.Equal(-3, b[2]);

            var c = b.WithValue("a", 1);
            Assert.Equal(1, c[0]);
        }

        [Fact]
        public void TestTryGetValue()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 7, 6, 5 };

            var a = new JointValues(jointsA, array);

            var success = a.TryGetValue("b", out double val1);
            Assert.True(success);
            Assert.Equal(6.0, val1);

            success = a.TryGetValue("d", out double val2);
            Assert.False(success);
        }

        [Fact]
        public void TestGetValue()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 7, 6, 5 };

            var a = new JointValues(jointsA, array);
            Assert.Equal(6, a.GetValue("b"));
            Assert.Equal(7, a.GetValue("a"));

            Assert.Throws<System.Exception>(() => a.GetValue("d"));
            // TODO: The function is derived of the documentation, which is not in accordance with the implementation
            // Assert.Throws<System.ArgumentNullException>(() => a.GetValue(null));
        }

        [Fact]
        public void TestReorder()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 7, 6, 5 };
            var a = new JointValues(jointsA, array);

            var jointsB = new JointSet("b", "c", "a");

            var b = a.Reorder(jointsB);
            // Assert that associations of names and value remain the same
            Assert.Equal(7, b.GetValue("a"));
            Assert.Equal(6, b.GetValue("b"));
            Assert.Equal(5, b.GetValue("c"));

            Assert.Equal(6, b[0]);
            Assert.Equal(5, b[1]);
            Assert.Equal(7, b[2]);

            Assert.Throws<System.ArgumentException>(() => a.Reorder(new JointSet("a", "b")));
        }

        [Fact]
        public void TestSelect()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 7, 6, 5 };
            var a = new JointValues(jointsA, array);

            var jointsB = new JointSet("c", "b");

            var b = a.Select(jointsB);

            Assert.Throws<System.Exception>(() => b.GetValue("a"));
            Assert.Equal(6, b.GetValue("b"));
            Assert.Equal(5, b.GetValue("c"));
            // Assert that the new jointset defines the order
            Assert.Equal(5, b[0]);
            Assert.Equal(6, b[1]);

            Assert.Throws<System.ArgumentException>(() => b.Select(jointsA));
        }

        [Fact]
        public void TestSetValues()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 7, 6, 5 };
            var a = new JointValues(jointsA, array);

            var b = a.SetValues(new JointSet("a", "b"), new double[] { 1, 2 });
            Assert.Equal(1, b[0]);
            Assert.Equal(2, b[1]);
            Assert.Equal(5, b[2]);

            Assert.Throws<System.ArgumentException>(() => a.SetValues(new JointSet("a", "d"), new double[] { 1, 2 }));
            // Throws a System.IndexOutOfRangeException when not the same size
            Assert.Throws<System.IndexOutOfRangeException>(() => a.SetValues(new JointSet("a"), new double[] { 1, 2 }));


            var c = new JointValues(new JointSet("a", "b"), new double[] { 1, 2 });
            var d = a.SetValues(new JointSet("a", "b"), new double[] { 1, 2 });
            Assert.Equal(1, d[0]);
            Assert.Equal(2, d[1]);
            Assert.Equal(5, d[2]);
        }

        [Fact]
        public void TestWithValue()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 7, 6, 5 };
            var a = new JointValues(jointsA, array);

            var b = a.WithValue("a", 1);
            Assert.Equal(1, b[0]);

            var c = a.WithValue(0, 2);
            Assert.Equal(2, c[0]);
            Assert.Throws<System.IndexOutOfRangeException>(() => a.WithValue(3, 1));
            Assert.Throws<System.Exception>(() => a.WithValue("d", 1));
        }

        [Fact]
        public void TestTransform()
        {
            var jointsA = new JointSet("a", "b", "c");
            var array = new double[] { 2, 2, 2 };
            var a = new JointValues(jointsA, array);

            Func<double, int, double> func1 = (value, i) => value * i;
            var b = a.Transform(func1);
            Assert.Equal(0, b[0]);
            Assert.Equal(2, b[1]);
            Assert.Equal(4, b[2]);

            Func<double, int, double> func2 = (value, i) => 0;
            var c = a.Transform(func2);
            Assert.Equal(0, c[0]);
            Assert.Equal(0, c[1]);
            Assert.Equal(0, c[2]);
        }

        [Fact]
        public void TestMath()
        {
            var a = new JointValues(new JointSet("a", "b", "c"), new double[] { 2, 2, 2 });
            var b = new JointValues(new JointSet("b", "a", "c"), new double[] { 2, 1, 3 });
            var c = a.Add(b);
            Assert.Equal(3, c[0]);
            Assert.Equal(4, c[1]);
            Assert.Equal(5, c[2]);

            var d = b.Add(-1);
            Assert.Equal(1, d[0]);
            Assert.Equal(0, d[1]);
            Assert.Equal(2, d[2]);

            var e = a.Subtract(b);
            Assert.Equal(1, e[0]);
            Assert.Equal(0, e[1]);
            Assert.Equal(-1, e[2]);

            var f = b.Subtract(1);
            Assert.True(f.Equals(d));

            var g = b.Divide(2);
            Assert.Equal(1, g[0]);
            Assert.Equal(0.5, g[1]);
            Assert.Equal(1.5, g[2]);

            var h = b.Multiply(2);
            Assert.Equal(4, h[0]);
            Assert.Equal(2, h[1]);
            Assert.Equal(6, h[2]);

            var i = b.Negate();
            Assert.Equal(-2, i[0]);
            Assert.Equal(-1, i[1]);
            Assert.Equal(-3, i[2]);
        }

        [Fact]
        public void TestRandom()
        {
            var joints = new JointSet("a", "b");
            var maxAcc = new double? [] {10, 10};
            var maxVel = new double? [] {50, 10};
            var minPos = new double? [] {-10, 0};
            var maxPos = new double? [] {50, 10};
            JointLimits jl = new JointLimits(joints, maxVel, maxAcc, minPos, maxPos );

            for(int i = 0; i< 100; ++i){
                var a = JointValues.Random(jl);
                Assert.True(a[0] >= minPos[0]);
                Assert.True(a[1] >= minPos[1]);
                Assert.True(a[0] <= maxPos[0]);
                Assert.True(a[1] <= maxPos[1]);
            }

            Assert.Throws<System.ArgumentNullException>(() => JointValues.Random(null));
            // assert that there must be a jointlimit for every name in jointset
            maxAcc = new double? [] {10, 10};
            maxVel = new double? [] {50, 10};
            minPos = new double? [] {null, 0};
            maxPos = new double? [] {50, 10};
            jl = new JointLimits(joints, maxVel, maxAcc, minPos, maxPos );
            Assert.Throws<System.ArgumentException>(() => JointValues.Random(jl));
        }
    }
}
