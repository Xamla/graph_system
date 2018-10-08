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


namespace Messages.rosgardener
{
	[ActionGoalMessage]
        public class RobochatQueryGoal : InnerActionMessage
    {
        			public Messages.rosgardener.MessageCommand command = new Messages.rosgardener.MessageCommand();



        public override string MD5Sum() { return "7750d252d88322c495659bb1c4a0da38"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"MessageCommand command"; }
		public override string MessageType { get { return "rosgardener/RobochatQueryGoal"; } }
        public override bool IsServiceComponent() { return false; }

        public RobochatQueryGoal()
        {
            
        }

        public RobochatQueryGoal(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public RobochatQueryGoal(byte[] serializedMessage, ref int currentIndex)
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
                command = new Messages.rosgardener.MessageCommand(serializedMessage, ref currentIndex);
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
                    command = new Messages.rosgardener.MessageCommand();
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
                command = new Messages.rosgardener.MessageCommand();
                command.Randomize();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            RobochatQueryGoal other;
            try
            {
                other = (RobochatQueryGoal)message;
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
        public class RobochatQueryResult : InnerActionMessage
    {
        			public Messages.rosgardener.MessageCommand diff = new Messages.rosgardener.MessageCommand();



        public override string MD5Sum() { return "16913035fef4737280aa75cc63c0edc5"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"MessageCommand diff"; }
		public override string MessageType { get { return "rosgardener/RobochatQueryResult"; } }
        public override bool IsServiceComponent() { return false; }

        public RobochatQueryResult()
        {
            
        }

        public RobochatQueryResult(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public RobochatQueryResult(byte[] serializedMessage, ref int currentIndex)
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

            
                //diff
                diff = new Messages.rosgardener.MessageCommand(serializedMessage, ref currentIndex);
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

            
                //diff
                if (diff == null)
                    diff = new Messages.rosgardener.MessageCommand();
                pieces.Add(diff.Serialize(true));

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

            
                //diff
                diff = new Messages.rosgardener.MessageCommand();
                diff.Randomize();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            RobochatQueryResult other;
            try
            {
                other = (RobochatQueryResult)message;
            }
            catch
            {
                return false;
            }

            
                ret &= diff.Equals(other.diff);

            return ret;
        }
    }



    //$ACTION_RESULT_MESSAGE


	[ActionFeedbackMessage]
        public class RobochatQueryFeedback : InnerActionMessage
    {
        


        public override string MD5Sum() { return "d41d8cd98f00b204e9800998ecf8427e"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @""; }
		public override string MessageType { get { return "rosgardener/RobochatQueryFeedback"; } }
        public override bool IsServiceComponent() { return false; }

        public RobochatQueryFeedback()
        {
            
        }

        public RobochatQueryFeedback(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public RobochatQueryFeedback(byte[] serializedMessage, ref int currentIndex)
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
            RobochatQueryFeedback other;
            try
            {
                other = (RobochatQueryFeedback)message;
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