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
        const string DEFAULT_WSG_ACTION_NAME = "gripper_control";
        const string DEFAULT_WSG_BASE_NAME = "/xamla/wsg_driver";
        const string DEFAULT_WSG_NAME = "wsg50";
        const string DEFAULT_ROBOTIQ_BASE_NAME = "/xamla/robotiq_driver";
        const string DEFAULT_ROBOTIQ_ACTION_NAME = "gripper_command";
        const string DEFAULT_ROBOTIQ_NAME = "robotiq2finger85";


        [StaticModule(ModuleType = "Xamla.Robotics.MoveGripper", Flow = true)]
        public async static Task<MoveGripperResult> MoveGripper(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_ROBOTIQ_ACTION_NAME)] string actionName,
            [InputPin(PropertyMode = PropertyMode.Default)] double position,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "50.0")] double maxEffort,
            CancellationToken cancel = default(CancellationToken))
        {
            return await MotionService.MoveGripper(actionName, position, maxEffort, cancel);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.RobotiqMove", Flow = true)]
        public async static Task<MoveGripperResult> RobotiqMove(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_ROBOTIQ_NAME)] string gripperName,
            [InputPin(PropertyMode = PropertyMode.Default)] double position,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "50.0")] double maxEffort,
            CancellationToken cancel = default(CancellationToken))
        {
            var actionName = $"{DEFAULT_ROBOTIQ_BASE_NAME}/{gripperName}/{DEFAULT_ROBOTIQ_ACTION_NAME}";
            return await MotionService.MoveGripper(actionName, position, maxEffort, cancel);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.WsgAcknowledgeError", Flow = true)]
        public async static Task WsgAcknowledgeError(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_WSG_NAME)] string gripperName,
            CancellationToken cancel = default(CancellationToken))
        {
            var actionName = $"{DEFAULT_WSG_BASE_NAME}/{gripperName}/{DEFAULT_WSG_ACTION_NAME}";
            var result = await MotionService.WsgGripperCommand(actionName, WsgCommand.AcknowledgeError, 0, 0, 0, true, cancel);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.WsgStop", Flow = true)]
        public async static Task WsgStop(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_WSG_NAME)] string gripperName,
            CancellationToken cancel = default(CancellationToken))
        {
            var actionName = $"{DEFAULT_WSG_BASE_NAME}/{gripperName}/{DEFAULT_WSG_ACTION_NAME}";
            var result = await MotionService.WsgGripperCommand(actionName, WsgCommand.Stop, 0, 0, 0, true, cancel);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.WsgHoming", Flow = true)]
        public async static Task WsgHoming(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_WSG_NAME)] string gripperName,
            CancellationToken cancel = default(CancellationToken))
        {
            var actionName = $"{DEFAULT_WSG_BASE_NAME}/{gripperName}/{DEFAULT_WSG_ACTION_NAME}";
            var result = await MotionService.WsgGripperCommand(actionName, WsgCommand.Homing, 0, 0, 0, true, cancel);
        }

        /// <summary>
        /// Move the gripper.
        /// </summary>
        /// <param name="gripperName">Gripper name</param>
        /// <param name="width">Target width in m</param>
        /// <param name="speed">Velocity in m/s</param>
        /// <param name="force">Maximum force in N</param>
        /// <param name="stopOnBlock">If true the gripper shuts down its motor and reports an error when it cannot move to the desired position (e.g. something blocks its path). If false the gripper will hold the part with half of the configured force and will not report an error. Can be used to pick parts with unknown width</param>
        /// <param name="cancel"></param>
        /// <returns>
        /// <return name="position">Gripper open width in mm after command.</return>
        /// <return name="force">Force</return>
        /// <return name="stalled">Stalled</return>
        /// <return name="reachedGoal">Reached Goal</return>
        /// <return name="status">Status</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.WsgMove", Flow = true)]
        public async static Task<Tuple<double, double, bool, bool, string>> WsgMove(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_WSG_NAME)] string gripperName,
            [InputPin(PropertyMode = PropertyMode.Default)] double width = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] double speed = 0.15,
            [InputPin(PropertyMode = PropertyMode.Default)] double force = 20,
            [InputPin(PropertyMode = PropertyMode.Default)] bool stopOnBlock = true,
            CancellationToken cancel = default(CancellationToken))
        {
            var actionName = $"{DEFAULT_WSG_BASE_NAME}/{gripperName}/{DEFAULT_WSG_ACTION_NAME}";
            var result = await MotionService.WsgGripperCommand(actionName, WsgCommand.Move, width, speed, force, stopOnBlock, cancel);
            var stalled = result.State == (int) WsgState.Error;
            var reachedGoal = result.State == (int) WsgState.Idle;
            return Tuple.Create(result.Width, result.Force, stalled, reachedGoal, result.Status);
        }

        /// <summary>
        /// Grasp object.
        /// </summary>
        /// <param name="gripperName">Gripper name</param>
        /// <param name="partWidth">Width of object in m</param>
        /// <param name="speed">Velocity in m/s</param>
        /// <param name="force">Maximum force in N</param>
        /// <param name="cancel"></param>
        /// <returns>
        /// <return name="position">Gripper open width in mm after command.</return>
        /// <return name="force"></return>
        /// <return name="stalled"></return>
        /// <return name="reachedGoal"></return>
        /// <return name="status"></return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.WsgGrasp", Flow = true)]
        public async static Task<Tuple<double, double, bool, bool, string>> WsgGrasp(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_WSG_NAME)] string gripperName,
            [InputPin(PropertyMode = PropertyMode.Default)] double partWidth = 0.002,
            [InputPin(PropertyMode = PropertyMode.Default)] double speed = 0.15,
            [InputPin(PropertyMode = PropertyMode.Default)] double force = 20,
            CancellationToken cancel = default(CancellationToken))
        {
            var actionName = $"{DEFAULT_WSG_BASE_NAME}/{gripperName}/{DEFAULT_WSG_ACTION_NAME}";
            var result = await MotionService.WsgGripperCommand(actionName, WsgCommand.Grasp, partWidth, speed, force, true, cancel);
            var stalled = result.State == (int) WsgState.Error;
            var reachedGoal = result.State == (int) WsgState.Holding;
            return Tuple.Create(result.Width, result.Force, stalled, reachedGoal, result.Status);
        }

        /// <summary>
        /// Relase object.
        /// </summary>
        /// <param name="gripperName">Gripper name</param>
        /// <param name="openWidth">Target open width in m</param>
        /// <param name="speed">Velocity in m/s</param>
        /// <param name="cancel"></param>
        /// <returns>
        /// <return name="position">Gripper open width in mm after command.</return>
        /// <return name="force"></return>
        /// <return name="stalled"></return>
        /// <return name="reachedGoal"></return>
        /// <return name="status"></return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.WsgRelease", Flow = true)]
        public async static Task<Tuple<double, double, bool, bool, string>> WsgRelease(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_WSG_NAME)] string gripperName,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "0.05")] double openWidth,
            [InputPin(PropertyMode = PropertyMode.Default)] double speed = 0.15,
            CancellationToken cancel = default(CancellationToken))
        {
            var actionName = $"{DEFAULT_WSG_BASE_NAME}/{gripperName}/{DEFAULT_WSG_ACTION_NAME}";
            var result = await MotionService.WsgGripperCommand(actionName, WsgCommand.Release, openWidth, speed, 0, true, cancel);
            var stalled = result.State == (int) WsgState.Error;
            var reachedGoal = result.State == (int) WsgState.Idle;
            return Tuple.Create(result.Width, result.Force, stalled, reachedGoal, result.Status);
        }
    }
}