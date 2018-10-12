using System;
using System.Numerics;
using Xamla.Utilities;

namespace Xamla.Robotics.Types
{
    public class Twist : IEquatable<Twist>
    {
        public string Frame { get; }
        public Vector3 Linear { get; }
        public Vector3 Angular { get; }

        public static readonly Twist Identity = new Twist(Vector3.Zero, Vector3.Zero);

        public Twist()
            : this(new Vector3(), new Vector3())
        {
        }

        public Twist(Vector3 linear, Vector3 angular, string frame = "")
        {
            this.Frame = frame;
            this.Linear = linear;
            this.Angular = angular;
        }

        public bool Equals(Twist other)
        {
            return object.Equals(this.Linear, other.Linear)
                && object.Equals(this.Angular, other.Angular);
        }

        public override int GetHashCode() =>
            HashHelper.GetHashCode(this.Frame, this.Linear, this.Angular);

        public override string ToString() =>
            $"Linear: {this.Linear}; Angular: {this.Angular}; Frame: '{this.Frame}';";
    }
}