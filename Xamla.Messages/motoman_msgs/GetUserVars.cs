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
    public class GetUserVars : RosService
    {
        public override string ServiceType { get { return "motoman_msgs/GetUserVars"; } }
        public override string ServiceDefinition() { return @"UserVarPrimitive[] variables
---
bool success
string message
int32 err_no
UserVarPrimitive[] variables"; }
        public override string MD5Sum() { return "65eb7d315acef52c57f04f0f209b23da"; }

        public GetUserVars()
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
				public Messages.motoman_msgs.UserVarPrimitive[] variables;


            public override string MD5Sum() { return "3e29dd4ed4f1a84ef2dc77383af69faa"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"UserVarPrimitive[] variables"; }
			public override string MessageType { get { return "motoman_msgs/GetUserVars__Request"; } }
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
                
                //variables
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (variables == null)
                    variables = new Messages.motoman_msgs.UserVarPrimitive[arraylength];
                else
                    Array.Resize(ref variables, arraylength);
                for (int i=0;i<variables.Length; i++) {
                    //variables[i]
                    variables[i] = new Messages.motoman_msgs.UserVarPrimitive(serializedMessage, ref currentIndex);
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
                
                //variables
                hasmetacomponents |= true;
                if (variables == null)
                    variables = new Messages.motoman_msgs.UserVarPrimitive[0];
                pieces.Add(BitConverter.GetBytes(variables.Length));
                for (int i=0;i<variables.Length; i++) {
                    //variables[i]
                    if (variables[i] == null)
                        variables[i] = new Messages.motoman_msgs.UserVarPrimitive();
                    pieces.Add(variables[i].Serialize(true));
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
                int arraylength=-1;
                Random rand = new Random();
                int strlength;
                byte[] strbuf, myByte;
                
                //variables
                arraylength = rand.Next(10);
                if (variables == null)
                    variables = new Messages.motoman_msgs.UserVarPrimitive[arraylength];
                else
                    Array.Resize(ref variables, arraylength);
                for (int i=0;i<variables.Length; i++) {
                    //variables[i]
                    variables[i] = new Messages.motoman_msgs.UserVarPrimitive();
                    variables[i].Randomize();
                }
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                motoman_msgs.GetUserVars.Request other = (Messages.motoman_msgs.GetUserVars.Request)____other;

                if (variables.Length != other.variables.Length)
                    return false;
                for (int __i__=0; __i__ < variables.Length; __i__++)
                {
                    ret &= variables[__i__].Equals(other.variables[__i__]);
                }
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public bool success;
				public string message = "";
				public int err_no;
				public Messages.motoman_msgs.UserVarPrimitive[] variables;



            public override string MD5Sum() { return "3e29dd4ed4f1a84ef2dc77383af69faa"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"bool success
string message
int32 err_no
UserVarPrimitive[] variables"; }
			public override string MessageType { get { return "motoman_msgs/GetUserVars__Response"; } }
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
                //variables
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (variables == null)
                    variables = new Messages.motoman_msgs.UserVarPrimitive[arraylength];
                else
                    Array.Resize(ref variables, arraylength);
                for (int i=0;i<variables.Length; i++) {
                    //variables[i]
                    variables[i] = new Messages.motoman_msgs.UserVarPrimitive(serializedMessage, ref currentIndex);
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
                //err_no
                scratch1 = new byte[Marshal.SizeOf(typeof(int))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(err_no, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //variables
                hasmetacomponents |= true;
                if (variables == null)
                    variables = new Messages.motoman_msgs.UserVarPrimitive[0];
                pieces.Add(BitConverter.GetBytes(variables.Length));
                for (int i=0;i<variables.Length; i++) {
                    //variables[i]
                    if (variables[i] == null)
                        variables[i] = new Messages.motoman_msgs.UserVarPrimitive();
                    pieces.Add(variables[i].Serialize(true));
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
                //err_no
                err_no = rand.Next();
                //variables
                arraylength = rand.Next(10);
                if (variables == null)
                    variables = new Messages.motoman_msgs.UserVarPrimitive[arraylength];
                else
                    Array.Resize(ref variables, arraylength);
                for (int i=0;i<variables.Length; i++) {
                    //variables[i]
                    variables[i] = new Messages.motoman_msgs.UserVarPrimitive();
                    variables[i].Randomize();
                }
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                motoman_msgs.GetUserVars.Response other = (Messages.motoman_msgs.GetUserVars.Response)____other;

                ret &= success == other.success;
                ret &= message == other.message;
                ret &= err_no == other.err_no;
                if (variables.Length != other.variables.Length)
                    return false;
                for (int __i__=0; __i__ < variables.Length; __i__++)
                {
                    ret &= variables[__i__].Equals(other.variables[__i__]);
                }
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
