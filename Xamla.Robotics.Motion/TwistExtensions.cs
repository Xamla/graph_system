using System;
using System.Numerics;
using Xamla.Robotics.Types;
using Uml.Robotics.Ros;

namespace Xamla.Robotics.Motion
{
    using std_msgs = Messages.std_msgs;
    using sensor_msgs = Messages.sensor_msgs;
    using geometry_msgs = Messages.geometry_msgs;
    using trajectory_msgs = Messages.trajectory_msgs;
    using xamlamoveit = Messages.xamlamoveit_msgs;

    public static class TwistExtensions
    {
        public static geometry_msgs.Twist ToTwistMessage(this Twist twist)
        {
            return new geometry_msgs.Twist()
            {
                linear = twist.Linear.ToVector3Message(),
                angular = twist.Angular.ToVector3Message()
            };
        }

        public static geometry_msgs.TwistStamped ToTwistStampedMessage(this Twist twist, std_msgs.Time stamp = null)
        {
            return new geometry_msgs.TwistStamped()
            {
                header = new std_msgs.Header() { stamp = stamp ?? TimeData.Zero.ToMessage(), frame_id = twist.Frame },
                twist = twist.ToTwistMessage(),
            };
        }
    }
}