namespace Xamla.Robotics.Types
{
    public class MoveGroupDescription
    {
        public string Name { get; }
        public string[] SubMoveGroupIds { get; }
        public JointSet JointSet { get; }
        public string[] EndEffectorNames { get; }
        public string[] EndEffectorLinkNames { get; }

        public MoveGroupDescription(string name, string[] subMoveGroupIds, JointSet jointSet, string[] endEffectorNames, string[] endEffectorLinkNames)
        {
            this.Name = name;
            this.SubMoveGroupIds = subMoveGroupIds;
            this.JointSet = jointSet;
            this.EndEffectorNames = endEffectorNames;
            this.EndEffectorLinkNames = endEffectorLinkNames;
        }
    }
}
