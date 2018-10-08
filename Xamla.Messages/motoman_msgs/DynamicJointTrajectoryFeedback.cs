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
    public class DynamicJointTrajectoryFeedback : RosMessage
    {

			public Header header = new Header();
			public short num_groups;
			public Messages.motoman_msgs.DynamicJointState[] joint_feedbacks;


        public override string MD5Sum() { return "84d3bbf7103790ff0a8946017b895a1a"; }
        public override bool HasHeader() { return true; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"Header header
int16 num_groups
DynamicJointState[] joint_feedbacks"; }
        public override string MessageType { get { return "motoman_msgs/DynamicJointTrajectoryFeedback"; } }
        public override bool IsServiceComponent() { return false; }

        public DynamicJointTrajectoryFeedback()
        {
            
        }

        public DynamicJointTrajectoryFeedback(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public DynamicJointTrajectoryFeedback(byte[] serializedMessage, ref int currentIndex)
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
            
            //header
            header = new Header(serializedMessage, ref currentIndex);
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
            //joint_feedbacks
            hasmetacomponents |= true;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (joint_feedbacks == null)
                joint_feedbacks = new Messages.motoman_msgs.DynamicJointState[arraylength];
            else
                Array.Resize(ref joint_feedbacks, arraylength);
            for (int i=0;i<joint_feedbacks.Length; i++) {
                //joint_feedbacks[i]
                joint_feedbacks[i] = new Messages.motoman_msgs.DynamicJointState(serializedMessage, ref currentIndex);
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
            
            //header
            if (header == null)
                header = new Header();
            pieces.Add(header.Serialize(true));
            //num_groups
            scratch1 = new byte[Marshal.SizeOf(typeof(short))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(num_groups, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //joint_feedbacks
            hasmetacomponents |= true;
            if (joint_feedbacks == null)
                joint_feedbacks = new Messages.motoman_msgs.DynamicJointState[0];
            pieces.Add(BitConverter.GetBytes(joint_feedbacks.Length));
            for (int i=0;i<joint_feedbacks.Length; i++) {
                //joint_feedbacks[i]
                if (joint_feedbacks[i] == null)
                    joint_feedbacks[i] = new Messages.motoman_msgs.DynamicJointState();
                pieces.Add(joint_feedbacks[i].Serialize(true));
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
            
            //header
            header = new Header();
            header.Randomize();
            //num_groups
            num_groups = (System.Int16)rand.Next(System.Int16.MaxValue + 1);
            //joint_feedbacks
            arraylength = rand.Next(10);
            if (joint_feedbacks == null)
                joint_feedbacks = new Messages.motoman_msgs.DynamicJointState[arraylength];
            else
                Array.Resize(ref joint_feedbacks, arraylength);
            for (int i=0;i<joint_feedbacks.Length; i++) {
                //joint_feedbacks[i]
                joint_feedbacks[i] = new Messages.motoman_msgs.DynamicJointState();
                joint_feedbacks[i].Randomize();
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.motoman_msgs.DynamicJointTrajectoryFeedback;
            if (other == null)
                return false;
            ret &= header.Equals(other.header);
            ret &= num_groups == other.num_groups;
            if (joint_feedbacks.Length != other.joint_feedbacks.Length)
                return false;
            for (int __i__=0; __i__ < joint_feedbacks.Length; __i__++)
            {
                ret &= joint_feedbacks[__i__].Equals(other.joint_feedbacks[__i__]);
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
