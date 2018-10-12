using System.Collections.Generic;
using System.Collections.Immutable;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// <c>ICollisionObject</c>s are used to inform the ROS movement planner about objects in the scene so it can plan collision free movements around them. 
    /// </summary>
    /// <remarks>
    /// Collision objects consist of a set of primitive shapes (e.g. boxes, spheres, etc ...) and they use the ROS TF system for defining the reference frame (coordinate system) in which they are located.
    /// </remarks>
    public interface ICollisionObject
    {
        /// <summary>
        /// Gets the name of the ROS TF in which the collision object is placed.
        /// </summary>
        string Frame { get; }

        /// <summary>
        /// Gets the list of primitive shapes, of which the collision object consists.
        /// </summary>
        /// <seealso cref="ICollisionPrimitive"/>
        IReadOnlyList<ICollisionPrimitive> Primitives { get; }
    }

    /// <summary>
    /// <c>CollisionObject</c>s are used to inform the ROS movement planner about objects in the scene so it can plan collision free movements around them. 
    /// </summary>
    /// <remarks>
    /// Collision objects consist of a set of primitive shapes (e.g. boxes, spheres, etc ...) and they use the ROS TF system for defining the reference frame (coordinate system) in which they are located.
    /// </remarks>
    public class CollisionObject
        : ICollisionObject
    {
        /// <summary>
        /// Gets the name of the default reference frame, which is 'world' by default.
        /// </summary>
        public const string DEFAULT_WORLD_FRAME = "world";

        /// <summary>
        /// Creates a new collision object in the given reference frame consisting of the given primitive shapes.
        /// </summary>
        /// <param name="frame">The name of the ROS TF (coordinate system) in which the collision object is placed. Default: <see cref="DEFAULT_WORLD_FRAME"/></param>
        /// <param name="primitives">An array of primitive shapes that form the collision object.</param>
        /// <seealso cref="ICollisionPrimitive"/>
        public CollisionObject(string frame = DEFAULT_WORLD_FRAME, params ICollisionPrimitive[] primitives)
            : this(frame, (IReadOnlyList<ICollisionPrimitive>)primitives)
        {
        }

        /// <summary>
        /// Creates a new collision object in the given reference frame consisting of the given primitive shapes.
        /// </summary>
        /// <param name="frame">The name of the ROS TF (coordinate system) in which the collision object is placed. Default: <see cref="DEFAULT_WORLD_FRAME"/></param>
        /// <param name="primitives">A list of primitive shapes that form the collision object. Default: null, which creates an empty collision object.</param>
        /// <seealso cref="ICollisionPrimitive"/>
        public CollisionObject(string frame = DEFAULT_WORLD_FRAME, IReadOnlyList<ICollisionPrimitive> primitives = null)
        {
            this.Frame = frame;
            this.Primitives = primitives ?? ImmutableList<ICollisionPrimitive>.Empty;
        }

        /// <summary>
        /// Gets the name of the ROS TF frame in which the collision object is placed.
        /// </summary>
        public string Frame { get; }

        /// <summary>
        /// Gets the list of primitive shapes, of which the collision object consists.
        /// </summary>
        public IReadOnlyList<ICollisionPrimitive> Primitives { get; }
    }
}
