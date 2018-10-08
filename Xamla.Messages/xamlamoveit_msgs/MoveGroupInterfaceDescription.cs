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
    public class MoveGroupInterfaceDescription : RosMessage
    {

			public string name = "";
			public string[] sub_move_group_ids;
			public string[] joint_names;
			public string[] end_effector_names;
			public string[] end_effector_link_names;


        public override string MD5Sum() { return "94b8f83414a4f16f348529e2c39ed8c4"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"string name
string[] sub_move_group_ids
string[] joint_names
string[] end_effector_names
string[] end_effector_link_names"; }
        public override string MessageType { get { return "xamlamoveit_msgs/MoveGroupInterfaceDescription"; } }
        public override bool IsServiceComponent() { return false; }

        public MoveGroupInterfaceDescription()
        {
            
        }

        public MoveGroupInterfaceDescription(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public MoveGroupInterfaceDescription(byte[] serializedMessage, ref int currentIndex)
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
            
            //name
            name = "";
            piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += 4;
            name = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
            currentIndex += piecesize;
            //sub_move_group_ids
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (sub_move_group_ids == null)
                sub_move_group_ids = new string[arraylength];
            else
                Array.Resize(ref sub_move_group_ids, arraylength);
            for (int i=0;i<sub_move_group_ids.Length; i++) {
                //sub_move_group_ids[i]
                sub_move_group_ids[i] = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                sub_move_group_ids[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
            }
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
            //end_effector_names
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (end_effector_names == null)
                end_effector_names = new string[arraylength];
            else
                Array.Resize(ref end_effector_names, arraylength);
            for (int i=0;i<end_effector_names.Length; i++) {
                //end_effector_names[i]
                end_effector_names[i] = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                end_effector_names[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
            }
            //end_effector_link_names
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (end_effector_link_names == null)
                end_effector_link_names = new string[arraylength];
            else
                Array.Resize(ref end_effector_link_names, arraylength);
            for (int i=0;i<end_effector_link_names.Length; i++) {
                //end_effector_link_names[i]
                end_effector_link_names[i] = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                end_effector_link_names[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
            
            //name
            if (name == null)
                name = "";
            scratch1 = Encoding.ASCII.GetBytes((string)name);
            thischunk = new byte[scratch1.Length + 4];
            scratch2 = BitConverter.GetBytes(scratch1.Length);
            Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
            Array.Copy(scratch2, thischunk, 4);
            pieces.Add(thischunk);
            //sub_move_group_ids
            hasmetacomponents |= false;
            if (sub_move_group_ids == null)
                sub_move_group_ids = new string[0];
            pieces.Add(BitConverter.GetBytes(sub_move_group_ids.Length));
            for (int i=0;i<sub_move_group_ids.Length; i++) {
                //sub_move_group_ids[i]
                if (sub_move_group_ids[i] == null)
                    sub_move_group_ids[i] = "";
                scratch1 = Encoding.ASCII.GetBytes((string)sub_move_group_ids[i]);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
            }
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
            //end_effector_names
            hasmetacomponents |= false;
            if (end_effector_names == null)
                end_effector_names = new string[0];
            pieces.Add(BitConverter.GetBytes(end_effector_names.Length));
            for (int i=0;i<end_effector_names.Length; i++) {
                //end_effector_names[i]
                if (end_effector_names[i] == null)
                    end_effector_names[i] = "";
                scratch1 = Encoding.ASCII.GetBytes((string)end_effector_names[i]);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
            }
            //end_effector_link_names
            hasmetacomponents |= false;
            if (end_effector_link_names == null)
                end_effector_link_names = new string[0];
            pieces.Add(BitConverter.GetBytes(end_effector_link_names.Length));
            for (int i=0;i<end_effector_link_names.Length; i++) {
                //end_effector_link_names[i]
                if (end_effector_link_names[i] == null)
                    end_effector_link_names[i] = "";
                scratch1 = Encoding.ASCII.GetBytes((string)end_effector_link_names[i]);
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
            
            //name
            strlength = rand.Next(100) + 1;
            strbuf = new byte[strlength];
            rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
            for (int __x__ = 0; __x__ < strlength; __x__++)
                if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                    strbuf[__x__] = (byte)(rand.Next(254) + 1);
            strbuf[strlength - 1] = 0; //null terminate
            name = Encoding.ASCII.GetString(strbuf);
            //sub_move_group_ids
            arraylength = rand.Next(10);
            if (sub_move_group_ids == null)
                sub_move_group_ids = new string[arraylength];
            else
                Array.Resize(ref sub_move_group_ids, arraylength);
            for (int i=0;i<sub_move_group_ids.Length; i++) {
                //sub_move_group_ids[i]
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                sub_move_group_ids[i] = Encoding.ASCII.GetString(strbuf);
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
            //end_effector_names
            arraylength = rand.Next(10);
            if (end_effector_names == null)
                end_effector_names = new string[arraylength];
            else
                Array.Resize(ref end_effector_names, arraylength);
            for (int i=0;i<end_effector_names.Length; i++) {
                //end_effector_names[i]
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                end_effector_names[i] = Encoding.ASCII.GetString(strbuf);
            }
            //end_effector_link_names
            arraylength = rand.Next(10);
            if (end_effector_link_names == null)
                end_effector_link_names = new string[arraylength];
            else
                Array.Resize(ref end_effector_link_names, arraylength);
            for (int i=0;i<end_effector_link_names.Length; i++) {
                //end_effector_link_names[i]
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                end_effector_link_names[i] = Encoding.ASCII.GetString(strbuf);
            }
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.xamlamoveit_msgs.MoveGroupInterfaceDescription;
            if (other == null)
                return false;
            ret &= name == other.name;
            if (sub_move_group_ids.Length != other.sub_move_group_ids.Length)
                return false;
            for (int __i__=0; __i__ < sub_move_group_ids.Length; __i__++)
            {
                ret &= sub_move_group_ids[__i__] == other.sub_move_group_ids[__i__];
            }
            if (joint_names.Length != other.joint_names.Length)
                return false;
            for (int __i__=0; __i__ < joint_names.Length; __i__++)
            {
                ret &= joint_names[__i__] == other.joint_names[__i__];
            }
            if (end_effector_names.Length != other.end_effector_names.Length)
                return false;
            for (int __i__=0; __i__ < end_effector_names.Length; __i__++)
            {
                ret &= end_effector_names[__i__] == other.end_effector_names[__i__];
            }
            if (end_effector_link_names.Length != other.end_effector_link_names.Length)
                return false;
            for (int __i__=0; __i__ < end_effector_link_names.Length; __i__++)
            {
                ret &= end_effector_link_names[__i__] == other.end_effector_link_names[__i__];
            }
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
