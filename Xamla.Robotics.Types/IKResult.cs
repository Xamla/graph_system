using System.Linq;
using System.Collections.Generic;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// Holds the result of a request to the inverse kinematic solver.
    /// </summary>
    public class IKResult
    {
        /// <summary>
        /// Creates a new instance of <c>IKResult</c>.
        /// </summary>
        /// <param name="path">The collection of found solutions</param>
        /// <param name="errorCodes">The list of errors that might have occurred</param>
        public IKResult(IJointPath path, IList<MoveItErrorCode> errorCodes)
        {
            this.Path = path;
            this.ErrorCodes = errorCodes;
        }

        /// <summary>
        /// Gets the collection of found solutions for the inverse kinematic request.
        /// </summary>
        public IJointPath Path { get; }

        /// <summary>
        /// Gets the collection of error codes that might have occurred during the inverse kinematic request.
        /// </summary>
        public IList<MoveItErrorCode> ErrorCodes { get; }

        /// <summary>
        /// Indicates whether all solutions were found for the batch inverse kinematic request.
        /// </summary>
        public bool Succeeded => this.ErrorCodes != null && this.ErrorCodes.All(errorCode => errorCode == MoveItErrorCode.SUCCESS);
    }
}
