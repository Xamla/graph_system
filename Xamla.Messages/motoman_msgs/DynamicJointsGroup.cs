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
    public class DynamicJointsGroup : RosMessage
    {

			public short group_number;
			public short num_joints;
			public short valid_fields;
			public double[] positions;
			public double[] velocities;
			public double[] accelerations;
			public double[] effort;
			public Duration time_from_start = new Duration();


        public override string MD5Sum() { return "fb56acaba1b90a9d7640af3e785681ca"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"int16 group_number
int16 num_joints
int16 valid_fields
float64[] positions
float64[] velocities
float64[] accelerations
float64[] effort
duration time_from_start"; }
        public override string MessageType { get { return "motoman_msgs/DynamicJointsGroup"; } }
        public override bool IsServiceComponent() { return false; }

        public DynamicJointsGroup()
        {
            
        }

        public DynamicJointsGroup(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public DynamicJointsGroup(byte[] serializedMessage, ref int currentIndex)
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

            //time_from_start
            time_from_start = new Duration(new TimeData(
                    BitConverter.ToUInt32(serializedMessage, currentIndex),
                    BitConverter.ToUInt32(serializedMessage, currentIndex+Marshal.SizeOf(typeof(System.Int32)))));
            currentIndex += 2*Marshal.SizeOf(typeof(System.Int32));
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

            //time_from_start
            pieces.Add(BitConverter.GetBytes(time_from_start.data.sec));
            pieces.Add(BitConverter.GetBytes(time_from_start.data.nsec));
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
            //time_from_start
            time_from_start = new Duration(new TimeData(
                    Convert.ToUInt32(rand.Next()),
                    Convert.ToUInt32(rand.Next())));
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.motoman_msgs.DynamicJointsGroup;
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
            ret &= time_from_start.data.Equals(other.time_from_start.data);
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
