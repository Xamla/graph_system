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
using xamlamoveit = Messages.xamlamoveit_msgs;

namespace Xamla.Graph.Modules.Robotics
{
    public static partial class StaticModules
    {

        class SteppedMotionMessage
        {
            public string GoalId { get; set; }
            public string MoveGroupName { get; set; }
            public double Progress { get; set; }

            public override string ToString()
                => JToken.FromObject(this).ToString(Newtonsoft.Json.Formatting.None);
        }

        private async static Task HandleStepwiseMotions(ISteppedMotionClient client, IMoveGroup group)
        {
            string channelName = "MotionDialog";
            string topic = "SteppedMotions";
            string disposition = "SteppedMotion";
            using (var RosRoboChatClient = new RosRoboChatActionClient(rosClient.GlobalNodeHandle))
            {
                RosRoboChatClient.CreateChat(channelName, topic);
                var messageBody = new SteppedMotionMessage { GoalId = client.GoalId, Progress = 0, MoveGroupName = group.Name }.ToString();
                string messageId = RosRoboChatClient.CallMessageCommand(channelName, "add", messageBody, null, new string[] { disposition });
                try
                {
                    var cancelReport = new CancellationTokenSource();
                    xamlamoveit.StepwiseMoveJResult result;
                    var updateProgressTask = UpdateProgress(client, RosRoboChatClient, channelName, messageId, disposition, group.Name, cancelReport.Token);
                    try
                    {
                        result = await client.GoalTask;
                    }
                    finally
                    {
                        cancelReport.Cancel();
                        await updateProgressTask.WhenCompleted();
                    }

                    if (result.result < 0)
                        throw new Exception($"SteppedMotion was not finished: { result.result }");
                }
                finally
                {
                    RosRoboChatClient.CallMessageCommand(channelName, "remove", null, messageId, new string[] { disposition });
                }
            }
        }

        async static Task UpdateProgress(
            ISteppedMotionClient client,
            RosRoboChatActionClient roboChatclient,
            string channelName,
            string messageId,
            string disposition,
            string moveGroupName,
            CancellationToken cancel
        )
        {
            await Task.Yield();

            double lastProgress = 0.0;
            while (!client.GoalTask.IsCanceled && !client.GoalTask.IsFaulted && !client.GoalTask.IsCompleted)
            {
                var state = client.MotionState;     // read immutable state snapshot once
                if (state.Progress != lastProgress)
                {
                    lastProgress = state.Progress;
                    var messageBody = new SteppedMotionMessage { GoalId = client.GoalId, Progress = lastProgress, MoveGroupName = moveGroupName };
                    roboChatclient.CallMessageCommand(channelName, "update", messageBody.ToString(), messageId, new string[] { disposition });
                }
                await Task.Delay(100, cancel);
            }
        }

