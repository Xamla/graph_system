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

namespace Messages.ximea_msgs
{
    public class StereoPair : RosMessage
    {

			public Header header = new Header();
			public Messages.sensor_msgs.Image[] images;
			public string[] serials;


        public override string MD5Sum() { return "03cb8360ea246b5bd73983c13f27a975"; }
        public override bool HasHeader() { return true; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"Header header
sensor_msgs/Image[] images
string[] serials"; }
        public override string MessageType { get { return "ximea_msgs/StereoPair"; } }
        public override bool IsServiceComponent() { return false; }

        public StereoPair()
        {
            
        }

        public StereoPair(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public StereoPair(byte[] serializedMessage, ref int currentIndex)
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
            //images
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (images == null)
                images = new Messages.sensor_msgs.Image[arraylength];
            else
                Array.Resize(ref images, arraylength);
            for (int i=0;i<images.Length; i++) {
                //images[i]
                images[i] = new Messages.sensor_msgs.Image(serializedMessage, ref currentIndex);
            }
            //serials
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (serials == null)
                serials = new string[arraylength];
            else
                Array.Resize(ref serials, arraylength);
            for (int i=0;i<serials.Length; i++) {
                //serials[i]
                serials[i] = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                serials[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
                header = new Header();
            pieces.Add(header.Serialize(true));
            //images
            hasmetacomponents |= false;
            if (images == null)
                images = new Messages.sensor_msgs.Image[0];
            pieces.Add(BitConverter.GetBytes(images.Length));
            for (int i=0;i<images.Length; i++) {
                //images[i]
                if (images[i] == null)
                    images[i] = new Messages.sensor_msgs.Image();
                pieces.Add(images[i].Serialize(true));
            }
            //serials
            hasmetacomponents |= false;
            if (serials == null)
                serials = new string[0];
            pieces.Add(BitConverter.GetBytes(serials.Length));
            for (int i=0;i<serials.Length; i++) {
                //serials[i]
                if (serials[i] == null)
                    serials[i] = "";
                scratch1 = Encoding.ASCII.GetBytes((string)serials[i]);
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
            header = new Header();
            header.Randomize();
            //images
            arraylength = rand.Next(10);
            if (images == null)
                images = new Messages.sensor_msgs.Image[arraylength];
            else
                Array.Resize(ref images, arraylength);
            for (int i=0;i<images.Length; i++) {
                //images[i]
                images[i] = new Messages.sensor_msgs.Image();
                images[i].Randomize();
            }
            //serials
            arraylength = rand.Next(10);
            if (serials == null)
                serials = new string[arraylength];
            else
                Array.Resize(ref serials, arraylength);
            for (int i=0;i<serials.Length; i++) {
                //serials[i]
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                serials[i] = Encoding.ASCII.GetString(strbuf);
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.ximea_msgs.StereoPair;
            if (other == null)
                return false;
            ret &= header.Equals(other.header);
            if (images.Length != other.images.Length)
                return false;
            for (int __i__=0; __i__ < images.Length; __i__++)
            {
                ret &= images[__i__].Equals(other.images[__i__]);
            }
            if (serials.Length != other.serials.Length)
                return false;
            for (int __i__=0; __i__ < serials.Length; __i__++)
            {
                ret &= serials[__i__] == other.serials[__i__];
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
