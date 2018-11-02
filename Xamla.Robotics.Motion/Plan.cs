using System;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class Plan
        : IPlan
    {
        public Plan(IMoveGroup moveGroup, IJointTrajectory trajectory, PlanParameters parameters)
        {
            this.MoveGroup = moveGroup;
            this.Trajectory = trajectory;
            this.Parameters = parameters;
        }

        public IMoveGroup MoveGroup { get; }
        public PlanParameters Parameters { get; }
        public IJointTrajectory Trajectory { get; }

        public Task ExecuteAsync(CancellationToken cancel = default(CancellationToken)) =>
            this.MoveGroup.MotionService.ExecuteJointTrajectoryAsync(this.Trajectory, this.Parameters.CollisionCheck, cancel);

        public ISteppedMotionClient ExecuteSupervised(CancellationToken cancel = default(CancellationToken)) =>
            this.MoveGroup.MotionService.ExecuteJointTrajectorySupervised(Trajectory, 1, Parameters.CollisionCheck, cancel);
    }
}
