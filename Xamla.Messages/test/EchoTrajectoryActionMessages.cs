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
        public class EchoTrajectoryGoal : InnerActionMessage
    {
        			public Messages.trajectory_msgs.JointTrajectory trajectory = new Messages.trajectory_msgs.JointTrajectory();



        public override string MD5Sum() { return "2a0eff76c870e8595636c2a562ca298e"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"trajectory_msgs/JointTrajectory trajectory"; }
		public override string MessageType { get { return "test/EchoTrajectoryGoal"; } }
        public override bool IsServiceComponent() { return false; }

        public EchoTrajectoryGoal()
        {
            
        }

        public EchoTrajectoryGoal(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public EchoTrajectoryGoal(byte[] serializedMessage, ref int currentIndex)
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

            
                //trajectory
                trajectory = new Messages.trajectory_msgs.JointTrajectory(serializedMessage, ref currentIndex);
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

            
                //trajectory
                if (trajectory == null)
                    trajectory = new Messages.trajectory_msgs.JointTrajectory();
                pieces.Add(trajectory.Serialize(true));

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

            
                //trajectory
                trajectory = new Messages.trajectory_msgs.JointTrajectory();
                trajectory.Randomize();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            EchoTrajectoryGoal other;
            try
            {
                other = (EchoTrajectoryGoal)message;
            }
            catch
            {
                return false;
            }

            
                ret &= trajectory.Equals(other.trajectory);

            return ret;
        }
    }



    //$ACTION_GOAL_MESSAGE

	[ActionResultMessage]
        public class EchoTrajectoryResult : InnerActionMessage
    {
        			public Messages.trajectory_msgs.JointTrajectory trajectory = new Messages.trajectory_msgs.JointTrajectory();



        public override string MD5Sum() { return "2a0eff76c870e8595636c2a562ca298e"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"trajectory_msgs/JointTrajectory trajectory"; }
		public override string MessageType { get { return "test/EchoTrajectoryResult"; } }
        public override bool IsServiceComponent() { return false; }

        public EchoTrajectoryResult()
        {
            
        }

        public EchoTrajectoryResult(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public EchoTrajectoryResult(byte[] serializedMessage, ref int currentIndex)
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

            
                //trajectory
                trajectory = new Messages.trajectory_msgs.JointTrajectory(serializedMessage, ref currentIndex);
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

            
                //trajectory
                if (trajectory == null)
                    trajectory = new Messages.trajectory_msgs.JointTrajectory();
                pieces.Add(trajectory.Serialize(true));

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

            
                //trajectory
                trajectory = new Messages.trajectory_msgs.JointTrajectory();
                trajectory.Randomize();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            EchoTrajectoryResult other;
            try
            {
                other = (EchoTrajectoryResult)message;
            }
            catch
            {
                return false;
            }

            
                ret &= trajectory.Equals(other.trajectory);

            return ret;
        }
    }



    //$ACTION_RESULT_MESSAGE


	[ActionFeedbackMessage]
        public class EchoTrajectoryFeedback : InnerActionMessage
    {
        


        public override string MD5Sum() { return "d41d8cd98f00b204e9800998ecf8427e"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @""; }
		public override string MessageType { get { return "test/EchoTrajectoryFeedback"; } }
        public override bool IsServiceComponent() { return false; }

        public EchoTrajectoryFeedback()
        {
            
        }

        public EchoTrajectoryFeedback(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public EchoTrajectoryFeedback(byte[] serializedMessage, ref int currentIndex)
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
            EchoTrajectoryFeedback other;
            try
            {
                other = (EchoTrajectoryFeedback)message;
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