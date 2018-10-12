using System;
using System.Collections.Generic;
using System.Text;

namespace Xamla.Robotics.Types
{
    public class ControllerStatusModel
    {
        public bool IsRunning { get; set; }
        public string InTopic { get; set; }
        public string OutTopic { get; set; }
        public string MoveGroupName { get; set; }
        public string[] JointNames { get; set; }
        public string StatusMessage { get; set; }
    }

    public class MoveGroupDescriptionModel
    {
        public string Name { get; set; }
        public string[] SubMoveGroupIds { get; set; }
        public string[] JointNames { get; set; }
        public string[] EndEffectorNames { get; set; }
        public string[] EndEffectorLinkNames { get; set; }
    }

    public class JointPathModel
    {
        public string[] JointNames { get; set; }
        public List<double[]> Points { get; set; }
    }

    public class JointLimitsModel
    {
        public string[] JointNames { get; set; }
        public double?[] MaxAcceleration { get; set; }
        public double?[] MaxVelocity { get; set; }
        public double?[] MinPosition { get; set; }
        public double?[] MaxPosition { get; set; }
    }

    public class JointValuesModel
    {
        public string[] JointNames { get; set; }
        public double[] Values { get; set; }
    }

    public class JointValuesCollisionModel
    {
        public int Index { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }

    public class JointStatesModel
    {
        public string[] JointNames { get; set; }
        public double[] Positions { get; set; }
        public double[] Velocities { get; set; }
        public double[] Efforts { get; set; }
    }

    public class JointTrajectoryPointModel
    {
        public TimeSpan TimeFromStart { get; set; }
        public double[] Positions { get; set; }
        public double[] Velocities { get; set; }
        public double[] Accelerations { get; set; }
        public double[] Effort { get; set; }
    }

    public class JointTrajectoryModel
    {
        public string[] JointNames { get; set; }
        public List<JointTrajectoryPointModel> Points { get; set; }
        public bool HasVelocity { get; set; }
        public bool HasAcceleration { get; set; }
        public bool HasEffort { get; set; }
        public bool IsValid { get; set; }
    }

    public class PlanParametersModel
    {
        public string MoveGroupName { get; set; }
        public string[] JointNames { get; set; }
        public double[] MaxVelocity { get; set; }
        public double[] MaxAcceleration { get; set; }
        public bool CollisionCheck { get; set; } = true;
        public double SampleResolution { get; set; } = 0.008;
    }

    public class PoseModel
    {
        public string Frame { get; set; }
        public double[] Translation { get; set; }
        public double[] Rotation { get; set; }

        public bool Equals(PoseModel other)
        {
            if (other == null ||
                this.Translation.Length != other.Translation.Length ||
                this.Rotation.Length != other.Rotation.Length ||
                this.Frame != other.Frame)
            {
                return false;
            }

            var result = true;
            for (var i = 0; i < this.Translation.Length; i += 1)
            {
                result = result && this.Translation[i] == other.Translation[i];
            }
            for (var i = 0; i < this.Rotation.Length; i += 1)
            {
                result = result && this.Rotation[i] == other.Rotation[i];
            }

            return result;
        }
    }

    public class TwistModel
    {
        public string Frame { get; set; }
        public double[] Linear { get; set; }
        public double[] Angular { get; set; }
    }

    public class CartesianPathModel
    {
        public List<PoseModel> Points { get; set; }
    }

    public class IKResultModel
    {
        public bool Suceeded { get; set; }
        public JointPathModel Path { get; set; }
        public string[] Errors { get; set; }
        public int[] ErrorCodes { get; set; }
    }

    public class JointValuesCollision
    {
        public int Index { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }

    public class CollisionPrimitiveModel
    {
        public CollisionPrimitiveKind Kind { get; set; }
        public double[] Parameters { get; set; }
        public PoseModel Pose { get; set; }
    }

    public class CollisionObjectModel
    {
        public string Frame { get; set; }
        public List<CollisionPrimitiveModel> Primitives { get; set; }
    }

    public class MoveGripperResultModel
    {
        public double Position { get; set; }
        public bool ReachedGoal { get; set; }
        public bool Stalled { get; set; }
    }

    public class WsgResultModel
    {
        public int State { get; set; }
        public double Width { get; set; }
        public double Force { get; set; }
        public string Status { get; set; }
    }

    public class SteppedMotionClientModel
    {
        public string GoalId { get; set; }
        public double Progress { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
