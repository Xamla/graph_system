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

namespace Messages.xamlamoveit_msgs
{
    public class GetJointPostureWorldView : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/GetJointPostureWorldView"; } }
        public override string ServiceDefinition() { return @"string element_path
---
JointValuesPoint point
bool success
string error"; }
        public override string MD5Sum() { return "99a135f2727063a4fae16fd8b1fa7e79"; }

        public GetJointPostureWorldView()
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
				public string element_path = "";


            public override string MD5Sum() { return "c22aa4457a84d4326c63a929f33b8f7c"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return false; }
            public override string MessageDefinition() { return @"string element_path"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetJointPostureWorldView__Request"; } }
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
                
                //element_path
                element_path = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                element_path = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
                
                //element_path
                if (element_path == null)
                    element_path = "";
                scratch1 = Encoding.ASCII.GetBytes((string)element_path);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
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
                
                //element_path
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                element_path = Encoding.ASCII.GetString(strbuf);
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetJointPostureWorldView.Request other = (Messages.xamlamoveit_msgs.GetJointPostureWorldView.Request)____other;

                ret &= element_path == other.element_path;
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.xamlamoveit_msgs.JointValuesPoint point = new Messages.xamlamoveit_msgs.JointValuesPoint();
				public bool success;
				public string error = "";



            public override string MD5Sum() { return "c22aa4457a84d4326c63a929f33b8f7c"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"JointValuesPoint point
bool success
string error"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetJointPostureWorldView__Response"; } }
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
                
                //point
                point = new Messages.xamlamoveit_msgs.JointValuesPoint(serializedMessage, ref currentIndex);
                //success
                success = serializedMessage[currentIndex++]==1;
                //error
                error = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                error = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
                
                //point
                if (point == null)
                    point = new Messages.xamlamoveit_msgs.JointValuesPoint();
                pieces.Add(point.Serialize(true));
                //success
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)success ? 1 : 0 );
                pieces.Add(thischunk);
                //error
                if (error == null)
                    error = "";
                scratch1 = Encoding.ASCII.GetBytes((string)error);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
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
                
                //point
                point = new Messages.xamlamoveit_msgs.JointValuesPoint();
                point.Randomize();
                //success
                success = rand.Next(2) == 1;
                //error
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                error = Encoding.ASCII.GetString(strbuf);
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetJointPostureWorldView.Response other = (Messages.xamlamoveit_msgs.GetJointPostureWorldView.Response)____other;

                ret &= point.Equals(other.point);
                ret &= success == other.success;
                ret &= error == other.error;
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
