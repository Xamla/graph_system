using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveJointsOperation
        : MoveJointsOperationBase
    {
        public MoveJointsOperation(MoveJointsArgs args)
            : base(args)
        {
        }

        public override IPlan Plan()
        {
            JointValues start = this.Start ?? this.MoveGroup.CurrentJointPositions; // get start joint values
            JointPath jointPath = new JointPath(this.MoveGroup.JointSet, start, this.Target); // generate joint path
            IJointTrajectory trajectory = this.MoveGroup.MotionService.PlanMoveJoints(jointPath, this.Parameters); // plan trajectory
            return new Plan(this.MoveGroup, trajectory, this.Parameters);
        }

        protected override IMoveJointsOperation Build(MoveJointsArgs args) =>
            new MoveJointsOperation(args);
    }
}
