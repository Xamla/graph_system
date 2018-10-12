using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    using wsg_50 = Messages.wsg_50;

    public class WeissWsgPropertiesModel
    {
        public string StatusTopic { get; set; }
        public string StatusTopicType { get; set; }
        public string ControlAction { get; set; }
        public string StatusService { get; set; }
        public string SetAccelerationService { get; set; }
        public double MinWidth { get; set; }
        public double MaxWidth { get; set; }
        public double MinSpeed { get; set; }
        public double MaxSpeed { get; set; }
        public double MinAcceleration { get; set; }
        public double MaxAcceleration { get; set; }
        public double MinForce { get; set; }
        public double MaxForce { get; set; }
        public double DefaultMoveWidth { get; set; }
        public double DefaultReleaseWidth { get; set; }
        public double DefaultGraspWidth { get; set; }
        public double DefaultSpeed { get; set; }
        public double DefaultForce { get; set; }
        public double DefaultAcceleration { get; set; }
    }

    public interface IWeissWsgServices
    {
        Task AcknowledgeError();
        Task Homing();
        Task Stop();
        Task Move(double position, double speed, double force, bool stopOnBlock = true);
        Task Grasp(double position, double speed, double force);
        Task Release(double position, double speed);
        Task<int> SetAcceleration(double acceleration);
        Task<WeissWsgPropertiesModel> GetProperties();
        Task<wsg_50.Status> GetStatus();
    }
}
