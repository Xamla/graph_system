using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class CollisionPrimitiveTests
    {
        [Fact]
        public void TestPlane()
        {
            // Assert exception if
            //      parameter == null
            Assert.Throws<System.ArgumentNullException>(() => new CollisionPrimitive(CollisionPrimitiveKind.Plane, null));
            //      incorrect parameter length
            Assert.Throws<System.ArgumentException>(() => new CollisionPrimitive(CollisionPrimitiveKind.Plane, new double[] { 1, 2, 3 }));
            //      all parameter equal zero
            Assert.Throws<System.ArgumentException>(() => new CollisionPrimitive(CollisionPrimitiveKind.Plane, new double[] { 0, 0, 0, 0 }));
            // should work only when plane
            new CollisionPrimitive(CollisionPrimitiveKind.Plane, new double[] { -1, -2, -3, -4 });
        }

        [Fact]
        public void TestBox()
        {
            var primType = CollisionPrimitiveKind.Box;
            // Assert exception if
            //      parameter == null
            Assert.Throws<System.ArgumentNullException>(() => new CollisionPrimitive(primType, null));
            //      incorrect parameter length
            Assert.Throws<System.ArgumentException>(() => new CollisionPrimitive(primType, new double[] { 1, 2, 3, 4 }));
            //      negative parameter
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new CollisionPrimitive(primType, new double[] { 1, 2, -3 }));
            new CollisionPrimitive(primType, new double[] { 1, 2, 3 });
        }

        [Fact]
        public void TestCone()
        {
            var primType = CollisionPrimitiveKind.Cone;
            // Assert exception if
            //      parameter == null
            Assert.Throws<System.ArgumentNullException>(() => new CollisionPrimitive(primType, null));
            //      incorrect parameter length
            Assert.Throws<System.ArgumentException>(() => new CollisionPrimitive(primType, new double[] { 1, 2, 3 }));
            //      negative parameter
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new CollisionPrimitive(primType, new double[] { 1, -2 }));
            new CollisionPrimitive(primType, new double[] { 1, 2 });
        }

        [Fact]
        public void TestSphere()
        {
            var primType = CollisionPrimitiveKind.Sphere;
            // Assert exception if
            //      parameter == null
            Assert.Throws<System.ArgumentNullException>(() => new CollisionPrimitive(primType, null));
            //      incorrect parameter length
            Assert.Throws<System.ArgumentException>(() => new CollisionPrimitive(primType, new double[] { 1, 2 }));
            //      negative parameter
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new CollisionPrimitive(primType, new double[] { -1 }));
            new CollisionPrimitive(primType, new double[] { 1 });
        }

        [Fact]
        public void TestCylinder()
        {
            var primType = CollisionPrimitiveKind.Cylinder;
            // Assert exception if
            //      parameter == null
            Assert.Throws<System.ArgumentNullException>(() => new CollisionPrimitive(primType, null));
            //      incorrect parameter length
            Assert.Throws<System.ArgumentException>(() => new CollisionPrimitive(primType, new double[] { 1, 2, 3 }));
            // should work only when plane
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new CollisionPrimitive(primType, new double[] { -1, 1 }));
            new CollisionPrimitive(primType, new double[] { 1, 2 });
        }
    }
}