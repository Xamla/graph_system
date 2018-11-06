using System;
using System.Linq;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Contains the supported types of primitive shapes from which collision objects can be build:
    /// </summary>
    /// <seealso cref="ICollisionPrimitive"/>
    /// <seealso cref="ICollisionObject"/>
    public enum CollisionPrimitiveKind
    {
        /// <summary>
        /// A plane is a surface spanning into two dimensions.
        /// </summary>
        Plane = 0,
        /// <summary>
        /// A box-shaped solid object that has six identical square faces.
        /// </summary>
        Box = 1,
        /// <summary>
        /// A solid 3-dimensional object shaped like a ball.
        /// </summary>
        Sphere = 2,
        /// <summary>
        /// A solid object with two identical flat ends that are circular and has one curved side.
        /// </summary>
        Cylinder = 3,
        /// <summary>
        /// A solid 3-dimensional object that has a circular base joined to point by a curved side.
        /// </summary>
        Cone = 4
    }

    /// <summary>
    /// <c>ICollisionPrimitive</c>s are the basic building blocks for collision objects. They are defined by a primitive type, its size and pose.
    /// </summary>
    /// <seealso cref="ICollisionObject"/>
    public interface ICollisionPrimitive
    {
        /// <summary>
        /// Gets the type of the current <c>ICollisionPrimitive</c>.
        /// </summary>
        CollisionPrimitiveKind Kind { get; }
        /// <summary>
        /// Gets the parameter array, which defines the size of the <c>CollisionPrimitiveKind</c>. Each <c>CollisionPrimitiveKind</c> may require a different amount of parameters.
        /// </summary>
        double[] Parameters { get; }
        /// <summary>
        /// Gets the <c>Pose</c> of the current <c>ICollisionPrimitive</c>.
        /// </summary>
        Pose Pose { get; }
    }

    /// <summary>
    /// <c>CollisionPrimitive</c>s are the basic building blocks for collision objects. They are defined by a primitive type, its size and pose.
    /// </summary>
    /// <seealso cref="ICollisionObject"/>
    public class CollisionPrimitive
        : ICollisionPrimitive
    {
        /// <summary>
        /// Gets the instance to the unit sphere, a sphere which size 1.0 at the identity pose.
        /// </summary>
        public static readonly CollisionPrimitive UnitSphere = CreateSphere(1.0, Pose.Identity);

        /// <summary>
        /// Gets the instance to the unit box, a box which size 1.0 in all directions at the identity pose.
        /// </summary>
        public static readonly CollisionPrimitive UnitBox = CreateBox(1.0, 1.0, 1.0, Pose.Identity);

        /// <summary>
        /// Creates a new <c>CollisionPrimitive</c> in the shape of a plane, defined by its scalar equation (ax + by + cz + d = 0), originating at the given pose.
        /// </summary>
        /// <param name="pose">The origin of the plane within the collision object.</param>
        /// <returns>A new instance of <c>CollisionPrimitive</c></returns>
        public static CollisionPrimitive CreatePlane(double a, double b, double c, double d, Pose pose)
            => new CollisionPrimitive(CollisionPrimitiveKind.Plane, new[] { a, b, c, d }, pose);

        /// <summary>
        /// Creates a new box shaped <c>CollisionPrimitive</c> with the given size at the given pose.
        /// </summary>
        /// <param name="x">Size in meters in X direction.</param>
        /// <param name="y">Size in meters in Y direction.</param>
        /// <param name="z">Size in meters in Z direction.</param>
        /// <param name="pose">The origin of the box within the collision object.</param>
        /// <returns>A new instance of <c>CollisionPrimitive</c></returns>
        public static CollisionPrimitive CreateBox(double x, double y, double z, Pose pose)
            => new CollisionPrimitive(CollisionPrimitiveKind.Box, new[] { x, y, z }, pose);

        /// <summary>
        /// Creates a new sphere shaped <c>CollisionPrimitive</c> with the given radius at the given pose.
        /// </summary>
        /// <param name="radius">The radius of the sphere in meters.</param>
        /// <param name="pose">The origin of the sphere within the collision object.</param>
        /// <returns>A new instance of <c>CollisionPrimitive</c></returns>
        public static CollisionPrimitive CreateSphere(double radius, Pose pose)
            => new CollisionPrimitive(CollisionPrimitiveKind.Sphere, new[] { radius }, pose);

        /// <summary>
        /// Creates a new cylinder shaped <c>CollisionPrimitive</c> with the given radius and height at the given pose.
        /// </summary>
        /// <param name="height">The height of the cylinder in meters.</param>
        /// <param name="radius">The radius of the cylinder in meters.</param>
        /// <param name="pose">The origin of the sphere within the collision object.</param>
        /// <returns>A new instance of <c>CollisionPrimitive</c></returns>
        public static CollisionPrimitive CreateCylinder(double height, double radius, Pose pose)
            => new CollisionPrimitive(CollisionPrimitiveKind.Cylinder, new[] { height, radius }, pose);

        /// <summary>
        /// Creates a new cone shaped <c>CollisionPrimitive</c> with the given radius and height at the given pose.
        /// </summary>
        /// <param name="height">The height of the cone in meters.</param>
        /// <param name="radius">The radius of the base plate of the cone in meters.</param>
        /// <param name="pose">The origin of the cone within the collision object.</param>
        /// <returns>A new instance of <c>CollisionPrimitive</c></returns>
        public static CollisionPrimitive CreateCone(double height, double radius, Pose pose)
            => new CollisionPrimitive(CollisionPrimitiveKind.Cone, new[] { height, radius }, pose);

        /// <summary>
        /// Gets the type of the current <c>CollisionPrimitive</c>.
        /// </summary>
        public CollisionPrimitiveKind Kind { get; }

        /// <summary>
        /// <para>Gets the parameter array of the current <c>CollisionPrimitive</c>
        /// For the CYLINDER and CONE types, the center line is oriented along the Z axis.  Therefore the CYLINDER_HEIGHT (CONE_HEIGHT) component of dimensions gives the height of the cylinder (cone).  The CYLINDER_RADIUS (CONE_RADIUS) component of dimensions gives the radius of the base of the cylinder (cone).  Cone and cylinder primitives are defined to be circular. The tip of the cone is pointing up, along +Z axis.</para>
        /// <para>
        /// The following lists explains the amount of parameters for each collision primitive kind:
        /// <list type="bullet">
        /// <item>
        /// <term>Plane</term>
        /// <description>
        /// Representation of a plane, using the plane equation ax + by + cz + d = 0
        /// <list type="bullet">
        /// <item><para>a := Parameters[0]</para></item>
        /// <item><para>b := Parameters[1]</para></item>
        /// <item><para>c := Parameters[2]</para></item>
        /// <item><para>d := Parameters[3]</para></item>
        /// </list>
        /// </description>
        /// </item>
        /// <item>
        /// <term>Box</term><description>
        /// Representation of a box
        /// <list type="bullet">
        /// <item><para>Box width in X := Parameters[0]</para></item>
        /// <item><para>Box width in Y := Parameters[1]</para></item>
        /// <item><para>Box width in Z := Parameters[2]</para></item>
        /// </list>
        /// </description>
        /// </item>
        /// <item>
        /// <term>Sphere</term><description>
        /// Representation of a sphere
        /// <list type="bullet">
        /// <item><para>Radius := Parameters[0]</para></item>
        /// </list>
        /// </description>
        /// </item>
        /// <item>
        /// <term>Cylinder</term><description>
        /// Representation of a cylinder
        /// <list type="bullet">
        /// <item><para>Height := Parameters[0]</para></item>
        /// <item><para>Radius := Parameters[1]</para></item>
        /// </list>
        /// </description>
        /// </item>
        /// <item>
        /// <term>Cone</term><description>
        /// Representation of a cone
        /// <list type="bullet">
        /// <item><para>Height := Parameters[0]</para></item>
        /// <item><para>Radius := Parameters[1]</para></item>
        /// </list>
        /// </description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        public double[] Parameters { get; }

        /// <summary>
        /// Gets the pose of the current <c>CollisionPrimitive</c>
        /// </summary>
        public Pose Pose { get; }

        static Tuple<CollisionPrimitiveKind, int>[] expectedParameterCount;
        static Tuple<CollisionPrimitiveKind, int>[] ExpectedParameterCount
        {
            get
            {
                if (expectedParameterCount == null)
                {
                    expectedParameterCount = new[]{
                        Tuple.Create(CollisionPrimitiveKind.Plane, 4),
                        Tuple.Create(CollisionPrimitiveKind.Box, 3),
                        Tuple.Create(CollisionPrimitiveKind.Sphere, 1),
                        Tuple.Create(CollisionPrimitiveKind.Cylinder, 2),
                        Tuple.Create(CollisionPrimitiveKind.Cone, 2),
                    };
                }
                return expectedParameterCount;
            }
        }

        /// <summary>
        /// Creates a new <c>CollisionPrimitive</c> from the given primitive type and parameters at the given location within the collision object. Note that different types of collision primitives, require different amounts of parameters. See <see cref="Parameters"/> for details.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="parameters"></param>
        /// <param name="pose"></param>
        /// <exception cref="ArgumentException">Thrown when an unknown <c>CollisionPrimitiveKind</c> is given.</exception>
        /// <exception cref="ArgumentException">Thrown when the amount of given parameters does not match the expectation for the given kind.</exception>
        /// <exception cref="ArgumentException">Thrown when the null vector is given as parameter for a plane.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when negative parameters are given.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/> equals null.</exception>
        public CollisionPrimitive(CollisionPrimitiveKind kind, double[] parameters, Pose pose = null)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var kindParameterInfo = ExpectedParameterCount.FirstOrDefault(x => x.Item1 == kind);
            if (kindParameterInfo == null)
                throw new ArgumentException($"Unknown collision primitive kind value {kind}.", nameof(kind));
            if (kindParameterInfo.Item2 != parameters.Length)
                throw new ArgumentException($"Expected {kindParameterInfo.Item2} parameters for collision primitive '{kind}' but {parameters.Length} were provided.", nameof(parameters));

            if (kind == CollisionPrimitiveKind.Plane)
            {
                if (parameters.Take(3).All(x => x == 0))
                    throw new ArgumentException("Invalid normal vector for plane specified.", nameof(parameters));
            }
            else
            {
                if (parameters.Any(x => x < 0))
                    throw new ArgumentOutOfRangeException($"Parameter for collision primitive '{kind}' must not be negative.", nameof(parameters));
            }

            if (pose == null)
                pose = Pose.Identity;

            this.Kind = kind;
            this.Parameters = parameters;
            this.Pose = pose;
        }

        /// <summary>
        /// Creates a new <c>CollisionPrimitive</c> located at the given pose within the collision object.
        /// </summary>
        /// <param name="value">The new pose of the collision primitive.</param>
        /// <returns>A new instance of <c>CollisionPrimitive</c></returns>
        public CollisionPrimitive WithPose(Pose value)
        {
            return new CollisionPrimitive(this.Kind, this.Parameters, value);
        }
    }
}
