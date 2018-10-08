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
    public class QueryLock : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/QueryLock"; } }
        public override string ServiceDefinition() { return @"bool release
string[] id_resources
string id_lock
---
bool success
string[] id_resources
string id_lock
time creation_date
time expiration_date
string error_msg"; }
        public override string MD5Sum() { return "e80966f1e16b7d765be3ee506b1a3910"; }

        public QueryLock()
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
				public bool release;
				public string[] id_resources;
				public string id_lock = "";


            public override string MD5Sum() { return "6c120145c20957c4a84446fb37724140"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return false; }
            public override string MessageDefinition() { return @"bool release
string[] id_resources
string id_lock"; }
			public override string MessageType { get { return "xamlamoveit_msgs/QueryLock__Request"; } }
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
                
                //release
                release = serializedMessage[currentIndex++]==1;
                //id_resources
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (id_resources == null)
                    id_resources = new string[arraylength];
                else
                    Array.Resize(ref id_resources, arraylength);
                for (int i=0;i<id_resources.Length; i++) {
                    //id_resources[i]
                    id_resources[i] = "";
                    piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                    currentIndex += 4;
                    id_resources[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                    currentIndex += piecesize;
                }
                //id_lock
                id_lock = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                id_lock = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
                
                //release
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)release ? 1 : 0 );
                pieces.Add(thischunk);
                //id_resources
                hasmetacomponents |= false;
                if (id_resources == null)
                    id_resources = new string[0];
                pieces.Add(BitConverter.GetBytes(id_resources.Length));
                for (int i=0;i<id_resources.Length; i++) {
                    //id_resources[i]
                    if (id_resources[i] == null)
                        id_resources[i] = "";
                    scratch1 = Encoding.ASCII.GetBytes((string)id_resources[i]);
                    thischunk = new byte[scratch1.Length + 4];
                    scratch2 = BitConverter.GetBytes(scratch1.Length);
                    Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                    Array.Copy(scratch2, thischunk, 4);
                    pieces.Add(thischunk);
                }
                //id_lock
                if (id_lock == null)
                    id_lock = "";
                scratch1 = Encoding.ASCII.GetBytes((string)id_lock);
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
                
                //release
                release = rand.Next(2) == 1;
                //id_resources
                arraylength = rand.Next(10);
                if (id_resources == null)
                    id_resources = new string[arraylength];
                else
                    Array.Resize(ref id_resources, arraylength);
                for (int i=0;i<id_resources.Length; i++) {
                    //id_resources[i]
                    strlength = rand.Next(100) + 1;
                    strbuf = new byte[strlength];
                    rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                    for (int __x__ = 0; __x__ < strlength; __x__++)
                        if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                            strbuf[__x__] = (byte)(rand.Next(254) + 1);
                    strbuf[strlength - 1] = 0; //null terminate
                    id_resources[i] = Encoding.ASCII.GetString(strbuf);
                }
                //id_lock
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                id_lock = Encoding.ASCII.GetString(strbuf);
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.QueryLock.Request other = (Messages.xamlamoveit_msgs.QueryLock.Request)____other;

                ret &= release == other.release;
                if (id_resources.Length != other.id_resources.Length)
                    return false;
                for (int __i__=0; __i__ < id_resources.Length; __i__++)
                {
                    ret &= id_resources[__i__] == other.id_resources[__i__];
                }
                ret &= id_lock == other.id_lock;
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public bool success;
				public string[] id_resources;
				public string id_lock = "";
				public Time creation_date = new Time();
				public Time expiration_date = new Time();
				public string error_msg = "";



            public override string MD5Sum() { return "6c120145c20957c4a84446fb37724140"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return false; }
            public override string MessageDefinition() { return @"bool success
string[] id_resources
string id_lock
time creation_date
time expiration_date
string error_msg"; }
			public override string MessageType { get { return "xamlamoveit_msgs/QueryLock__Response"; } }
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
                //id_resources
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (id_resources == null)
                    id_resources = new string[arraylength];
                else
                    Array.Resize(ref id_resources, arraylength);
                for (int i=0;i<id_resources.Length; i++) {
                    //id_resources[i]
                    id_resources[i] = "";
                    piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                    currentIndex += 4;
                    id_resources[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                    currentIndex += piecesize;
                }
                //id_lock
                id_lock = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                id_lock = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
                //creation_date
                creation_date = new Time(new TimeData(
                        BitConverter.ToUInt32(serializedMessage, currentIndex),
                        BitConverter.ToUInt32(serializedMessage, currentIndex+Marshal.SizeOf(typeof(System.Int32)))));
                currentIndex += 2*Marshal.SizeOf(typeof(System.Int32));
                //expiration_date
                expiration_date = new Time(new TimeData(
                        BitConverter.ToUInt32(serializedMessage, currentIndex),
                        BitConverter.ToUInt32(serializedMessage, currentIndex+Marshal.SizeOf(typeof(System.Int32)))));
                currentIndex += 2*Marshal.SizeOf(typeof(System.Int32));
                //error_msg
                error_msg = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                error_msg = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
                
                //success
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)success ? 1 : 0 );
                pieces.Add(thischunk);
                //id_resources
                hasmetacomponents |= false;
                if (id_resources == null)
                    id_resources = new string[0];
                pieces.Add(BitConverter.GetBytes(id_resources.Length));
                for (int i=0;i<id_resources.Length; i++) {
                    //id_resources[i]
                    if (id_resources[i] == null)
                        id_resources[i] = "";
                    scratch1 = Encoding.ASCII.GetBytes((string)id_resources[i]);
                    thischunk = new byte[scratch1.Length + 4];
                    scratch2 = BitConverter.GetBytes(scratch1.Length);
                    Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                    Array.Copy(scratch2, thischunk, 4);
                    pieces.Add(thischunk);
                }
                //id_lock
                if (id_lock == null)
                    id_lock = "";
                scratch1 = Encoding.ASCII.GetBytes((string)id_lock);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
                //creation_date
                pieces.Add(BitConverter.GetBytes(creation_date.data.sec));
                pieces.Add(BitConverter.GetBytes(creation_date.data.nsec));
                //expiration_date
                pieces.Add(BitConverter.GetBytes(expiration_date.data.sec));
                pieces.Add(BitConverter.GetBytes(expiration_date.data.nsec));
                //error_msg
                if (error_msg == null)
                    error_msg = "";
                scratch1 = Encoding.ASCII.GetBytes((string)error_msg);
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
                
                //success
                success = rand.Next(2) == 1;
                //id_resources
                arraylength = rand.Next(10);
                if (id_resources == null)
                    id_resources = new string[arraylength];
                else
                    Array.Resize(ref id_resources, arraylength);
                for (int i=0;i<id_resources.Length; i++) {
                    //id_resources[i]
                    strlength = rand.Next(100) + 1;
                    strbuf = new byte[strlength];
                    rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                    for (int __x__ = 0; __x__ < strlength; __x__++)
                        if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                            strbuf[__x__] = (byte)(rand.Next(254) + 1);
                    strbuf[strlength - 1] = 0; //null terminate
                    id_resources[i] = Encoding.ASCII.GetString(strbuf);
                }
                //id_lock
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                id_lock = Encoding.ASCII.GetString(strbuf);
                //creation_date
                creation_date = new Time(new TimeData(
                        Convert.ToUInt32(rand.Next()),
                        Convert.ToUInt32(rand.Next())));
                //expiration_date
                expiration_date = new Time(new TimeData(
                        Convert.ToUInt32(rand.Next()),
                        Convert.ToUInt32(rand.Next())));
                //error_msg
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                error_msg = Encoding.ASCII.GetString(strbuf);
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.QueryLock.Response other = (Messages.xamlamoveit_msgs.QueryLock.Response)____other;

                ret &= success == other.success;
                if (id_resources.Length != other.id_resources.Length)
                    return false;
                for (int __i__=0; __i__ < id_resources.Length; __i__++)
                {
                    ret &= id_resources[__i__] == other.id_resources[__i__];
                }
                ret &= id_lock == other.id_lock;
                ret &= creation_date.data.Equals(other.creation_date.data);
                ret &= expiration_date.data.Equals(other.expiration_date.data);
                ret &= error_msg == other.error_msg;
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
