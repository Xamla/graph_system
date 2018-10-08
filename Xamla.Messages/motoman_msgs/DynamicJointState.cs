using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using uint8 = System.Byte;
using Uml.Robotics.Ros;
using Messages.geometry_msgs;
using Messages.sensor_msgs;
using Messages.actionlib_msgs;

using Messages.std_msgs;
using String=System.String;

namespace Messages.motoman_msgs
{
    public class DynamicJointState : RosMessage
    {

			public short group_number;
			public short num_joints;
			public short valid_fields;
			public double[] positions;
			public double[] velocities;
			public double[] accelerations;
			public double[] effort;
			public double[] positions_desired;
			public double[] positions_errors;
			public double[] velocities_desired;
			public double[] velocities_errors;
			public double[] accelerations_desired;
			public double[] accelerations_errors;
			public double[] effort_errors;
			public double[] effort_desired;


        public override string MD5Sum() { return "c44649b8de969b98f15adea419fa49a4"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"int16 group_number
int16 num_joints
int16 valid_fields
float64[] positions
float64[] velocities
float64[] accelerations
float64[] effort
float64[] positions_desired
float64[] positions_errors
float64[] velocities_desired
float64[] velocities_errors
float64[] accelerations_desired
float64[] accelerations_errors
float64[] effort_errors
float64[] effort_desired"; }
        public override string MessageType { get { return "motoman_msgs/DynamicJointState"; } }
        public override bool IsServiceComponent() { return false; }

        public DynamicJointState()
        {
            
        }

        public DynamicJointState(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public DynamicJointState(byte[] serializedMessage, ref int currentIndex)
        {
            Deserialize(serializedMessage, ref currentIndex);
        }



        public override void Deserialize(byte[] serializedMessage, ref int currentIndex)
        {
            int arraylength = -1;
            bool hasmetacomponents = false;
            object __thing;
            int piecesize = 0;
            byte[] thischunk, scratch1, scratch2;
            IntPtr h;
            
            //group_number
            piecesize = Marshal.SizeOf(typeof(short));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            group_number = (short)Marshal.PtrToStructure(h, typeof(short));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //num_joints
            piecesize = Marshal.SizeOf(typeof(short));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            num_joints = (short)Marshal.PtrToStructure(h, typeof(short));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //valid_fields
            piecesize = Marshal.SizeOf(typeof(short));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            valid_fields = (short)Marshal.PtrToStructure(h, typeof(short));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //positions
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (positions == null)
                positions = new double[arraylength];
            else
                Array.Resize(ref positions, arraylength);
// Start Xamla
                //positions
                piecesize = Marshal.SizeOf(typeof(double)) * positions.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, positions, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //velocities
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (velocities == null)
                velocities = new double[arraylength];
            else
                Array.Resize(ref velocities, arraylength);
// Start Xamla
                //velocities
                piecesize = Marshal.SizeOf(typeof(double)) * velocities.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, velocities, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //accelerations
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (accelerations == null)
                accelerations = new double[arraylength];
            else
                Array.Resize(ref accelerations, arraylength);
// Start Xamla
                //accelerations
                piecesize = Marshal.SizeOf(typeof(double)) * accelerations.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, accelerations, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //effort
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (effort == null)
                effort = new double[arraylength];
            else
                Array.Resize(ref effort, arraylength);
// Start Xamla
                //effort
                piecesize = Marshal.SizeOf(typeof(double)) * effort.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, effort, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //positions_desired
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (positions_desired == null)
                positions_desired = new double[arraylength];
            else
                Array.Resize(ref positions_desired, arraylength);
// Start Xamla
                //positions_desired
                piecesize = Marshal.SizeOf(typeof(double)) * positions_desired.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, positions_desired, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //positions_errors
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (positions_errors == null)
                positions_errors = new double[arraylength];
            else
                Array.Resize(ref positions_errors, arraylength);
// Start Xamla
                //positions_errors
                piecesize = Marshal.SizeOf(typeof(double)) * positions_errors.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, positions_errors, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //velocities_desired
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (velocities_desired == null)
                velocities_desired = new double[arraylength];
            else
                Array.Resize(ref velocities_desired, arraylength);
// Start Xamla
                //velocities_desired
                piecesize = Marshal.SizeOf(typeof(double)) * velocities_desired.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, velocities_desired, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //velocities_errors
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (velocities_errors == null)
                velocities_errors = new double[arraylength];
            else
                Array.Resize(ref velocities_errors, arraylength);
// Start Xamla
                //velocities_errors
                piecesize = Marshal.SizeOf(typeof(double)) * velocities_errors.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, velocities_errors, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //accelerations_desired
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (accelerations_desired == null)
                accelerations_desired = new double[arraylength];
            else
                Array.Resize(ref accelerations_desired, arraylength);
// Start Xamla
                //accelerations_desired
                piecesize = Marshal.SizeOf(typeof(double)) * accelerations_desired.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, accelerations_desired, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //accelerations_errors
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (accelerations_errors == null)
                accelerations_errors = new double[arraylength];
            else
                Array.Resize(ref accelerations_errors, arraylength);
// Start Xamla
                //accelerations_errors
                piecesize = Marshal.SizeOf(typeof(double)) * accelerations_errors.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, accelerations_errors, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //effort_errors
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (effort_errors == null)
                effort_errors = new double[arraylength];
            else
                Array.Resize(ref effort_errors, arraylength);
// Start Xamla
                //effort_errors
                piecesize = Marshal.SizeOf(typeof(double)) * effort_errors.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, effort_errors, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //effort_desired
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (effort_desired == null)
                effort_desired = new double[arraylength];
            else
                Array.Resize(ref effort_desired, arraylength);
// Start Xamla
                //effort_desired
                piecesize = Marshal.SizeOf(typeof(double)) * effort_desired.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, effort_desired, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

        }

        public override byte[] Serialize(bool partofsomethingelse)
        {
            int currentIndex=0, length=0;
            bool hasmetacomponents = false;
            byte[] thischunk, scratch1, scratch2;
            List<byte[]> pieces = new List<byte[]>();
            GCHandle h;
            IntPtr ptr;
            int x__size;
            
            //group_number
            scratch1 = new byte[Marshal.SizeOf(typeof(short))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(group_number, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //num_joints
            scratch1 = new byte[Marshal.SizeOf(typeof(short))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(num_joints, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //valid_fields
            scratch1 = new byte[Marshal.SizeOf(typeof(short))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(valid_fields, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //positions
            hasmetacomponents |= false;
            if (positions == null)
                positions = new double[0];
            pieces.Add(BitConverter.GetBytes(positions.Length));
// Start Xamla
                //positions
                x__size = Marshal.SizeOf(typeof(double)) * positions.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(positions, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //velocities
            hasmetacomponents |= false;
            if (velocities == null)
                velocities = new double[0];
            pieces.Add(BitConverter.GetBytes(velocities.Length));
// Start Xamla
                //velocities
                x__size = Marshal.SizeOf(typeof(double)) * velocities.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(velocities, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //accelerations
            hasmetacomponents |= false;
            if (accelerations == null)
                accelerations = new double[0];
            pieces.Add(BitConverter.GetBytes(accelerations.Length));
// Start Xamla
                //accelerations
                x__size = Marshal.SizeOf(typeof(double)) * accelerations.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(accelerations, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //effort
            hasmetacomponents |= false;
            if (effort == null)
                effort = new double[0];
            pieces.Add(BitConverter.GetBytes(effort.Length));
// Start Xamla
                //effort
                x__size = Marshal.SizeOf(typeof(double)) * effort.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(effort, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //positions_desired
            hasmetacomponents |= false;
            if (positions_desired == null)
                positions_desired = new double[0];
            pieces.Add(BitConverter.GetBytes(positions_desired.Length));
// Start Xamla
                //positions_desired
                x__size = Marshal.SizeOf(typeof(double)) * positions_desired.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(positions_desired, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //positions_errors
            hasmetacomponents |= false;
            if (positions_errors == null)
                positions_errors = new double[0];
            pieces.Add(BitConverter.GetBytes(positions_errors.Length));
// Start Xamla
                //positions_errors
                x__size = Marshal.SizeOf(typeof(double)) * positions_errors.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(positions_errors, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //velocities_desired
            hasmetacomponents |= false;
            if (velocities_desired == null)
                velocities_desired = new double[0];
            pieces.Add(BitConverter.GetBytes(velocities_desired.Length));
// Start Xamla
                //velocities_desired
                x__size = Marshal.SizeOf(typeof(double)) * velocities_desired.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(velocities_desired, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //velocities_errors
            hasmetacomponents |= false;
            if (velocities_errors == null)
                velocities_errors = new double[0];
            pieces.Add(BitConverter.GetBytes(velocities_errors.Length));
// Start Xamla
                //velocities_errors
                x__size = Marshal.SizeOf(typeof(double)) * velocities_errors.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(velocities_errors, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //accelerations_desired
            hasmetacomponents |= false;
            if (accelerations_desired == null)
                accelerations_desired = new double[0];
            pieces.Add(BitConverter.GetBytes(accelerations_desired.Length));
// Start Xamla
                //accelerations_desired
                x__size = Marshal.SizeOf(typeof(double)) * accelerations_desired.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(accelerations_desired, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //accelerations_errors
            hasmetacomponents |= false;
            if (accelerations_errors == null)
                accelerations_errors = new double[0];
            pieces.Add(BitConverter.GetBytes(accelerations_errors.Length));
// Start Xamla
                //accelerations_errors
                x__size = Marshal.SizeOf(typeof(double)) * accelerations_errors.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(accelerations_errors, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //effort_errors
            hasmetacomponents |= false;
            if (effort_errors == null)
                effort_errors = new double[0];
            pieces.Add(BitConverter.GetBytes(effort_errors.Length));
// Start Xamla
                //effort_errors
                x__size = Marshal.SizeOf(typeof(double)) * effort_errors.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(effort_errors, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //effort_desired
            hasmetacomponents |= false;
            if (effort_desired == null)
                effort_desired = new double[0];
            pieces.Add(BitConverter.GetBytes(effort_desired.Length));
// Start Xamla
                //effort_desired
                x__size = Marshal.SizeOf(typeof(double)) * effort_desired.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(effort_desired, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            // combine every array in pieces into one array and return it
            int __a_b__f = pieces.Sum((__a_b__c)=>__a_b__c.Length);
            int __a_b__e=0;
            byte[] __a_b__d = new byte[__a_b__f];
            foreach(var __p__ in pieces)
            {
                Array.Copy(__p__,0,__a_b__d,__a_b__e,__p__.Length);
                __a_b__e += __p__.Length;
            }
            return __a_b__d;
        }

        public override void Randomize()
        {
            int arraylength = -1;
            Random rand = new Random();
            int strlength;
            byte[] strbuf, myByte;
            
            //group_number
            group_number = (System.Int16)rand.Next(System.Int16.MaxValue + 1);
            //num_joints
            num_joints = (System.Int16)rand.Next(System.Int16.MaxValue + 1);
            //valid_fields
            valid_fields = (System.Int16)rand.Next(System.Int16.MaxValue + 1);
            //positions
            arraylength = rand.Next(10);
            if (positions == null)
                positions = new double[arraylength];
            else
                Array.Resize(ref positions, arraylength);
            for (int i=0;i<positions.Length; i++) {
                //positions[i]
                positions[i] = (rand.Next() + rand.NextDouble());
            }
            //velocities
            arraylength = rand.Next(10);
            if (velocities == null)
                velocities = new double[arraylength];
            else
                Array.Resize(ref velocities, arraylength);
            for (int i=0;i<velocities.Length; i++) {
                //velocities[i]
                velocities[i] = (rand.Next() + rand.NextDouble());
            }
            //accelerations
            arraylength = rand.Next(10);
            if (accelerations == null)
                accelerations = new double[arraylength];
            else
                Array.Resize(ref accelerations, arraylength);
            for (int i=0;i<accelerations.Length; i++) {
                //accelerations[i]
                accelerations[i] = (rand.Next() + rand.NextDouble());
            }
            //effort
            arraylength = rand.Next(10);
            if (effort == null)
                effort = new double[arraylength];
            else
                Array.Resize(ref effort, arraylength);
            for (int i=0;i<effort.Length; i++) {
                //effort[i]
                effort[i] = (rand.Next() + rand.NextDouble());
            }
            //positions_desired
            arraylength = rand.Next(10);
            if (positions_desired == null)
                positions_desired = new double[arraylength];
            else
                Array.Resize(ref positions_desired, arraylength);
            for (int i=0;i<positions_desired.Length; i++) {
                //positions_desired[i]
                positions_desired[i] = (rand.Next() + rand.NextDouble());
            }
            //positions_errors
            arraylength = rand.Next(10);
            if (positions_errors == null)
                positions_errors = new double[arraylength];
            else
                Array.Resize(ref positions_errors, arraylength);
            for (int i=0;i<positions_errors.Length; i++) {
                //positions_errors[i]
                positions_errors[i] = (rand.Next() + rand.NextDouble());
            }
            //velocities_desired
            arraylength = rand.Next(10);
            if (velocities_desired == null)
                velocities_desired = new double[arraylength];
            else
                Array.Resize(ref velocities_desired, arraylength);
            for (int i=0;i<velocities_desired.Length; i++) {
                //velocities_desired[i]
                velocities_desired[i] = (rand.Next() + rand.NextDouble());
            }
            //velocities_errors
            arraylength = rand.Next(10);
            if (velocities_errors == null)
                velocities_errors = new double[arraylength];
            else
                Array.Resize(ref velocities_errors, arraylength);
            for (int i=0;i<velocities_errors.Length; i++) {
                //velocities_errors[i]
                velocities_errors[i] = (rand.Next() + rand.NextDouble());
            }
            //accelerations_desired
            arraylength = rand.Next(10);
            if (accelerations_desired == null)
                accelerations_desired = new double[arraylength];
            else
                Array.Resize(ref accelerations_desired, arraylength);
            for (int i=0;i<accelerations_desired.Length; i++) {
                //accelerations_desired[i]
                accelerations_desired[i] = (rand.Next() + rand.NextDouble());
            }
            //accelerations_errors
            arraylength = rand.Next(10);
            if (accelerations_errors == null)
                accelerations_errors = new double[arraylength];
            else
                Array.Resize(ref accelerations_errors, arraylength);
            for (int i=0;i<accelerations_errors.Length; i++) {
                //accelerations_errors[i]
                accelerations_errors[i] = (rand.Next() + rand.NextDouble());
            }
            //effort_errors
            arraylength = rand.Next(10);
            if (effort_errors == null)
                effort_errors = new double[arraylength];
            else
                Array.Resize(ref effort_errors, arraylength);
            for (int i=0;i<effort_errors.Length; i++) {
                //effort_errors[i]
                effort_errors[i] = (rand.Next() + rand.NextDouble());
            }
            //effort_desired
            arraylength = rand.Next(10);
            if (effort_desired == null)
                effort_desired = new double[arraylength];
            else
                Array.Resize(ref effort_desired, arraylength);
            for (int i=0;i<effort_desired.Length; i++) {
                //effort_desired[i]
                effort_desired[i] = (rand.Next() + rand.NextDouble());
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.motoman_msgs.DynamicJointState;
            if (other == null)
                return false;
            ret &= group_number == other.group_number;
            ret &= num_joints == other.num_joints;
            ret &= valid_fields == other.valid_fields;
            if (positions.Length != other.positions.Length)
                return false;
            for (int __i__=0; __i__ < positions.Length; __i__++)
            {
                ret &= positions[__i__] == other.positions[__i__];
            }
            if (velocities.Length != other.velocities.Length)
                return false;
            for (int __i__=0; __i__ < velocities.Length; __i__++)
            {
                ret &= velocities[__i__] == other.velocities[__i__];
            }
            if (accelerations.Length != other.accelerations.Length)
                return false;
            for (int __i__=0; __i__ < accelerations.Length; __i__++)
            {
                ret &= accelerations[__i__] == other.accelerations[__i__];
            }
            if (effort.Length != other.effort.Length)
                return false;
            for (int __i__=0; __i__ < effort.Length; __i__++)
            {
                ret &= effort[__i__] == other.effort[__i__];
            }
            if (positions_desired.Length != other.positions_desired.Length)
                return false;
            for (int __i__=0; __i__ < positions_desired.Length; __i__++)
            {
                ret &= positions_desired[__i__] == other.positions_desired[__i__];
            }
            if (positions_errors.Length != other.positions_errors.Length)
                return false;
            for (int __i__=0; __i__ < positions_errors.Length; __i__++)
            {
                ret &= positions_errors[__i__] == other.positions_errors[__i__];
            }
            if (velocities_desired.Length != other.velocities_desired.Length)
                return false;
            for (int __i__=0; __i__ < velocities_desired.Length; __i__++)
            {
                ret &= velocities_desired[__i__] == other.velocities_desired[__i__];
            }
            if (velocities_errors.Length != other.velocities_errors.Length)
                return false;
            for (int __i__=0; __i__ < velocities_errors.Length; __i__++)
            {
                ret &= velocities_errors[__i__] == other.velocities_errors[__i__];
            }
            if (accelerations_desired.Length != other.accelerations_desired.Length)
                return false;
            for (int __i__=0; __i__ < accelerations_desired.Length; __i__++)
            {
                ret &= accelerations_desired[__i__] == other.accelerations_desired[__i__];
            }
            if (accelerations_errors.Length != other.accelerations_errors.Length)
                return false;
            for (int __i__=0; __i__ < accelerations_errors.Length; __i__++)
            {
                ret &= accelerations_errors[__i__] == other.accelerations_errors[__i__];
            }
            if (effort_errors.Length != other.effort_errors.Length)
                return false;
            for (int __i__=0; __i__ < effort_errors.Length; __i__++)
            {
                ret &= effort_errors[__i__] == other.effort_errors[__i__];
            }
            if (effort_desired.Length != other.effort_desired.Length)
                return false;
            for (int __i__=0; __i__ < effort_desired.Length; __i__++)
            {
                ret &= effort_desired[__i__] == other.effort_desired[__i__];
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
