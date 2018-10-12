namespace Xamla.Robotics.Types
{
    public enum WsgCommand
    {
        Stop,
        Move,
        Grasp,
        Release,
        Homing,
        AcknowledgeError
    }

    public enum WsgState
    {
        Idle = 0,
        Grasping = 1,
        NoPartFound = 2,
        PartLost = 3,
        Holding = 4,
        Releasing = 5,
        Positioning = 6,
        Error = 7,
        Unknown = -1
    }

    public class WsgResult
    {
        public int State { get; }
        public double Width { get; }
        public double Force { get; }
        public string Status { get; }

        public WsgResult(int state, double width, double force, string status)
        {
            this.State = state;
            this.Width = width;
            this.Force = force;
            this.Status = status;
        }
    }
}
