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
using Rosvita.Project;
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Xamla.Graph.Modules.Robotics
{
    public enum PropertySource
    {
        Constant,
        WorldView,
    }

    public class JointValuesProperty
    {
        public PropertySource Source { get; set; }
        public JointValues Value { get; set; }
        public string Path { get; set; }

        public JointValuesProperty()
            : this(JointValues.Empty)
        {
        }

        public JointValuesProperty(JointValues value)
        {
            this.Source = PropertySource.Constant;
            this.Value = value;
        }

        public JointValuesProperty(string path)
        {
            this.Source = PropertySource.WorldView;
            this.Path = path;
        }
    }

    public class PoseProperty
    {
        public PropertySource Source { get; set; }
        public Pose Value { get; set; }
        public string Path { get; set; }

        public PoseProperty()
            : this(Pose.Identity)
        {
        }

        public PoseProperty(Pose value)
        {
            this.Source = PropertySource.Constant;
            this.Value = value;
        }

        public PoseProperty(string path)
        {
            this.Source = PropertySource.WorldView;
            this.Path = path;
        }
    }

    public class RoboticsMotionConverter
        : ITypeConversionProvider
    {
        public JointValuesProperty JointValuesToJointValuesProperty(JointValues x) => new JointValuesProperty(x);
        public JointValuesProperty StringToJointValuesProperty(string s) => new JointValuesProperty(s);
        public PoseProperty PoseToPoseProperty(Pose x) => new PoseProperty(x);
        public PoseProperty StringToPoseProperty(string s) => new PoseProperty(s);
        public CartesianPath PosesToCartesianPath(IEnumerable<Pose> x) => new CartesianPath(x);

        public JointPath JointValuesToJointPath(IEnumerable<JointValues> x)
        {
            var pointList = x.ToList();
            return pointList.Count > 0 ? new JointPath(pointList[0].JointSet, pointList) : JointPath.Empty;
        }

        public IEnumerable<ITypeConverter> GetConverters()
        {
            return new []
            {
                TypeConverter.Create<JointValues, JointValuesProperty>(JointValuesToJointValuesProperty),
                    TypeConverter.Create<string, JointValuesProperty>(StringToJointValuesProperty),
                    TypeConverter.Create<Pose, PoseProperty>(PoseToPoseProperty),
                    TypeConverter.Create<Pose, string>(x => x?.ToString()),
                    TypeConverter.Create<string, PoseProperty>(StringToPoseProperty),
                    TypeConverter.Create<IEnumerable<Pose>, CartesianPath>(PosesToCartesianPath),
                    TypeConverter.Create<IEnumerable<JointValues>, JointPath>(JointValuesToJointPath)
            };
        }

        public IEnumerable<IDynamicTypeConverter> GetDynamicConverters()
        {
            return null;
        }

        public Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>> > GetSerializers()
        {
            return customSerializedTypes;
        }

        static readonly Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>> > customSerializedTypes = new Dictionary<Type, Tuple<Func<object, JToken>, Func<JToken, object>> > { };

        static JToken SerializePose(object poseObject)
        {
            var model = ((Pose) poseObject)?.ToModel();
            return model != null ? JObject.FromObject(model) : (JToken)JValue.CreateNull();
        }

        static object DeserializePose(JToken modelObject)
        {
            var model = modelObject?.ToObject<PoseModel>();
            return model?.ToPose();
        }

        static RoboticsMotionConverter()
        {
            customSerializedTypes.Add(typeof(Pose), Tuple.Create<Func<object, JToken>, Func<JToken, object>>(SerializePose, DeserializePose));
        }
    }

    public static partial class StaticModules
    {
        static object gate = new object();
        static IManagedConnection connection;
        static RpcAdapter rpcAdapter;
        static IRosClientLibrary rosClient;
        static IMotionService motionService;
        static IWorldViewService worldViewService;
        static IWorldViewClient worldViewClient;

        internal static void Init(ILoggerFactory loggerFactory, IManagedConnection connection, RpcAdapter rpcAdapter, IWorldViewClient worldViewClient, IRosClientLibrary rosClient)
        {
            StaticModules.rpcAdapter = rpcAdapter;
            StaticModules.connection = connection;
            StaticModules.worldViewClient = worldViewClient;

            StaticModules.rosClient = rosClient;

            rosClient.OnRosMasterConnected += RosClient_OnRosMasterConnected;
            rosClient.OnRosMasterDisconnected += RosClient_OnRosMasterDisconnected;

            worldViewService = rpcAdapter.CreateProxy<IWorldViewService>(new Jid("api@rosvita"));
        }

        private static async Task<Pose> ResolveProperty(PoseProperty p)
        {
            if (p == null)
                return null;
            if (p.Source == PropertySource.Constant)
                return p.Value;
            return (await worldViewService.GetPoseByPath(p.Path)).Value.ToPose();
        }

        private static async Task<JointValues> ResolveProperty(JointValuesProperty p)
        {
            if (p == null)
                return null;
            if (p.Source == PropertySource.Constant)
                return p.Value;
            return (await worldViewService.GetJointValuesByPath(p.Path)).Value.ToJointValues();
        }

        private static void RosClient_OnRosMasterConnected(object sender, EventArgs e)
        {
            lock (gate)
            {
                motionService = new MotionService(rosClient.GlobalNodeHandle);
            }
        }

        private static void RosClient_OnRosMasterDisconnected(object sender, EventArgs e)
        {
            lock (gate)
            {
                motionService?.Dispose();
                motionService = null;
            }
        }

        static IMotionService MotionService
        {
            get
            {
                lock (gate)
                {
                    if (motionService == null)
                        throw new Exception("ROS client library is not ready.");

                    return motionService;
                }
            }
        }

        /// <summary>
        /// Wait for n milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">Time to wait in milliseconds.</param>
        /// <param name="cancel"></param>
        [StaticModule(ModuleType = "Xamla.Robotics.Sleep", Flow = true)]
        [ModuleTypeAlias("Sleep", IncludeInCatalog = true)]
        public static Task Sleep(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "500")] int millisecondsDelay = 500,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            return Task.Delay(millisecondsDelay, cancel);
        }

        /// <summary>
        /// Combines several JointValue objects into a JointPath.
        /// </summary>
        /// <param name="points">JointValues object to inclued in the generated path.</param>
        /// <returns>
        /// <return name="jointPath">A JointPath object that contains the points provided via the module's input pins.</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.JointPath.FromPoints", Flow = true)]
        public static IJointPath JointPathFromPoints(
            params JointValues[] points
        )
        {
            if (points == null || points.Length == 0)
                return JointPath.Empty;
            var jointSet = points[0].JointSet;
            return new JointPath(jointSet, points);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.JointPath.Concat", Flow = true)]
        public static IJointPath JointPathConcat(IJointPath arg0, IJointPath arg1)
        {
            if (arg0 == null)
                throw new ArgumentNullException(nameof(arg0));
            if (arg1 == null)
                throw new ArgumentNullException(nameof(arg1));
            return arg0.Concat(arg1);
        }

        /// <summary>
        /// Appends one or more points (JointValues) to a JointPath. A newly created
        /// JointPath object with the appended points is returned.
        /// </summary>
        /// <param name="path">Existing JointPath instance</param>
        /// <param name="point">Point to append</param>
        /// <returns>
        /// <return name="path">The created joint path object with points appended.</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.JointPath.Append", Flow = true)]
        public static IJointPath JointPathAppend(
            IJointPath path,
            params JointValues[] point
        )
        {
            return path.Append(point);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.JointPath.Sub", Flow = true)]
        public static IJointPath JointPathSub(
            IJointPath path,
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = "0")] int startIndex,
            [InputPin(PropertyMode = PropertyMode.Default)] int endIndex
        )
        {
            return path.Sub(startIndex, endIndex);
        }

        /// <summary>
        /// Generates random values for a JointSet within the specified joint limits.
        /// </summary>
        /// <param name="limits">The joint limits object specifying the sampling range (min/max per joint values) to use.</param>
        /// <param name="rng">(optional) The random number generator to use. The default generator will be used if this parameter is null.</param>
        /// <returns></returns>
        [StaticModule(ModuleType = "Xamla.Robotics.RandomJointValues", Flow = true)]
        public static JointValues RandomJointValues(JointLimits limits, Random rng = null)
        {
            return JointValues.Random(limits, rng ?? ThreadSafeRandom.Generator);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.QueryAvailableMoveGroups", Flow = true)]
        public static IList<MoveGroupDescription> QueryAvailableMoveGroups()
        {
            return MotionService.QueryAvailableMoveGroups();
        }

        [StaticModule(ModuleType = "Xamla.Robotics.QueryJointLimits", Flow = true)]
        public static JointLimits QueryJointLimits(
            [InputPin(PropertyMode = PropertyMode.Allow)] JointSet joints
        )
        {
            return MotionService.QueryJointLimits(joints);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.QueryJointStates", Flow = true)]
        public static JointStates QueryJointStates(
            [InputPin(PropertyMode = PropertyMode.Allow)] JointSet joints
        )
        {
            return MotionService.QueryJointStates(joints);
        }

        /// <summary>
        /// Query pose of robot by computing the forward kinematic function.
        /// </summary>
        /// <param name="moveGroupName">Name of moule group</param>
        /// <param name="jointPositions">Joint value</param>
        /// <param name="endEffectorLink">Name of the end effector link</param>
        /// <returns>
        /// <return name="pose">The cartesian pose of the specified end effector link.</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.QueryPose", Flow = true)]
        public static Pose QueryPose(
            [InputPin(PropertyMode = PropertyMode.Default)] string moveGroupName,
            [InputPin(PropertyMode = PropertyMode.Allow)] JointValues jointPositions,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorLink = ""
        )
        {
            return MotionService.QueryPose(moveGroupName, jointPositions, endEffectorLink);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.QueryPoseMany", Flow = true)]
        public static IList<Pose> QueryPoseMany(
            [InputPin(PropertyMode = PropertyMode.Default)] string moveGroupName,
            [InputPin(PropertyMode = PropertyMode.Allow)] IJointPath waypoints,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorLink = ""
        )
        {
            return MotionService.QueryPoseMany(moveGroupName, waypoints, endEffectorLink);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.QueryCollisionFreeJointPath", Flow = true)]
        public static IJointPath QueryCollisionFreeJointPath(
            [InputPin(PropertyMode = PropertyMode.Default)] string moveGroupName,
            [InputPin(PropertyMode = PropertyMode.Allow)] IJointPath waypoints
        )
        {
            return MotionService.QueryCollisionFreeJointPath(moveGroupName, waypoints);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.QueryJointTrajectory", Flow = true)]
        public static IJointTrajectory QueryJointTrajectory(
            [InputPin(PropertyMode = PropertyMode.Allow)] IJointPath waypoints,
            [InputPin(PropertyMode = PropertyMode.Default)] double[] maxVelocity = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double[] maxAcceleration = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxDeviation = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] double dt = 0.008
        )
        {
            return MotionService.QueryJointTrajectory(waypoints, maxVelocity, maxAcceleration, maxDeviation, dt);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.QueryJointPathCollisions", Flow = true)]
        public static IList<JointValuesCollision> QueryJointPathCollisions(
            [InputPin(PropertyMode = PropertyMode.Default)] string moveGroupName,
            [InputPin(PropertyMode = PropertyMode.Allow)] IJointPath points
        )
        {
            return MotionService.QueryJointPathCollisions(moveGroupName, points);
        }

        /// <summary>
        /// Creates a PlanParameters object that can combines several planning related options and can be used
        /// as input for different motion planning modules.
        /// </summary>
        /// <param name="endEffectorName">(optional) Name of the EndEffector. If left empty the first configured EndEffector will be used.</param>
        /// <param name="maxXYZVelocity">(optional) max velocity value for xyz position. If left emtpy (null) the max velocity value will be queried and used.</param>
        /// <param name="maxXYZAcceleration">(optional) max acceleration value for xyz position. If left emtpy (null) the max acceleration value will be queried and used.</param>
        /// <param name="maxAngularVelocity">(optional) max angular velocity value. If left emtpy (null) the max angular velocity value will be queried and used.</param>
        /// <param name="maxAngularAcceleration">(optional) max angular acceleration value. If left emtpy (null) the max angular acceleration value will be queried and used.</param>
        /// <param name="ikJumpThreshold">(optional) Safty parameter to check if the redundancy resolution switches during the trajectory generation process.</param>
        /// <param name="maxDeviation">(optional) In case waypoints are used, this parameter will allow blending between the segments.</param>
        /// <param name="sampleResolution">The time resolution for trajectory generation (dt).</param>
        /// <param name="checkCollision">Indicates whether generated trajectories should be tested for collision.</param>
        /// <param name="velocityScaling">Scaling parameter for maxVelocity and maxAcceleration.</param>
        /// <returns>
        /// <return name="parameters">All PlanParameters in one object that can be used by other planning functions.</return>
        /// <return name="maxXYZVelocity">Max velocity (m/s)</return>
        /// <return name="maxXYZAcceleration">Max acceleration (m/s²)</return>
        /// <return name="maxAngularVelocity">Max velocity (radians/s)</return>
        /// <return name="maxAngularAcceleration">Max acceleration (radians/s²)</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.CreateTaskSpacePlanParameters", Flow = true)]
        public static Tuple<TaskSpacePlanParameters, double, double, double, double> CreateTaskSpacePlanParameters(
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorName = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxXYZVelocity = 0.2,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxXYZAcceleration = 0.8,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxAngularVelocity = 0.03,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxAngularAcceleration = 0.1,
            [InputPin(PropertyMode = PropertyMode.Default)] double ikJumpThreshold = 0.1,
            [InputPin(PropertyMode = PropertyMode.Default)] double maxDeviation = 0.0,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.008,
            [InputPin(PropertyMode = PropertyMode.Default)] bool checkCollision = true,
            [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1
        )
        {
            var parameters = MotionService.CreateTaskSpacePlanParameters(endEffectorName, maxXYZVelocity, maxXYZAcceleration, maxAngularVelocity, maxAngularAcceleration, sampleResolution, ikJumpThreshold, maxDeviation, checkCollision, velocityScaling);
            return Tuple.Create(parameters, parameters.MaxXYZVelocity, parameters.MaxXYZAcceleration, parameters.MaxAngularVelocity, parameters.MaxAngularAcceleration);
        }

        /// <summary>
        /// Creates a PlanParameters object that can combines several planning related options and can be used
        /// as input for different motion planning modules.
        /// </summary>
        /// <param name="moveGroupName">(optional) Name of the MoveGroup. If left empty the first configured MoveGroup will be used.</param>
        /// <param name="joints">(optional) The JointSet to use.</param>
        /// <param name="maxVelocity">(optional) Array of per joint max velocity values. If left emtpy (null) the robot's joint limits will be queried and used.</param>
        /// <param name="maxAcceleration">(optional) Array of per joint max acceleration values. If left empty (null) the robot's joint limits will be queried and used.</param>
        /// <param name="sampleResolution">The time resolution for trajectory generation (dt).</param>
        /// <param name="checkCollision">Indicates whether generated trajectories should be tested for collision.</param>
        /// <param name="velocityScaling">Scaling parameter for maxVelocity and maxAcceleration.</param>
        /// <returns>
        /// <return name="parameters">All PlanParameters in one object that can be used by other planning functions.</return>
        /// <return name="joints">JointSet of the generated PlanParameters</return>
        /// <return name="maxVelocity">Max velocity per joint values (radians/s for revolute joints)</return>
        /// <return name="maxAcceleration">Max acceleration pre joint values</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.CreatePlanParameters", Flow = true)]
        public static Tuple<PlanParameters, JointSet, double[], double[]> CreatePlanParameters(
            [InputPin(PropertyMode = PropertyMode.Default)] string moveGroupName = null,
            [InputPin(PropertyMode = PropertyMode.Default)] JointSet joints = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double[] maxVelocity = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double[] maxAcceleration = null,
            [InputPin(PropertyMode = PropertyMode.Default)] double sampleResolution = 0.008,
             [InputPin(PropertyMode = PropertyMode.Default)] bool checkCollision = true,
             [InputPin(PropertyMode = PropertyMode.Default)] double velocityScaling = 1
        )
        {
            var parameters = MotionService.CreatePlanParameters(moveGroupName, joints, maxVelocity, maxAcceleration, sampleResolution, checkCollision, velocityScaling);
            return Tuple.Create(parameters, parameters.JointSet, parameters.MaxVelocity, parameters.MaxAcceleration);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.PlanCollisionFreeJointPathSegment", Flow = true)]
        public static IJointPath PlanCollisionFreeJointPathSegment(
            [InputPin(PropertyMode = PropertyMode.Allow)] JointValues start,
            [InputPin(PropertyMode = PropertyMode.Allow)] JointValues goal,
            [InputPin(PropertyMode = PropertyMode.Allow)] PlanParameters parameters
        )
        {
            return MotionService.PlanCollisionFreeJointPath(start, goal, parameters);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.PlanCollisionFreeJointPath", Flow = true)]
        public static IJointPath PlanCollisionFreeJointPath(
            [InputPin(PropertyMode = PropertyMode.Allow)] IJointPath waypoints,
            [InputPin(PropertyMode = PropertyMode.Allow)] PlanParameters parameters
        )
        {
            return MotionService.PlanCollisionFreeJointPath(waypoints, parameters);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.PlanMoveCartesian", Flow = true)]
        public static ICartesianPath PlanMoveCartesian(
            [InputPin(PropertyMode = PropertyMode.Allow)] ICartesianPath path,
            [InputPin(PropertyMode = PropertyMode.Default)] int numSteps,
            [InputPin(PropertyMode = PropertyMode.Allow)] PlanParameters parameters
        )
        {
            return MotionService.PlanMoveCartesian(path, numSteps, parameters);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.PlanMoveJoints", Flow = true)]
        [ModuleTypeAlias("PlanMoveJ", IncludeInCatalog = true)]
        public static IJointTrajectory PlanMoveJoints(
            [InputPin(PropertyMode = PropertyMode.Allow)] IJointPath path,
            [InputPin(PropertyMode = PropertyMode.Allow)] PlanParameters parameters
        )
        {
            return MotionService.PlanMoveJoints(path, parameters);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.ExecuteJointTrajectory", Flow = true)]
        public static Task<int> ExecuteJointTrajectory(
            [InputPin(PropertyMode = PropertyMode.Allow)] IJointTrajectory trajectory,
            [InputPin(PropertyMode = PropertyMode.Default)] bool checkCollision,
            CancellationToken cancel = default(CancellationToken))
        {
            return MotionService.ExecuteJointTrajectory(trajectory, checkCollision, cancel);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.InverseKinematic", Flow = true)]
        public static JointValues InverseKinematic(
            [InputPin(PropertyMode = PropertyMode.Allow)] Pose pose,
            [InputPin(PropertyMode = PropertyMode.Allow)] PlanParameters parameters,
            [InputPin(PropertyMode = PropertyMode.Allow)] JointValues jointPositionSeed = null,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorLink = "",
            [InputPin(PropertyMode = PropertyMode.Default)] TimeSpan? timeout = null,
            [InputPin(PropertyMode = PropertyMode.Default)] int attempts = 1
        )
        {
            return MotionService.InverseKinematic(pose, parameters, jointPositionSeed, endEffectorLink, timeout, attempts);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.InverseKinematicMany", Flow = true)]
        public static IKResult InverseKinematicMany(
            [InputPin(PropertyMode = PropertyMode.Allow)] IEnumerable<Pose> points,
            [InputPin(PropertyMode = PropertyMode.Allow)] PlanParameters parameters,
            [InputPin(PropertyMode = PropertyMode.Allow)] JointValues jointPositionSeed = null,
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorLink = "",
            [InputPin(PropertyMode = PropertyMode.Default)] TimeSpan? timeout = null,
            [InputPin(PropertyMode = PropertyMode.Default)] int attempts = 1,
            [InputPin(PropertyMode = PropertyMode.Default)] bool constSeed = false
        )
        {
            return MotionService.InverseKinematicMany(points, parameters, jointPositionSeed, endEffectorLink, timeout, attempts, constSeed);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.PlanCartesianPathSegment", Flow = true)]
        public static IJointPath PlanCartesianPathSegment(
            [InputPin(PropertyMode = PropertyMode.Allow)] Pose start,
            [InputPin(PropertyMode = PropertyMode.Allow)] Pose goal,
            [InputPin(PropertyMode = PropertyMode.Default)] int numSteps,
            [InputPin(PropertyMode = PropertyMode.Allow)] PlanParameters parameters
        )
        {
            return MotionService.PlanCartesianPath(start, goal, numSteps, parameters);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.PlanCartesianPath", Flow = true)]
        public static IJointPath PlanCartesianPath(
            [InputPin(PropertyMode = PropertyMode.Allow)] ICartesianPath waypoints,
            [InputPin(PropertyMode = PropertyMode.Default)] int numSteps,
            [InputPin(PropertyMode = PropertyMode.Allow)] PlanParameters parameters
        )
        {
            return MotionService.PlanCartesianPath(waypoints, numSteps, parameters);
        }

        [StaticModule(ModuleType = "Xamla.Robotics.CurrentPose", Flow = true)]
        public static PoseModel CurrentPose(
            [InputPin(PropertyMode = PropertyMode.Default)] string endEffectorName
        )
        {
            if (endEffectorName == null)
                throw new ArgumentNullException(nameof(endEffectorName), "The EndEffectorName needs to be specified for this moduel.");

            var endEffector = MotionService.QueryAvailableEndEffectors().FirstOrDefault(x => x.Name == endEffectorName);
            if (endEffector == null)
                throw new Exception($"EndEffector '{endEffectorName}' not available.");

            var currentJointValues = MotionService.GetCurrentJointValues(endEffector.JointSet);
            return MotionService.QueryPose(endEffector.MoveGroupName, currentJointValues, endEffector.LinkName).ToModel();
        }

        [StaticModule(ModuleType = "Xamla.Robotics.CurrentJointValues", Flow = true)]
        public static JointValues CurrentJointValues(
            [InputPin(PropertyMode = PropertyMode.Default)] string moveGroupName
        )
        {
            if (moveGroupName == null)
                throw new ArgumentNullException(nameof(moveGroupName), "The MoveGroupName needs to be specified for this moduel.");

            var moveGroup = MotionService.QueryAvailableMoveGroups().FirstOrDefault(x => x.Name == moveGroupName);

            if (moveGroup == null)
                throw new Exception($"MoveGroup '{moveGroupName}' not available.");

            return MotionService.GetCurrentJointValues(moveGroup.JointSet);
        }

        /// <summary>
        /// Query Pose by prefix.
        /// </summary>
        /// <param name="folderPath">WorldView path to the folder containing the poses.</param>
        /// <param name="prefix">Prefix of Poses stored in WorldView</param>
        /// /// <param name="recursive">Whether to retrieve poses in child folders of `folderPath`, too.</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>
        /// <return name="waypoints">Poses</return>
        /// <return name="count">Number of found waypoints with specified prefix</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.QueryPosesByPrefix", Flow = true)]
        [ModuleTypeAlias("GetPoses", IncludeInCatalog = true)]
        public async static Task<Tuple<IList<Pose>, int>> QueryPosesByPrefix(
            [InputPin(PropertyMode = PropertyMode.Default)] string folderPath = "",
            [InputPin(PropertyMode = PropertyMode.Default)] string prefix = "",
            [InputPin(PropertyMode = PropertyMode.Default)] bool recursive = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (prefix == null)
                throw new ArgumentNullException("Required property 'Prefix' for QueryPosesByPrefix module was not specified.", nameof(prefix));

            var containerTrace = await worldViewService.ResolvePath(folderPath);
            var posesQuery = new ElementQuery { ContainerTrace = containerTrace, Recursive = recursive };
            var poses = await worldViewService.QueryPoses(posesQuery);
            IList<Pose> targetPoses = poses
                .Where(x => x.DisplayName.StartsWith(prefix))
                .OrderBy(x => x.DisplayName)
                .Select(x => x.Value.ToPose())
                .ToList();
            return Tuple.Create(targetPoses, targetPoses.Count);
        }

        /// <summary>
        /// Query JointValues by prefix.
        /// </summary>
        /// <param name="folderPath">WorldView path to the folder containing the joint values.</param>
        /// <param name="prefix">Prefix of JointValues stored in WorldView</param>
        /// <param name="recursive">Whether to retrieve joint values in child folders of `folderPath`, too.</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>
        /// <return name="waypoints">Joint Values</return>
        /// <return name="count">Number of found waypoints with specified prefix</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.QueryJointValuesByPrefix", Flow = true)]
        [ModuleTypeAlias("GetJointVectors", IncludeInCatalog = true)]
        public async static Task<Tuple<IList<JointValues>, int>> QueryJointValuesByPrefix(
            [InputPin(PropertyMode = PropertyMode.Default)] string folderPath = "",
            [InputPin(PropertyMode = PropertyMode.Default)] string prefix = "",
            [InputPin(PropertyMode = PropertyMode.Default)] bool recursive = true,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            if (prefix == null)
                throw new ArgumentNullException("Required property 'Prefix' for QueryPosesByPrefix module was not specified.", nameof(prefix));

            var containerTrace = await worldViewService.ResolvePath(folderPath);
            var jointValuesQuery = new ElementQuery { ContainerTrace = containerTrace, Recursive = recursive };
            var jointValues = await worldViewService.QueryJointValues(jointValuesQuery);
            IList<JointValues> targetJointValues = jointValues
                .Where(x => x.DisplayName.StartsWith(prefix))
                .OrderBy(x => x.DisplayName)
                .Select(x => x.Value.ToJointValues())
                .ToList();
            return Tuple.Create(targetJointValues, targetJointValues.Count);
        }
    }

    [Module(ModuleType = "Xamla.Robotics.ForLoop", Description = "For loop. Allows code to be executed repeatedly.")]
    public class ForLoop
        : SingleInstanceMethodModule
    {
        int current;

        public ForLoop(IGraphRuntime runtime)
            : base(runtime)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "flowOut", Description = "Flow when loop was completed")]
        [OutputPin(Name = "loop", Description = "Flow to execute loop body")]
        [OutputPin(Name = "i", Description = "Current counter value")]
        public Tuple<Flow, Flow, int> Loop(
            [InputPin(PropertyMode = PropertyMode.Never, Description = "Outer flow to begin loop execution")] Flow flowIn,
            [InputPin(PropertyMode = PropertyMode.Never, Description = "Connect the end of the loop body here")] Flow loopEnd,
            [InputPin(PropertyMode = PropertyMode.Default, Description = "Initial value for counting")] int startValue = 0,
            [InputPin(PropertyMode = PropertyMode.Default, Description = "Inrement of the counter variable after each evaluation of the loop body.")] int increment = 1,
            [InputPin(PropertyMode = PropertyMode.Default, Description = "Exit loop when the counter variable becomes greater or equal to this value.")] int endValue = 100
        )
        {
            if (flowIn != null)
            {
                current = startValue;
            }
            else
            {
                current += increment;
            }

            if (current < endValue)
            {
                return Tuple.Create<Flow, Flow, int>(null, Flow.Default, current);
            }
            else
            {
                return Tuple.Create<Flow, Flow, int>(Flow.Default, null, current);
            }
        }
    }
}