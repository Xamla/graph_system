using System;

using Xunit;

namespace Xamla.Robotics.Types.Tests
{

    public class JointTrajectoryPointsTests
    {
        private static JointTrajectoryPoint GetPoint()
        {
            var timeSpan = new TimeSpan(100);
            var jointSet = new JointSet("a", "b", "c");
            var pos = new JointValues(jointSet, new double[] { 0, 1, 2 });
            var vel = new JointValues(jointSet, new double[] { 3, 4, 5 });
            var acc = new JointValues(jointSet, new double[] { 6, 7, 8 });
            var eff = new JointValues(jointSet, new double[] { 9, 10, 11 });
            var p = new JointTrajectoryPoint(timeSpan, pos, vel, acc, eff);
            return p;
        }

        [Fact]
        public void TestInit()
        {
            var timeSpan = new TimeSpan(100);
            var p = GetPoint();
            Assert.Equal(timeSpan, p.TimeFromStart);
        }

        [Fact]
        public void TestAddTimeOffSet()
        {
            var timeSpan = new TimeSpan(200);
            var p = GetPoint();
            var q = p.AddTimeOffset(timeSpan);
            Assert.Equal(q.TimeFromStart - timeSpan, p.TimeFromStart);
        }

        [Fact]
        public void TestMerge()
        {
            var p1 = GetPoint();
            var timeSpan = p1.TimeFromStart;
            var falseTimeSpan = new TimeSpan(200);
            var jointSet = new JointSet("d", "e");
            var pos = new JointValues(jointSet, new double[] { 0, 1 });
            var vel = new JointValues(jointSet, new double[] { 3, 4 });
            var acc = new JointValues(jointSet, new double[] { 6, 7 });
            var eff = new JointValues(jointSet, new double[] { 9, 10 });
            var p2 = new JointTrajectoryPoint(timeSpan, pos, vel, acc, eff);
            var p = p1.Merge(p2);
            var p3 = new JointTrajectoryPoint(falseTimeSpan, pos, vel, acc, eff);
            Assert.Throws<Exception>(() => p1.Merge(p3) );
        }
    }
}