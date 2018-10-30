namespace Xamla.Robotics.Types
{
    /// <summary>
    /// The EndEffectorPose class combines a <c>Pose</c> and a link name to specify
    /// the position of an end effector of a robot. The link name must be resolvable
    /// as tf frame.
    /// </summary>
    public class EndEffectorPose
    {
        /// <summary>
        /// Creates a <c>EndEffectorPose</c> object.
        /// </summary>
        /// <param name="pose">Target pose of end effector</param>
        /// <param name="linkName">Name of the link corresponding to the end effector in the URDF model which is to be positioned.</param>
        public EndEffectorPose(Pose pose, string linkName)
        {
            this.Pose = pose;
            this.LinkName = linkName;
        }

        /// <summary>
        /// Cartesian pose defining the position and orientation of the end effector.
        /// </summary>
        public Pose Pose { get; }

        /// <summary>
        /// Name of the link corresponding to the end effector in the URDF model which is to be positioned.
        /// </summary>
        public string LinkName { get; }
    }
}
