using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamla.Robotics.Types;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Implementations of <c>IMoveGroup</c> encapsulate move functionality.
    /// </summary> 
    public interface IMoveGroup : IDisposable
    {
        /// <summary>
        /// Name of the <c>IMoveGroup</c>.
        /// </summary> 
        string Name { get; }
        /// <summary>
        ///  An instance of <c>IMotionService</c>,  which is used to communicate with the motion server.
        /// </summary> 
        /// <remarks>
        /// Used to communicate with the motion server.
        /// </remarks> 
        IMotionService MotionService { get; }
        /// <summary>
        /// The current velocity scaling.
        /// </summary> 
        double VelocityScaling { get; set; }
        /// <summary>
        /// Sets the collision check.
        /// </summary> 
        /// <remarks>
        /// If true the trajectory planing tries to plan a collision free trajectory. Before executing a trajectory, a collision check is performed.
        /// </remarks> 
        bool CollisionCheck { get; set; }
        /// <summary>
        /// Trajectory point sampling frequency.
        /// </summary> 
        double SampleResolution { get; set; }
        /// <summary>
        /// Defines the maximal deviation from the trajectory points.
        /// </summary>         
        double MaxDeviation { get; set; }
        /// <summary>
        /// Maximal allowed inverse kinematics jump threshold.
        /// </summary>         
        double IkJumpThreshold { get; set; }
        /// <summary>
        /// List of endeffector names the move group contains.
        /// </summary>   
        IEnumerable<string> EndEffectorNames { get; }
        /// <summary>
        /// The default end effector name.
        /// </summary>  
        string DefaultEndEffectorName { get; set; }
        /// <summary>
        /// Get an <c>IEndEffector</c> instance. 
        /// </summary>  
        /// <param name="endEffectorName">Name of the <c>IEndEffector</c> instance. The default end effector is used when null.</param>  
        /// <returns>Returns the <c>IEndEffector</c> instance referenced by  <c>endEffectorName</c>.</returns>  
        IEndEffector GetEndEffector(string endEffectorName = null);
        /// <summary>
        /// The default end effector.
        /// </summary>  
        IEndEffector DefaultEndEffector { get; }
        /// <summary>
        /// Get the current <c>Pose</c> of and end effector.  
        /// </summary>  
        /// <param name="endEffectorName">Name of the <c>IEndEffector</c> instance. The default end effector is used when null.</param>  
        /// <returns>Returns the current <c>Pose</c> of the end effector referenced by  <c>endEffectorName</c>.</returns>  
        Pose GetCurrentPose(string endEffectorName = null);
        /// <summary>
        /// Joint set of the <c>IMoveGroup</c>.  
        /// </summary> 
        JointSet JointSet { get; }
        /// <summary>
        /// The current <c>JointStates</c> of the of the <c>IMoveGroup</c> joints.  
        /// </summary>  
        JointStates CurrentJointStates { get; }
        /// <summary>
        /// The current joint positions of the of the <c>IMoveGroup</c> joints.  
        /// </summary>  
        JointValues CurrentJointPositions { get; }
        /// <summary>
        /// Default instance of <c>PlanParameters</c>. 
        /// </summary>  
        PlanParameters DefaultPlanParameters { get; }
        /// <summary>
        /// Builds an instance of <c>PlanParameters</c>. 
        /// </summary>  
        /// <remarks>
        /// For parameter == null, the default values in DefaultPlanParameters are used. 
        /// </remarks> 
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="collisionCheck">If collision check is activated.</param>  
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space.</param>  
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>  
        /// <returns>Returns a new <c>PlanParameters</c> instance based on <c>DefaultPlanParameters</c> and parameters.</returns>  
        PlanParameters BuildPlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null);
        /// <summary>
        /// Builds an instance of <c>TaskSpacePlanParameters</c>. 
        /// </summary>  
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="collisionCheck">If collision check is activated.</param>  
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space.</param>  
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>  
        /// <param name="endEffectorName">Name of the end effector.</param>  
        /// <returns>Returns a new <c>TaskSpacePlanParameters</c> instance.</returns>  
        TaskSpacePlanParameters BuildTaskSpacePlanParameters(double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, string endEffectorName = null);
        /// <summary>
        /// Plans a collision free trajectory given <c>JointValues</c>.
        /// </summary>  
        /// <remarks>
        /// Asynchronous.
        /// </remarks>
        /// <param name="target">Target joint positions.</param>  
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="sampleResolution">Trajectory point sampling frequency.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>  
        (IJointTrajectory, PlanParameters) PlanMoveJointsCollisionFreeAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Plans a trajectory given <c>JointValues</c>.
        /// </summary>  
        /// <param name="target">Target joint positions.</param>  
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="collisionCheck">If collision check is activated.</param>  
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space.</param>  
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>  
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>  
        (IJointTrajectory, PlanParameters) PlanMoveJoints(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null);
        /// <summary>
        /// Moves the joints given <c>JointValues</c>.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="target">Target joint positions.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>Task</c> instance.</returns>  
        Task MoveJointsAsync(JointValues target, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>JointValues</c>.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="target">Target joint positions.</param>  
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="collisionCheck">If collision check is activated.</param>  
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>Task</c> instance.</returns>  
        Task MoveJointsAsync(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>JointValues</c> collision free.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="target">Target joint positions.</param>  
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="sampleResolution">Trajectory point sampling frequency.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>Task</c> instance.</returns>  
        Task MoveJointsCollisionFreeAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Plans a trajectory given <c>IJointPath</c>.
        /// </summary>  
        /// <param name="waypoints">Joint path.</param>  
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="collisionCheck">If collision check is activated.</param>  
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space.</param>  
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>  
        /// <returns>Returns a tuple of <c>(IJointTrajectory, PlanParameters)</c> instances.</returns>  
        (IJointTrajectory, PlanParameters) PlanMoveJointPath(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null);
        /// <summary>
        /// Moves the joints given <c>IJointPath</c>.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="waypoints">Joint path.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>Task</c> instance.</returns>  
        Task MoveJointPathAsync(IJointPath waypoints, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>IJointPath</c>.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="waypoints">Joint path.</param>  
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="collisionCheck">If collision check is activated.</param>  
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space.</param>  
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>Task</c> instance.</returns>  
        Task MoveJointPathAsync(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>IJointPath</c>.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="waypoints">Joint path.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>ISteppedMotionClient</c> instance.</returns>  
        ISteppedMotionClient MoveJointPathSupervisedAsync(IJointPath waypoints, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>IJointPath</c>.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="waypoints">Joint path.</param>  
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="collisionCheck">If collision check is activated.</param>  
        /// <param name="maxDeviation">Maximal deviation from trajectory points when it is a fly-by-point in joint space.</param>  
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>ISteppedMotionClient</c> instance.</returns>  
        ISteppedMotionClient MoveJointPathSupervisedAsync(IJointPath waypoints, double? velocityScaling = null, bool? collisionCheck = null, double? maxDeviation = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>JointValues</c>.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="target">Target joint positions.</param>
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>ISteppedMotionClient</c> instance.</returns>  
        ISteppedMotionClient MoveJointsSupervisedAsync(JointValues target, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>JointValues</c>.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="target">Target joint positions.</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="collisionCheck">If collision check is activated.</param>  
        /// <param name="accelerationScaling">Scaling factor which is applied on the maximal possible joint accelerations.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>ISteppedMotionClient</c> instance.</returns>  
        ISteppedMotionClient MoveJointsSupervisedAsync(JointValues target, double? velocityScaling = null, bool? collisionCheck = null, double? accelerationScaling = null, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>JointValues</c> collision free.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="target">Target joint positions.</param>
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>ISteppedMotionClient</c> instance.</returns>  
        ISteppedMotionClient MoveJointsCollisionFreeSupervisedAsync(JointValues target, CancellationToken cancel = default(CancellationToken));
        /// <summary>
        /// Moves the joints given <c>JointValues</c> collision free.
        /// </summary> 
        /// <remarks>
        /// Asynchronous.
        /// </remarks> 
        /// <param name="target">Target joint positions.</param>
        /// <param name="velocityScaling">Scaling factor which is applied on the maximal possible joint velocities.</param>  
        /// <param name="sampleResolution">Trajectory point sampling frequency.</param>  
        /// <param name="cancel">CancellationToken.</param>  
        /// <returns>Returns a <c>ISteppedMotionClient</c> instance.</returns>  
        ISteppedMotionClient MoveJointsCollisionFreeSupervisedAsync(JointValues target, double? velocityScaling, double? sampleResolution, CancellationToken cancel = default(CancellationToken));
    }
}
