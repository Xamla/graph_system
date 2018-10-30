using System;

using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class JointPAthTests
    {
        [Fact]
        public void TestInit()
        {
            var joints = new JointSet("a", "b", "c");
            var start = new JointValues(joints, new double[] { 1, 2, 3 });
            var goal = new JointValues(joints, new double[] { 0, 0, 0 });
            var badGoal = new JointValues(new JointSet("d", "b", "c"), new double[] { 0, 0, 0 });

            var p1 = new JointPath(start);
            Assert.Equal(start, p1[0]);

            var p2 = new JointPath(start, goal);
            Assert.Equal(start, p2[0]);
            Assert.Equal(goal, p2[1]);

            Assert.Throws<System.ArgumentException>(() => new JointPath(start, badGoal));

            var p3 = new JointPath(joints, start, goal);
            Assert.Equal(start, p3[0]);
            Assert.Equal(goal, p3[1]);

            var p4 = new JointPath(joints, new JointValues[] { start, goal });
            Assert.Equal(start, p4[0]);
            Assert.Equal(goal, p4[1]);
        }


        [Fact]
        public void TestAppend()
        {
            var joints = new JointSet("a", "b", "c");
            var val1 = new JointValues(joints, new double[] { 1, 2, 3 });
            var val2 = new JointValues(joints, new double[] { 5, 5, 5 });
            var val3 = new JointValues(joints, new double[] { 0, 0, 0 });
            var p = new JointPath(val1).Append(new JointValues[] { val2, val3 });

            Assert.Equal(val1, p[0]);
            Assert.Equal(val2, p[1]);
            Assert.Equal(val3, p[2]);
        }

        [Fact]
        public void TestPrepend()
        {
            var joints = new JointSet("a", "b", "c");
            var val1 = new JointValues(joints, new double[] { 1, 2, 3 });
            var val2 = new JointValues(joints, new double[] { 5, 5, 5 });
            var val3 = new JointValues(joints, new double[] { 0, 0, 0 });
            var p = new JointPath(val1).Prepend(new JointValues[] { val2, val3 });

            Assert.Equal(val2, p[0]);
            Assert.Equal(val3, p[1]);
            Assert.Equal(val1, p[2]);
        }


        [Fact]
        public void TestConcat()
        {
            var joints = new JointSet("a", "b", "c");
            var val1 = new JointValues(joints, new double[] { 1, 2, 3 });
            var val2 = new JointValues(joints, new double[] { 5, 5, 5 });
            var val3 = new JointValues(joints, new double[] { 0, 0, 0 });
            var p1 = new JointPath(val1);
            var p2 = new JointPath(val2, val3);
            var p = p1.Concat(p2);

            Assert.Equal(val1, p[0]);
            Assert.Equal(val2, p[1]);
            Assert.Equal(val3, p[2]);
        }


        [Fact]
        public void TestSub()
        {
            var joints = new JointSet("a", "b", "c");
            var val1 = new JointValues(joints, new double[] { 1, 2, 3 });
            var val2 = new JointValues(joints, new double[] { 5, 5, 5 });
            var val3 = new JointValues(joints, new double[] { 0, 0, 0 });
            var val4 = new JointValues(joints, new double[] { 2, 2, 2 });
            var pBig = new JointPath(joints, val1, val2, val3, val4);

            var p = pBig.Sub(1, 3);
            Assert.Equal(val2, p[0]);
            Assert.Equal(val3, p[1]);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => p[2]);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => pBig.Sub(1, 5));
        }

        [Fact]
        public void TestTransform()
        {
            var joints = new JointSet("a", "b", "c");
            var val1 = new JointValues(joints, new double[] { 1, 2, 3 });
            var val2 = new JointValues(joints, new double[] { 5, 5, 5 });
            var val3 = new JointValues(joints, new double[] { 0, 0, 0 });
            var p1 = new JointPath(joints, val1, val2, val3);

            Func<JointValues, int, JointValues> func = (jVals, i) => jVals.Add(i);
            var p = p1.Transform(func);
            Assert.Equal(val1, p[0]);
            Assert.Equal(new JointValues(joints, new double[] { 6, 6, 6 }), p[1]);
            Assert.Equal(new JointValues(joints, new double[] { 2, 2, 2 }), p[2]);

        }

    }
}