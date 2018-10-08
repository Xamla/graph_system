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
    public class DynamicJointPoint : RosMessage
    {

			public short num_groups;
			public Messages.motoman_msgs.DynamicJointsGroup[] groups;


        public override string MD5Sum() { return "f91ca86c2821b55c8430ab0088bfe5df"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"int16 num_groups
DynamicJointsGroup[] groups"; }
        public override string MessageType { get { return "motoman_msgs/DynamicJointPoint"; } }
        public override bool IsServiceComponent() { return false; }

        public DynamicJointPoint()
        {
            
        }

        public DynamicJointPoint(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public DynamicJointPoint(byte[] serializedMessage, ref int currentIndex)
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
            
            //num_groups
            piecesize = Marshal.SizeOf(typeof(short));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            num_groups = (short)Marshal.PtrToStructure(h, typeof(short));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //groups
            hasmetacomponents |= true;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (groups == null)
                groups = new Messages.motoman_msgs.DynamicJointsGroup[arraylength];
            else
                Array.Resize(ref groups, arraylength);
            for (int i=0;i<groups.Length; i++) {
                //groups[i]
                groups[i] = new Messages.motoman_msgs.DynamicJointsGroup(serializedMessage, ref currentIndex);
            }
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
            
            //num_groups
            scratch1 = new byte[Marshal.SizeOf(typeof(short))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(num_groups, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //groups
            hasmetacomponents |= true;
            if (groups == null)
                groups = new Messages.motoman_msgs.DynamicJointsGroup[0];
            pieces.Add(BitConverter.GetBytes(groups.Length));
            for (int i=0;i<groups.Length; i++) {
                //groups[i]
                if (groups[i] == null)
                    groups[i] = new Messages.motoman_msgs.DynamicJointsGroup();
                pieces.Add(groups[i].Serialize(true));
            }
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
            
            //num_groups
            num_groups = (System.Int16)rand.Next(System.Int16.MaxValue + 1);
            //groups
            arraylength = rand.Next(10);
            if (groups == null)
                groups = new Messages.motoman_msgs.DynamicJointsGroup[arraylength];
            else
                Array.Resize(ref groups, arraylength);
            for (int i=0;i<groups.Length; i++) {
                //groups[i]
                groups[i] = new Messages.motoman_msgs.DynamicJointsGroup();
                groups[i].Randomize();
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.motoman_msgs.DynamicJointPoint;
            if (other == null)
                return false;
            ret &= num_groups == other.num_groups;
            if (groups.Length != other.groups.Length)
                return false;
            for (int __i__=0; __i__ < groups.Length; __i__++)
            {
                ret &= groups[__i__].Equals(other.groups[__i__]);
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
