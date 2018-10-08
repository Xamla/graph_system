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

namespace Messages.xamla_sysmon
{
    public class SystemStatus : RosMessage
    {

			public Messages.std_msgs.Header header = new Messages.std_msgs.Header();
			public int system_status;
			public string[] topics;
			public int[] err_code;
			public string[] topic_msg;


        public override string MD5Sum() { return "1aa2dc96140bb55034af278dcf1946d7"; }
        public override bool HasHeader() { return true; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"std_msgs/Header header
int32 system_status
string[] topics
int32[] err_code
string[] topic_msg"; }
        public override string MessageType { get { return "xamla_sysmon/SystemStatus"; } }
        public override bool IsServiceComponent() { return false; }

        public SystemStatus()
        {
            
        }

        public SystemStatus(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public SystemStatus(byte[] serializedMessage, ref int currentIndex)
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
            header = new Messages.std_msgs.Header(serializedMessage, ref currentIndex);
            //system_status
            piecesize = Marshal.SizeOf(typeof(int));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            system_status = (int)Marshal.PtrToStructure(h, typeof(int));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //topics
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (topics == null)
                topics = new string[arraylength];
            else
                Array.Resize(ref topics, arraylength);
            for (int i=0;i<topics.Length; i++) {
                //topics[i]
                topics[i] = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                topics[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
            }
            //err_code
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (err_code == null)
                err_code = new int[arraylength];
            else
                Array.Resize(ref err_code, arraylength);
// Start Xamla
                //err_code
                piecesize = Marshal.SizeOf(typeof(int)) * err_code.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, err_code, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //topic_msg
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (topic_msg == null)
                topic_msg = new string[arraylength];
            else
                Array.Resize(ref topic_msg, arraylength);
            for (int i=0;i<topic_msg.Length; i++) {
                //topic_msg[i]
                topic_msg[i] = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                topic_msg[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
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
                header = new Messages.std_msgs.Header();
            pieces.Add(header.Serialize(true));
            //system_status
            scratch1 = new byte[Marshal.SizeOf(typeof(int))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(system_status, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //topics
            hasmetacomponents |= false;
            if (topics == null)
                topics = new string[0];
            pieces.Add(BitConverter.GetBytes(topics.Length));
            for (int i=0;i<topics.Length; i++) {
                //topics[i]
                if (topics[i] == null)
                    topics[i] = "";
                scratch1 = Encoding.ASCII.GetBytes((string)topics[i]);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
            }
            //err_code
            hasmetacomponents |= false;
            if (err_code == null)
                err_code = new int[0];
            pieces.Add(BitConverter.GetBytes(err_code.Length));
// Start Xamla
                //err_code
                x__size = Marshal.SizeOf(typeof(int)) * err_code.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(err_code, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //topic_msg
            hasmetacomponents |= false;
            if (topic_msg == null)
                topic_msg = new string[0];
            pieces.Add(BitConverter.GetBytes(topic_msg.Length));
            for (int i=0;i<topic_msg.Length; i++) {
                //topic_msg[i]
                if (topic_msg[i] == null)
                    topic_msg[i] = "";
                scratch1 = Encoding.ASCII.GetBytes((string)topic_msg[i]);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
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
            header = new Messages.std_msgs.Header();
            header.Randomize();
            //system_status
            system_status = rand.Next();
            //topics
            arraylength = rand.Next(10);
            if (topics == null)
                topics = new string[arraylength];
            else
                Array.Resize(ref topics, arraylength);
            for (int i=0;i<topics.Length; i++) {
                //topics[i]
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                topics[i] = Encoding.ASCII.GetString(strbuf);
            }
            //err_code
            arraylength = rand.Next(10);
            if (err_code == null)
                err_code = new int[arraylength];
            else
                Array.Resize(ref err_code, arraylength);
            for (int i=0;i<err_code.Length; i++) {
                //err_code[i]
                err_code[i] = rand.Next();
            }
            //topic_msg
            arraylength = rand.Next(10);
            if (topic_msg == null)
                topic_msg = new string[arraylength];
            else
                Array.Resize(ref topic_msg, arraylength);
            for (int i=0;i<topic_msg.Length; i++) {
                //topic_msg[i]
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                topic_msg[i] = Encoding.ASCII.GetString(strbuf);
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.xamla_sysmon.SystemStatus;
            if (other == null)
                return false;
            ret &= header.Equals(other.header);
            ret &= system_status == other.system_status;
            if (topics.Length != other.topics.Length)
                return false;
            for (int __i__=0; __i__ < topics.Length; __i__++)
            {
                ret &= topics[__i__] == other.topics[__i__];
            }
            if (err_code.Length != other.err_code.Length)
                return false;
            for (int __i__=0; __i__ < err_code.Length; __i__++)
            {
                ret &= err_code[__i__] == other.err_code[__i__];
            }
            if (topic_msg.Length != other.topic_msg.Length)
                return false;
            for (int __i__=0; __i__ < topic_msg.Length; __i__++)
            {
                ret &= topic_msg[__i__] == other.topic_msg[__i__];
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
