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
        /// Executes a Motion Linear in JointSpace between the current JointValues and the target JointValues while avoiding obstacles.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MoveJointsCollisionFree", Flow = true)]
        public async static Task MoveJointsCollisionFree(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "JointValues")] JointValuesProperty target,
            [InputPin(PropertyMode = PropertyMode.Default)] string moveGroupName = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Required property 'target' for MoveJointsCollisionFree module was not specified.");

            var targetJointValues = await ResolveProperty(target);
            using(var group = MotionService.CreateMoveGroup(moveGroupName))
            {
                await group.MoveJointsCollisionFreeAsync(targetJointValues, velocityScaling, sampleResolution, cancel);
            }
        }

        /// <summary>
        /// Executes a Motion in JointSpace between the current pose and the target pose while avoiding obstacles.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MovePoseCollisionFree", Flow = true)]
        [ModuleTypeAlias("MovePCollisionFree", IncludeInCatalog = true)]
        public async static Task MovePoseCollisionFree(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "Pose")] PoseProperty target,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorName = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "JointValues")] JointValuesProperty seed = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Required property 'target' for MovePoseCollisionFree module was not specified.");

            var endEffector = MotionService.QueryAvailableEndEffectors().FirstOrDefault(x => x.Name == endEffectorName);
            if (endEffector == null)
                throw new Exception($"EndEffector '{endEffectorName}' not available.");

            var targetPose = await ResolveProperty(target);
            var seedValues = await ResolveProperty(seed);
            using(var group = MotionService.CreateMoveGroup(endEffector.MoveGroupName, endEffector.Name))
            {
                if (seed == null)
                    seedValues = group.CurrentJointPositions;

                group.SampleResolution = sampleResolution;
                await group.GetEndEffector(endEffector.Name).MovePoseCollisionFreeAsync(targetPose, seedValues, cancel);
            }
        }
    }
}