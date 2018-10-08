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
    public class JobPosData : RosMessage
    {

			public uint ctrl_grp;
			public int pos_type;
			public int var_index;
			public uint attr;
			public uint attr_ext;
			public int[] pos = new int[8];


        public override string MD5Sum() { return "315f6339a11315e562e361993977a66c"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"uint32 ctrl_grp
int32 pos_type
int32 var_index
uint32 attr
uint32 attr_ext
int32[8] pos"; }
        public override string MessageType { get { return "motoman_msgs/JobPosData"; } }
        public override bool IsServiceComponent() { return false; }

        public JobPosData()
        {
            
        }

        public JobPosData(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public JobPosData(byte[] serializedMessage, ref int currentIndex)
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
            
            //ctrl_grp
            piecesize = Marshal.SizeOf(typeof(uint));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            ctrl_grp = (uint)Marshal.PtrToStructure(h, typeof(uint));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //pos_type
            piecesize = Marshal.SizeOf(typeof(int));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            pos_type = (int)Marshal.PtrToStructure(h, typeof(int));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //var_index
            piecesize = Marshal.SizeOf(typeof(int));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            var_index = (int)Marshal.PtrToStructure(h, typeof(int));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //attr
            piecesize = Marshal.SizeOf(typeof(uint));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            attr = (uint)Marshal.PtrToStructure(h, typeof(uint));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //attr_ext
            piecesize = Marshal.SizeOf(typeof(uint));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            attr_ext = (uint)Marshal.PtrToStructure(h, typeof(uint));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //pos
            hasmetacomponents |= false;
            if (pos == null)
                pos = new int[8];
            else
                Array.Resize(ref pos, 8);
// Start Xamla
                //pos
                piecesize = Marshal.SizeOf(typeof(int)) * pos.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, pos, 0, piecesize);
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
            
            //ctrl_grp
            scratch1 = new byte[Marshal.SizeOf(typeof(uint))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(ctrl_grp, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //pos_type
            scratch1 = new byte[Marshal.SizeOf(typeof(int))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(pos_type, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //var_index
            scratch1 = new byte[Marshal.SizeOf(typeof(int))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(var_index, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //attr
            scratch1 = new byte[Marshal.SizeOf(typeof(uint))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(attr, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //attr_ext
            scratch1 = new byte[Marshal.SizeOf(typeof(uint))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(attr_ext, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //pos
            hasmetacomponents |= false;
            if (pos == null)
                pos = new int[0];
// Start Xamla
                //pos
                x__size = Marshal.SizeOf(typeof(int)) * pos.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(pos, 0, scratch1, 0, x__size);
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
            
            //ctrl_grp
            ctrl_grp = (uint)rand.Next();
            //pos_type
            pos_type = rand.Next();
            //var_index
            var_index = rand.Next();
            //attr
            attr = (uint)rand.Next();
            //attr_ext
            attr_ext = (uint)rand.Next();
            //pos
            if (pos == null)
                pos = new int[8];
            else
                Array.Resize(ref pos, 8);
            for (int i=0;i<pos.Length; i++) {
                //pos[i]
                pos[i] = rand.Next();
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.motoman_msgs.JobPosData;
            if (other == null)
                return false;
            ret &= ctrl_grp == other.ctrl_grp;
            ret &= pos_type == other.pos_type;
            ret &= var_index == other.var_index;
            ret &= attr == other.attr;
            ret &= attr_ext == other.attr_ext;
            if (pos.Length != other.pos.Length)
                return false;
            for (int __i__=0; __i__ < pos.Length; __i__++)
            {
                ret &= pos[__i__] == other.pos[__i__];
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
