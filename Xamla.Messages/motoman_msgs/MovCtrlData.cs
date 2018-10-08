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
    public class MovCtrlData : RosMessage
    {

			public byte intp_type;
			public byte intp_kind;
			public byte speed_type;
			public int speed_value;
			public Messages.motoman_msgs.JobPosData[] posData;


        public override string MD5Sum() { return "ae080490ba264284231828079d06a337"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"uint8 intp_type
uint8 intp_kind
uint8 speed_type
int32 speed_value
JobPosData[] posData"; }
        public override string MessageType { get { return "motoman_msgs/MovCtrlData"; } }
        public override bool IsServiceComponent() { return false; }

        public MovCtrlData()
        {
            
        }

        public MovCtrlData(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public MovCtrlData(byte[] serializedMessage, ref int currentIndex)
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
            
            //intp_type
            intp_type=serializedMessage[currentIndex++];
            //intp_kind
            intp_kind=serializedMessage[currentIndex++];
            //speed_type
            speed_type=serializedMessage[currentIndex++];
            //speed_value
            piecesize = Marshal.SizeOf(typeof(int));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            speed_value = (int)Marshal.PtrToStructure(h, typeof(int));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //posData
            hasmetacomponents |= true;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (posData == null)
                posData = new Messages.motoman_msgs.JobPosData[arraylength];
            else
                Array.Resize(ref posData, arraylength);
            for (int i=0;i<posData.Length; i++) {
                //posData[i]
                posData[i] = new Messages.motoman_msgs.JobPosData(serializedMessage, ref currentIndex);
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
            
            //intp_type
            pieces.Add(new[] { (byte)intp_type });
            //intp_kind
            pieces.Add(new[] { (byte)intp_kind });
            //speed_type
            pieces.Add(new[] { (byte)speed_type });
            //speed_value
            scratch1 = new byte[Marshal.SizeOf(typeof(int))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(speed_value, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //posData
            hasmetacomponents |= true;
            if (posData == null)
                posData = new Messages.motoman_msgs.JobPosData[0];
            pieces.Add(BitConverter.GetBytes(posData.Length));
            for (int i=0;i<posData.Length; i++) {
                //posData[i]
                if (posData[i] == null)
                    posData[i] = new Messages.motoman_msgs.JobPosData();
                pieces.Add(posData[i].Serialize(true));
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
            
            //intp_type
            myByte = new byte[1];
            rand.NextBytes(myByte);
            intp_type= myByte[0];
            //intp_kind
            myByte = new byte[1];
            rand.NextBytes(myByte);
            intp_kind= myByte[0];
            //speed_type
            myByte = new byte[1];
            rand.NextBytes(myByte);
            speed_type= myByte[0];
            //speed_value
            speed_value = rand.Next();
            //posData
            arraylength = rand.Next(10);
            if (posData == null)
                posData = new Messages.motoman_msgs.JobPosData[arraylength];
            else
                Array.Resize(ref posData, arraylength);
            for (int i=0;i<posData.Length; i++) {
                //posData[i]
                posData[i] = new Messages.motoman_msgs.JobPosData();
                posData[i].Randomize();
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.motoman_msgs.MovCtrlData;
            if (other == null)
                return false;
            ret &= intp_type == other.intp_type;
            ret &= intp_kind == other.intp_kind;
            ret &= speed_type == other.speed_type;
            ret &= speed_value == other.speed_value;
            if (posData.Length != other.posData.Length)
                return false;
            for (int __i__=0; __i__ < posData.Length; __i__++)
            {
                ret &= posData[__i__].Equals(other.posData[__i__]);
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
