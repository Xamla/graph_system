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
    public class JointValuesPoint : RosMessage
    {

			public double[] positions;
			public string[] joint_names;


        public override string MD5Sum() { return "a337b2e1bbb3fce52011a533813a3c64"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"float64[] positions
string[] joint_names"; }
        public override string MessageType { get { return "xamlamoveit_msgs/JointValuesPoint"; } }
        public override bool IsServiceComponent() { return false; }

        public JointValuesPoint()
        {
            
        }

        public JointValuesPoint(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public JointValuesPoint(byte[] serializedMessage, ref int currentIndex)
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

            //joint_names
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (joint_names == null)
                joint_names = new string[arraylength];
            else
                Array.Resize(ref joint_names, arraylength);
            for (int i=0;i<joint_names.Length; i++) {
                //joint_names[i]
                joint_names[i] = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                joint_names[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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

            //joint_names
            hasmetacomponents |= false;
            if (joint_names == null)
                joint_names = new string[0];
            pieces.Add(BitConverter.GetBytes(joint_names.Length));
            for (int i=0;i<joint_names.Length; i++) {
                //joint_names[i]
                if (joint_names[i] == null)
                    joint_names[i] = "";
                scratch1 = Encoding.ASCII.GetBytes((string)joint_names[i]);
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
            //joint_names
            arraylength = rand.Next(10);
            if (joint_names == null)
                joint_names = new string[arraylength];
            else
                Array.Resize(ref joint_names, arraylength);
            for (int i=0;i<joint_names.Length; i++) {
                //joint_names[i]
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                joint_names[i] = Encoding.ASCII.GetString(strbuf);
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.xamlamoveit_msgs.JointValuesPoint;
            if (other == null)
                return false;
            if (positions.Length != other.positions.Length)
                return false;
            for (int __i__=0; __i__ < positions.Length; __i__++)
            {
                ret &= positions[__i__] == other.positions[__i__];
            }
            if (joint_names.Length != other.joint_names.Length)
                return false;
            for (int __i__=0; __i__ < joint_names.Length; __i__++)
            {
                ret &= joint_names[__i__] == other.joint_names[__i__];
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
