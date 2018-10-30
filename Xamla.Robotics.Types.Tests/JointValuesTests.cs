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
    }
}
