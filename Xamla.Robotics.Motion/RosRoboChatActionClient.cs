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

    /// <summary>
    /// A Ros Client to send messages and commands RosRoboChar
    /// </summary>
    public class RosRoboChatActionClient
        : IDisposable
    {
        const string ROBOCHAT_CHANNEL_SERVICE_NAME = "/robochat/channel_command";
        const string ROBOCHAT_MESSAGE_SERVICE_NAME = "/robochat/message_command";
        const string ROBOCHAT_ACTION_NAME = "/robochat/query";

        ServiceClient<rosgardener.SetChannelCommand> channelCommandService;
        ServiceClient<rosgardener.SetMessageCommand> messageCommandService;
        ActionClient<rosgardener.RobochatQueryGoal, rosgardener.RobochatQueryResult, rosgardener.RobochatQueryFeedback> rosRoboChatActionClient;

        /// <summary>
        /// Creates a instance of <c>RosRoboChatActionClient</c>
        /// </summary>
        /// <param name="nodeHandle">A node handle</param>
        public RosRoboChatActionClient(NodeHandle nodeHandle)
        {
            channelCommandService = nodeHandle.ServiceClient<rosgardener.SetChannelCommand>(ROBOCHAT_CHANNEL_SERVICE_NAME);
            messageCommandService = nodeHandle.ServiceClient<rosgardener.SetMessageCommand>(ROBOCHAT_MESSAGE_SERVICE_NAME);
            rosRoboChatActionClient = new ActionClient<rosgardener.RobochatQueryGoal, rosgardener.RobochatQueryResult, rosgardener.RobochatQueryFeedback>(ROBOCHAT_ACTION_NAME, nodeHandle);
        }

        /// <summary>
        /// Clean up
        /// </summary>
        public void Dispose()
        {
            channelCommandService?.Dispose();
            channelCommandService = null;

            messageCommandService?.Dispose();
            messageCommandService = null;

            rosRoboChatActionClient?.Shutdown();
            rosRoboChatActionClient = null;
        }

        /// <summary>
        /// Calls the robochat message service with a command
        /// </summary>
        /// <param name="channelName">The name of the channel</param>
        /// <param name="command">The command to be executed</param>
        /// <param name="messageBody">A message</param>
        /// <param name="messageId">A message id</param>
        /// <param name="arguments">A list of arguments for the command</param>
        /// <returns>The response of the message call.</returns>
        /// <exception cref="ServiceCallFailedException">Thrown when call to service failed.</exception>
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

        /// <summary>
        /// Create a chat
        /// </summary>
        /// <param name="name">The name of the chat</param>
        /// <param name="topic">A topic</param>
        /// <param name="backlog">The size of the backlog</param>
        public void CreateChat(string name, string topic, int backlog = 1000) =>
            CallChannelCommand(name, "create", topic, backlog.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Delete a chat
        /// </summary>
        /// <param name="name">Name of the chat to be deleted</param>
        public void DeleteChat(string name) =>
            CallChannelCommand(name, "delete");

        /// <summary>
        /// Clear a chat
        /// </summary>
        /// <param name="name">Name of the chat to be cleared</param>
        public void ClearChat(string name) =>
            CallChannelCommand(name, "clear");

        /// <summary>
        /// List chat
        /// </summary>
        /// <param name="name">Name of the chat</param>
        public void ListChats(string name) =>
            CallChannelCommand(name, "list");

        /// <summary>
        /// Sets the the topic of a chat 
        /// </summary>
        /// <param name="name">Name of the chat</param>
        /// <param name="topic">The topic name</param>
        public void SetTopicChat(string name, string topic) =>
            CallChannelCommand(name, "set_topic", topic);

        /// <summary>
        /// Create a text message
        /// </summary>
        /// <param name="channel_name">Name of a channel</param>
        /// <param name="text">The text of the message </param>
        public void CreateTextMessage(string channel_name, string text) =>
            CallMessageCommand(channel_name, "add", text);

        /// <summary>
        /// Delete a text message
        /// </summary>
        /// <param name="channel_name">Name of a channel</param>
        /// <param name="messageId">The id of the message to be deleted</param>
        public void DeleteTextMessage(string channel_name, string messageId) =>
            CallMessageCommand(channel_name, "remove", null, messageId);

        /// <summary>
        /// Update the content of a text message
        /// </summary>
        /// <param name="channel_name">Name of a channel</param>
        /// <param name="messageId">The id of the message to be updated</param>
        /// <param name="text">The updated text</param>
        public void UpdateTextMessage(string channel_name, string messageId, string text) =>
            CallMessageCommand(channel_name, "update", text, messageId);

        /// <summary>
        /// Queries the users interaction with Ros asynchronously 
        /// </summary>
        /// <param name="channelName">Name of the channel</param>
        /// <param name="command">The command to be executed</param>
        /// <param name="arguments">List of arguments of the command </param>
        /// <param name="messageId">The id of the message</param>
        /// <param name="messageBody">The message content</param>
        /// <param name="cancel">CancellationToken</param>
        /// <returns>An instance of <c>Task</c>, which returns the result of the query as a instance of <c>rosgardener.RobochatQueryResult</c>.</returns>
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