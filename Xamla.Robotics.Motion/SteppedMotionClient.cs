using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Messages;
using Messages.actionlib_msgs;
using Messages.std_msgs;
using Uml.Robotics.Ros;
using Uml.Robotics.Ros.ActionLib;
using Xamla.Robotics.Types;
using Xamla.Utilities;

namespace Xamla.Robotics.Motion
{
    using xamlamoveit = Messages.xamlamoveit_msgs;

    /// <summary>
    /// Client to perform supervised trajectory execution
    /// </summary>
    public class SteppedMotionClient
        : ISteppedMotionClient
    {
        const string NEXT_TOPIC = "/xamlaMoveActions/next";
        const string PREVIOUS_TOPIC = "/xamlaMoveActions/prev";
        const string FEEDBACK_TOPIC = "/xamlaMoveActions/feedback";
        const string MOVEJ_ACTION_NAME = "moveJ_step_action";

        private NodeHandle nodeHandle;
        private Subscriber feedbackSubscriber;
        private Publisher<GoalID> nextPublisher;
        private Publisher<GoalID> previousPublisher;

        private ActionClient<xamlamoveit.StepwiseMoveJGoal, xamlamoveit.StepwiseMoveJResult, xamlamoveit.StepwiseMoveJFeedback> stepwiseMoveJActionClient;
        private ClientGoalHandle<xamlamoveit.StepwiseMoveJGoal, xamlamoveit.StepwiseMoveJResult, xamlamoveit.StepwiseMoveJFeedback> goalHandle;
        private GoalID goalId;

        private SteppedMotionState motionState;
        private IDisposable cancelSubscription;
        private double velocityScaling;


        /// <summary>
        /// Creates a new <c>SteppedMotionClient</c>
        /// </summary>
        /// <param name="nodeHandle">Handle of a ROS node</param>
        /// <param name="trajectory">Trajectory which should be executed supervised</param>
        /// <param name="velocityScaling">Scaling factor to reduce or increase the trajectory velocities range [0.0 - 1.0]</param>
        /// <param name="checkCollision">If true collision check is performed</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>An instance of <c>StepMotionClient</c>.</returns>
        /// <exception cref="Exception">Thrown when ActionSever is not available.</exception>
        public SteppedMotionClient(NodeHandle nodeHandle, IJointTrajectory trajectory, double velocityScaling, bool checkCollision, CancellationToken cancel = default(CancellationToken))
        {
            this.nodeHandle = nodeHandle;
            this.velocityScaling = velocityScaling;
            this.stepwiseMoveJActionClient = new ActionClient<xamlamoveit.StepwiseMoveJGoal, xamlamoveit.StepwiseMoveJResult, xamlamoveit.StepwiseMoveJFeedback>(MOVEJ_ACTION_NAME, nodeHandle);

            if (!this.stepwiseMoveJActionClient.WaitForActionServerToStart(TimeSpan.FromSeconds(5)))
            {
                this.stepwiseMoveJActionClient.Shutdown();
                throw new Exception($"ActionServer '{MOVEJ_ACTION_NAME}' is not available.");
            }

            var g = this.stepwiseMoveJActionClient.CreateGoal();
            g.check_collision = checkCollision;
            g.veloctiy_scaling = velocityScaling;
            g.trajectory.joint_names = trajectory.JointSet.ToArray();
            g.trajectory.points = trajectory.Select(x => x.ToJointTrajectoryPointMessage()).ToArray();
            goalHandle = this.stepwiseMoveJActionClient.SendGoal(g);
            cancelSubscription = cancel.Register(goalHandle.Cancel);
            goalId = goalHandle.GoaldId;

            motionState = new SteppedMotionState(goalId.id, null, 0, 0); // set initial motion state

            nextPublisher = nodeHandle.Advertise<GoalID>(NEXT_TOPIC, 1);
            previousPublisher = nodeHandle.Advertise<GoalID>(PREVIOUS_TOPIC, 1);
            feedbackSubscriber = nodeHandle.Subscribe<xamlamoveit.TrajectoryProgress>(FEEDBACK_TOPIC, 10, OnFeedbackMessage);
        }

        /// <summary>
        /// Current ros action goal id
        /// </summary>
        public string GoalId =>
            this.goalId.id;

        /// <summary>
        /// Current state of the supervised trajectory execution
        /// </summary>
        public SteppedMotionState MotionState =>
            this.motionState;

        /// <summary>
        /// The goal task as an instance of <c>Task</c> producing an instance of <c>xamlamoveit.StepwiseMoveJResult</c> as result.
        /// </summary>
        public Task<xamlamoveit.StepwiseMoveJResult> GoalTask =>
            this.goalHandle.GoalTask;

        /// <summary>
        /// Request at supervised executor to perform next step 
        /// </summary>
        public void Next()
        {
            nextPublisher.Publish(goalId);
        }

        /// <summary>
        /// Request at supervised executor to perform previous step 
        /// </summary>
        public void Previous()
        {
            previousPublisher.Publish(goalId);
        }

        /// <summary>
        /// Callback function reacting to feedback message
        /// </summary>
        /// <param name="feedback">The feedback message indicating the progress of the trajectory</param>
        private void OnFeedbackMessage(xamlamoveit.TrajectoryProgress feedback)
        {
            this.motionState = new SteppedMotionState(this.goalId.id, feedback.error_msg, (int)feedback.error_code, feedback.progress);
        }

        /// <summary>
        /// Clean up
        /// </summary>
        public void Dispose()
        {
            feedbackSubscriber?.Dispose();
            nextPublisher?.Dispose();
            previousPublisher?.Dispose();
            stepwiseMoveJActionClient?.Shutdown();
            cancelSubscription?.Dispose();
        }
    }
}
