using Newtonsoft.Json;

namespace Xamla.Robotics.Types.JsonConverters
{
    public static class RoboticsJsonConverters
    {
        public static readonly JsonConverter[] All = new JsonConverter[] {
            new CartesianPathJsonConverter(),
            new CollisionPrimitiveJsonConverter(),
            new JointLimitsJsonConverter(),
            new JointPathJsonConverter(),
            new JointSetJsonConverter(),
            new JointStatesJsonConverter(),
            new JointTrajectorysJsonConverter(),
            new JointValuesJsonConverter(),
            new PlanParametersJsonConverter(),
            new PoseJsonConverter(),
            new TwistJsonConverter(),
        };
    }
}
