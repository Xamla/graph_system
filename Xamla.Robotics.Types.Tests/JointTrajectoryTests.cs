using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamla.Utilities;

using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class JointTrajectoryTests
    {
        static Random rng = ThreadSafeRandom.Generator;

        private static JointTrajectoryPoint GetPoint(double seconds, JointSet joints)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            int count = joints.Count;
            var values = rng.Normal(0, 1).Take(count).ToArray();
            var pos = new JointValues(joints, values);
            values = rng.Normal(0, 1).Take(count).ToArray();
            var vel = new JointValues(joints, values);
            values = rng.Normal(0, 1).Take(count).ToArray();
            var acc = new JointValues(joints, values);
            values = rng.Normal(0, 1).Take(count).ToArray();
            var eff = new JointValues(joints, values);
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
            Assert.Throws<ArgumentException>(() => new JointTrajectory(joints, new JointTrajectoryPoint[] { p1, p3, p2 }));
        }


        [Fact]
        public void TestPrepend()
        {
            var joints = new JointSet("a", "b", "c");
            var p1 = GetPoint(100, joints);
            var p2 = GetPoint(200, joints);
            var p3 = GetPoint(300, joints);
            var t1 = new JointTrajectory(joints, new JointTrajectoryPoint[] { p3 });
            var points = new JointTrajectoryPoint[] { p1, p2 };
            var t = t1.Prepend(points);
            Assert.Equal(3, t.Count);
            Assert.Equal(p1, t[0]);
            Assert.Equal(p2, t[1]);
            // assert that duration of t1 gets added to t2
            Assert.Equal(p3.WithTimeFromStart(TimeSpan.FromSeconds(500)), t[2]);
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
            Assert.Equal(p3.WithTimeFromStart(TimeSpan.FromSeconds(500)), t[2]);
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
            Assert.Equal(p3.WithTimeFromStart(TimeSpan.FromSeconds(500)), t[2]);
        }


        [Fact]
        public void TestEvaluateAt()
        {
            void AssertEqualPoints(JointTrajectoryPoint a, JointTrajectoryPoint b)
            {
                bool Compare(JointValues aa, JointValues bb)
                {
                    double delta = Math.Abs(aa.MaxNorm() - bb.MaxNorm());
                    return delta < 1E-6;
                }
                Assert.Equal(a.TimeFromStart, b.TimeFromStart);
                Assert.True(a.JointSet == b.JointSet);
                Assert.True(Compare(a.Positions, b.Positions));
                Assert.True(Compare(a.Velocities, b.Velocities));
            }
            var joints = new JointSet("a", "b", "c");
            JointTrajectoryPoint[] points = new JointTrajectoryPoint[10];
            for (int i = 0; i < 10; ++i)
            {
                points[i] = GetPoint(i, joints);
            }
            Assert.Equal(points.Count(), 10);
            var traj = new JointTrajectory(joints, points);
            for (int i = 0; i < 50; ++i)
            {
                // Create negative and out of bound time values, to assure exceptions are thrown accordingly
                double time = rng.NextDouble() * 12 - 2;
                var index = (int)time;
                var k = Math.Min(index + 1, 9);
                var timeSpan = TimeSpan.FromSeconds(time);
                if (time < 0 || time > 9)
                    Assert.Throws<ArgumentOutOfRangeException>(() => traj.EvaluateAt(timeSpan));
                else
                {
                    var pointGT = traj[index].InterpolateCubic(traj[k], TimeSpan.FromSeconds(time));
                    // time = Math.Min(time, 9);
                    // time = Math.Max(time, 0);
                    var pointEval = traj.EvaluateAt(timeSpan);
                    AssertEqualPoints(pointGT, pointEval);
                }
            }
        }

        [Fact]
        public void TestMerge()
        {
            var timeSpan = TimeSpan.FromSeconds(100);
            var jointsA = new JointSet("a", "b", "c");
            var jointsB = new JointSet("e", "f");

            var pA1 = GetPoint(0, jointsA);
            var pA2 = GetPoint(200, jointsA);
            var pA3 = GetPoint(300, jointsA);
            var pB1 = GetPoint(0, jointsB);
            var pB2 = GetPoint(200, jointsB);
            var pB3 = GetPoint(300, jointsB);

            var t1 = new JointTrajectory(jointsA, new JointTrajectoryPoint[] { pA1, pA2, pA3 });
            var t2 = new JointTrajectory(jointsB, new JointTrajectoryPoint[] { pB1, pB2, pB3 });
            var t = t1.Merge(t2);
            Assert.True(Math.Abs(t[1].TimeFromStartSeconds - 150) < 1E-9);
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
            Assert.Equal(p2.WithTimeFromStart(TimeSpan.FromSeconds(200)), t[0]);
            Assert.Equal(p3.WithTimeFromStart(TimeSpan.FromSeconds(300)), t[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tBig.Sub(0, tBig.Count));
            Assert.Throws<ArgumentOutOfRangeException>(() => tBig.Sub(-1, tBig.Count - 1));
        }
    }
}