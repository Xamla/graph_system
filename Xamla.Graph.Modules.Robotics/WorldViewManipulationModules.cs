using Rosvita.Project;
using Rosvita.RosGardener.Contracts;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamla.Graph.MethodModule;
using Xamla.Robotics.Motion;
using Xamla.Robotics.Types;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.Robotics
{
    public static partial class StaticModules
    {

        /// <summary>
        /// Calculate twist between two poses.
        /// </summary>
        /// <param name="source">Pose stored in WorldView</param>
        /// <param name="destination">Pose stored in WorldView</param>
        /// <param name="cancel"></param>
        /// <returns>
        /// <return name="twist">Twist between poses</return>
        /// <return name="position">Euclidean distance in meter between poses</return>
        /// <return name="angle">Euclidean distance in rad between poses</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.CalculateTwist", Flow = true)]
        public async static Task<Tuple<TwistModel, float, float>> CalculateTwist(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "Pose")] PoseProperty source,
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "Pose")] PoseProperty destination,
            CancellationToken cancel = default(CancellationToken))
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "Required property 'source' of CalculateTwist module was not specified.");

            if (destination == null)
                throw new ArgumentNullException(nameof(destination), "Required property 'destination' of CalculateTwist module was not specified.");

            var sourcePose = await ResolveProperty(source);
            var destinationPose = await ResolveProperty(destination);

            if (sourcePose.Frame != destinationPose.Frame)
            {

                // ## AKo: If both poses have a different base frame we need to resolve absolute poses.
                // e.g. it would be possible to use ROS tf to get the actual poses or calculate the difference.

                throw new NotSupportedException("Currently twist can only calculated if both poses share the same reference frame.");
            }

            var transformationA = sourcePose.TransformMatrix;
            var transformationInverted = transformationA;
            var transformationB = destinationPose.TransformMatrix;

            Matrix4x4.Invert(transformationA, out transformationInverted);
            var deltaPose = new Pose(Matrix4x4.Multiply(transformationB, transformationInverted), sourcePose.Frame);
            Twist resultTwist = deltaPose.ToTwist();
            return Tuple.Create(resultTwist.ToModel(), resultTwist.Linear.Length(), resultTwist.Angular.Length());
        }

        /// <summary>
        /// Check if a pose is inside a defined Cuboid.
        /// </summary>
        /// <param name="source">Pose to be checked</param>
        /// <param name="center">Pose origin to cuboid</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="depth">Depth</param>
        /// <param name="cancel"></param>
        /// <returns>
        /// <return name="isInCuboid"></return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.WithinCuboid", Flow = true)]
        public async static Task<bool> WithinCuboid(
            [InputPin(PropertyMode = PropertyMode.Never)] PoseModel source,
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "Pose")] PoseProperty center,
            [InputPin(PropertyMode = PropertyMode.Default)] double width = 0.1,
            [InputPin(PropertyMode = PropertyMode.Default)] double height = 0.1,
            [InputPin(PropertyMode = PropertyMode.Default)] double depth = 0.1,
            CancellationToken cancel = default(CancellationToken))
        {
            if (center == null)
                throw new ArgumentNullException(nameof(center), "Required property 'center' of CheckCurrentPose module was not specified.");

            if (source == null)
                throw new ArgumentNullException(nameof(source), "Required property 'source' of CheckCurrentPose module was not specified.");

            var centerPose = await ResolveProperty(center);
            var currentPose = source.ToPose();

            var dist = Vector3.Abs(Vector3.Subtract(centerPose.Translation, currentPose.Translation));

            return dist.X < width && dist.Y < height && dist.Z < depth;
        }

        // World View

        /// <summary>
        /// Remove an element from the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path plus element name which should be removed from the WorldView document. Folder names separated by '/'.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.RemoveElement", Flow = true)]
        public static async Task RemoveElement(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, null);
            await worldViewService.RemoveElement(trace);
        }

        /// <summary>
        /// Add a folder to the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path plus folder name where to add the folder in the WorldView document. Folder names separated by '/'.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.AddFolder", Flow = true)]
        public static async Task AddFolder(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath
        )
        {
            var path = new PosixPath(elementPath);
            string containerPath = path.RemoveLastElement().ToPosixPath();
            string displayName = path.LastElement;

            var containerTrace = await worldViewService.ResolvePath(containerPath);
            await worldViewService.AddFolder(containerTrace, displayName);
        }

        /// <summary>
        /// Add a pose to the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path plus pose name where to add the pose in the WorldView document. Folder names separated by '/'.</param>
        /// <param name="pose">The pose to add in the WorldView document.</param>
        /// <param name="transient">If pose is added as transient it only exist for this session and is not saved in the project context.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.AddPose", Flow = true)]
        public static async Task AddPose(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath,
            [InputPin(PropertyMode = PropertyMode.Allow)] Pose pose,
            [InputPin(PropertyMode = PropertyMode.Default)] bool transient = false
        )
        {
            var path = new PosixPath(elementPath);
            string containerPath = path.RemoveLastElement().ToPosixPath();
            string displayName = path.LastElement;

            var containerTrace = await worldViewService.ResolvePath(containerPath);
            await worldViewService.AddPose(containerTrace, pose.ToModel(), displayName, transient);
        }

        /// <summary>
        /// Retrieves a pose from the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path to the pose in the WorldView document. Folder names separated by '/'.</param>
        /// <returns>
        /// <return name="pose">The requested pose from the WorldView document.</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.GetPose", Flow = true)]
        public async static Task<Pose> GetPose(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath = "world"
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, WorldViewElementType.Pose);
            var element = await worldViewService.GetPoseByTrace(trace);
            return element.Value.ToPose();
        }

        /// <summary>
        /// Update existing pose
        /// </summary>
        /// <param name="elementPath">The element path to the pose in the WorldView document which is the update target. Folder names separated by '/'.</param>
        /// <param name="pose">Pose which contains the update.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.UpdatePose", Flow = true)]
        public static async Task UpdatePose(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath,
            [InputPin(PropertyMode = PropertyMode.Allow)] Pose pose
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, WorldViewElementType.Pose);
            var element = await worldViewService.GetPoseByTrace(trace);
            element.Value = pose.ToModel();
            await worldViewService.UpdatePose(element);
        }

        /// <summary>
        /// Query poses
        /// </summary>
        /// <param name="folderPath">Folder path which defines in which folder the pose query starts</param>
        /// <param name="recursive">If true the query is performed recursive</param>
        /// <returns>
        /// <return name="poses">Found poses</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.QueryPoses", Flow = true)]
        public async static Task<IList<Pose>> QueryPoses(
            [InputPin(PropertyMode = PropertyMode.Default)] string folderPath,
            [InputPin(PropertyMode = PropertyMode.Default)] bool recursive = false
        )
        {
            var trace = await worldViewService.ResolvePath(folderPath, WorldViewElementType.Folder);
            var query = new ElementQuery();
            query.ContainerTrace = trace;
            query.Recursive = recursive;
            var response = await worldViewService.QueryPoses(query);
            return (IList<Pose>)response.Select(x => x.Value.ToPose()).ToArray();
        }

        /// <summary>
        /// Add a joint values to the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path plus joint values name where to add the pose in the WorldView document. Folder names separated by '/'.</param>
        /// <param name="jointValues">The joint values to add in the WorldView document.</param>
        /// <param name="transient">If joint values is added as transient it only exist for this session and is not saved in the project context.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.AddJointValues", Flow = true)]
        public static async Task AddJointValues(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath,
            [InputPin(PropertyMode = PropertyMode.Allow)] JointValues jointValues,
            [InputPin(PropertyMode = PropertyMode.Default)] bool transient = false
        )
        {
            var path = new PosixPath(elementPath);
            string containerPath = path.RemoveLastElement().ToPosixPath();
            string displayName = path.LastElement;

            var containerTrace = await worldViewService.ResolvePath(containerPath);
            await worldViewService.AddJointValues(containerTrace, jointValues.ToModel(), displayName, transient);
        }

        /// <summary>
        /// Retrieves a joint values from the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path to the joint values in the WorldView document. Folder names separated by '/'.</param>
        /// <returns>
        /// <return name="jointValues">The requested joint values from the WorldView document.</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.GetJointValues", Flow = true)]
        public static async Task<JointValues> GetJointValues(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, WorldViewElementType.JointValues);
            var element = await worldViewService.GetJointValuesByTrace(trace);
            return element.Value.ToJointValues();
        }

        /// <summary>
        /// Update existing joint values
        /// </summary>
        /// <param name="elementPath">The element path to the joint values in the WorldView document which is the update target. Folder names separated by '/'.</param>
        /// <param name="jointValues">Joint values which contains the update.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.UpdateJointValues", Flow = true)]
        public static async Task UpdateJointValues(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath,
            [InputPin(PropertyMode = PropertyMode.Allow)] JointValues jointValues
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, WorldViewElementType.JointValues);
            var element = await worldViewService.GetJointValuesByTrace(trace);
            element.Value = jointValues.ToModel();
            await worldViewService.UpdateJointValues(element);
        }

        /// <summary>
        /// Query joint values
        /// </summary>
        /// <param name="folderPath">Folder path which defines in which folder the joint values query starts</param>
        /// <param name="recursive">If true the query is performed recursive</param>
        /// <returns>
        /// <return name="jointValues">Found joint values</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.QueryJointValues", Flow = true)]
        public async static Task<IList<JointValues>> QueryJointValues(
            [InputPin(PropertyMode = PropertyMode.Default)] string folderPath,
            [InputPin(PropertyMode = PropertyMode.Default)] bool recursive = false
        )
        {
            var trace = await worldViewService.ResolvePath(folderPath, WorldViewElementType.Folder);
            var query = new ElementQuery();
            query.ContainerTrace = trace;
            query.Recursive = recursive;
            var response = await worldViewService.QueryJointValues(query);
            return (IList<JointValues>)response.Select(x => x.Value.ToJointValues()).ToArray();
        }

        /// <summary>
        /// Add a cartesian path to the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path plus cartesian path name where to add the pose in the WorldView document. Folder names separated by '/'.</param>
        /// <param name="cartesianPath">The cartesian path to add in the WorldView document.</param>
        /// <param name="transient">If cartesian path is added as transient it only exist for this session and is not saved in the project context.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.AddCartesianPath", Flow = true)]
        public static async Task AddCartesianPath(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath,
            [InputPin(PropertyMode = PropertyMode.Allow)] CartesianPath cartesianPath,
            [InputPin(PropertyMode = PropertyMode.Default)] bool transient = false
        )
        {
            var path = new PosixPath(elementPath);
            string containerPath = path.RemoveLastElement().ToPosixPath();
            string displayName = path.LastElement;

            var containerTrace = await worldViewService.ResolvePath(containerPath);
            await worldViewService.AddCartesianPath(containerTrace, cartesianPath.ToModel(), displayName, transient);
        }

        /// <summary>
        /// Retrieves a cartesian path from the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path to the cartesian path in the WorldView document. Folder names separated by '/'.</param>
        /// <returns>
        /// <return name="cartesianPath">The requested cartesian path from the WorldView document.</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.GetCartesianPath", Flow = true)]
        public async static Task<ICartesianPath> GetCartesianPath(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, WorldViewElementType.CartesianPath);
            var element = await worldViewService.GetCartesianPathByTrace(trace);
            return element.Value.ToCartesianPath();
        }

        /// <summary>
        /// Update existing cartesian path
        /// </summary>
        /// <param name="elementPath">The element path to the cartesian path in the WorldView document which is the update target. Folder names separated by '/'.</param>
        /// <param name="cartesianPath">Cartesian path which contains the update.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.UpdateCartesianPath", Flow = true)]
        public static async Task UpdateCartesianPath(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath,
            [InputPin(PropertyMode = PropertyMode.Allow)] CartesianPath cartesianPath
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, WorldViewElementType.JointValues);
            var element = await worldViewService.GetCartesianPathByTrace(trace);
            element.Value = cartesianPath.ToModel();
            await worldViewService.UpdateCartesianPath(element);
        }

        /// <summary>
        /// Query cartesian paths
        /// </summary>
        /// <param name="folderPath">Folder path which defines in which folder the cartesian paths query starts</param>
        /// <param name="recursive">If true the query is performed recursive</param>
        /// <returns>
        /// <return name="cartesianPaths">Found cartesian paths</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.QueryCartesianPaths", Flow = true)]
        public async static Task<IList<CartesianPath>> QueryCartesianPaths(
            [InputPin(PropertyMode = PropertyMode.Default)] string folderPath,
            [InputPin(PropertyMode = PropertyMode.Default)] bool recursive = false
        )
        {
            var trace = await worldViewService.ResolvePath(folderPath, WorldViewElementType.Folder);
            var query = new ElementQuery();
            query.ContainerTrace = trace;
            query.Recursive = recursive;
            var response = await worldViewService.QueryCartesianPaths(query);
            return (IList<CartesianPath>)response.Select(x => x.Value.ToCartesianPath()).ToArray();
        }

        /// <summary>
        /// Add a collision object to the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path plus collision object name where to add the pose in the WorldView document. Folder names separated by '/'.</param>
        /// <param name="collisionObject">The collision object to add in the WorldView document.</param>
        /// <param name="transient">If collision object is added as transient it only exist for this session and is not saved in the project context.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.AddCollisionObject", Flow = true)]
        public static async Task AddCollisionObject(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath,
            [InputPin(PropertyMode = PropertyMode.Allow)] CollisionObject collisionObject,
            [InputPin(PropertyMode = PropertyMode.Default)] bool transient = false
        )
        {
            var path = new PosixPath(elementPath);
            string containerPath = path.RemoveLastElement().ToPosixPath();
            string displayName = path.LastElement;

            var containerTrace = await worldViewService.ResolvePath(containerPath);
            await worldViewService.AddCollisionObject(containerTrace, collisionObject.ToModel(), displayName, transient);
        }

        /// <summary>
        /// Retrieves a collision object from the WorldView document.
        /// </summary>
        /// <param name="elementPath">The element path to the collision object in the WorldView document. Folder names separated by '/'.</param>
        /// <returns>
        /// <return name="collisionObject">The requested collision object from the WorldView document.</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.GetCollisionObject", Flow = true)]
        public async static Task<ICollisionObject> GetCollisionObject(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, WorldViewElementType.CartesianPath);
            var element = await worldViewService.GetCollisionObjectByTrace(trace);
            return element.Value.ToCollisionObject();
        }

        /// <summary>
        /// Update existing collision object
        /// </summary>
        /// <param name="elementPath">The element path to the collision object in the WorldView document which is the update target. Folder names separated by '/'.</param>
        /// <param name="collisionObject">Collision object which contains the update.</param>
        [StaticModule(ModuleType = "Rosvita.WorldView.UpdateCollisionObject", Flow = true)]
        public static async Task UpdateCollisionObject(
            [InputPin(PropertyMode = PropertyMode.Default)] string elementPath,
            [InputPin(PropertyMode = PropertyMode.Allow)] CollisionObject collisionObject
        )
        {
            var trace = await worldViewService.ResolvePath(elementPath, WorldViewElementType.JointValues);
            var element = await worldViewService.GetCollisionObjectByTrace(trace);
            element.Value = collisionObject.ToModel();
            await worldViewService.UpdateCollisionObject(element);
        }

        /// <summary>
        /// Query collision objects
        /// </summary>
        /// <param name="folderPath">Folder path which defines in which folder the collision objects query starts</param>
        /// <param name="recursive">If true the query is performed recursive</param>
        /// <returns>
        /// <return name="collisionObjects">Found collision objects</return>
        /// </returns>
        [StaticModule(ModuleType = "Rosvita.WorldView.QueryCollisionObjects", Flow = true)]
        public async static Task<IList<CollisionObject>> QueryCollisionObjects(
            [InputPin(PropertyMode = PropertyMode.Default)] string folderPath,
            [InputPin(PropertyMode = PropertyMode.Default)] bool recursive = false
        )
        {
            var trace = await worldViewService.ResolvePath(folderPath, WorldViewElementType.Folder);
            var query = new ElementQuery();
            query.ContainerTrace = trace;
            query.Recursive = recursive;
            var response = await worldViewService.QueryCollisionObjects(query);
            return (IList<CollisionObject>)response.Select(x => x.Value.ToCollisionObject()).ToArray();
        }
    }
}