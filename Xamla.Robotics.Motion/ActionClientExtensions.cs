using System;
using System.Threading;
using System.Threading.Tasks;
using Uml.Robotics.Ros;
using Uml.Robotics.Ros.ActionLib;

namespace Xamla.Robotics.Motion
{
    using actionlib_msgs = Messages.actionlib_msgs;

    /// <summary>
    /// Implementation of <c>Exception</c> to indicate when an action has failed.
    /// </summary>
    public class ActionFailedExeption
        : Exception
    {
        /// <summary>
        /// Get the goal status 
        /// </summary>
        /// <param name="goalStatus">The goal status as an instance of <c>actionlib_msgs.GoalStatus</c></param>
        /// <returns>Returns the goal status as <c>string</c>.</returns>
        public static string GetGoalStatusString(actionlib_msgs.GoalStatus goalStatus)
        {
            if (goalStatus == null)
                return "null";

            if (Enum.IsDefined(typeof(GoalStatus), goalStatus.status))
            {
                GoalStatus status = (GoalStatus)goalStatus.status;
                return status.ToString("g");
            }
            return $"INVALID GOAL STATUS {goalStatus.status}";
        }

        /// <summary>
        /// Create an instance of <c>ActionFailedExeption</c> using an action name and a goal status 
        /// </summary>
        /// <param name="actionName">The name of the action</param>
        /// <param name="goalStatus">The goal status as an instance of <c>actionlib_msgs.GoalStatus</c></param>
        public ActionFailedExeption(string actionName, actionlib_msgs.GoalStatus goalStatus)
            : base($"The action '{actionName}' failed with final goal status '{GetGoalStatusString(goalStatus)}': {goalStatus?.text}")
        {
            this.ActionName = actionName;
            this.FinalGoalStatus = ((GoalStatus?)goalStatus?.status) ?? GoalStatus.LOST;
            this.StatusText = goalStatus?.text;
        }

        /// <summary>
        /// The name of the action
        /// </summary>
        public string ActionName { get; }

        /// <summary>
        /// The status of the final goal TODO: Or final status of the goal?
        /// </summary>
        public GoalStatus FinalGoalStatus { get; }

        /// <summary>
        /// The status as <c>string</c>
        /// </summary>
        public string StatusText { get; }
    }
}
