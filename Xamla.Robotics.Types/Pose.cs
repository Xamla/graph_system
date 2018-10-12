﻿using System;
using System.Numerics;
using Xamla.Utilities;

namespace Xamla.Robotics.Types
{
    /// <summary>
    /// A <c>Pose</c> contains the translation (a three dimensional vector) and the rotation (a quaternion) in a certain coordinate system defined by the name of a ROS TF frame. By default a <c>Pose</c> uses the world coordinate system as base, i.e. the identity <c>Pose</c>.
    /// </summary>
    /// <remarks>
    /// This class offers basic operations, for comparison, translating and rotation normalization.
    /// </remarks>
    /// <see href="http://wiki.ros.org/Papers/TePRA2013_Foote?action=AttachFile&do=view&target=TePRA2013_Foote.pdf">In depth information about ROS TF</see>
    public class Pose : IEquatable<Pose>
    {
        /// <summary>
        /// Default <c>Pose</c> with translation set to the zero vector and rotation set to the identity quaternion.
        /// </summary>
        public static readonly Pose Identity = new Pose(Vector3.Zero, Quaternion.Identity);

        public Pose()
            : this(new Vector3(), new Quaternion())
        {
        }

        /// <summary>
        /// Creates a new <c>Pose</c> from a translation vector and a rotation quaternion.
        /// </summary>
        /// <param name="translation">Translation in X, Y and Z axis in meters.</param>
        /// <param name="rotation">Rotation as quaternion.</param>
        /// <param name="frame">Name of the ROS TF parent frame. Default: empty string.</param>
        /// <param name="normalizeRotation">If true the rotation quaternion gets normalized.</param>
        public Pose(Vector3 translation, Quaternion rotation, string frame = "", bool normalizeRotation = false)
        {
            this.Frame = frame;
            this.Translation = translation;
            this.Rotation = normalizeRotation ? NormalizeQuaternion(rotation) : rotation;
        }

        /// <summary>
        /// Creates a new <c>Pose</c> from a transformation matrix.
        /// </summary>
        /// <param name="transformMatrix">4x4 transformation matrix from which translation and rotation is derived.</param>
        /// <param name="frame">Name of the ROS TF parent frame. Default: empty string.</param>
        public Pose(Matrix4x4 transformMatrix, string frame = "")
            : this(
                new Vector3(transformMatrix.M41, transformMatrix.M42, transformMatrix.M43),
                Quaternion.CreateFromRotationMatrix(transformMatrix),
                frame
            )
        {
        }

        /// <summary>
        /// The name of the parent ROS TF Frame, which is used as origin for the <c>Pose</c>.
        /// </summary>
        /// <remarks>If null or empty the world frame (i.e. identity) is used.</remarks>
        public string Frame { get; }

        /// <summary>
        /// The translation in X, Y and Z axis in meters.
        /// </summary>
        public Vector3 Translation { get; }

        /// <summary>
        /// The rotation as quaternion.
        /// </summary>
        public Quaternion Rotation { get; }

        /// <summary>
        /// A 4x4 matrix containing only the rotation component of the transformation matrix.
        /// </summary>
        public Matrix4x4 RotationMatrix =>
            Matrix4x4.CreateFromQuaternion(this.Rotation);

        /// <summary>
        /// A 4x4 matrix containing rotation and translation
        /// </summary>
        public Matrix4x4 TransformMatrix
        {
            get
            {
                var result = Matrix4x4.CreateFromQuaternion(this.Rotation);
                result.Translation = this.Translation;
                return result;
            }
        }

        /// <summary>
        /// Creates a new <c>Pose</c> translated in the axes x, y and z.
        /// </summary>
        /// <param name="x">Translation in X in meters.</param>
        /// <param name="y">Translation in Y in meters.</param>
        /// <param name="z">Translation in Z in meters.</param>
        /// <returns>A new <c>Pose</c></returns>
        public Pose Translate(float x, float y, float z) =>
            Translate(new Vector3(x, y, z));

        /// <summary>
        /// Creates a new <c>Pose</c> translated by the given vector.
        /// </summary>
        /// <param name="offset">Translation in X, Y and Z given as <c>Vector3</c> in meters.</param>
        /// <returns>A new <c>Pose</c></returns>
        public Pose Translate(Vector3 offset) =>
            new Pose(this.Translation + offset, this.Rotation, this.Frame);

        /// <summary>
        /// Create a new <c>Pose</c> with the current rotation quaternion normalized.
        /// </summary>
        /// <returns>A new <c>Pose</c></returns>
        public Pose NormalizeRotation() =>
            new Pose(this.Translation, NormalizeQuaternion(this.Rotation), this.Frame);

        /// <summary>
        /// Test if the current Pose equals the given other <c>Pose</c>.
        /// </summary>
        /// <param name="other">Another <c>Pose</c> that should be tested for similarity.</param>
        /// <returns>True if the two <c>Poses</c> are the same object or if their values are equal. False otherwise.</returns>
        public bool Equals(Pose other)
        {
            if (other == null)
                return false;
            if (Object.ReferenceEquals(this, other))
                return true;
            return object.Equals(this.Frame, other.Frame)
                && object.Equals(this.Translation, other.Translation)
                && object.Equals(this.Rotation, other.Rotation);
        }

        /// <summary>
        /// Test if the current Pose equals the given other <c>object</c>.
        /// </summary>
        /// <param name="other">Another <c>object</c> that should be tested for similarity.</param>
        /// <returns>True if the <c>Poses</c> and the given <c> object</c>are the same object or if their values are equal. False otherwise.</returns>
        public override bool Equals(object obj) =>
            Equals(obj as Pose);

        /// <summary>
        /// Creates a hash code over Frame, Translation and Rotation.
        /// </summary>
        public override int GetHashCode() =>
            HashHelper.GetHashCode(this.Frame, this.Translation, this.Rotation);

        /// <summary>Creates a human readable text representation of Translation, Rotation and Frame.</summary>
        public override string ToString() =>
            $"translation: {this.Translation}; Rotation: {this.Rotation}; Frame: '{this.Frame}';";

        private static Quaternion NormalizeQuaternion(Quaternion q)
        {
            q = Quaternion.Normalize(q);
            if (q.W < 0)
                q = Quaternion.Negate(q);
            return q;
        }
    }
}
