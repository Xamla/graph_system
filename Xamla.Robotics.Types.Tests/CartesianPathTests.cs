using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xamla.Types;
using Xamla.Utilities;
using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class JointCartesianPathTests
    {
        PoseHelper poseHelper = new PoseHelper();

        [Fact]
        public void TestInit()
        {
            int count = 50;
            List<Pose> poses = poseHelper.RandomPoses(count).ToList();
            var path = new CartesianPath(poses);
            int i = 0;
            foreach (var pose in poses)
            {
                Assert.Equal(pose, path[i]);
                ++i;
            }
            Assert.Equal(count, path.Count());
        }

        [Fact]
        public void TestAppend()
        {

        }
    }
}