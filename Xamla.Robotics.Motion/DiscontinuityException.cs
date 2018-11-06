using System;

namespace Xamla.Robotics.Motion
{
    public class DiscontinuityException
        : Exception
    {
        public DiscontinuityException(string message)
            : base(message)
        {
        }
    }
}
