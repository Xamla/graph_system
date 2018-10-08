using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using uint8 = System.Byte;
using Uml.Robotics.Ros;


using Messages.std_msgs;
using String=System.String;
using Messages.geometry_msgs;

namespace Messages.test
{
    public class EchoTrajectory : RosService
    {
        public override string ServiceType { get { return "test/EchoTrajectory"; } }
        public override string ServiceDefinition() { return @"trajectory_msgs/JointTrajectory trajectory
---
trajectory_msgs/JointTrajectory trajectory"; }
        public override string MD5Sum() { return "0fba266499a73d14a90e301a20ed75d9"; }

        public EchoTrajectory()
        {
            InitSubtypes(new Request(), new Response());
        }

        public Response Invoke(Func<Request, Response> fn, Request req)
        {
            RosServiceDelegate rsd = (m)=>{
                Request r = m as Request;
                if (r == null)
                    throw new Exception("Invalid Service Request Type");
                return fn(r);
            };
            return (Response)GeneralInvoke(rsd, (RosMessage)req);
        }

        public Request req { get { return (Request)RequestMessage; } set { RequestMessage = (RosMessage)value; } }
        public Response resp { get { return (Response)ResponseMessage; } set { ResponseMessage = (RosMessage)value; } }

        public class Request : RosMessage
        {
				public Messages.trajectory_msgs.JointTrajectory trajectory = new Messages.trajectory_msgs.JointTrajectory();


            public override string MD5Sum() { return "2a0eff76c870e8595636c2a562ca298e"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"trajectory_msgs/JointTrajectory trajectory"; }
			public override string MessageType { get { return "test/EchoTrajectory__Request"; } }
            public override bool IsServiceComponent() { return true; }

            public Request()
            {
                
            }

            public Request(byte[] serializedMessage)
            {
                Deserialize(serializedMessage);
            }

            public Request(byte[] serializedMessage, ref int currentIndex)
            {
                Deserialize(serializedMessage, ref currentIndex);
            }

    

            public override void Deserialize(byte[] serializedMessage, ref int currentIndex)
            {
                int arraylength=-1;
                bool hasmetacomponents = false;
                byte[] thischunk, scratch1, scratch2;
                object __thing;
                int piecesize=0;
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
                //combine every array in pieces into one array and return it
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
                int arraylength=-1;
                Random rand = new Random();
                int strlength;
                byte[] strbuf, myByte;
                
                //trajectory
                trajectory = new Messages.trajectory_msgs.JointTrajectory();
                trajectory.Randomize();
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                test.EchoTrajectory.Request other = (Messages.test.EchoTrajectory.Request)____other;

                ret &= trajectory.Equals(other.trajectory);
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.trajectory_msgs.JointTrajectory trajectory = new Messages.trajectory_msgs.JointTrajectory();



            public override string MD5Sum() { return "2a0eff76c870e8595636c2a562ca298e"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"trajectory_msgs/JointTrajectory trajectory"; }
			public override string MessageType { get { return "test/EchoTrajectory__Response"; } }
            public override bool IsServiceComponent() { return true; }

            public Response()
            {
                
            }

            public Response(byte[] serializedMessage)
            {
                Deserialize(serializedMessage);
            }

            public Response(byte[] serializedMessage, ref int currentIndex)
            {
                Deserialize(serializedMessage, ref currentIndex);
            }

	

            public override void Deserialize(byte[] serializedMessage, ref int currentIndex)
            {
                int arraylength = -1;
                bool hasmetacomponents = false;
                int piecesize = 0;
                byte[] thischunk, scratch1, scratch2;
                IntPtr h;
                object __thing;
                
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
                //combine every array in pieces into one array and return it
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

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                test.EchoTrajectory.Response other = (Messages.test.EchoTrajectory.Response)____other;

                ret &= trajectory.Equals(other.trajectory);
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
