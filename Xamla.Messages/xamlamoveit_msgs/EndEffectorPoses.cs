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
    public class EndEffectorPoses : RosMessage
    {

			public Messages.geometry_msgs.PoseStamped[] poses;
			public string[] link_names;


        public override string MD5Sum() { return "612d3d190d8bfecce6f8b32bad351232"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"geometry_msgs/PoseStamped[] poses
string[] link_names"; }
        public override string MessageType { get { return "xamlamoveit_msgs/EndEffectorPoses"; } }
        public override bool IsServiceComponent() { return false; }

        public EndEffectorPoses()
        {
            
        }

        public EndEffectorPoses(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public EndEffectorPoses(byte[] serializedMessage, ref int currentIndex)
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
            
            //poses
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (poses == null)
                poses = new Messages.geometry_msgs.PoseStamped[arraylength];
            else
                Array.Resize(ref poses, arraylength);
            for (int i=0;i<poses.Length; i++) {
                //poses[i]
                poses[i] = new Messages.geometry_msgs.PoseStamped(serializedMessage, ref currentIndex);
            }
            //link_names
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (link_names == null)
                link_names = new string[arraylength];
            else
                Array.Resize(ref link_names, arraylength);
            for (int i=0;i<link_names.Length; i++) {
                //link_names[i]
                link_names[i] = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                link_names[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
            
            //poses
            hasmetacomponents |= false;
            if (poses == null)
                poses = new Messages.geometry_msgs.PoseStamped[0];
            pieces.Add(BitConverter.GetBytes(poses.Length));
            for (int i=0;i<poses.Length; i++) {
                //poses[i]
                if (poses[i] == null)
                    poses[i] = new Messages.geometry_msgs.PoseStamped();
                pieces.Add(poses[i].Serialize(true));
            }
            //link_names
            hasmetacomponents |= false;
            if (link_names == null)
                link_names = new string[0];
            pieces.Add(BitConverter.GetBytes(link_names.Length));
            for (int i=0;i<link_names.Length; i++) {
                //link_names[i]
                if (link_names[i] == null)
                    link_names[i] = "";
                scratch1 = Encoding.ASCII.GetBytes((string)link_names[i]);
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
            
            //poses
            arraylength = rand.Next(10);
            if (poses == null)
                poses = new Messages.geometry_msgs.PoseStamped[arraylength];
            else
                Array.Resize(ref poses, arraylength);
            for (int i=0;i<poses.Length; i++) {
                //poses[i]
                poses[i] = new Messages.geometry_msgs.PoseStamped();
                poses[i].Randomize();
            }
            //link_names
            arraylength = rand.Next(10);
            if (link_names == null)
                link_names = new string[arraylength];
            else
                Array.Resize(ref link_names, arraylength);
            for (int i=0;i<link_names.Length; i++) {
                //link_names[i]
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                link_names[i] = Encoding.ASCII.GetString(strbuf);
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.xamlamoveit_msgs.EndEffectorPoses;
            if (other == null)
                return false;
            if (poses.Length != other.poses.Length)
                return false;
            for (int __i__=0; __i__ < poses.Length; __i__++)
            {
                ret &= poses[__i__].Equals(other.poses[__i__]);
            }
            if (link_names.Length != other.link_names.Length)
                return false;
            for (int __i__=0; __i__ < link_names.Length; __i__++)
            {
                ret &= link_names[__i__] == other.link_names[__i__];
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
