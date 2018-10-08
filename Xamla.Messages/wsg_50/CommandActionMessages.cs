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


namespace Messages.wsg_50
{
	[ActionGoalMessage]
        public class CommandGoal : InnerActionMessage
    {
        			public Messages.wsg_50.Command command = new Messages.wsg_50.Command();



        public override string MD5Sum() { return "ff5b3a57fd28a3137490b65344f9287f"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"Command command"; }
		public override string MessageType { get { return "wsg_50/CommandGoal"; } }
        public override bool IsServiceComponent() { return false; }

        public CommandGoal()
        {
            
        }

        public CommandGoal(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public CommandGoal(byte[] serializedMessage, ref int currentIndex)
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
                command = new Messages.wsg_50.Command(serializedMessage, ref currentIndex);
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
                    command = new Messages.wsg_50.Command();
                pieces.Add(command.Serialize(true));

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
                command = new Messages.wsg_50.Command();
                command.Randomize();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            CommandGoal other;
            try
            {
                other = (CommandGoal)message;
            }
            catch
            {
                return false;
            }

            
                ret &= command.Equals(other.command);

            return ret;
        }
    }



    //$ACTION_GOAL_MESSAGE

	[ActionResultMessage]
        public class CommandResult : InnerActionMessage
    {
        			public Messages.wsg_50.Status status = new Messages.wsg_50.Status();



        public override string MD5Sum() { return "9e2023eb0ed3d0e222acb4c59b97af57"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"Status status"; }
		public override string MessageType { get { return "wsg_50/CommandResult"; } }
        public override bool IsServiceComponent() { return false; }

        public CommandResult()
        {
            
        }

        public CommandResult(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public CommandResult(byte[] serializedMessage, ref int currentIndex)
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

            
                //status
                status = new Messages.wsg_50.Status(serializedMessage, ref currentIndex);
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

            
                //status
                if (status == null)
                    status = new Messages.wsg_50.Status();
                pieces.Add(status.Serialize(true));

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

            
                //status
                status = new Messages.wsg_50.Status();
                status.Randomize();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            CommandResult other;
            try
            {
                other = (CommandResult)message;
            }
            catch
            {
                return false;
            }

            
                ret &= status.Equals(other.status);

            return ret;
        }
    }



    //$ACTION_RESULT_MESSAGE


	[ActionFeedbackMessage]
        public class CommandFeedback : InnerActionMessage
    {
        			public Messages.wsg_50.Status status = new Messages.wsg_50.Status();



        public override string MD5Sum() { return "9e2023eb0ed3d0e222acb4c59b97af57"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"Status status"; }
		public override string MessageType { get { return "wsg_50/CommandFeedback"; } }
        public override bool IsServiceComponent() { return false; }

        public CommandFeedback()
        {
            
        }

        public CommandFeedback(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public CommandFeedback(byte[] serializedMessage, ref int currentIndex)
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

            
                //status
                status = new Messages.wsg_50.Status(serializedMessage, ref currentIndex);
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

            
                //status
                if (status == null)
                    status = new Messages.wsg_50.Status();
                pieces.Add(status.Serialize(true));

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

            
                //status
                status = new Messages.wsg_50.Status();
                status.Randomize();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            CommandFeedback other;
            try
            {
                other = (CommandFeedback)message;
            }
            catch
            {
                return false;
            }

            
                ret &= status.Equals(other.status);

            return ret;
        }
    }



    //$ACTION_FEEDBACK_MESSAGE
}