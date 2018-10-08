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

namespace Messages.motoman_msgs
{
    public class GetStatus : RosService
    {
        public override string ServiceType { get { return "motoman_msgs/GetStatus"; } }
        public override string ServiceDefinition() { return @"---
bool controller_ready
bool power_on
motoman_msgs/OperationMode operation_mode
motoman_msgs/PlayStatus play_status
motoman_msgs/JobStatus cur_job"; }
        public override string MD5Sum() { return "b1bb890b94d27482caf6353e294a152a"; }

        public GetStatus()
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


            public override string MD5Sum() { return "d41d8cd98f00b204e9800998ecf8427e"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return false; }
            public override string MessageDefinition() { return @""; }
			public override string MessageType { get { return "motoman_msgs/GetStatus__Request"; } }
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
                
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                motoman_msgs.GetStatus.Request other = (Messages.motoman_msgs.GetStatus.Request)____other;

                return ret;
            }
        }

        public class Response : RosMessage
        {
				public bool controller_ready;
				public bool power_on;
				public Messages.motoman_msgs.OperationMode operation_mode = new Messages.motoman_msgs.OperationMode();
				public Messages.motoman_msgs.PlayStatus play_status = new Messages.motoman_msgs.PlayStatus();
				public Messages.motoman_msgs.JobStatus cur_job = new Messages.motoman_msgs.JobStatus();



            public override string MD5Sum() { return "d41d8cd98f00b204e9800998ecf8427e"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"bool controller_ready
bool power_on
motoman_msgs/OperationMode operation_mode
motoman_msgs/PlayStatus play_status
motoman_msgs/JobStatus cur_job"; }
			public override string MessageType { get { return "motoman_msgs/GetStatus__Response"; } }
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
                
                //controller_ready
                controller_ready = serializedMessage[currentIndex++]==1;
                //power_on
                power_on = serializedMessage[currentIndex++]==1;
                //operation_mode
                operation_mode = new Messages.motoman_msgs.OperationMode(serializedMessage, ref currentIndex);
                //play_status
                play_status = new Messages.motoman_msgs.PlayStatus(serializedMessage, ref currentIndex);
                //cur_job
                cur_job = new Messages.motoman_msgs.JobStatus(serializedMessage, ref currentIndex);
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
                
                //controller_ready
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)controller_ready ? 1 : 0 );
                pieces.Add(thischunk);
                //power_on
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)power_on ? 1 : 0 );
                pieces.Add(thischunk);
                //operation_mode
                if (operation_mode == null)
                    operation_mode = new Messages.motoman_msgs.OperationMode();
                pieces.Add(operation_mode.Serialize(true));
                //play_status
                if (play_status == null)
                    play_status = new Messages.motoman_msgs.PlayStatus();
                pieces.Add(play_status.Serialize(true));
                //cur_job
                if (cur_job == null)
                    cur_job = new Messages.motoman_msgs.JobStatus();
                pieces.Add(cur_job.Serialize(true));
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
                
                //controller_ready
                controller_ready = rand.Next(2) == 1;
                //power_on
                power_on = rand.Next(2) == 1;
                //operation_mode
                operation_mode = new Messages.motoman_msgs.OperationMode();
                operation_mode.Randomize();
                //play_status
                play_status = new Messages.motoman_msgs.PlayStatus();
                play_status.Randomize();
                //cur_job
                cur_job = new Messages.motoman_msgs.JobStatus();
                cur_job.Randomize();
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                motoman_msgs.GetStatus.Response other = (Messages.motoman_msgs.GetStatus.Response)____other;

                ret &= controller_ready == other.controller_ready;
                ret &= power_on == other.power_on;
                ret &= operation_mode.Equals(other.operation_mode);
                ret &= play_status.Equals(other.play_status);
                ret &= cur_job.Equals(other.cur_job);
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
