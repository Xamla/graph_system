using System;

namespace Xamla.Graph.Models
{
    public class ExceptionModel
        : NodeModel
    {
        public string ExceptionTypeName { get; set; }
        public string ExceptionFullType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string StringRepresentation { get; set; }

        public ExceptionModel()
            : base("Exception")
        {
            this.Id = "Exception";
        }

        public static ExceptionModel FromException(Exception e)
        {
            if (e == null)
                return null;

            var model = new ExceptionModel()
            {
                ExceptionTypeName = e.GetType().FullName,
                ExceptionFullType = e.GetType().ToString(),
                Message = e.Message,
                StackTrace = e.StackTrace,
                StringRepresentation = e.ToString(),
            };

            if (e is ModuleException moduleException)
            {
                model.Trace = moduleException.ModuleTrace;
                model.ObjectId = moduleException.ModuleObjectId;
            }

            return model;
        }
    }
}
