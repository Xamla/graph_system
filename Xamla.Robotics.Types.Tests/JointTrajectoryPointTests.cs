using System;

using Xunit;

namespace Xamla.Robotics.Types.Tests
{

<<<<<<< HEAD
    public class JointTrajectoryPointTests
=======
    public class JointTrajectoryPointsTests
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab
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
<<<<<<< HEAD
        public void TestInterpolation()
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
            var velocity = new JointValues(joints, new double[] { 1, 1, 1 });
            var acceleration = new JointValues(joints, new double[] { 0, 0, 0 });
            var positionA = new JointValues(joints, new double[] { 0, 0, 0 });
            var positionB = new JointValues(joints, new double[] { 1, 1, 1 });
            var positionC = new JointValues(joints, new double[] { 2, 2, 2 });
            var timeA = TimeSpan.FromSeconds(1);
            var timeB = TimeSpan.FromSeconds(2);
            var timeC = TimeSpan.FromSeconds(3);
            var pointA = new JointTrajectoryPoint(timeA, positionA, velocity, acceleration);
            var pointB = new JointTrajectoryPoint(timeB, positionB, velocity, acceleration);
            var pointC = new JointTrajectoryPoint(timeC, positionC, velocity, acceleration);
            JointTrajectoryPoint evalA = pointA.InterpolateCubic(pointC, timeA);
            AssertEqualPoints(evalA, pointA);
            JointTrajectoryPoint evalC = pointA.InterpolateCubic(pointC, timeC);
            AssertEqualPoints(evalC, pointC);
            // This one is interpolated between A and C
            JointTrajectoryPoint evalB = pointA.InterpolateCubic(pointC, timeB);
            AssertEqualPoints(evalB, pointB);

            // Assert that value out of boundaries are clipped
            var timeAOut = new TimeSpan(0);
            var timeCOut = new TimeSpan(400000);
            JointTrajectoryPoint evalAOut = pointA.InterpolateCubic(pointC, timeAOut);
            AssertEqualPoints(evalAOut, pointA.WithTimeFromStart(timeAOut));
            JointTrajectoryPoint evalCOut = pointA.InterpolateCubic(pointC, timeCOut);
            AssertEqualPoints(evalCOut, pointA.WithTimeFromStart(timeCOut));
        }

        [Fact]
=======
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab
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