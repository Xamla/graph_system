using System;
using System.Collections.Generic;
using System.Text;

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
