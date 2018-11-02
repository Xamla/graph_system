using System;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveJointPathArgs
        : MoveArgs
    {
        public JointValues Start { get; set; }
        public IJointPath Waypoints { get; set; }
    }

    public interface IMoveJointPathOperation
        : IMoveOperation
    {
        JointValues Start { get; }
        IJointPath Waypoints { get; }

        IMoveJointPathOperation WithStart(JointValues value);
        IMoveJointPathOperation WithCollisionCheck(bool value = true);
        IMoveJointPathOperation WithVelocityScaling(double value);

        IMoveJointPathOperation WithArgs(
            double? velocityScaling = null,
            bool? collisionCheck = null,
            double? maxDeviation = null,
            double? sampleResolution = null,
            double? accelerationScaling = null
        );

        IMoveJointPathOperation With(Func<MoveJointPathArgs, MoveJointPathArgs> mutator);
        IMoveJointPathOperation With(Action<MoveJointPathArgs> mutator);

        MoveJointPathArgs ToArgs();
    }
}
