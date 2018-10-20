namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// An enumeration of every goal status
    /// </summary>
    public enum GoalStatus
        : byte      // must match type of actionlib_msgs.GoalStatus.status field
    {
        PENDING = 0,
        ACTIVE = 1,
        PREEMPTED = 2,
        SUCCEEDED = 3,
        ABORTED = 4,
        REJECTED = 5,
        PREEMPTING = 6,
        RECALLING = 7,
        RECALLED = 8,
        LOST = 9,
    }
}
