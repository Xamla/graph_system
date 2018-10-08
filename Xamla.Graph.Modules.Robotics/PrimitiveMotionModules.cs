using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rosvita.RestApi;
using Rosvita.RosGardener.Contracts;
using Rosvita.RosMonitor;
using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.MessageRouter.Client;
using Xamla.MessageRouter.Common;
using Xamla.Robotics.Motion;
using Xamla.Robotics.Types;
using Xamla.Types.Converters;
using Xamla.Utilities;

namespace Xamla.Graph.Modules.Robotics
{
    public static partial class StaticModules
    {
        /// <summary>
        /// Executes a Motion Linear in JointSpace between the current joint values and the target joint values.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MoveJoints", Flow = true)]
        [ModuleTypeAlias("MoveJ", IncludeInCatalog = true)]
        public async static Task MoveJoints(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "JointValues")] JointValuesProperty target,
            [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = false,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Required property 'target' for MoveJ module was not specified.");

            var targetJointValues = await ResolveProperty(target);
            using(var group = MotionService.CreateMoveGroupForJointSet(targetJointValues.JointSet))
            {
                group.SampleResolution = sampleResolution;
                await group.MoveJointsAsync(targetJointValues, velocityScaling, collisionChecking, null, cancel);
            }
        }

        /// <summary>
        /// Executes a Motion Linear in JointSpace between the current pose and the target Pose.
        /// </summary>
        /*
        [StaticModule(ModuleType = "Xamla.Robotics.MovePose", Flow = true)]
        public async static Task MovePose(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "Pose")] PoseProperty target,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorName = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = false,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "JointValues")] JointValuesProperty seed = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Required property 'target' of MovePose module was not specified.");
            var targetPose = await ResolveProperty(target);
            var seedValues = await ResolveProperty(seed);

            var endEffector = MotionService.QueryAvailableEndEffectors().FirstOrDefault(x => x.Name == endEffectorName);
            if (endEffector == null)
                throw new Exception($"EndEffector '{endEffectorName}' not available.");

            using(var group = MotionService.CreateMoveGroup(endEffector.MoveGroupName, endEffector.Name))
            {
                if (seed == null)
                    seedValues = group.CurrentJointPositions;

                group.SampleResolution = sampleResolution;
                await group.GetEndEffector(endEffector.Name).MovePoseAsync(targetPose, seedValues, velocityScaling, collisionChecking, null, cancel);
            }
        }
         */

        [StaticModule(ModuleType = "Xamla.Robotics.MovePoseLinear", Flow = true)]
        [ModuleTypeAlias("MoveL", IncludeInCatalog = true)]
        public async static Task MovePoseLinear(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "Pose")] PoseProperty target,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorName = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = false,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default)] double ikJumpThreshold = 1.6,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Required property 'target' of MovePoseLinear module was not specified.");
            var targetPose = await ResolveProperty(target);

            var endEffector = MotionService.QueryAvailableEndEffectors().FirstOrDefault(x => x.Name == endEffectorName);
            if (endEffector == null)
                throw new Exception($"EndEffector '{endEffectorName}' not available.");

            using(var group = MotionService.CreateMoveGroup(endEffector.MoveGroupName, endEffector.Name))
            {
                group.SampleResolution = sampleResolution;
                group.IkJumpThreshold = ikJumpThreshold;
                await group.GetEndEffector(endEffector.Name).MovePoseLinearAsync(targetPose, velocityScaling, collisionChecking, null, cancel);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.MoveLWaypoints", Flow = true)]
        public async static Task MoveLWaypoints(
            [InputPin(PropertyMode = PropertyMode.Never)] IEnumerable<Pose> waypoints = null,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorName = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = false,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default)] double ikJumpThreshold = 1.6,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxDeviation = 0.0,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (waypoints == null)
                throw new ArgumentNullException("Required property 'waypoints' for MoveLWayPoints module was not specified.", nameof(waypoints));

            if (waypoints.ToList().Count == 0)
                throw new ArgumentException("Required property 'waypoints' is empty.", nameof(waypoints));

            var endEffector = MotionService.QueryAvailableEndEffectors().FirstOrDefault(x => x.Name == endEffectorName);
            if (endEffector == null)
                throw new Exception($"EndEffector '{endEffectorName}' not available.");

            CartesianPath path = new CartesianPath(waypoints);
            using(var group = MotionService.CreateMoveGroup(endEffector.MoveGroupName, endEffector.Name))
            {
                group.SampleResolution = sampleResolution;
                group.IkJumpThreshold = ikJumpThreshold;
                var planParameters = group.BuildTaskSpacePlanParameters(velocityScaling, collisionChecking, maxDeviation, null, endEffector.Name);
                var (trajectory, parameters) = group.GetEndEffector(endEffector.Name).PlanMovePoseLinearWaypoints(path, planParameters);

                await MotionService.ExecuteJointTrajectory(trajectory, parameters.CollisionCheck, cancel);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.MoveJWaypoints", Flow = true)]
        public async static Task MoveJWaypoints(
            [InputPin(PropertyMode = PropertyMode.Never)] IEnumerable<JointValues> waypoints = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = false,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxDeviation = 0.01,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (waypoints == null)
                throw new ArgumentNullException("Required property 'waypoints' for MoveJWaypoints module was not specified.", nameof(waypoints));
            if (waypoints.ToList().Count == 0)
                throw new ArgumentException("Required property 'waypoints' is empty.", nameof(waypoints));
            JointPath path = new JointPath(waypoints.First().JointSet, waypoints);
            using(var group = MotionService.CreateMoveGroupForJointSet(path.JointSet))
            {
                group.SampleResolution = sampleResolution;
                await group.MoveJointPathAsync(path, velocityScaling, collisionChecking, maxDeviation, null, cancel);
            }
        }
    }
}