using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class JointTrajectoryTests
    {
        private static JointTrajectoryPoint GetPoint(long ticks, JointSet joints)
        {
            var timeSpan = new TimeSpan(ticks);
            var pos = new JointValues(joints, new double[] { 0, 1, 2 });
            var vel = new JointValues(joints, new double[] { 3, 4, 5 });
            var acc = new JointValues(joints, new double[] { 6, 7, 8 });
            var eff = new JointValues(joints, new double[] { 9, 10, 11 });
            var p = new JointTrajectoryPoint(timeSpan, pos, vel, acc, eff);
            return p;
        }

        [Fact]
        public void TestInit()
        {
            var joints = new JointSet("a", "b", "c");
            var p1 = GetPoint(100, joints);
            var p2 = GetPoint(200, joints);
            var p3 = GetPoint(300, joints);
            var t = new JointTrajectory(joints, new JointTrajectoryPoint[] { p1, p2, p3 });
            Assert.Equal(3, t.Count);
            Assert.Equal(p1, t[0]);
            Assert.Equal(p2, t[1]);
            Assert.Equal(p3, t[2]);
            Assert.Throws<System.ArgumentException>(() => new JointTrajectory(joints, new JointTrajectoryPoint[] { p1, p3, p2 }));
        }

        [Fact]
        public void TestAppend()
        {
            var joints = new JointSet("a", "b", "c");
            var p1 = GetPoint(100, joints);
            var p2 = GetPoint(200, joints);
            var p3 = GetPoint(300, joints);
            var t1 = new JointTrajectory(joints, new JointTrajectoryPoint[] { p1, p2 });
            var points = new JointTrajectoryPoint[] { p3 };
            var t = t1.Append(points);
            Assert.Equal(3, t.Count);
            Assert.Equal(p1, t[0]);
            Assert.Equal(p2, t[1]);
            // assert that duration of t1 gets added to t2
            Assert.Equal(GetPoint(500, joints), t[2]);
        }

        [Fact]
        public void TestConcat()
        {
            var joints = new JointSet("a", "b", "c");
            var p1 = GetPoint(100, joints);
            var p2 = GetPoint(200, joints);
            var p3 = GetPoint(300, joints);
            var t1 = new JointTrajectory(joints, new JointTrajectoryPoint[] { p1, p2 });
            var t2 = new JointTrajectory(joints, new JointTrajectoryPoint[] { p3 });
            var t = t1.Concat(t2);
            Assert.Equal(3, t.Count);
            Assert.Equal(p1, t[0]);
            Assert.Equal(p2, t[1]);
            // assert that duration of t1 gets added to t2
            Assert.Equal(GetPoint(500, joints), t[2]);
        }

        [Fact]
        public void TestSub()
        {
            var joints = new JointSet("a", "b", "c");
            var p1 = GetPoint(50, joints);
            var p2 = GetPoint(200, joints);
            var p3 = GetPoint(300, joints);
            var p4 = GetPoint(800, joints);
            var tBig = new JointTrajectory(joints, new JointTrajectoryPoint[] { p1, p2, p3, p4 });
            var t = tBig.Sub(1, 3);
            Assert.Equal(2, t.Count);
            // assert correct duration
            Assert.Equal(GetPoint(200, joints), t[0]);
            Assert.Equal(GetPoint(300, joints), t[1]);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => tBig.Sub(0, tBig.Count ));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => tBig.Sub(-1, tBig.Count-1 ));
        }
    }
}