        /// <summary>
        /// Executes a Motion Linear in JointSpace between the current joint values and the target joint values with user interaction.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MoveJointsSupervised", Flow = true)]
        [ModuleTypeAlias("MoveJSafe", IncludeInCatalog = true)]
        public async static Task MoveJointsSupervised(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "JointValues")] JointValuesProperty target,
            [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = true,
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
                using (var client = group.MoveJointsSupervisedAsync(targetJointValues, velocityScaling, collisionChecking, null, cancel))
                {
                    await HandleStepwiseMotions(client, group);
                }
            }

        }
        /// <summary>
        /// Executes a Motion Linear in JointSpace between the current joint values and the target joint values with user interaction.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MoveJWaypointsSupervised", Flow = true)]
        [ModuleTypeAlias("MoveJWaypointsSafe", IncludeInCatalog = true)]
        public async static Task MoveJWaypointsSupervised(
            [InputPin(PropertyMode = PropertyMode.Never)] IEnumerable<JointValues> waypoints = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = true,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxDeviation = 0.0,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = false,
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
                using (var client = group.MoveJointPathSupervisedAsync(path, velocityScaling, collisionChecking, maxDeviation, null, cancel))
                {
                    await HandleStepwiseMotions(client, group);
                }
            }
        }

        /// <summary>
        /// Executes a Motion Linear in JointSpace between the current pose and the target Pose.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MovePoseSupervised", Flow = true)]
        [ModuleTypeAlias("MovePSafe", IncludeInCatalog = true)]
        public async static Task MovePoseSupervised(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "Pose")] PoseProperty target, [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorName = null, [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = false, [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1, [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05, [InputPin(PropertyMode = PropertyMode.Default, Editor = "JointValues")] JointValuesProperty seed = null, [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Required property 'target' of MovePoseSupervised module was not specified.");
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
                group.CollisionCheck = collisionChecking;
                group.VelocityScaling = velocityScaling;
                using(var client = group.GetEndEffector(endEffectorName).MovePoseSupervisedAsync(targetPose, seedValues, cancel))
                {
                    await HandleStepwiseMotions(client, group);
                }
            }
        }

        /// <summary>
        /// Executes a Motion Linear in JointSpace between the current pose and the target Pose.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MovePoseWaypointsSupervised", Flow = true)]
        [ModuleTypeAlias("MovePWaypointsSafe", IncludeInCatalog = true)]
        public async static Task MovePoseWaypointsSupervised(
            [InputPin(PropertyMode = PropertyMode.Never)] IEnumerable<Pose> waypoints = null,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorName = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool collisionChecking = false,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "JointValues")] JointValuesProperty seed = null,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (waypoints == null)
                throw new ArgumentNullException("Required property 'waypoints' for MovePoseLinearWaypointsSupervised module was not specified.", nameof(waypoints));

            if (waypoints.ToList().Count == 0)
                throw new ArgumentException("Required property 'waypoints' is empty.", nameof(waypoints));

            var seedValues = await ResolveProperty(seed);

            var endEffector = MotionService.QueryAvailableEndEffectors().FirstOrDefault(x => x.Name == endEffectorName);
            if (endEffector == null)
                throw new Exception($"EndEffector '{endEffectorName}' not available.");

            CartesianPath path = new CartesianPath(waypoints);
            using(var group = MotionService.CreateMoveGroup(endEffector.MoveGroupName, endEffector.Name))
            {
                if (seed == null)
                    seedValues = group.CurrentJointPositions;

                group.SampleResolution = sampleResolution;
                group.CollisionCheck = collisionChecking;
                group.VelocityScaling = velocityScaling;
                using (var client = group.GetEndEffector(endEffectorName).MoveCartesianPathSupervisedAsync(path, seedValues, cancel))
                {
                    await HandleStepwiseMotions(client, group);
                }
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.MovePoseLinearSupervised", Flow = true)]
        [ModuleTypeAlias("MoveLSafe", IncludeInCatalog = true)]
        public async static Task MovePoseLinearSupervised(
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
                using (var client = group.GetEndEffector(endEffector.Name).MovePoseLinearSupervisedAsync(targetPose, velocityScaling, collisionChecking, null, cancel))
                {
                    await HandleStepwiseMotions(client, group);
                }
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.MovePoseLinearWaypointsSupervised", Flow = true)]
        [ModuleTypeAlias("MoveLWaypointsSafe", IncludeInCatalog = true)]
        public async static Task MovePoseLinearWaypointsSupervised(
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
                throw new ArgumentNullException("Required property 'waypoints' for MovePoseLinearWaypointsSupervised module was not specified.", nameof(waypoints));

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

                using(var client = group.GetEndEffector(endEffector.Name).MoveCartesianPathLinearSupervisedAsync(path, velocityScaling, collisionChecking, maxDeviation, null, cancel))
                {
                    await HandleStepwiseMotions(client, group);
                }
            }
        }
        /// <summary>
        /// Executes a Motion Linear in JointSpace between the current JointValues and the target JointValues while avoiding obstacles.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MoveJointsCollisionFreeSupervised", Flow = true)]
        [ModuleTypeAlias("MoveJCollisionFreeSafe", IncludeInCatalog = true)]
        public async static Task MoveJointsCollisionFreeSupervised(
            [InputPin(PropertyMode = PropertyMode.Default, Editor = "JointValues")] JointValuesProperty target,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.05,
            [InputPin(PropertyMode = PropertyMode.Default)] bool cacheResult = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Required property 'target' for MoveJointsCollisionFreeSupervised module was not specified.");

            var targetJointValues = await ResolveProperty(target);
            using(var group = MotionService.CreateMoveGroupForJointSet(targetJointValues.JointSet))
            {
                group.SampleResolution = sampleResolution;
                using (var client = group.MoveJointsCollisionFreeSupervisedAsync(targetJointValues, velocityScaling, sampleResolution, cancel))
                {
                    await HandleStepwiseMotions(client, group);
                }
            }
        }

        /// <summary>
        /// Executes a Motion in JointSpace between the current pose and the target pose while avoiding obstacles.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.MovePoseCollisionFreeSupervised", Flow = true)]
        [ModuleTypeAlias("MovePCollisionFreeSafe", IncludeInCatalog = true)]
        public async static Task MovePoseCollisionFreeSupervised(
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
                throw new ArgumentNullException(nameof(target), "Required property 'target' for MovePoseCollisionFreeSupervised module was not specified.");

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
                using (var client = group.GetEndEffector(endEffector.Name).MovePoseCollisionFreeSupervisedAsync(targetPose, seedValues, cancel))
                {
                    await HandleStepwiseMotions(client, group);
                }
            }
        }
    }
}