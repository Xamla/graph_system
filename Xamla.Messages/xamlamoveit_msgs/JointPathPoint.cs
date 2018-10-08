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
    public class JointPathPoint : RosMessage
    {

			public double[] positions;


        public override string MD5Sum() { return "3b33cf2d1f9005e741ee291d519b97ee"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"float64[] positions"; }
        public override string MessageType { get { return "xamlamoveit_msgs/JointPathPoint"; } }
        public override bool IsServiceComponent() { return false; }

        public JointPathPoint()
        {
            
        }

        public JointPathPoint(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public JointPathPoint(byte[] serializedMessage, ref int currentIndex)
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
            
            //positions
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (positions == null)
                positions = new double[arraylength];
            else
                Array.Resize(ref positions, arraylength);
// Start Xamla
                //positions
                piecesize = Marshal.SizeOf(typeof(double)) * positions.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, positions, 0, piecesize);
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
            
            //positions
            hasmetacomponents |= false;
            if (positions == null)
                positions = new double[0];
            pieces.Add(BitConverter.GetBytes(positions.Length));
// Start Xamla
                //positions
                x__size = Marshal.SizeOf(typeof(double)) * positions.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(positions, 0, scratch1, 0, x__size);
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
            
            //positions
            arraylength = rand.Next(10);
            if (positions == null)
                positions = new double[arraylength];
            else
                Array.Resize(ref positions, arraylength);
            for (int i=0;i<positions.Length; i++) {
                //positions[i]
                positions[i] = (rand.Next() + rand.NextDouble());
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.xamlamoveit_msgs.JointPathPoint;
            if (other == null)
                return false;
            if (positions.Length != other.positions.Length)
                return false;
            for (int __i__=0; __i__ < positions.Length; __i__++)
            {
                ret &= positions[__i__] == other.positions[__i__];
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
