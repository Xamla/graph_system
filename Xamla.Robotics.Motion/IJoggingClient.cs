﻿using Xamla.Robotics.Types;

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


    /// <summary>
    /// Implementations of <c>IWeissWsgServices</c>  TODO: description
    /// </summary>
    public interface IJoggingClient
    {
        /// <summary>
        /// Get the status of the controller 
        /// </summary>
        /// <returns>Returns an instance of <c>ControllerStatusModel</c>.</returns>
        ControllerStatusModel GetStatus();

        /// <summary>
        /// Send velocities to TODO: Where to?
        /// </summary>
        /// <param name="velocities">Velocities to sent</param>
        void SendVelocities(JointValues velocities);

        /// <summary>
        /// Send points to TODO: Where to?
        /// </summary>
        /// <param name="setpoint">The setpoint to be sent</param>
        void SendSetpoint(Pose setpoint);

        /// <summary>
        /// Send twist to TODO: Where to?
        /// </summary>
        /// <param name="twist">The twist to be sent</param>
        void SendTwist(Twist twist);

        /// <summary>
        /// Get the name of the move group
        /// </summary>
        /// <returns>Returns a list of strings containing the names TODO: More precise</returns>
        string[] GetMoveGroupName();

        /// <summary>
        /// Set the name of the move group
        /// </summary>
        /// <param name="value">The name to be set</param>
        void SetMoveGroupName(string value);

        /// <summary>
        /// Get the velocity scaling 
        /// </summary>
        /// <returns>Returns the velocity scaling</returns>
        double GetVelocityScaling();
       
        /// <summary>
        /// Set the velocity scaling factor
        /// </summary>
        /// <param name="value">Scaling factor to be set</param>
        void SetVelocityScaling(double value);

        /// <summary>
        /// Get flag by name
        /// </summary>
        /// <param name="name">Name of the flag</param>
        /// <returns>Returns true if flag is set, false otherwise</returns>
        bool GetFlag(string name);

        /// <summary>
        /// Set flag of given name
        /// </summary>
        /// <param name="name">Name of the flag</param>
        /// <param name="value">The value the flag should be set</param>
        void SetFlag(string name, bool value);

        /// <summary>
        /// Start TODO: Definition
        /// </summary>
        void Start();

        /// <summary>
        /// Stop TODO: Definition
        /// </summary>
        void Stop();
    }
}