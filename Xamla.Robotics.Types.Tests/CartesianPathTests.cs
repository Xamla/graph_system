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
        public void TestPrepend()
        {
            int count = 50;
            List<Pose> poses1 = poseHelper.RandomPoses(count).ToList();
            List<Pose> poses2 = poseHelper.RandomPoses(count).ToList();
            List<Pose> poses = poses1.Concat(poses2).ToList();
            ICartesianPath path = new CartesianPath(poses2);
            path = path.Prepend(poses1);
            for (int i = 0; i < 2 * count; ++i)
            {
                Assert.Equal(poses[i], path[i]);
            }
            Assert.Equal(2 * count, path.Count());
        }

        [Fact]
        public void TestAppend()
        {
            int count = 50;
            List<Pose> poses1 = poseHelper.RandomPoses(count).ToList();
            List<Pose> poses2 = poseHelper.RandomPoses(count).ToList();
            List<Pose> poses = poses1.Concat(poses2).ToList();
            ICartesianPath path = new CartesianPath(poses1);
            path = path.Append(poses2);
            for (int i = 0; i < 2 * count; ++i)
            {
                Assert.Equal(poses[i], path[i]);
            }
            Assert.Equal(2 * count, path.Count());
        }

        [Fact]
        public void TestConcat()
        {
            int count = 50;
            List<Pose> poses1 = poseHelper.RandomPoses(count).ToList();
            List<Pose> poses2 = poseHelper.RandomPoses(count).ToList();
            List<Pose> poses = poses1.Concat(poses2).ToList();
            ICartesianPath path1 = new CartesianPath(poses1);
            ICartesianPath path2 = new CartesianPath(poses2);
            ICartesianPath path = path1.Concat(path2);
            for (int i = 0; i < 2 * count; ++i)
            {
                Assert.Equal(poses[i], path[i]);
            }
            Assert.Equal(2 * count, path.Count());
        }

        [Fact]
        public void TestSub()
        {
            int count = 10;
            List<Pose> poses = poseHelper.RandomPoses(count).ToList();
            ICartesianPath pathBig = new CartesianPath(poses);
            int beg = 2;
            int end = 5;
            ICartesianPath path = pathBig.Sub(beg, end);
            Assert.Equal(end - beg , path.Count());
            for (int i = 0; i < end  - beg; ++i)
            {
                Assert.Equal(pathBig[i + beg], path[i]);
            }

            Assert.Throws<System.ArgumentOutOfRangeException>(() => path.Sub(0, count+1 ));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => path.Sub(-1, count-1 ));
        }
    }
}