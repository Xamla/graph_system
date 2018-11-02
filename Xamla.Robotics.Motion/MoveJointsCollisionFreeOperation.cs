using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveJointsCollisionFreeOperation
        : MoveJointsOperationBase
    {
        public MoveJointsCollisionFreeOperation(MoveJointsArgs args)
            : base(args)
        {
        }

        public override IPlan Plan()
        {
            JointValues start = this.Start ?? this.MoveGroup.CurrentJointPositions; // get start joint values
            IJointPath jointPath = this.MoveGroup.MotionService.PlanCollisionFreeJointPath(start, this.Target, this.Parameters); // generate joint path
            IJointTrajectory trajectory = this.MoveGroup.MotionService.PlanMoveJoints(jointPath, this.Parameters); // plan trajectory

            return new Plan(this.MoveGroup, trajectory, this.Parameters);
        }

        protected override IMoveJointsOperation Build(MoveJointsArgs args) =>
            new MoveJointsCollisionFreeOperation(args);
    }
}
