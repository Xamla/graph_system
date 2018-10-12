using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;
using System.Collections.Immutable;

namespace Xamla.Robotics.Types
{
    public static class ModelConversionExtensions
    {
        public static double[] ToModel(this Vector3 v) =>
            new double[] { v.X, v.Y, v.Z };

        public static double[] ToModel(this Quaternion q) =>
            new double[] { q.X, q.Y, q.Z, q.W };

        public static MoveGroupDescriptionModel ToModel(this MoveGroupDescription description) =>
            new MoveGroupDescriptionModel
            {
                Name = description.Name,
                JointNames = description.JointSet.ToArray(),
                EndEffectorNames = description.EndEffectorNames,
                SubMoveGroupIds = description.SubMoveGroupIds,
                EndEffectorLinkNames = description.EndEffectorLinkNames,
            };

        public static JointPathModel ToModel(this IJointPath path) =>
            new JointPathModel { JointNames = path.JointSet.ToArray(), Points = path.Select(x => x.Values).ToList() };

        public static IJointPath ToJointPath(this JointPathModel model)
        {
            var jointSet = new JointSet(model.JointNames);
            return new JointPath(jointSet, model.Points.Select(x => new JointValues(jointSet, x)));
        }

        public static JointLimitsModel ToModel(this JointLimits limits) =>
            new JointLimitsModel { JointNames = limits.JointSet.ToArray(), MaxVelocity = limits.MaxVelocity, MaxAcceleration = limits.MaxAcceleration, MinPosition = limits.MinPosition, MaxPosition = limits.MaxPosition };

        public static JointLimits ToJointLimits(this JointLimitsModel model) =>
            new JointLimits(new JointSet(model.JointNames), model.MaxVelocity, model.MaxAcceleration, model.MinPosition, model.MaxPosition);

        public static JointValuesModel ToModel(this JointValues values) =>
            new JointValuesModel { JointNames = values.JointSet.ToArray(), Values = values.Values };

        public static JointValues ToJointValues(this JointValuesModel model) =>
            new JointValues(new JointSet(model.JointNames), model.Values);

        public static JointStatesModel ToModel(this JointStates states) =>
            new JointStatesModel { JointNames = states.JointSet?.ToArray(), Positions = states.Positions?.Values, Velocities = states.Velocities?.Values, Efforts = states.Efforts?.Values };

        public static JointStates ToJointStates(this JointStatesModel model)
        {
            var jointSet = new JointSet(model.JointNames);
            return new JointStates(
                new JointValues(jointSet, model.Positions),
                model.Velocities != null ? new JointValues(jointSet, model.Velocities) : null,
                model.Efforts != null ? new JointValues(jointSet, model.Efforts) : null
            );
        }

        private static JointValues GetJointValues(JointSet jointSet, double[] values) =>
            values != null ? new JointValues(jointSet, values) : null;

        public static JointTrajectoryPoint ToJointTrajectoryPoint(this JointTrajectoryPointModel model, JointSet jointSet) =>
            new JointTrajectoryPoint(model.TimeFromStart, GetJointValues(jointSet, model.Positions), GetJointValues(jointSet, model.Velocities), GetJointValues(jointSet, model.Accelerations), GetJointValues(jointSet, model.Effort));

        public static JointTrajectoryPointModel ToModel(this JointTrajectoryPoint point) =>
            new JointTrajectoryPointModel
            {
                TimeFromStart = point.TimeFromStart,
                Positions = point.Positions?.Values,
                Velocities = point.Velocities?.Values,
                Accelerations = point.Accelerations?.Values,
                Effort = point.Effort?.Values
            };

        public static IJointTrajectory ToJointTrajectory(this JointTrajectoryModel model)
        {
            var jointSet = new JointSet(model.JointNames);
            return new JointTrajectory(jointSet, model.Points.Select(x => x.ToJointTrajectoryPoint(jointSet)), model.IsValid);
        }

        public static JointTrajectoryModel ToModel(this IJointTrajectory trajectory) =>
            new JointTrajectoryModel
            {
                JointNames = trajectory.JointSet.ToArray(),
                Points = trajectory.Select(x => x.ToModel()).ToList(),
                HasAcceleration = trajectory.HasAccelaration,
                HasVelocity = trajectory.HasVelocity,
                HasEffort = trajectory.HasEffort,
                IsValid = trajectory.IsValid
            };

