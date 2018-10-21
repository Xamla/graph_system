namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Enumeration of all possible command that can be sent to WSG Gripper.
    /// </summary>
    public enum WsgCommand
    {
        Stop,
        Move,
        Grasp,
        Release,
        Homing,
        AcknowledgeError
    }

    /// <summary>
    /// Enumeration of all Possible states that can be reported by WSG Gripper.
    /// </summary>
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

    /// <summary>
    /// Class describing a group of measurements [State, Width, Force, Status] of WSG Gripper.
    /// </summary>
    public class WsgResult
    {
        /// <summary>
        /// Contains code of which <c>WsgState</c> the Gripper is currently in.
        /// </summary>
        public int State { get; }

        /// <summary>
        /// Width between the two Gripping fingers [in meters].
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// Force currently exerted by the Gripper [in Newton].
        /// </summary>
        public double Force { get; }

        /// <summary>
        /// A string message explaining current status.
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// Creat a new <c>WsgResult</c> object.
        /// </summary>
        /// <param name="state">Contains code of which <c>WsgState</c> the Gripper is currently in.</param>
        /// <param name="width">Width between the two Gripping fingers [in meters].</param>
        /// <param name="force">Force currently exerted by the Gripper [in Newton].</param>
        /// <param name="status">A string message explaining current status.</param>
        public WsgResult(int state, double width, double force, string status)
        {
            this.State = state;
            this.Width = width;
            this.Force = force;
            this.Status = status;
        }
    }
}

