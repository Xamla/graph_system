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
        /// <summary>
        /// Goal id of the ROS action which is used to perform the stepped motion operation
        /// </summary>
        public string GoalId { get; }

        /// <summary>
        /// String representation of the error
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Numerical representation of the error
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Progress of the stepped motion operation in range [0.0 - 1.0]
        /// </summary>
        public double Progress { get; }

        /// <summary>
        /// Creates a new instance of <c>SteppedMotionState</c>
        /// </summary>
        /// <param name="goalId">Goal id of the ROS action which is used to perform the stepped motion operation</param>
        /// <param name="errorMessage">String representation of the error</param>
        /// <param name="errorCode">Numerical representation of the error</param>
        /// <param name="progress">Progress of the stepped motion operation in range [0.0 - 1.0]</param>
        internal SteppedMotionState(string goalId, string errorMessage, int errorCode, double progress)
        {
            this.GoalId = goalId;
            this.ErrorMessage = errorMessage;
            this.ErrorCode = errorCode;
            this.Progress = progress;
        }
    }


    /// <summary>
    /// Implementations of <c>ISteppedMotionClient</c> perform supervised trajectory execution
    /// </summary>
    public interface ISteppedMotionClient
        : IDisposable
    {
        /// <summary>
        /// Current state of the supervised trajectory execution
        /// </summary>
        SteppedMotionState MotionState { get; }

        /// <summary>
        /// Current ros action goal id
        /// </summary>
        string GoalId { get; }

        /// <summary>
        /// Request at supervised executor to perform next step
        /// </summary>
        void Next();

        /// <summary>
        /// Request at supervised executor to perform previous step
        /// </summary>
        void Previous();

        /// <summary>
        /// Instance of <c>Task</c> producing an instance of <c>xamlamoveit.StepwiseMoveJResult</c> as result.
        /// </summary>
        Task<xamlamoveit.StepwiseMoveJResult> GoalTask { get; }
    }
}
