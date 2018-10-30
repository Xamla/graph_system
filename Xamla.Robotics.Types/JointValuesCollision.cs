namespace Xamla.Robotics.Types
{
    public class JointValuesCollision
    {
        public JointValuesCollision(int index, string message, int errorCode)
        {
            this.Index = index;
            this.Message = message;
            this.ErrorCode = errorCode;
        }

        public int Index { get; }
        public string Message { get; }
        public int ErrorCode { get; }
    }
}
