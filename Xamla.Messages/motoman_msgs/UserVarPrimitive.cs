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
    public class UserVarPrimitive : RosMessage
    {

			public const int MP_VAR_B = 0;
			public const int MP_VAR_I = 1;
			public const int MP_VAR_D = 2;
			public const int MP_VAR_R = 3;
			public const int MP_VAR_S = 4;
			public const int MP_VAR_P = 5;
			public const int MP_VAR_BP = 6;
			public const int MP_VAR_EX = 7;
			public int var_type;
			public int var_no;
			public long int_value;
			public double float_value;
			public string string_value = "";


        public override string MD5Sum() { return "7344b8cf5abe866623bf249cb97a5a24"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"int32 MP_VAR_B=0
int32 MP_VAR_I=1
int32 MP_VAR_D=2
int32 MP_VAR_R=3
int32 MP_VAR_S=4
int32 MP_VAR_P=5
int32 MP_VAR_BP=6
int32 MP_VAR_EX=7
int32 var_type
int32 var_no
int64 int_value
float64 float_value
string string_value"; }
        public override string MessageType { get { return "motoman_msgs/UserVarPrimitive"; } }
        public override bool IsServiceComponent() { return false; }

        public UserVarPrimitive()
        {
            
        }

        public UserVarPrimitive(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public UserVarPrimitive(byte[] serializedMessage, ref int currentIndex)
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
            
            //var_type
            piecesize = Marshal.SizeOf(typeof(int));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            var_type = (int)Marshal.PtrToStructure(h, typeof(int));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //var_no
            piecesize = Marshal.SizeOf(typeof(int));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            var_no = (int)Marshal.PtrToStructure(h, typeof(int));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //int_value
            piecesize = Marshal.SizeOf(typeof(long));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            int_value = (long)Marshal.PtrToStructure(h, typeof(long));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //float_value
            piecesize = Marshal.SizeOf(typeof(double));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            float_value = (double)Marshal.PtrToStructure(h, typeof(double));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //string_value
            string_value = "";
            piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += 4;
            string_value = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
            currentIndex += piecesize;
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
            
            //var_type
            scratch1 = new byte[Marshal.SizeOf(typeof(int))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(var_type, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //var_no
            scratch1 = new byte[Marshal.SizeOf(typeof(int))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(var_no, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //int_value
            scratch1 = new byte[Marshal.SizeOf(typeof(long))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(int_value, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //float_value
            scratch1 = new byte[Marshal.SizeOf(typeof(double))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(float_value, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //string_value
            if (string_value == null)
                string_value = "";
            scratch1 = Encoding.ASCII.GetBytes((string)string_value);
            thischunk = new byte[scratch1.Length + 4];
            scratch2 = BitConverter.GetBytes(scratch1.Length);
            Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
            Array.Copy(scratch2, thischunk, 4);
            pieces.Add(thischunk);
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
            
            //var_type
            var_type = rand.Next();
            //var_no
            var_no = rand.Next();
            //int_value
            int_value = (System.Int64)(rand.Next() << 32) | rand.Next();
            //float_value
            float_value = (rand.Next() + rand.NextDouble());
            //string_value
            strlength = rand.Next(100) + 1;
            strbuf = new byte[strlength];
            rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
            for (int __x__ = 0; __x__ < strlength; __x__++)
                if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                    strbuf[__x__] = (byte)(rand.Next(254) + 1);
            strbuf[strlength - 1] = 0; //null terminate
            string_value = Encoding.ASCII.GetString(strbuf);
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.motoman_msgs.UserVarPrimitive;
            if (other == null)
                return false;
            ret &= var_type == other.var_type;
            ret &= var_no == other.var_no;
            ret &= int_value == other.int_value;
            ret &= float_value == other.float_value;
            ret &= string_value == other.string_value;
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
