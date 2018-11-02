using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public interface IPlan
    {
        IMoveGroup MoveGroup { get; }
        PlanParameters Parameters { get; }
        IJointTrajectory Trajectory { get; }

        Task ExecuteAsync(CancellationToken cancel = default(CancellationToken));
        ISteppedMotionClient ExecuteSupervised(CancellationToken cancel = default(CancellationToken));
    }
}
