using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uml.Robotics.Ros;
using Uml.Robotics.Ros.ActionLib;

namespace Xamla.Robotics.Motion
{
    using rosgardener = Messages.rosgardener;

    public class RosRoboChatActionClient
        : IDisposable
    {
        const string ROBOCHAT_CHANNEL_SERVICE_NAME = "/robochat/channel_command";
        const string ROBOCHAT_MESSAGE_SERVICE_NAME = "/robochat/message_command";
        const string ROBOCHAT_ACTION_NAME = "/robochat/query";

        ServiceClient<rosgardener.SetChannelCommand> channelCommandService;
        ServiceClient<rosgardener.SetMessageCommand> messageCommandService;
        ActionClient<rosgardener.RobochatQueryGoal, rosgardener.RobochatQueryResult, rosgardener.RobochatQueryFeedback> rosRoboChatActionClient;

        public RosRoboChatActionClient(NodeHandle nodeHandle)
        {
            channelCommandService = nodeHandle.ServiceClient<rosgardener.SetChannelCommand>(ROBOCHAT_CHANNEL_SERVICE_NAME);
            messageCommandService = nodeHandle.ServiceClient<rosgardener.SetMessageCommand>(ROBOCHAT_MESSAGE_SERVICE_NAME);
            rosRoboChatActionClient = new ActionClient<rosgardener.RobochatQueryGoal, rosgardener.RobochatQueryResult, rosgardener.RobochatQueryFeedback>(ROBOCHAT_ACTION_NAME, nodeHandle);
        }

        public void Dispose()
        {
            channelCommandService?.Dispose();
            channelCommandService = null;

            messageCommandService?.Dispose();
            messageCommandService = null;

            rosRoboChatActionClient?.Shutdown();
            rosRoboChatActionClient = null;
        }

        public string CallMessageCommand(string channelName, string command, string messageBody, string messageId = null, params string[] arguments)
        {
            var srv = new rosgardener.SetMessageCommand();
            srv.req.command.header.channel_name = channelName;
            srv.req.command.header.command = command;
            srv.req.command.message_id = messageId ?? "";
            srv.req.command.message_body = messageBody ?? "";

            if (arguments != null)
                srv.req.command.header.arguments = arguments;

            if (!messageCommandService.Call(srv))
                throw new ServiceCallFailedException(ROBOCHAT_MESSAGE_SERVICE_NAME);

            return srv.resp.response.message_id;
        }

        private void CallChannelCommand(string name, string command, params string[] arguments)
        {
            var srv = new rosgardener.SetChannelCommand();
            srv.req.command.channel_name = name;
            srv.req.command.command = command;
            if (arguments != null)
                srv.req.command.arguments = arguments;

            if (!channelCommandService.Call(srv))
                throw new ServiceCallFailedException(ROBOCHAT_CHANNEL_SERVICE_NAME);
        }

        public void CreateChat(string name, string topic, int backlog = 1000) =>
            CallChannelCommand(name, "create", topic, backlog.ToString(NumberFormatInfo.InvariantInfo));

        public void DeleteChat(string name) =>
            CallChannelCommand(name, "delete");

        public void ClearChat(string name) =>
            CallChannelCommand(name, "clear");

        public void ListChats(string name) =>
            CallChannelCommand(name, "list");

        public void SetTopicChat(string name, string topic) =>
            CallChannelCommand(name, "set_topic", topic);

        public void CreateTextMessage(string channel_name, string text) =>
            CallMessageCommand(channel_name, "add", text);

        public void DeleteTextMessage(string channel_name, string messageId) =>
            CallMessageCommand(channel_name, "remove", null, messageId);

        public void UpdateTextMessage(string channel_name, string messageId, string text) =>
            CallMessageCommand(channel_name, "update", text, messageId);

        public async Task<rosgardener.RobochatQueryResult> QueryUserInteraction(string channelName, string command, string[] arguments = null, string messageId = null, string messageBody = null, CancellationToken cancel = default(CancellationToken))
        {
            var goal = rosRoboChatActionClient.CreateGoal();
            goal.command.header.channel_name = channelName;
            goal.command.header.command = command;
            if (arguments != null)
                goal.command.header.arguments = arguments;

            if (messageId != null)
                goal.command.message_id = messageId;
            if (messageBody != null)
                goal.command.message_body = messageBody;

            rosgardener.RobochatQueryResult result = await rosRoboChatActionClient.SendGoalAsync(goal, null, null, cancel);
            return result;
        }
    }
}