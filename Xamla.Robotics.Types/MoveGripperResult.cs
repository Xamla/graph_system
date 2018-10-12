namespace Xamla.Robotics.Types
{
    public class MoveGripperResult
    {
        public double Position { get; }
        public bool ReachedGoal { get; }
        public bool Stalled { get; }

        public MoveGripperResult(double position, bool reachedGoal, bool stalled)
        {
            this.Position = position;
            this.ReachedGoal = reachedGoal;
            this.Stalled = stalled;
        }
    }
}
