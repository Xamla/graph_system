using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MovePoseLinearOperation
        : MovePoseOperationBase
    {
        public MovePoseLinearOperation(MovePoseArgs args)
            : base(args)
        {
        }

        public override IPlan Plan()
        {
            JointValues seed = this.Seed ?? this.MoveGroup.CurrentJointPositions;  // seed jointvalues
            Pose startPose = this.StartPose ?? this.EndEffector.CurrentPose;  // start pose
            CartesianPath posePath = new CartesianPath(startPose, this.TargetPose);  // generate taskspace path
            IJointTrajectory trajectory = this.MoveGroup.MotionService.PlanMoveCartesianPathLinear(posePath, seed, this.TaskSpaceParameters);
            return new Plan(this.MoveGroup, trajectory, this.Parameters);
        }

        protected override IMovePoseOperation Build(MovePoseArgs args) =>
            new MovePoseLinearOperation(args);
    }
}
