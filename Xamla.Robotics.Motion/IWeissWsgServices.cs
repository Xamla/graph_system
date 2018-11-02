using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    using wsg_50 = Messages.wsg_50;

    /// <summary>
    /// Property model for a Weiss Wsg gripper
    /// </summary>
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

    /// <summary>
    /// Implementations of <c>IWeissWsgServices</c> represent the abilities / properties of a Weiss Wsg gripper
    /// </summary>
    public interface IWeissWsgServices
    {
        /// <summary>
        /// Asynchronous acknowledge an error
        /// </summary>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task AcknowledgeError();

        /// <summary>
        /// Asynchronous action to home the gripper
        /// </summary>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task Homing();

        /// <summary>
        /// Asynchronous action to perform stop the gripper
        /// </summary>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task Stop();

        /// <summary>
        /// Asynchronous action to perform a movement
        /// </summary>
        /// <param name="position">Requested position [m]</param>
        /// <param name="speed">Requested speed [m/s]</param>
        /// <param name="force">Force which should maximally applied [N]</param>
        /// <param name="stopOnBlock">If true stop if maximal force is applied</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task Move(double position, double speed, double force, bool stopOnBlock = true);

        /// <summary>
        /// Asynchronous action to perform a grasp
        /// </summary>
        /// <param name="position">Requested position [m]</param>
        /// <param name="speed">Requested speed [m/s]</param>
        /// <param name="force"></param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task Grasp(double position, double speed, double force);

        /// <summary>
        /// Asynchronous action to release the gripper from a grasp
        /// </summary>
        /// <param name="position">Requested position [m]</param>
        /// <param name="speed">Requested speed [m/s]</param>
        /// <returns>Returns a <c>Task</c> instance.</returns>
        Task Release(double position, double speed);

        /// <summary>
        /// Set gripper acceleration
        /// </summary>
        /// <param name="acceleration">Requested acceleration [m/(s^2)]</param>
       /// <returns>Returns a <c>Task</c> instance  producing an <c>int</c> as result.</returns>
        Task<int> SetAcceleration(double acceleration);

        /// <summary>
        /// Get properties
        /// </summary>
        /// <returns>Returns a <c>Task</c> instance  producing an instance of <c>WeissWsgPropertiesModel</c> as result.</returns>
        Task<WeissWsgPropertiesModel> GetProperties();

        /// <summary>
        /// Get status
        /// </summary>
        /// <returns>Returns a <c>Task</c> instance  producing an instance of <c>wsg_50.Status</c> as result.</returns>
        Task<wsg_50.Status> GetStatus();
    }
}
