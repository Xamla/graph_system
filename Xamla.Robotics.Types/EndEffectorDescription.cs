namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Holds the properties, which define an end effector.
    /// </summary>
    public class EndEffectorDescription
    {
        /// <summary>
        /// Gets the name of the end effector
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the IDs of the sub movegroups the end effector uses. (Not supported yet.)
        /// </summary>
        public string[] SubMoveGroupIds { get; }

        /// <summary>
        /// Gets the <c>JointSet</c> holding the joints that are moved when moving this end effector in Cartesian space.
        /// </summary>
        public JointSet JointSet { get; }

        /// <summary>
        /// Gets the name of the movegroup which is used when this end effector is moved in Cartesian space.
        /// </summary>
        public string MoveGroupName { get; }

        /// <summary>
        /// Gets the name of the link which is at the end of the kinematic chain of this end effector.
        /// </summary>
        public string LinkName { get; }

        /// <summary>
        /// Creates a new <c>EndEffectorDescription</c>, from the given properties that define an end effector.
        /// </summary>
        /// <param name="name">The name of the end effector</param>
        /// <param name="subMoveGroupIds">The IDs of the sub movegroups the end effector uses. (Not supported yet.)</param>
        /// <param name="jointSet">The <see cref="JointSet"></see> holding the joints that are moved when the end effector is moved in Cartesian space./></param>
        /// <param name="moveGroupName">The name of the movegroup that is used when the end effector is moved in Cartesian space.</param>
        /// <param name="endEffectorLinkName">The name of the link that is located at the end of the kinematic chain of the end effector.</param>
        public EndEffectorDescription(string name, string[] subMoveGroupIds, JointSet jointSet, string moveGroupName, string endEffectorLinkName)
        {
            this.Name = name;
            this.SubMoveGroupIds = subMoveGroupIds;
            this.JointSet = jointSet;
            this.MoveGroupName = moveGroupName;
            this.LinkName = endEffectorLinkName;
        }
    }
}
