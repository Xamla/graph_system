using System;
using System.Numerics;
using Xamla.Robotics.Types;
using Uml.Robotics.Ros;

namespace Xamla.Robotics.Motion
{
    /// <summary>
    /// Extensions of the <c>ISteppedMotionClient</c> interface
    /// </summary>
    public static class SteppedMotionClientExtensions
    {
        /// <summary>
        /// Extends the interface <c>ISteppedMotionClient</c> with a method <c>ToModel</c>, which creates an instance of <c>SteppedMotionClientModel</c> from a state snapshot.
        /// </summary>
        /// <param name="client">The client to be extended</param>
        /// <returns>Returns an instance of  <c>SteppedMotionClientModel</c>.</returns>
        public static SteppedMotionClientModel ToModel(this ISteppedMotionClient client)
        {
            var state = client.MotionState;     // read immutable state snapshot once
            return new SteppedMotionClientModel { GoalId = client.GoalId, Progress = state.Progress, ErrorCode = state.ErrorCode, ErrorMessage = state.ErrorMessage };
        }
    }
}