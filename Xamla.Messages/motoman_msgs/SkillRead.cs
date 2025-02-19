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
    public class SkillRead : RosService
    {
        public override string ServiceType { get { return "motoman_msgs/SkillRead"; } }
        public override string ServiceDefinition() { return @"---
bool success
string message
string[] cmd
bool[] skill_pending"; }
        public override string MD5Sum() { return "97ddd096fe9af1f371bf2cba9597db30"; }

        public SkillRead()
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
			public override string MessageType { get { return "motoman_msgs/SkillRead__Request"; } }
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
                motoman_msgs.SkillRead.Request other = (Messages.motoman_msgs.SkillRead.Request)____other;

                return ret;
            }
        }

        public class Response : RosMessage
        {
				public bool success;
				public string message = "";
				public string[] cmd;
				public bool[] skill_pending;



            public override string MD5Sum() { return "d41d8cd98f00b204e9800998ecf8427e"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return false; }
            public override string MessageDefinition() { return @"bool success
string message
string[] cmd
bool[] skill_pending"; }
			public override string MessageType { get { return "motoman_msgs/SkillRead__Response"; } }
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
                
                //success
                success = serializedMessage[currentIndex++]==1;
                //message
                message = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                message = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
                //cmd
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (cmd == null)
                    cmd = new string[arraylength];
                else
                    Array.Resize(ref cmd, arraylength);
                for (int i=0;i<cmd.Length; i++) {
                    //cmd[i]
                    cmd[i] = "";
                    piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                    currentIndex += 4;
                    cmd[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                    currentIndex += piecesize;
                }
                //skill_pending
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (skill_pending == null)
                    skill_pending = new bool[arraylength];
                else
                    Array.Resize(ref skill_pending, arraylength);
                for (int i=0;i<skill_pending.Length; i++) {
                    //skill_pending[i]
                    skill_pending[i] = serializedMessage[currentIndex++]==1;
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
                
                //success
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)success ? 1 : 0 );
                pieces.Add(thischunk);
                //message
                if (message == null)
                    message = "";
                scratch1 = Encoding.ASCII.GetBytes((string)message);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
                //cmd
                hasmetacomponents |= false;
                if (cmd == null)
                    cmd = new string[0];
                pieces.Add(BitConverter.GetBytes(cmd.Length));
                for (int i=0;i<cmd.Length; i++) {
                    //cmd[i]
                    if (cmd[i] == null)
                        cmd[i] = "";
                    scratch1 = Encoding.ASCII.GetBytes((string)cmd[i]);
                    thischunk = new byte[scratch1.Length + 4];
                    scratch2 = BitConverter.GetBytes(scratch1.Length);
                    Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                    Array.Copy(scratch2, thischunk, 4);
                    pieces.Add(thischunk);
                }
                //skill_pending
                hasmetacomponents |= false;
                if (skill_pending == null)
                    skill_pending = new bool[0];
                pieces.Add(BitConverter.GetBytes(skill_pending.Length));
                for (int i=0;i<skill_pending.Length; i++) {
                    //skill_pending[i]
                    thischunk = new byte[1];
                    thischunk[0] = (byte) ((bool)skill_pending[i] ? 1 : 0 );
                    pieces.Add(thischunk);
                }
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
                
                //success
                success = rand.Next(2) == 1;
                //message
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                message = Encoding.ASCII.GetString(strbuf);
                //cmd
                arraylength = rand.Next(10);
                if (cmd == null)
                    cmd = new string[arraylength];
                else
                    Array.Resize(ref cmd, arraylength);
                for (int i=0;i<cmd.Length; i++) {
                    //cmd[i]
                    strlength = rand.Next(100) + 1;
                    strbuf = new byte[strlength];
                    rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                    for (int __x__ = 0; __x__ < strlength; __x__++)
                        if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                            strbuf[__x__] = (byte)(rand.Next(254) + 1);
                    strbuf[strlength - 1] = 0; //null terminate
                    cmd[i] = Encoding.ASCII.GetString(strbuf);
                }
                //skill_pending
                arraylength = rand.Next(10);
                if (skill_pending == null)
                    skill_pending = new bool[arraylength];
                else
                    Array.Resize(ref skill_pending, arraylength);
                for (int i=0;i<skill_pending.Length; i++) {
                    //skill_pending[i]
                    skill_pending[i] = rand.Next(2) == 1;
                }
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                motoman_msgs.SkillRead.Response other = (Messages.motoman_msgs.SkillRead.Response)____other;

                ret &= success == other.success;
                ret &= message == other.message;
                if (cmd.Length != other.cmd.Length)
                    return false;
                for (int __i__=0; __i__ < cmd.Length; __i__++)
                {
                    ret &= cmd[__i__] == other.cmd[__i__];
                }
                if (skill_pending.Length != other.skill_pending.Length)
                    return false;
                for (int __i__=0; __i__ < skill_pending.Length; __i__++)
                {
                    ret &= skill_pending[__i__] == other.skill_pending[__i__];
                }
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
