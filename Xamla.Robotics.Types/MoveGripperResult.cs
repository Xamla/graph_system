namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Holds the response to a gripper commando.
    /// </summary>
    public class MoveGripperResult
    {
        /// <summary>
        /// Gets the reported position value of the gripper.
        /// </summary>
        public double Position { get; }

        /// <summary>
        /// Indicates whether the gripper has reached the given goal position.
        /// </summary>
        public bool ReachedGoal { get; }

        /// <summary>
        /// Indicates whether the gripper was blocked during the movement towards the goal position.
        /// </summary>
        public bool Stalled { get; }

        public MoveGripperResult(double position, bool reachedGoal, bool stalled)
        {
            this.Position = position;
            this.ReachedGoal = reachedGoal;
            this.Stalled = stalled;
        }
    }
}
