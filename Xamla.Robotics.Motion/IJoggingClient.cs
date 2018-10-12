using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    public enum JoggingErrorCode
    {
        OK = 1,
        INVALID_IK = -1,
        SELF_COLLISION = -2,
        SCENE_COLLISION = -3,
        FRAME_TRANSFORM_FAILURE = -4,
        IK_JUMP_DETECTED = -5,
        CLOSE_TO_SINGULARITY = -6,
        JOINT_LIMITS_VIOLATED = -7,
        INVALID_LINK_NAME = -8,
        TASK_SPACE_JUMP_DETECTED = -9,
    }

    public class JoggingControllerStatusModel
    {
        public double[] JointDistance { get; set; }
        public double[] CartesianDistance { get; set; }
        public JoggingErrorCode ErrorCode { get; set; }
        public bool Converged { get; set; }
        public bool SelfCollisionCheckEnabled { get; set; }
        public bool JointLimitsCheckEnabled { get; set; }
        public bool SceneCollisionCheckEnabled { get; set; }
    }

    public static class JoggingFlagName
    {
        public const string SelfCollisionCheckEnabled = "self_collision_check_enabled";
        public const string SceneCollisionCeckEnabled = "scene_collision_check_enabled";
        public const string JointLimitsCheckEnabled = "joint_limits_check_enabled";
    }

    public interface IJoggingClient
    {
        ControllerStatusModel GetStatus();
        void SendVelocities(JointValues velocities);
        void SendSetpoint(Pose setpoint);
        void SendTwist(Twist twist);

        string[] GetMoveGroupName();
        void SetMoveGroupName(string value);

        double GetVelocityScaling();
        void SetVelocityScaling(double value);

        bool GetFlag(string name);
        void SetFlag(string name, bool value);

        void Start();
        void Stop();
    }
}