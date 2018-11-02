using System;

using Xunit;

namespace Xamla.Robotics.Types.Tests
{

    public class JointTrajectoryPointsTests
    {
        private static JointTrajectoryPoint GetPoint()
        {
            var timeSpan = new TimeSpan(100);
            var pos = new JointValues(new JointSet("a", "b", "c"), new double[] { 0, 1, 2 });
            var vel = new JointValues(new JointSet("a", "b", "c"), new double[] { 3, 4, 5 });
            var acc = new JointValues(new JointSet("a", "b", "c"), new double[] { 6, 7, 8 });
            var eff = new JointValues(new JointSet("a", "b", "c"), new double[] { 9, 10, 11 });
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
    }
}