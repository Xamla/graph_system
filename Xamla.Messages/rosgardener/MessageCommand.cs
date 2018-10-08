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

namespace Messages.rosgardener
{
    public class MessageCommand : RosMessage
    {

			public Messages.rosgardener.ChannelCommand header = new Messages.rosgardener.ChannelCommand();
			public string message_id = "";
			public string message_body = "";
			public Messages.std_msgs.ByteMultiArray[] attachments;


        public override string MD5Sum() { return "212daa075ce4d3f453f431092a947110"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"ChannelCommand header
string message_id
string message_body
std_msgs/ByteMultiArray[] attachments"; }
        public override string MessageType { get { return "rosgardener/MessageCommand"; } }
        public override bool IsServiceComponent() { return false; }

        public MessageCommand()
        {
            
        }

        public MessageCommand(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public MessageCommand(byte[] serializedMessage, ref int currentIndex)
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
            header = new Messages.rosgardener.ChannelCommand(serializedMessage, ref currentIndex);
            //message_id
            message_id = "";
            piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += 4;
            message_id = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
            currentIndex += piecesize;
            //message_body
            message_body = "";
            piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += 4;
            message_body = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
            currentIndex += piecesize;
            //attachments
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (attachments == null)
                attachments = new Messages.std_msgs.ByteMultiArray[arraylength];
            else
                Array.Resize(ref attachments, arraylength);
            for (int i=0;i<attachments.Length; i++) {
                //attachments[i]
                attachments[i] = new Messages.std_msgs.ByteMultiArray(serializedMessage, ref currentIndex);
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
                header = new Messages.rosgardener.ChannelCommand();
            pieces.Add(header.Serialize(true));
            //message_id
            if (message_id == null)
                message_id = "";
            scratch1 = Encoding.ASCII.GetBytes((string)message_id);
            thischunk = new byte[scratch1.Length + 4];
            scratch2 = BitConverter.GetBytes(scratch1.Length);
            Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
            Array.Copy(scratch2, thischunk, 4);
            pieces.Add(thischunk);
            //message_body
            if (message_body == null)
                message_body = "";
            scratch1 = Encoding.ASCII.GetBytes((string)message_body);
            thischunk = new byte[scratch1.Length + 4];
            scratch2 = BitConverter.GetBytes(scratch1.Length);
            Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
            Array.Copy(scratch2, thischunk, 4);
            pieces.Add(thischunk);
            //attachments
            hasmetacomponents |= false;
            if (attachments == null)
                attachments = new Messages.std_msgs.ByteMultiArray[0];
            pieces.Add(BitConverter.GetBytes(attachments.Length));
            for (int i=0;i<attachments.Length; i++) {
                //attachments[i]
                if (attachments[i] == null)
                    attachments[i] = new Messages.std_msgs.ByteMultiArray();
                pieces.Add(attachments[i].Serialize(true));
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
            header = new Messages.rosgardener.ChannelCommand();
            header.Randomize();
            //message_id
            strlength = rand.Next(100) + 1;
            strbuf = new byte[strlength];
            rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
            for (int __x__ = 0; __x__ < strlength; __x__++)
                if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                    strbuf[__x__] = (byte)(rand.Next(254) + 1);
            strbuf[strlength - 1] = 0; //null terminate
            message_id = Encoding.ASCII.GetString(strbuf);
            //message_body
            strlength = rand.Next(100) + 1;
            strbuf = new byte[strlength];
            rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
            for (int __x__ = 0; __x__ < strlength; __x__++)
                if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                    strbuf[__x__] = (byte)(rand.Next(254) + 1);
            strbuf[strlength - 1] = 0; //null terminate
            message_body = Encoding.ASCII.GetString(strbuf);
            //attachments
            arraylength = rand.Next(10);
            if (attachments == null)
                attachments = new Messages.std_msgs.ByteMultiArray[arraylength];
            else
                Array.Resize(ref attachments, arraylength);
            for (int i=0;i<attachments.Length; i++) {
                //attachments[i]
                attachments[i] = new Messages.std_msgs.ByteMultiArray();
                attachments[i].Randomize();
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.rosgardener.MessageCommand;
            if (other == null)
                return false;
            ret &= header.Equals(other.header);
            ret &= message_id == other.message_id;
            ret &= message_body == other.message_body;
            if (attachments.Length != other.attachments.Length)
                return false;
            for (int __i__=0; __i__ < attachments.Length; __i__++)
            {
                ret &= attachments[__i__].Equals(other.attachments[__i__]);
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