        public static PlanParametersModel ToModel(this PlanParameters parameters) =>
            new PlanParametersModel
            {
                JointNames = parameters.JointSet.ToArray(),
                CollisionCheck = parameters.CollisionCheck,
                MoveGroupName = parameters.MoveGroupName,
                MaxAcceleration = parameters.MaxAcceleration,
                MaxVelocity = parameters.MaxVelocity,
                SampleResolution = parameters.SampleResolution
            };

        public static PlanParameters ToPlanParameters(this PlanParametersModel model) =>
            new PlanParameters(model.MoveGroupName, new JointSet(model.JointNames), model.MaxVelocity, model.MaxAcceleration, model.SampleResolution, model.CollisionCheck);

        public static PoseModel ToModel(this Pose pose) =>
            new PoseModel { Frame = pose.Frame, Translation = pose.Translation.ToModel(), Rotation = pose.Rotation.ToModel() };

        public static Pose ToPose(this PoseModel model, bool normalizeRotation = false)
        {
            if (model.Translation == null || model.Rotation == null)
            {
                return new Pose(
                    new Vector3(float.NaN, float.NaN, float.NaN),
                    new Quaternion(float.NaN, float.NaN, float.NaN, float.NaN),
                    model.Frame
                );
            } else
            {
                return new Pose(
                    new Vector3((float)model.Translation[0], (float)model.Translation[1], (float)model.Translation[2]),
                    new Quaternion((float)model.Rotation[0], (float)model.Rotation[1], (float)model.Rotation[2], (float)model.Rotation[3]),
                    model.Frame,
                    normalizeRotation
                );
            }
        }

        public static TwistModel ToModel(this Twist twist) =>
            new TwistModel { Frame = twist.Frame, Linear = twist.Linear.ToModel(), Angular = twist.Angular.ToModel() };

        public static Twist ToTwist(this TwistModel model) =>
            new Twist(
                new Vector3((float)model.Linear[0], (float)model.Linear[1], (float)model.Linear[2]),
                new Vector3((float)model.Angular[0], (float)model.Angular[1], (float)model.Angular[2]),
                model.Frame
            );

        public static CartesianPathModel ToModel(this ICartesianPath path) =>
            new CartesianPathModel { Points = path.Select(x => x.ToModel()).ToList() };

        public static ICartesianPath ToCartesianPath(this CartesianPathModel model) =>
            new CartesianPath(model.Points.Select(x => x.ToPose()));

        public static IKResultModel ToModel(this IKResult result) =>
            new IKResultModel {
                Suceeded = result.Suceeded,
                Path = result.Path?.ToModel(),
                ErrorCodes = result.ErrorCodes.Cast<int>().ToArray(),
                Errors = result.ErrorCodes.Select(x => x.ToString()).ToArray()
            };

        public static JointValuesCollisionModel ToModel(this JointValuesCollision obj) =>
            new JointValuesCollisionModel { Index = obj.Index, ErrorCode = obj.ErrorCode, Message = obj.Message };

        public static CollisionPrimitiveModel ToModel(this ICollisionPrimitive obj) =>
            new CollisionPrimitiveModel { Kind = obj.Kind, Parameters = obj.Parameters, Pose = obj.Pose.ToModel() };

        public static ICollisionPrimitive ToCollisionPrimitive(this CollisionPrimitiveModel model) =>
            new CollisionPrimitive(model.Kind, model.Parameters, model.Pose.ToPose(true));

        public static MoveGripperResultModel ToModel(this MoveGripperResult obj) =>
            new MoveGripperResultModel { Position = obj.Position, ReachedGoal = obj.ReachedGoal, Stalled = obj.Stalled };

        public static MoveGripperResult ToMoveGripperResult(this MoveGripperResultModel model) =>
            new MoveGripperResult(position: model.Position, reachedGoal: model.ReachedGoal, stalled: model.Stalled);

        public static WsgResultModel ToModel(this WsgResult obj) =>
            new WsgResultModel { State = obj.State, Force = obj.Force, Width = obj.Width, Status = obj.Status };

        public static WsgResult ToWsgResult(this WsgResultModel model) =>
            new WsgResult(model.State, model.Width, model.Force, model.Status);

        public static ICollisionObject ToCollisionObject(this CollisionObjectModel model) =>
            new CollisionObject(model.Frame, model.Primitives.Select(x => x.ToCollisionPrimitive()).ToImmutableList());

        public static CollisionObjectModel ToModel(this ICollisionObject obj) =>
            new CollisionObjectModel { Frame = obj.Frame, Primitives = obj.Primitives.Select(x => x.ToModel()).ToList() };
    }
}
