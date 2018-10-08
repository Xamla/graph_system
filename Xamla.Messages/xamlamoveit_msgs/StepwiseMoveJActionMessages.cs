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


namespace Messages.xamlamoveit_msgs
{
	[ActionGoalMessage]
        public class StepwiseMoveJGoal : InnerActionMessage
    {
        			public Messages.trajectory_msgs.JointTrajectory trajectory = new Messages.trajectory_msgs.JointTrajectory();
			public bool check_collision;
			public double veloctiy_scaling;



        public override string MD5Sum() { return "8e8d3dc14424c3a4700ab87491e2f28f"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return true; }
        public override string MessageDefinition() { return @"trajectory_msgs/JointTrajectory trajectory
bool check_collision
float64 veloctiy_scaling"; }
		public override string MessageType { get { return "xamlamoveit_msgs/StepwiseMoveJGoal"; } }
        public override bool IsServiceComponent() { return false; }

        public StepwiseMoveJGoal()
        {
            
        }

        public StepwiseMoveJGoal(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public StepwiseMoveJGoal(byte[] serializedMessage, ref int currentIndex)
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
                //check_collision
                check_collision = serializedMessage[currentIndex++]==1;
                //veloctiy_scaling
                piecesize = Marshal.SizeOf(typeof(double));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                veloctiy_scaling = (double)Marshal.PtrToStructure(h, typeof(double));
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

            
                //trajectory
                if (trajectory == null)
                    trajectory = new Messages.trajectory_msgs.JointTrajectory();
                pieces.Add(trajectory.Serialize(true));
                //check_collision
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)check_collision ? 1 : 0 );
                pieces.Add(thischunk);
                //veloctiy_scaling
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(veloctiy_scaling, h.AddrOfPinnedObject(), false);
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

            
                //trajectory
                trajectory = new Messages.trajectory_msgs.JointTrajectory();
                trajectory.Randomize();
                //check_collision
                check_collision = rand.Next(2) == 1;
                //veloctiy_scaling
                veloctiy_scaling = (rand.Next() + rand.NextDouble());
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            StepwiseMoveJGoal other;
            try
            {
                other = (StepwiseMoveJGoal)message;
            }
            catch
            {
                return false;
            }

            
                ret &= trajectory.Equals(other.trajectory);
                ret &= check_collision == other.check_collision;
                ret &= veloctiy_scaling == other.veloctiy_scaling;

            return ret;
        }
    }



    //$ACTION_GOAL_MESSAGE

	[ActionResultMessage]
        public class StepwiseMoveJResult : InnerActionMessage
    {
        			public int result;



        public override string MD5Sum() { return "034a8e20d6a306665e3a5b340fab3f09"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"int32 result"; }
		public override string MessageType { get { return "xamlamoveit_msgs/StepwiseMoveJResult"; } }
        public override bool IsServiceComponent() { return false; }

        public StepwiseMoveJResult()
        {
            
        }

        public StepwiseMoveJResult(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public StepwiseMoveJResult(byte[] serializedMessage, ref int currentIndex)
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

            
                //result
                piecesize = Marshal.SizeOf(typeof(int));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                result = (int)Marshal.PtrToStructure(h, typeof(int));
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

            
                //result
                scratch1 = new byte[Marshal.SizeOf(typeof(int))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(result, h.AddrOfPinnedObject(), false);
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

            
                //result
                result = rand.Next();
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            StepwiseMoveJResult other;
            try
            {
                other = (StepwiseMoveJResult)message;
            }
            catch
            {
                return false;
            }

            
                ret &= result == other.result;

            return ret;
        }
    }



    //$ACTION_RESULT_MESSAGE


	[ActionFeedbackMessage]
        public class StepwiseMoveJFeedback : InnerActionMessage
    {
        			public bool isconverged;



        public override string MD5Sum() { return "8cdd7ff86298bccaacf17f4bc4cf27ae"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"bool isconverged"; }
		public override string MessageType { get { return "xamlamoveit_msgs/StepwiseMoveJFeedback"; } }
        public override bool IsServiceComponent() { return false; }

        public StepwiseMoveJFeedback()
        {
            
        }

        public StepwiseMoveJFeedback(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public StepwiseMoveJFeedback(byte[] serializedMessage, ref int currentIndex)
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

            
                //isconverged
                isconverged = serializedMessage[currentIndex++]==1;
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

            
                //isconverged
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)isconverged ? 1 : 0 );
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

            
                //isconverged
                isconverged = rand.Next(2) == 1;
        }

        public override bool Equals(RosMessage message)
        {
            if (message == null)
            {
                return false;
            }
            bool ret = true;
            StepwiseMoveJFeedback other;
            try
            {
                other = (StepwiseMoveJFeedback)message;
            }
            catch
            {
                return false;
            }

            
                ret &= isconverged == other.isconverged;

            return ret;
        }
    }



    //$ACTION_FEEDBACK_MESSAGE
}