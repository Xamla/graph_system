using Messages.xamlamoveit_msgs;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Ros.Async;
using Xamla.Robotics.Types.Models;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Extensions of the <c>ISteppedMotionClient</c> interface
    /// </summary>
    public static class SteppedMotionClientExtensions
    {
        /// <summary>
        /// Extends the interface <c>ISteppedMotionClient</c> with a method <c>ToModel</c>, which creates an instance of <c>SteppedMotionClientModel</c> from a state snapshot.
        /// </summary>
        /// <param name="client">The client to be extended</param>
        /// <returns>Returns an instance of  <c>SteppedMotionClientModel</c>.</returns>
        public static SteppedMotionClientModel ToModel(this ISteppedMotionClient client)
        {
            var state = client.MotionState;     // read immutable state snapshot once
            return new SteppedMotionClientModel { GoalId = client.GoalId, Progress = state.Progress, ErrorCode = state.ErrorCode, ErrorMessage = state.ErrorMessage };
        }

        class SteppedMotionMessage
        {
            public string GoalId { get; set; }
            public string MoveGroupName { get; set; }
            public double Progress { get; set; }

            public override string ToString()
                => JToken.FromObject(this).ToString(Newtonsoft.Json.Formatting.None);
        }

        public async static Task HandleStepwiseMotions(this ISteppedMotionClient client, IMoveGroup group, bool disposeClient = false)
        {
            try
            {
                const string channelName = "MotionDialog";
                const string topic = "SteppedMotions";
                const string disposition = "SteppedMotion";
                using (var RosRoboChatClient = new RosRoboChatActionClient(group.MotionService.NodeHandle))
                {
                    RosRoboChatClient.CreateChat(channelName, topic);
                    var messageBody = new SteppedMotionMessage { GoalId = client.GoalId, Progress = 0, MoveGroupName = group.Name }.ToString();
                    string messageId = RosRoboChatClient.CallMessageCommand(channelName, "add", messageBody, null, new string[] { disposition });
                    try
                    {
                        var cancelReport = new CancellationTokenSource();
                        StepwiseMoveJResult result;
                        var updateProgressTask = UpdateProgress(client, RosRoboChatClient, channelName, messageId, disposition, group.Name, cancelReport.Token);
                        try
                        {
                            result = await client.GoalTask;
                        }
                        finally
                        {
                            cancelReport.Cancel();
                            await updateProgressTask.WhenCompleted();
                        }

                        if (result.result < 0)
                            throw new Exception($"SteppedMotion was not finished: { result.result }");
                    }
                    finally
                    {
                        RosRoboChatClient.CallMessageCommand(channelName, "remove", null, messageId, new string[] { disposition });
                    }
                }
            }
            finally
            {
                if (disposeClient)
                {
                    client.Dispose();
                }
            }
        }

        async static Task UpdateProgress(
            ISteppedMotionClient client,
            RosRoboChatActionClient roboChatclient,
            string channelName,
            string messageId,
            string disposition,
            string moveGroupName,
            CancellationToken cancel
        )
        {
            await Task.Yield();

            double lastProgress = 0.0;
            while (!client.GoalTask.IsCanceled && !client.GoalTask.IsFaulted && !client.GoalTask.IsCompleted)
            {
                var state = client.MotionState;     // read immutable state snapshot once
                if (state.Progress != lastProgress)
                {
                    lastProgress = state.Progress;
                    var messageBody = new SteppedMotionMessage { GoalId = client.GoalId, Progress = lastProgress, MoveGroupName = moveGroupName };
                    roboChatclient.CallMessageCommand(channelName, "update", messageBody.ToString(), messageId, new string[] { disposition });
                }
                await Task.Delay(100, cancel);
            }
        }
    }
}
