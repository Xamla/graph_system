using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xamla.Types;
using Xamla.Utilities;
using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class PoseTests
    {
        Random rng = ThreadSafeRandom.Generator;

        public Vector3 RandomVector(double sigma = 1)
        {
            var x = rng.Normal(0, sigma).Take(3).ToArray();
            return new Vector3((float)x[0], (float)x[1], (float)x[2]);
        }

        public Vector3 RandomAxis() =>
            Vector3.Normalize(RandomVector(1));

        public Quaternion RandomRotation() =>
            Quaternion.CreateFromAxisAngle(RandomAxis(), (float)((rng.NextDouble()-0.5) * 4 * Math.PI));

        public Pose RandomPose() =>
            new Pose(RandomVector(100), RandomRotation());

        public IEnumerable<Pose> RandomPoses()
        {
            while (true)
                yield return RandomPose();
        }

        public IEnumerable<Pose> RandomPoses(int n) =>
            RandomPoses().Take(n);

        public float SeqAbsDiff(IEnumerable<float> a, IEnumerable<float> b) =>
            a.Zip(b, (x, y) => Math.Abs(x - y)).Sum();

        [Fact]
        public void TestInverse()
        {
            // compare to matrix inverse
            foreach (var p in RandomPoses(100))
            {
                Pose q = p.Inverse();

                Matrix4x4 m1 = q.TransformMatrix;
                Assert.True(Matrix4x4.Invert(p.TransformMatrix, out Matrix4x4 m2));

                double delta = SeqAbsDiff(m1.ToRowMajorArray(), m2.ToRowMajorArray());
                Assert.True(delta < 1E-3);
            }

            // compare via multiply by inverse
            foreach (var p in RandomPoses(100))
            {
                Pose q = p.Inverse();
                Pose i = p * q;

                var m1 = i.TransformMatrix;
                var m2 = Pose.Identity.TransformMatrix;

                double delta = SeqAbsDiff(m1.ToRowMajorArray(), m2.ToRowMajorArray());
                Assert.True(delta < 1E-3);
            }
        }

        [Fact]
        public void TestMultiply()
        {
            foreach (var (p,q) in RandomPoses(100).Zip(RandomPoses(100), (x,y) => (x,y)))
            {
                var m1 = (p * q).TransformMatrix;
                var m2 = p.TransformMatrix * q.TransformMatrix;

                double delta = SeqAbsDiff(m1.ToRowMajorArray(), m2.ToRowMajorArray());
                Assert.True(delta < 1E-3);
            }

            for (int i = 0; i < 100; i++)
            {
                var r = RandomPoses(3).ToArray();
                var (a, b, c) = (r[0], r[1], r[2]);

                var m1 = (a * b * c).TransformMatrix;
                var m2 = a.TransformMatrix * b.TransformMatrix * c.TransformMatrix;

                double delta = SeqAbsDiff(m1.ToRowMajorArray(), m2.ToRowMajorArray());
                Assert.True(delta < 1E-3);
            }
        }
    }
}
