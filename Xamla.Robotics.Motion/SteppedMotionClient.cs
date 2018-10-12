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

        public string GoalId =>
            this.goalId.id;

        public SteppedMotionState MotionState =>
            this.motionState;

        public Task<xamlamoveit.StepwiseMoveJResult> GoalTask =>
            this.goalHandle.GoalTask;

        public void Next()
        {
            nextPublisher.Publish(goalId);
        }

        public void Previous()
        {
            previousPublisher.Publish(goalId);
        }

        private void OnFeedbackMessage(xamlamoveit.TrajectoryProgress feedback)
        {
            this.motionState = new SteppedMotionState(this.goalId.id, feedback.error_msg, (int)feedback.error_code, feedback.progress);
        }

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
