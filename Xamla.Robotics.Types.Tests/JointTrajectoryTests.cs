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

        private static JointTrajectoryPoint GetPoint(long ticks, JointSet joints)
        {
            var timeSpan = new TimeSpan(ticks);
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
            Assert.Throws<System.ArgumentException>(() => new JointTrajectory(joints, new JointTrajectoryPoint[] { p1, p3, p2 }));
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
            Assert.Equal(p3.WithTimeFromStart(new TimeSpan(500)), t[2]);
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
            Assert.Equal(p3.WithTimeFromStart(new TimeSpan(500)), t[2]);
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
            Assert.Equal(p3.WithTimeFromStart(new TimeSpan(500)), t[2]);
        }


        [Fact]
        public void TestEvaluateAt()
        {
            void AssertEqualPoints(JointTrajectoryPoint a, JointTrajectoryPoint b)
            {
                if(a.TimeFromStart == b.TimeFromStart && a.JointSet.Equals(b.JointSet))
                    return;
                else
                    Assert.Equal("a", "b");
            }
            var joints = new JointSet("a", "b", "c");
            JointTrajectoryPoint[] points = new JointTrajectoryPoint[10] ;
            for (int i = 0; i < 10; ++i){
                points[i] = GetPoint(i, joints);
            }
            Assert.Equal(points.Count(), 10);
            var t = new JointTrajectory(joints, points);
            for (int i = 0; i < 50; ++i)
            {
                int time = rng.Next(10);
                var timeSpan = new TimeSpan(time);
                int delay = rng.Next(-2, 2);
                var delaySpan = new TimeSpan(delay);
                var pA = t.EvaluateAt(timeSpan, TimeSpan.Zero);
                AssertEqualPoints(pA, points[time]);
                var pB = t.EvaluateAt(timeSpan, delaySpan);
                int newTime = time-delay;
                if (newTime >= 0 && newTime < 10 ){
                    AssertEqualPoints(pA, points[newTime].WithTimeFromStart(new TimeSpan(time)));
                }
            }
        }


        [Fact]
        public void TestMerge()
        {
            var timeSpan = new TimeSpan(100);
            var jointsA = new JointSet("a", "b", "c");
            var jointsB = new JointSet("e","f");

            var p1 = GetPoint(100, jointsA);
            var p2 = GetPoint(100, jointsB);
            var p3 = GetPoint(300, jointsB);
            var t1 = new JointTrajectory(jointsA, new JointTrajectoryPoint[] { p1 });
            var t2 = new JointTrajectory(jointsB, new JointTrajectoryPoint[] { p2 });
            var t = t1.Merge(t2);
            Assert.Equal(1, t.Count);

            var t3 = new JointTrajectory(jointsB, new JointTrajectoryPoint[] { p2, p3 });
            Assert.Throws<System.ArgumentException>(() => t1.Merge(t3));
            var t4 = new JointTrajectory(jointsB, new JointTrajectoryPoint[] { p3 });       
            Assert.Throws<System.Exception>(() => t1.Merge(t4));
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
            Assert.Equal(p2.WithTimeFromStart(new TimeSpan(200)), t[0]);
            Assert.Equal(p3.WithTimeFromStart(new TimeSpan(300)), t[1]);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => tBig.Sub(0, tBig.Count ));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => tBig.Sub(-1, tBig.Count-1 ));
        }
    }
}