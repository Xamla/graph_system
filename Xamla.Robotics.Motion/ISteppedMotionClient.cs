using System;
using System.Threading.Tasks;

namespace Xamla.Robotics.Motion
{
    using xamlamoveit = Messages.xamlamoveit_msgs;

    /// <summary>
    /// Immutable object carrying a snapshot of a stepped motion client.
    /// </summary>
    public class SteppedMotionState
    {
        public string GoalId { get; }
        public string ErrorMessage { get; }
        public int ErrorCode { get; }
        public double Progress { get; }

        internal SteppedMotionState(string goalId, string errorMessage, int errorCode, double progress)
        {
            this.GoalId = goalId;
            this.ErrorMessage = errorMessage;
            this.ErrorCode = errorCode;
            this.Progress = progress;
        }
    }



    /// <summary>
    /// Implementations of <c>ISteppedMotionClient</c>  TODO: explain.
    /// </summary> 
    public interface ISteppedMotionClient
        : IDisposable
    {
        SteppedMotionState MotionState { get; }
        string GoalId { get; }
        void Next();
        void Previous();
        Task<xamlamoveit.StepwiseMoveJResult> GoalTask { get; }
    }
}
