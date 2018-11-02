using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveCartesianPathLinearOperation
        : MoveCartesianPathOperation
    {
        public MoveCartesianPathLinearOperation(MoveCartesianPathArgs args)
            : base(args)
        {
        }

        public override IPlan Plan()
        {
            if (this.Waypoints.Count == 0)
                return new Plan(this.MoveGroup, JointTrajectory.Empty, this.Parameters);

            JointValues seed = this.Seed ?? this.MoveGroup.CurrentJointPositions;  // get current posture for seed jointvalues
            Pose startPose = this.StartPose ?? this.EndEffector.CurrentPose;  // get current pose
            ICartesianPath path = this.Waypoints.Prepend(startPose);  // generate joint path
            IJointTrajectory trajectory = this.MoveGroup.MotionService.PlanMoveCartesianPathLinear(path, seed, this.TaskSpaceParameters);  // plan trajectory
            return new Plan(this.MoveGroup, trajectory, this.Parameters);
        }

        protected override IMoveCartesianPathOperation Build(MoveCartesianPathArgs args) =>
            new MoveCartesianPathLinearOperation(args);
    }
}
