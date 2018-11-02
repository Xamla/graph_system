using System;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveJointsArgs
        : MoveArgs
    {
        public JointValues Start { get; set; }
        public JointValues Target { get; set; }
    }

    public interface IMoveJointsOperation
        : IMoveOperation
    {
        JointValues Start { get; }
        JointValues Target { get; }

        IMoveJointsOperation WithStart(JointValues value);
        IMoveJointsOperation WithCollisionCheck(bool value = true);
        IMoveJointsOperation WithVelocityScaling(double value);

        IMoveJointsOperation WithArgs(
            double? velocityScaling = null,
            bool? collisionCheck = null,
            double? maxDeviation = null,
            double? sampleResolution = null,
            double? accelerationScaling = null
        );

        IMoveJointsOperation With(Func<MoveJointsArgs, MoveJointsArgs> mutator);
        IMoveJointsOperation With(Action<MoveJointsArgs> mutator);
        MoveJointsArgs ToArgs();
    }
}
