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


    /// <summary>
    /// Extensionmethods for various types concerning their representation as messages
    /// </summary>
    public static class PoseExtensions
    {
        /// <summary>
        /// Extends type <c>TimeData</c> with a method <c>ToMessage</c>, which creates a time me.ssage
        /// </summary>
        /// <param name="time">A <c>TimeData</c> object</param>
        /// <returns>Returns an instance of <c>std_msgs.Time</c>.</returns>
        public static std_msgs.Time ToMessage(this TimeData time)
        {
            return new std_msgs.Time() { data = time };
        }

        /// <summary>
        /// Extends type <c>Quaternion</c> with a method <c>ToQuaternionMessage</c>, which creates a message.
        /// </summary>
        /// <param name="value">A <c>Quaternion</c> object</param>
        /// <returns>Returns an instance of <c>geometry_msgs.Quaternion</c>.</returns>
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

        /// <summary>
        /// Extends type <c>Vector3</c> with a method <c>ToPointMessage</c>, which creates a message.
        /// </summary>
        /// <param name="value">A <c>Vector3</c> object</param>
        /// <returns>Returns an instance of <c>geometry_msgs.Point</c>.</returns>
        public static geometry_msgs.Point ToPointMessage(this Vector3 value)
        {
            return new geometry_msgs.Point()
            {
                x = value.X,
                y = value.Y,
                z = value.Z,
            };
        }

        /// <summary>
        /// Extends type <c>Vector3</c> with a method <c>ToVector3Message</c>, which creates a message.
        /// </summary>
        /// <param name="value">A <c>Vector3</c> object</param>
        /// <returns>Returns an instance of <c>geometry_msgs.Vector3</c>.</returns>
        public static geometry_msgs.Vector3 ToVector3Message(this Vector3 value)
        {
            return new geometry_msgs.Vector3()
            {
                x = value.X,
                y = value.Y,
                z = value.Z,
            };
        }
        
        /// <summary>
        /// Extends type <c>Quaternion</c> with a method <c>ToQuaternionMessageStamped</c>, which creates a time stamped message.
        /// </summary>
        /// <param name="value">A <c>Quaternion</c> object</param>
        /// <param name="stamp">A <c>std_msgs.Time</c> message object</param>
        /// <param name="frameId">A string representing the frameId</param>
        /// <returns>Returns an instance of <c>geometry_msgs.QuaternionStamped</c>.</returns>
        public static geometry_msgs.QuaternionStamped ToQuaternionMessageStamped(this Quaternion value, std_msgs.Time stamp, string frameId = "")
        {
            return new geometry_msgs.QuaternionStamped()
            {
                header = new std_msgs.Header() { stamp = stamp, frame_id = frameId },
                quaternion = value.ToQuaternionMessage()
            };
        }


        /// <summary>
        /// Extends type <c>Pose</c> with a method <c>ToPoseMessage</c>, which creates a message.
        /// </summary>
        /// <param name="pose">A <c>Pose</c> object</param>
        /// <returns>Returns an instance of <c>geometry_msgs.Pose</c>.</returns>
        public static geometry_msgs.Pose ToPoseMessage(this Pose pose)
        {
            return new geometry_msgs.Pose()
            {
                orientation = pose.Rotation.ToQuaternionMessage(),
                position = pose.Translation.ToPointMessage()
            };
        }


        /// <summary>
        /// Extends type <c>Pose</c> with a method <c>ToPoseStampedMessage</c>, which creates a time stamped message.
        /// </summary>
        /// <param name="pose">A <c>Pose</c> object</param>
        /// <param name="stamp">A <c>std_msgs.Time</c> message object</param>
        /// <returns>Returns an instance of <c>geometry_msgs.PoseStamped</c>.</returns>
        public static geometry_msgs.PoseStamped ToPoseStampedMessage(this Pose pose, std_msgs.Time stamp = null)
        {
            return new geometry_msgs.PoseStamped()
            {
                header = new std_msgs.Header() { stamp = stamp ?? TimeData.Zero.ToMessage(), frame_id = pose.Frame },
                pose = pose.ToPoseMessage(),
            };
        }

        /// <summary>
        /// Extends type <c>geometry_msgs.Point</c> with a method <c>ToVector3</c>, which creates a corresponding vector.
        /// </summary>
        /// <param name="point">The point message</param>
        /// <returns>Returns an instance of <c>Vector3</c>.</returns>
        public static Vector3 ToVector3(this geometry_msgs.Point point)
        {
            return new Vector3((float)point.x, (float)point.y, (float)point.z);
        }

        /// <summary>
        /// Extends type <c>geometry_msgs.Quaternion</c> with a method <c>ToQuaternion</c>, which creates a corresponding quaternion.
        /// </summary>
        /// <param name="point">The quaternion message</param>
        /// <returns>Returns an instance of <c>Quaternion</c>.</returns>
        public static Quaternion ToQuaternion(this geometry_msgs.Quaternion value)
        {
            return new Quaternion((float)value.x, (float)value.y, (float)value.z, (float)value.w);
        }

        /// <summary>
        /// Extends type <c>geometry_msgs.Pose</c> with a method <c>ToPose</c>, which creates a corresponding pose.
        /// </summary>
        /// <param name="pose">A pose message</param>
        /// <param name="frame">A string representing the frame</param>
        /// <returns>Returns an instance of <c>Pose</c>.</returns>
        public static Pose ToPose(this geometry_msgs.Pose pose, string frame = "")
        {
            return new Pose(pose.position.ToVector3(), pose.orientation.ToQuaternion(), frame);
        }

        /// <summary>
        /// Extends type <c>geometry_msgs.PoseStamped</c> with a method <c>ToPose</c>, which creates a corresponding pose.
        /// </summary>
        /// <param name="poseStamped">A time stamped pose message</param>
        /// <returns>Returns an instance of <c>Pose</c>.</returns>
        public static Pose ToPose(this geometry_msgs.PoseStamped poseStamped)
        {
            return ToPose(poseStamped.pose, poseStamped.header.frame_id);
        }

        /// <summary>
        /// Extends type <c>JointValues</c> with a method <c>ToJointPathPointMessage</c>, which creates a message.
        /// </summary>
        /// <param name="jointValues">A <c>JointValues</c> object</param>
        /// <returns>Returns an instance of <c>xamlamoveit.JointPathPoint</c>.</returns>
        public static xamlamoveit.JointPathPoint ToJointPathPointMessage(this JointValues jointValues)
        {
            return new xamlamoveit.JointPathPoint()
            {
                positions = jointValues.Values
            };
        }

        /// <summary>
        /// Extends type <c>sensor_msgs.JointState</c> with a method <c>ToJointState</c>, which creates corresponding jointstates
        /// </summary>
        /// <param name="jointState">A time joint states message</param>
        /// <returns>Returns an instance of <c>JointStates</c>.</returns>
        public static JointStates ToJointState(this sensor_msgs.JointState jointState)
        {
            var joints = new JointSet(jointState.name);
            JointValues getJointValues(double[] v) => v != null && v.Length > 0 ? new JointValues(joints, v) : null;
            return new JointStates(getJointValues(jointState.position), getJointValues(jointState.velocity), getJointValues(jointState.effort));
        }


        /// <param name="jointState">A time joint states message</param>
        /// <returns>Returns an instance of <c>JointStates</c>.</returns>
        /// 
        
        /// <summary>
        /// Extends type <c>xamlamoveit.JointPathPoint</c> with a method <c>ToJointValues</c>, which creates corresponding jointvalues 
        /// from a jointset.
        /// </summary>
        /// <param name="point">A point message</param>
        /// <param name="jointSet">A jointSet object</param>
        /// <returns>Returns an instance of <c>JointValues</c>.</returns>
        public static JointValues ToJointValues(this xamlamoveit.JointPathPoint point, JointSet jointSet)
        {
            return new JointValues(jointSet, point.positions);
        }

        /// <summary>
        /// Extends type <c>TimeSpan</c> with a method <c>ToDurationMessage</c>, which creates a message.
        /// </summary>
        /// <param name="jointValues">A <c>TimeSpan</c> object</param>
        /// <returns>Returns an instance of <c>std_msgs.Duration</c>.</returns>
        public static std_msgs.Duration ToDurationMessage(this TimeSpan span)
        {
            return new std_msgs.Duration()
            {
                data = TimeData.FromTimeSpan(span)
            };
        }

        /// <summary>
        /// Extends type <c>JointTrajectoryPoint</c> with a method <c>ToJointTrajectoryPointMessage</c>, which creates a joint trajectory message.
        /// </summary>
        /// <param name="point">A point</param>
        /// <returns>Returns an instance of <c>trajectory_msgs.JointTrajectoryPoint</c>.</returns>
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

        /// <summary>
        /// Extends type <c>Pose</c> with a method <c>ToTwist</c>, which creates a <c>Twist</c> object from a pose.
        /// </summary>
        /// <param name="source">A pose</param>
        /// <returns>Returns an instance of <c>Twist</c> calculated from the pose.</returns>
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
