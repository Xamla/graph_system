using System;
using Xunit;

namespace Xamla.Robotics.Types.Tests
{

    public class PlanParametersBuilderTests
    {
        [Fact]
        public void TestScaleAcceleration()
        {
            var builder = new PlanParameters.Builder();
            builder.MaxAcceleration = new double[] { 1000, -4 };
            Assert.Equal(builder.MaxAcceleration, new double[] { 1000, -4 });
<<<<<<< HEAD
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleAcceleration(1.1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleAcceleration(-0.1));
=======
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.ScaleAcceleration(1.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.ScaleAcceleration(-0.1));
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab
            Assert.Equal(builder.ScaleAcceleration(0.25).MaxAcceleration, new double[] { 250, -1 });
        }

        [Fact]
        public void TestScaleVelocity()
        {
            var builder = new PlanParameters.Builder();
            builder.MaxVelocity = new double[] { 1000, -4 };
            Assert.Equal(builder.MaxVelocity, new double[] { 1000, -4 });
<<<<<<< HEAD
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleVelocity(1.1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleVelocity(-0.1));
=======
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.ScaleVelocity(1.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.ScaleVelocity(-0.1));
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab
            Assert.Equal(builder.ScaleVelocity(0.25).MaxVelocity, new double[] { 250, -1 });
        }
    }

    public class PlanParametersTests
    {
        [Fact]
        public void TestInit()
        {
            var badBuilderA = new PlanParameters.Builder();
            badBuilderA.JointSet = new JointSet("a", "b", "c");
            badBuilderA.MaxVelocity = new double[] { 1, 2, 3 };
            badBuilderA.MaxAcceleration = new double[] { 1, 2 };
<<<<<<< HEAD
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new PlanParameters(badBuilderA));
=======
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlanParameters(badBuilderA));
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab

            var badBuilderB = new PlanParameters.Builder();
            badBuilderB.JointSet = new JointSet("a", "b", "c");
            badBuilderB.MaxVelocity = new double[] { 1, 2 };
            badBuilderB.MaxAcceleration = new double[] { 1, 2, 3 };
<<<<<<< HEAD
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new PlanParameters(badBuilderB));
=======
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlanParameters(badBuilderB));
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab

            var builder = new PlanParameters.Builder();
            builder.JointSet = new JointSet("a", "b", "c");
            builder.MaxVelocity = new double[] { 1, 2, 3 };
            builder.MaxAcceleration = new double[] { 1, 2, 3 };
            new PlanParameters(builder);
        }

        [Fact]
        public void TestWithCollision()
        {
            var builder = new PlanParameters.Builder();
            var parameters = new PlanParameters(builder);
<<<<<<< HEAD
            Assert.Equal(true, parameters.WithCollisionCheck(true).CollisionCheck);
            Assert.Equal(false, parameters.WithCollisionCheck(false).CollisionCheck);
=======
            Assert.True(parameters.WithCollisionCheck(true).CollisionCheck);
            Assert.False(parameters.WithCollisionCheck(false).CollisionCheck);
>>>>>>> ec039312842f62c6d9d445bd0e60c96ad9d2f2ab
        }

        [Fact]
        public void TestWithSampleResolution()
        {
            var builder = new PlanParameters.Builder();
            var parameters = new PlanParameters(builder);
            Assert.Equal(5, parameters.WithSampleResolution(5).SampleResolution);
            Assert.Equal(-3, parameters.WithSampleResolution(-3).SampleResolution);
        }

        [Fact]
        public void TestWithMaxDeviation()
        {
            var builder = new PlanParameters.Builder();
            var parameters = new PlanParameters(builder);
            Assert.Equal(5, parameters.WithMaxDeviation(5).MaxDeviation);
            Assert.Equal(-3, parameters.WithMaxDeviation(-3).MaxDeviation);
        }
    }
}
