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

    public static class PoseExtensions
    {
        public static std_msgs.Time ToMessage(this TimeData time)
        {
            return new std_msgs.Time() { data = time };
        }

        public static geometry_msgs.Quaternion ToQuaternionMessage(this Quaternion value)
        {
            return new geometry_msgs.Quaternion()
            {
                x = value.X,
                y = value.Y,
                z = value.Z,
                w = value.W
            };
        }

        public static geometry_msgs.Point ToPointMessage(this Vector3 value)
        {
            return new geometry_msgs.Point()
            {
                x = value.X,
                y = value.Y,
                z = value.Z,
            };
        }

        public static geometry_msgs.Vector3 ToVector3Message(this Vector3 value)
        {
            return new geometry_msgs.Vector3()
            {
                x = value.X,
                y = value.Y,
                z = value.Z,
            };
        }

        public static geometry_msgs.QuaternionStamped ToQuaternionMessageStamped(this Quaternion value, std_msgs.Time stamp, string frameId = "")
        {
            return new geometry_msgs.QuaternionStamped()
            {
                header = new std_msgs.Header() { stamp = stamp, frame_id = frameId },
                quaternion = value.ToQuaternionMessage()
            };
        }

        public static geometry_msgs.Pose ToPoseMessage(this Pose pose)
        {
            return new geometry_msgs.Pose()
            {
                orientation = pose.Rotation.ToQuaternionMessage(),
                position = pose.Translation.ToPointMessage()
            };
        }

        public static geometry_msgs.PoseStamped ToPoseStampedMessage(this Pose pose, std_msgs.Time stamp = null)
        {
            return new geometry_msgs.PoseStamped()
            {
                header = new std_msgs.Header() { stamp = stamp ?? TimeData.Zero.ToMessage(), frame_id = pose.Frame },
                pose = pose.ToPoseMessage(),
            };
        }

        public static Vector3 ToVector3(this geometry_msgs.Point point)
        {
            return new Vector3((float)point.x, (float)point.y, (float)point.z);
        }

        public static Quaternion ToQuaternion(this geometry_msgs.Quaternion value)
        {
            return new Quaternion((float)value.x, (float)value.y, (float)value.z, (float)value.w);
        }

        public static Pose ToPose(this geometry_msgs.Pose pose, string frame = "")
        {
            return new Pose(pose.position.ToVector3(), pose.orientation.ToQuaternion(), frame);
        }

        public static Pose ToPose(this geometry_msgs.PoseStamped poseStamped)
        {
            return ToPose(poseStamped.pose, poseStamped.header.frame_id);
        }

        public static xamlamoveit.JointPathPoint ToJointPathPointMessage(this JointValues jointValues)
        {
            return new xamlamoveit.JointPathPoint()
            {
                positions = jointValues.Values
            };
        }

        public static JointStates ToJointState(this sensor_msgs.JointState jointState)
        {
            var joints = new JointSet(jointState.name);
            JointValues getJointValues(double[] v) => v != null && v.Length > 0 ? new JointValues(joints, v) : null;
            return new JointStates(getJointValues(jointState.position), getJointValues(jointState.velocity), getJointValues(jointState.effort));
        }

        public static JointValues ToJointValues(this xamlamoveit.JointPathPoint point, JointSet jointSet)
        {
            return new JointValues(jointSet, point.positions);
        }

        public static std_msgs.Duration ToDurationMessage(this TimeSpan span)
        {
            return new std_msgs.Duration()
            {
                data = TimeData.FromTimeSpan(span)
            };
        }

        public static trajectory_msgs.JointTrajectoryPoint ToJointTrajectoryPointMessage(this JointTrajectoryPoint point)
        {
            return new trajectory_msgs.JointTrajectoryPoint()
            {
                time_from_start = point.TimeFromStart.ToDurationMessage(),
                positions = point.Positions?.Values,
                velocities = point.Velocities?.Values,
                accelerations = point.Accelerations?.Values,
                effort = point.Effort?.Values,
            };
        }

        public static Twist ToTwist(this Pose source)
        {
            var rotation = source.Rotation;
            double angle = 2 * Math.Acos(rotation.W);
            double s = Math.Sqrt(1 - rotation.W * rotation.W);

            if (s < double.Epsilon)
                s = 1;

            var axis = new Vector3((float)(rotation.X / s), (float)(rotation.Y / s), (float)(rotation.Z / s));
            return new Twist(source.Translation, Vector3.Multiply((float)angle, axis), source.Frame);
        }
    }
}
