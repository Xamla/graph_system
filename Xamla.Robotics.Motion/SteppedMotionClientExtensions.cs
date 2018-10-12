using System;
using System.Numerics;
using Xamla.Robotics.Types;
using Uml.Robotics.Ros;

namespace Xamla.Robotics.Motion
{
    public static class SteppedMotionClientExtensions
    {
        public static SteppedMotionClientModel ToModel(this ISteppedMotionClient client)
        {
            var state = client.MotionState;     // read immutable state snapshot once
            return new SteppedMotionClientModel { GoalId = client.GoalId, Progress = state.Progress, ErrorCode = state.ErrorCode, ErrorMessage = state.ErrorMessage };
        }
    }
}