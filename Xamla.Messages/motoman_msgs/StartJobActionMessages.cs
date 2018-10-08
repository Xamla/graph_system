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


namespace Messages.motoman_msgs
{
	[ActionGoalMessage]
        public class StartJobGoal : InnerActionMessage
    {
        			public short  task_no;
			public string job_name = "";



        public override string MD5Sum() { return "260f24f5a3856d9aa2d260116d8b13a3"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"int16  task_no
string job_name"; }
		public override string MessageType { get { return "motoman_msgs/StartJobGoal"; } }
        public override bool IsServiceComponent() { return false; }

        public StartJobGoal()
        {
            
        }

        public StartJobGoal(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public StartJobGoal(byte[] serializedMessage, ref int currentIndex)
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

            
                //task_no
                piecesize = Marshal.SizeOf(typeof(short));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                task_no = (short)Marshal.PtrToStructure(h, typeof(short));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //job_name
                job_name = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                job_name = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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

            
                //task_no
                scratch1 = new byte[Marshal.SizeOf(typeof(short))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(task_no, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //job_name
                if (job_name == null)
                    job_name = "";
                scratch1 = Encoding.ASCII.GetBytes((string)job_name);
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

            
                //task_no
                task_no = (System.Int16)rand.Next(System.Int16.MaxValue + 1);
                //job_name
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                job_name = Encoding.ASCII.GetString(strbuf);
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            StartJobGoal other;
            try
            {
                other = (StartJobGoal)message;
            }
            catch
            {
                return false;
            }

            
                ret &= task_no == other.task_no;
                ret &= job_name == other.job_name;

            return ret;
        }
    }



    //$ACTION_GOAL_MESSAGE

	[ActionResultMessage]
        public class StartJobResult : InnerActionMessage
    {
        			public bool success;
			public string status_message = "";
			public int err_no;



        public override string MD5Sum() { return "025304c8394914e678c1968d750eb5ef"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"bool success
string status_message
int32 err_no"; }
		public override string MessageType { get { return "motoman_msgs/StartJobResult"; } }
        public override bool IsServiceComponent() { return false; }

        public StartJobResult()
        {
            
        }

        public StartJobResult(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public StartJobResult(byte[] serializedMessage, ref int currentIndex)
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

            
                //success
                success = serializedMessage[currentIndex++]==1;
                //status_message
                status_message = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                status_message = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
                //err_no
                piecesize = Marshal.SizeOf(typeof(int));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                err_no = (int)Marshal.PtrToStructure(h, typeof(int));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
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

            
                //success
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)success ? 1 : 0 );
                pieces.Add(thischunk);
                //status_message
                if (status_message == null)
                    status_message = "";
                scratch1 = Encoding.ASCII.GetBytes((string)status_message);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
                //err_no
                scratch1 = new byte[Marshal.SizeOf(typeof(int))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(err_no, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);

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

            
                //success
                success = rand.Next(2) == 1;
                //status_message
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                status_message = Encoding.ASCII.GetString(strbuf);
                //err_no
                err_no = rand.Next();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            StartJobResult other;
            try
            {
                other = (StartJobResult)message;
            }
            catch
            {
                return false;
            }

            
                ret &= success == other.success;
                ret &= status_message == other.status_message;
                ret &= err_no == other.err_no;

            return ret;
        }
    }



    //$ACTION_RESULT_MESSAGE


	[ActionFeedbackMessage]
        public class StartJobFeedback : InnerActionMessage
    {
        			public string status_message = "";
			public ushort job_line;
			public ushort step;
			public string job_name = "";



        public override string MD5Sum() { return "0673950f1bf18865e26731731183a623"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"string status_message
uint16 job_line
uint16 step
string job_name"; }
		public override string MessageType { get { return "motoman_msgs/StartJobFeedback"; } }
        public override bool IsServiceComponent() { return false; }

        public StartJobFeedback()
        {
            
        }

        public StartJobFeedback(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public StartJobFeedback(byte[] serializedMessage, ref int currentIndex)
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

            
                //status_message
                status_message = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                status_message = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
                //job_line
                piecesize = Marshal.SizeOf(typeof(ushort));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                job_line = (ushort)Marshal.PtrToStructure(h, typeof(ushort));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //step
                piecesize = Marshal.SizeOf(typeof(ushort));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                step = (ushort)Marshal.PtrToStructure(h, typeof(ushort));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //job_name
                job_name = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                job_name = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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

            
                //status_message
                if (status_message == null)
                    status_message = "";
                scratch1 = Encoding.ASCII.GetBytes((string)status_message);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
                //job_line
                scratch1 = new byte[Marshal.SizeOf(typeof(ushort))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(job_line, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //step
                scratch1 = new byte[Marshal.SizeOf(typeof(ushort))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(step, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //job_name
                if (job_name == null)
                    job_name = "";
                scratch1 = Encoding.ASCII.GetBytes((string)job_name);
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

            
                //status_message
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                status_message = Encoding.ASCII.GetString(strbuf);
                //job_line
                job_line = (System.UInt16)rand.Next(System.UInt16.MaxValue + 1);
                //step
                step = (System.UInt16)rand.Next(System.UInt16.MaxValue + 1);
                //job_name
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                job_name = Encoding.ASCII.GetString(strbuf);
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            StartJobFeedback other;
            try
            {
                other = (StartJobFeedback)message;
            }
            catch
            {
                return false;
            }

            
                ret &= status_message == other.status_message;
                ret &= job_line == other.job_line;
                ret &= step == other.step;
                ret &= job_name == other.job_name;

            return ret;
        }
    }



    //$ACTION_FEEDBACK_MESSAGE
}