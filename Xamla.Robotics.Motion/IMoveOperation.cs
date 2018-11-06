using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public class MoveArgs
    {
        public IMoveGroup MoveGroup { get; set; }
        public double? VelocityScaling { get; set; }
        public double? AccelerationScaling { get; set; }
        public bool? CollisionCheck { get; set; }
        public double? SampleResolution { get; set; }
        public double? MaxDeviation { get; set; }
    }

    public interface IMoveOperation
    {
        IMoveGroup MoveGroup { get; }
        double? VelocityScaling { get; }
        double? AccelerationScaling { get; }
        PlanParameters Parameters { get; }
        IPlan Plan();
    }
}
