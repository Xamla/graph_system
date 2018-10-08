using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types
{
    // Xamla Exception Ranges:
    // Internal exceptions:                    <     0
    // Reserved for base exceptions:        0  -    99
    // HttpStatusCode                     100  -   599
    // Proprietäre Statuscodes            900  -   999
    // FileProcessorBase:                1000  -  1999
    // MachineManagement:                2000  -  2999
    // ComputeNodeProcessor:             3000  -  3999
    // ActivityLogProcessor:             4000  -  4999
    // ContainerProcessor:               5000  -  5999
    // UserProcessor:                    6000  -  6999
    // SnapshotProcessor:                7000  -  7999
    // SharedComputeNodeProcessor:       8000  -  8999
    // SessionProcessor                  9000  -  9999
    // PublicProjectsController         10000  - 10999
    // SyncProcsessor                   11000  - 11999
    // FileRepositoryError              12000  - 12999
    // XDataError                       13000  - 13999
    // DatabaseError                    14000  - 14999
    // FileApiError                     15000  - 15999
    // ContainerApiError                16000  - 16999
    // SyncJobApiError                  17000  - 17499
    // SyncClientError                  17500  - 17999
    // NodeManagerError                 18000  - 18999
    // MessageProcessor                 19000  - 19999
    // ManagedMachineProcessor          20000  - 20999
    // SharedKeyAuthError               21000  - 21999
    // CacheRequestError                22000  - 22999
    // SessionApiError                  23000  - 23999
    // UserRepositoryError              24000  - 24999
    // ContingentError                  25000  - 25999
    // AccountApiError                  26000  - 26999
    // FileRepositoryError              27000  - 27999

    public static class XamlaError
    {
        public const int GenericError = -1;

        public const int UserNotFound = 0;
        public const int UserIdIsInvalid = 1;
        public const int UserNotAuthenticated = 2;
        public const int ContainerAccessDenied = 3;
        public const int ContainerNotFound = 4;
        public const int PublicContainerNotFound = 5;
        public const int InvalidParameterValue = 6;
        public const int SnapshotFullFileListNotFound = 7;
        public const int ContainerNameIsInvalid = 9;
        public const int QueueMessageTimeoutUpdateFailed = 10;
        public const int SchemaNotFoundById = 11;
        public const int SchemaNotFoundByName = 12;
        public const int ItemNotFound = 13;
        public const int AccessDenied = 14;
        public const int ArgumentNull = 15;
        public const int ArgumentOutOfRange = 16;
        public const int InvalidOperation = 17;
        public const int InvalidFormat = 18;
        public const int OperationFailed = 19;
    }

    public class XamlaException
        : Exception
    {
        public XamlaException(string message, int xamlaErrorCode)
            : base(message)
        {
            this.HttpStatusCode = HttpStatusCode.InternalServerError;
            this.XamlaErrorCode = xamlaErrorCode;
        }

        public XamlaException(string message, HttpStatusCode httpStatusCode, int xamlaErrorCode)
            : base(message)
        {
            this.HttpStatusCode = httpStatusCode;
            this.XamlaErrorCode = xamlaErrorCode;
        }

        public XamlaException(string message, Exception innerException, int xamlaErrorCode)
            : base(message, innerException)
        {
            this.HttpStatusCode = HttpStatusCode.InternalServerError;
            this.XamlaErrorCode = xamlaErrorCode;
        }

        public HttpStatusCode HttpStatusCode { get; }

        public int XamlaErrorCode { get; }
    }
}
