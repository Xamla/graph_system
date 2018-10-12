using System;
using System.Threading;
using System.Threading.Tasks;
using Uml.Robotics.Ros;
using Uml.Robotics.Ros.ActionLib;

namespace Xamla.Robotics.Motion
{
    using actionlib_msgs = Messages.actionlib_msgs;

    public class ActionFailedExeption
        : Exception
    {
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

        public ActionFailedExeption(string actionName, actionlib_msgs.GoalStatus goalStatus)
            : base($"The action '{actionName}' failed with final goal status '{GetGoalStatusString(goalStatus)}': {goalStatus?.text}")
        {
            this.ActionName = actionName;
            this.FinalGoalStatus = ((GoalStatus?)goalStatus?.status) ?? GoalStatus.LOST;
            this.StatusText = goalStatus?.text;
        }

        public string ActionName { get; }
        public GoalStatus FinalGoalStatus { get; }
        public string StatusText { get; }
    }
}
