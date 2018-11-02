using System;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MovePoseArgs
        : MoveArgs
    {
        public IEndEffector EndEffector { get; set; }
        public JointValues Seed { get; set; }
        public Pose TargetPose { get; set; }
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

    public interface IMovePoseOperation
        : IMoveOperation
    {
        IEndEffector EndEffector { get; }
        TaskSpacePlanParameters TaskSpaceParameters { get; }

        Pose TargetPose { get; }
        Pose StartPose { get; }
        JointValues Seed { get; }
        JointValues Start { get; }

        IMovePoseOperation With(Func<MovePoseArgs, MovePoseArgs> mutator);
        IMovePoseOperation With(Action<MovePoseArgs> mutator);
        MovePoseArgs ToArgs();
    }
}
