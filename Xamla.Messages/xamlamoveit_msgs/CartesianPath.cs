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
    public class CartesianPath : RosMessage
    {

			public Messages.geometry_msgs.PoseStamped[] points;


        public override string MD5Sum() { return "0ac6d244346982eba936c77e43e67b98"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"geometry_msgs/PoseStamped[] points"; }
        public override string MessageType { get { return "xamlamoveit_msgs/CartesianPath"; } }
        public override bool IsServiceComponent() { return false; }

        public CartesianPath()
        {
            
        }

        public CartesianPath(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public CartesianPath(byte[] serializedMessage, ref int currentIndex)
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
            
            //points
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (points == null)
                points = new Messages.geometry_msgs.PoseStamped[arraylength];
            else
                Array.Resize(ref points, arraylength);
            for (int i=0;i<points.Length; i++) {
                //points[i]
                points[i] = new Messages.geometry_msgs.PoseStamped(serializedMessage, ref currentIndex);
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
            
            //points
            hasmetacomponents |= false;
            if (points == null)
                points = new Messages.geometry_msgs.PoseStamped[0];
            pieces.Add(BitConverter.GetBytes(points.Length));
            for (int i=0;i<points.Length; i++) {
                //points[i]
                if (points[i] == null)
                    points[i] = new Messages.geometry_msgs.PoseStamped();
                pieces.Add(points[i].Serialize(true));
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
            
            //points
            arraylength = rand.Next(10);
            if (points == null)
                points = new Messages.geometry_msgs.PoseStamped[arraylength];
            else
                Array.Resize(ref points, arraylength);
            for (int i=0;i<points.Length; i++) {
                //points[i]
                points[i] = new Messages.geometry_msgs.PoseStamped();
                points[i].Randomize();
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.xamlamoveit_msgs.CartesianPath;
            if (other == null)
                return false;
            if (points.Length != other.points.Length)
                return false;
            for (int __i__=0; __i__ < points.Length; __i__++)
            {
                ret &= points[__i__].Equals(other.points[__i__]);
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
