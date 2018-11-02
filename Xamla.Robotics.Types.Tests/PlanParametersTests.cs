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
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleAcceleration(1.1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleAcceleration(-0.1));
            Assert.Equal(builder.ScaleAcceleration(0.25).MaxAcceleration, new double[] { 250, -1 });
        }

        [Fact]
        public void TestScaleVelocity()
        {
            var builder = new PlanParameters.Builder();
            builder.MaxVelocity = new double[] { 1000, -4 };
            Assert.Equal(builder.MaxVelocity, new double[] { 1000, -4 });
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleVelocity(1.1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleVelocity(-0.1));
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
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new PlanParameters(badBuilderA));

            var badBuilderB = new PlanParameters.Builder();
            badBuilderB.JointSet = new JointSet("a", "b", "c");
            badBuilderB.MaxVelocity = new double[] { 1, 2 };
            badBuilderB.MaxAcceleration = new double[] { 1, 2, 3 };
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new PlanParameters(badBuilderB));

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
            Assert.Equal(true, parameters.WithCollisionCheck(true).CollisionCheck);
            Assert.Equal(false, parameters.WithCollisionCheck(false).CollisionCheck);
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
