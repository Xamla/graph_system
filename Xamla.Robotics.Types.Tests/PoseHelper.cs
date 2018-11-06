using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Xamla.Utilities;

namespace Xamla.Robotics.Types.Tests
{
    public class PoseHelper
    {
        Random rng = ThreadSafeRandom.Generator;
<<<<<<< HEAD
        public PoseHelper(){

=======

        public PoseHelper()
        {
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab
        }

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
<<<<<<< HEAD
            new Pose(RandomVector(100), RandomRotation());
=======
            new Pose(RandomVector(100), RandomRotation(), "world", true);
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab

        public IEnumerable<Pose> RandomPoses()
        {
            while (true)
                yield return RandomPose();
        }

        public IEnumerable<Pose> RandomPoses(int n) =>
            RandomPoses().Take(n);

        public double RandomDouble() =>
            rng.NextDouble();
    }
}