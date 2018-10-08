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

namespace Messages.xamlamoveit_msgs
{
    public class TrajectoryProgress : RosMessage
    {

			public ulong index;
			public ulong num_of_points;
			public double progress;
			public double control_frequency;
			public string error_msg = "";
			public long error_code;


        public override string MD5Sum() { return "465c1866dce8e0eb90aa2606f55bac91"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"uint64 index
uint64 num_of_points
float64 progress
float64 control_frequency
string error_msg
int64 error_code"; }
        public override string MessageType { get { return "xamlamoveit_msgs/TrajectoryProgress"; } }
        public override bool IsServiceComponent() { return false; }

        public TrajectoryProgress()
        {
            
        }

        public TrajectoryProgress(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public TrajectoryProgress(byte[] serializedMessage, ref int currentIndex)
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
            
            //index
            piecesize = Marshal.SizeOf(typeof(ulong));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            index = (ulong)Marshal.PtrToStructure(h, typeof(ulong));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //num_of_points
            piecesize = Marshal.SizeOf(typeof(ulong));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            num_of_points = (ulong)Marshal.PtrToStructure(h, typeof(ulong));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //progress
            piecesize = Marshal.SizeOf(typeof(double));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            progress = (double)Marshal.PtrToStructure(h, typeof(double));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //control_frequency
            piecesize = Marshal.SizeOf(typeof(double));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            control_frequency = (double)Marshal.PtrToStructure(h, typeof(double));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //error_msg
            error_msg = "";
            piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += 4;
            error_msg = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
            currentIndex += piecesize;
            //error_code
            piecesize = Marshal.SizeOf(typeof(long));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            error_code = (long)Marshal.PtrToStructure(h, typeof(long));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
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
            
            //index
            scratch1 = new byte[Marshal.SizeOf(typeof(ulong))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(index, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //num_of_points
            scratch1 = new byte[Marshal.SizeOf(typeof(ulong))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(num_of_points, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //progress
            scratch1 = new byte[Marshal.SizeOf(typeof(double))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(progress, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //control_frequency
            scratch1 = new byte[Marshal.SizeOf(typeof(double))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(control_frequency, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //error_msg
            if (error_msg == null)
                error_msg = "";
            scratch1 = Encoding.ASCII.GetBytes((string)error_msg);
            thischunk = new byte[scratch1.Length + 4];
            scratch2 = BitConverter.GetBytes(scratch1.Length);
            Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
            Array.Copy(scratch2, thischunk, 4);
            pieces.Add(thischunk);
            //error_code
            scratch1 = new byte[Marshal.SizeOf(typeof(long))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(error_code, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
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
            
            //index
            index = (System.UInt64)((uint)(rand.Next() << 32)) | (uint)rand.Next();
            //num_of_points
            num_of_points = (System.UInt64)((uint)(rand.Next() << 32)) | (uint)rand.Next();
            //progress
            progress = (rand.Next() + rand.NextDouble());
            //control_frequency
            control_frequency = (rand.Next() + rand.NextDouble());
            //error_msg
            strlength = rand.Next(100) + 1;
            strbuf = new byte[strlength];
            rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
            for (int __x__ = 0; __x__ < strlength; __x__++)
                if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                    strbuf[__x__] = (byte)(rand.Next(254) + 1);
            strbuf[strlength - 1] = 0; //null terminate
            error_msg = Encoding.ASCII.GetString(strbuf);
            //error_code
            error_code = (System.Int64)(rand.Next() << 32) | rand.Next();
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.xamlamoveit_msgs.TrajectoryProgress;
            if (other == null)
                return false;
            ret &= index == other.index;
            ret &= num_of_points == other.num_of_points;
            ret &= progress == other.progress;
            ret &= control_frequency == other.control_frequency;
            ret &= error_msg == other.error_msg;
            ret &= error_code == other.error_code;
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
