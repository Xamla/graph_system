using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using Uml.Robotics.Ros;
using Messages.actionlib_msgs;
using Messages.geometry_msgs;
using Messages.std_msgs;


namespace Messages.test
{
	[ActionGoalMessage]
        public class SystemCommandGoal : InnerActionMessage
    {
        			public string command = "";



        public override string MD5Sum() { return "cba5e21e920a3a2b7b375cb65b64cdea"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"string command"; }
		public override string MessageType { get { return "test/SystemCommandGoal"; } }
        public override bool IsServiceComponent() { return false; }

        public SystemCommandGoal()
        {
            
        }

        public SystemCommandGoal(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public SystemCommandGoal(byte[] serializedMessage, ref int currentIndex)
        {
            Deserialize(serializedMessage, ref currentIndex);
        }

        

        public void Deserialize(byte[] serializedMessage, int currentIndex)
        {
            Deserialize(serializedMessage, currentIndex);
        }

        public override void Deserialize(byte[] serializedMessage, ref int currentIndex)
        {
            int arraylength = -1;
            bool hasmetacomponents = false;
            object __thing;
            int piecesize = 0;
            byte[] thischunk, scratch1, scratch2;
            IntPtr h;

            
                //command
                command = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                command = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
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

            
                //command
                if (command == null)
                    command = "";
                scratch1 = Encoding.ASCII.GetBytes((string)command);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);

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

            
                //command
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                command = Encoding.ASCII.GetString(strbuf);
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            SystemCommandGoal other;
            try
            {
                other = (SystemCommandGoal)message;
            }
            catch
            {
                return false;
            }

            
                ret &= command == other.command;

            return ret;
        }
    }



    //$ACTION_GOAL_MESSAGE

	[ActionResultMessage]
        public class SystemCommandResult : InnerActionMessage
    {
        			public int return_code;
			public string std_out = "";
			public string std_err = "";



        public override string MD5Sum() { return "7cefa4da4dae2b13453d56f0d76c62c0"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"int32 return_code
string std_out
string std_err"; }
		public override string MessageType { get { return "test/SystemCommandResult"; } }
        public override bool IsServiceComponent() { return false; }

        public SystemCommandResult()
        {
            
        }

        public SystemCommandResult(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public SystemCommandResult(byte[] serializedMessage, ref int currentIndex)
        {
            Deserialize(serializedMessage, ref currentIndex);
        }

        

        public void Deserialize(byte[] serializedMessage, int currentIndex)
        {
            Deserialize(serializedMessage, currentIndex);
        }

        public override void Deserialize(byte[] serializedMessage, ref int currentIndex)
        {
            int arraylength = -1;
            bool hasmetacomponents = false;
            object __thing;
            int piecesize = 0;
            byte[] thischunk, scratch1, scratch2;
            IntPtr h;

            
                //return_code
                piecesize = Marshal.SizeOf(typeof(int));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                return_code = (int)Marshal.PtrToStructure(h, typeof(int));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //std_out
                std_out = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                std_out = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
                //std_err
                std_err = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                std_err = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
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

            
                //return_code
                scratch1 = new byte[Marshal.SizeOf(typeof(int))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(return_code, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //std_out
                if (std_out == null)
                    std_out = "";
                scratch1 = Encoding.ASCII.GetBytes((string)std_out);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
                //std_err
                if (std_err == null)
                    std_err = "";
                scratch1 = Encoding.ASCII.GetBytes((string)std_err);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);

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

            
                //return_code
                return_code = rand.Next();
                //std_out
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                std_out = Encoding.ASCII.GetString(strbuf);
                //std_err
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                std_err = Encoding.ASCII.GetString(strbuf);
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            SystemCommandResult other;
            try
            {
                other = (SystemCommandResult)message;
            }
            catch
            {
                return false;
            }

            
                ret &= return_code == other.return_code;
                ret &= std_out == other.std_out;
                ret &= std_err == other.std_err;

            return ret;
        }
    }



    //$ACTION_RESULT_MESSAGE


	[ActionFeedbackMessage]
        public class SystemCommandFeedback : InnerActionMessage
    {
        


        public override string MD5Sum() { return "d41d8cd98f00b204e9800998ecf8427e"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @""; }
		public override string MessageType { get { return "test/SystemCommandFeedback"; } }
        public override bool IsServiceComponent() { return false; }

        public SystemCommandFeedback()
        {
            
        }

        public SystemCommandFeedback(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public SystemCommandFeedback(byte[] serializedMessage, ref int currentIndex)
        {
            Deserialize(serializedMessage, ref currentIndex);
        }

        

        public void Deserialize(byte[] serializedMessage, int currentIndex)
        {
            Deserialize(serializedMessage, currentIndex);
        }

        public override void Deserialize(byte[] serializedMessage, ref int currentIndex)
        {
            int arraylength = -1;
            bool hasmetacomponents = false;
            object __thing;
            int piecesize = 0;
            byte[] thischunk, scratch1, scratch2;
            IntPtr h;

            
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

            
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            SystemCommandFeedback other;
            try
            {
                other = (SystemCommandFeedback)message;
            }
            catch
            {
                return false;
            }

            

            return ret;
        }
    }



    //$ACTION_FEEDBACK_MESSAGE
}