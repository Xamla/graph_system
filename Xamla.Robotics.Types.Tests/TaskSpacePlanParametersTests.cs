using System;
using Xunit;

namespace Xamla.Robotics.Types.Tests
{

    public class TaskSpaceTaskSpacePlanParametersBuilderTests
    {
        [Fact]
        public void TestScaleAcceleration()
        {
            var builder = new TaskSpacePlanParameters.Builder();
            builder.MaxAngularAcceleration = 1000;
            builder.MaxXYZAcceleration = -4;
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleAcceleration(1.1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleAcceleration(-0.1));
            Assert.Equal(250, builder.ScaleAcceleration(0.25).MaxAngularAcceleration);
            Assert.Equal(-0.25, builder.ScaleAcceleration(0.25).MaxXYZAcceleration);
        }

        [Fact]
        public void TestScaleVelocity()
        {
            var builder = new TaskSpacePlanParameters.Builder();
            builder.MaxAngularVelocity = 1000;
            builder.MaxXYZVelocity = -4;
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleVelocity(1.1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => builder.ScaleVelocity(-0.1));
            Assert.Equal(250, builder.ScaleVelocity(0.25).MaxAngularVelocity);
            Assert.Equal(-0.25, builder.ScaleVelocity(0.25).MaxXYZVelocity);
        }
    }

    public class TaskSpaceTaskSpacePlanParametersTests
    {
        [Fact]
        public void TestWithCollision()
        {
            var builder = new TaskSpacePlanParameters.Builder();
            var parameters = new TaskSpacePlanParameters(builder);
            Assert.Equal(true, parameters.WithCollisionCheck(true).CollisionCheck);
            Assert.Equal(false, parameters.WithCollisionCheck(false).CollisionCheck);
        }

        [Fact]
        public void TestWithSampleResolution()
        {
            var builder = new TaskSpacePlanParameters.Builder();
            var parameters = new TaskSpacePlanParameters(builder);
            Assert.Equal(5, parameters.WithSampleResolution(5).SampleResolution);
            Assert.Equal(-3, parameters.WithSampleResolution(-3).SampleResolution);
        }

        [Fact]
        public void TestWithMaxDeviation()
        {
            var builder = new TaskSpacePlanParameters.Builder();
            var parameters = new TaskSpacePlanParameters(builder);
            Assert.Equal(5, parameters.WithMaxDeviation(5).MaxDeviation);
            Assert.Equal(-3, parameters.WithMaxDeviation(-3).MaxDeviation);
        }

        [Fact]
        public void TestWithIkJumpThreshold()
        {
            var builder = new TaskSpacePlanParameters.Builder();
            var parameters = new TaskSpacePlanParameters(builder);
            Assert.Equal(5, parameters.WithIkJumpThreshold(5).IkJumpThreshold);
            Assert.Equal(-3, parameters.WithIkJumpThreshold(-3).IkJumpThreshold);
        }
    }
}
