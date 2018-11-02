using System;
using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class JointLimitsTests
    {
        [Fact]
        public void TestInit()
        {
            var joints = new JointSet("a", "b");
            var tooManyJoints = new JointSet("a", "b", "c");
            var notEnoughJoints = new JointSet("a");
            var maxAcc = new double?[] { 10, 10 };
            var maxVel = new double?[] { 50, 10 };
            var minPos = new double?[] { -10, 0 };
            var maxPos = new double?[] { 50, 10 };
            JointLimits jl = new JointLimits(joints, maxVel, maxAcc, minPos, maxPos);

            Assert.Throws<ArgumentNullException>(() => new JointLimits(joints, null, maxAcc, minPos, maxPos));
            Assert.Throws<ArgumentException>(() => new JointLimits(tooManyJoints, maxVel, maxAcc, minPos, maxPos));
            Assert.Throws<ArgumentException>(() => new JointLimits(notEnoughJoints, maxVel, maxAcc, minPos, maxPos));
        }
    }
}