using System;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveCartesianPathArgs
        : MoveArgs
    {
        public IEndEffector EndEffector { get; set; }
        public JointValues Seed { get; set; }
        public ICartesianPath Waypoints { get; set; }
        public double IkJumpThreshold { get; set; }

        /// <summary>
        /// Optional. If not specified current end effector pose is used.
        /// </summary>
        public Pose StartPose { get; set; }

        /// <summary>
        /// Optional. If not specified current robot joint positions are used for planning.
        /// </summary>
        public JointValues Start { get; set; }
    }

    public interface IMoveCartesianPathOperation
        : IMoveOperation
    {
        IEndEffector EndEffector { get; }
        TaskSpacePlanParameters TaskSpaceParameters { get; }

        ICartesianPath Waypoints { get; }
        Pose StartPose { get; }
        JointValues Seed { get; }
        JointValues Start { get; }

        IMoveCartesianPathOperation WithArgs(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? sampleResolution = null, double? accelerationScaling = null);
        IMoveCartesianPathOperation With(Func<MoveCartesianPathArgs, MoveCartesianPathArgs> mutator);
        IMoveCartesianPathOperation With(Action<MoveCartesianPathArgs> mutator);
        MoveCartesianPathArgs ToArgs();
    }
}
