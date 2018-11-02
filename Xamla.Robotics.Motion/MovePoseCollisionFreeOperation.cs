using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MovePoseCollisionFreeOperation
       : MovePoseOperationBase
    {
        public MovePoseCollisionFreeOperation(MovePoseArgs args)
            : base(args)
        {
        }

        public override IPlan Plan()
        {
            JointValues targetJoints = this.EndEffector.InverseKinematic(this.TargetPose, this.Parameters.CollisionCheck);
            return this.MoveGroup.MoveJointsCollisionFree(targetJoints).With(a => this.ToArgs()).Plan();
        }

        protected override IMovePoseOperation Build(MovePoseArgs args) =>
            new MovePoseCollisionFreeOperation(args);
    }
}
