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
    public class QueryJointValuesWorldView : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/QueryJointValuesWorldView"; } }
        public override string ServiceDefinition() { return @"string prefix
string folder_path
bool recursive
---
JointValuesPoint[] points
string[] names
string[] element_paths
bool success
string error"; }
        public override string MD5Sum() { return "6ee79cc336fbeb5f05a563a56ef79076"; }

        public QueryJointValuesWorldView()
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
				public string prefix = "";
				public string folder_path = "";
				public bool recursive;


            public override string MD5Sum() { return "c5fb2e5f6de62522810f37ff4681c996"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return false; }
            public override string MessageDefinition() { return @"string prefix
string folder_path
bool recursive"; }
			public override string MessageType { get { return "xamlamoveit_msgs/QueryJointValuesWorldView__Request"; } }
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
                
                //prefix
                prefix = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                prefix = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
                //folder_path
                folder_path = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                folder_path = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
                //recursive
                recursive = serializedMessage[currentIndex++]==1;
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
                
                //prefix
                if (prefix == null)
                    prefix = "";
                scratch1 = Encoding.ASCII.GetBytes((string)prefix);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
                //folder_path
                if (folder_path == null)
                    folder_path = "";
                scratch1 = Encoding.ASCII.GetBytes((string)folder_path);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
                //recursive
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)recursive ? 1 : 0 );
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
                
                //prefix
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                prefix = Encoding.ASCII.GetString(strbuf);
                //folder_path
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                folder_path = Encoding.ASCII.GetString(strbuf);
                //recursive
                recursive = rand.Next(2) == 1;
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.QueryJointValuesWorldView.Request other = (Messages.xamlamoveit_msgs.QueryJointValuesWorldView.Request)____other;

                ret &= prefix == other.prefix;
                ret &= folder_path == other.folder_path;
                ret &= recursive == other.recursive;
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.xamlamoveit_msgs.JointValuesPoint[] points;
				public string[] names;
				public string[] element_paths;
				public bool success;
				public string error = "";



            public override string MD5Sum() { return "c5fb2e5f6de62522810f37ff4681c996"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"JointValuesPoint[] points
string[] names
string[] element_paths
bool success
string error"; }
			public override string MessageType { get { return "xamlamoveit_msgs/QueryJointValuesWorldView__Response"; } }
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
                
                //points
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.JointValuesPoint[arraylength];
                else
                    Array.Resize(ref points, arraylength);
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    points[i] = new Messages.xamlamoveit_msgs.JointValuesPoint(serializedMessage, ref currentIndex);
                }
                //names
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (names == null)
                    names = new string[arraylength];
                else
                    Array.Resize(ref names, arraylength);
                for (int i=0;i<names.Length; i++) {
                    //names[i]
                    names[i] = "";
                    piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                    currentIndex += 4;
                    names[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                    currentIndex += piecesize;
                }
                //element_paths
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (element_paths == null)
                    element_paths = new string[arraylength];
                else
                    Array.Resize(ref element_paths, arraylength);
                for (int i=0;i<element_paths.Length; i++) {
                    //element_paths[i]
                    element_paths[i] = "";
                    piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                    currentIndex += 4;
                    element_paths[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                    currentIndex += piecesize;
                }
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
                
                //points
                hasmetacomponents |= true;
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.JointValuesPoint[0];
                pieces.Add(BitConverter.GetBytes(points.Length));
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    if (points[i] == null)
                        points[i] = new Messages.xamlamoveit_msgs.JointValuesPoint();
                    pieces.Add(points[i].Serialize(true));
                }
                //names
                hasmetacomponents |= false;
                if (names == null)
                    names = new string[0];
                pieces.Add(BitConverter.GetBytes(names.Length));
                for (int i=0;i<names.Length; i++) {
                    //names[i]
                    if (names[i] == null)
                        names[i] = "";
                    scratch1 = Encoding.ASCII.GetBytes((string)names[i]);
                    thischunk = new byte[scratch1.Length + 4];
                    scratch2 = BitConverter.GetBytes(scratch1.Length);
                    Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                    Array.Copy(scratch2, thischunk, 4);
                    pieces.Add(thischunk);
                }
                //element_paths
                hasmetacomponents |= false;
                if (element_paths == null)
                    element_paths = new string[0];
                pieces.Add(BitConverter.GetBytes(element_paths.Length));
                for (int i=0;i<element_paths.Length; i++) {
                    //element_paths[i]
                    if (element_paths[i] == null)
                        element_paths[i] = "";
                    scratch1 = Encoding.ASCII.GetBytes((string)element_paths[i]);
                    thischunk = new byte[scratch1.Length + 4];
                    scratch2 = BitConverter.GetBytes(scratch1.Length);
                    Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                    Array.Copy(scratch2, thischunk, 4);
                    pieces.Add(thischunk);
                }
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
                
                //points
                arraylength = rand.Next(10);
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.JointValuesPoint[arraylength];
                else
                    Array.Resize(ref points, arraylength);
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    points[i] = new Messages.xamlamoveit_msgs.JointValuesPoint();
                    points[i].Randomize();
                }
                //names
                arraylength = rand.Next(10);
                if (names == null)
                    names = new string[arraylength];
                else
                    Array.Resize(ref names, arraylength);
                for (int i=0;i<names.Length; i++) {
                    //names[i]
                    strlength = rand.Next(100) + 1;
                    strbuf = new byte[strlength];
                    rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                    for (int __x__ = 0; __x__ < strlength; __x__++)
                        if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                            strbuf[__x__] = (byte)(rand.Next(254) + 1);
                    strbuf[strlength - 1] = 0; //null terminate
                    names[i] = Encoding.ASCII.GetString(strbuf);
                }
                //element_paths
                arraylength = rand.Next(10);
                if (element_paths == null)
                    element_paths = new string[arraylength];
                else
                    Array.Resize(ref element_paths, arraylength);
                for (int i=0;i<element_paths.Length; i++) {
                    //element_paths[i]
                    strlength = rand.Next(100) + 1;
                    strbuf = new byte[strlength];
                    rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                    for (int __x__ = 0; __x__ < strlength; __x__++)
                        if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                            strbuf[__x__] = (byte)(rand.Next(254) + 1);
                    strbuf[strlength - 1] = 0; //null terminate
                    element_paths[i] = Encoding.ASCII.GetString(strbuf);
                }
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
                xamlamoveit_msgs.QueryJointValuesWorldView.Response other = (Messages.xamlamoveit_msgs.QueryJointValuesWorldView.Response)____other;

                if (points.Length != other.points.Length)
                    return false;
                for (int __i__=0; __i__ < points.Length; __i__++)
                {
                    ret &= points[__i__].Equals(other.points[__i__]);
                }
                if (names.Length != other.names.Length)
                    return false;
                for (int __i__=0; __i__ < names.Length; __i__++)
                {
                    ret &= names[__i__] == other.names[__i__];
                }
                if (element_paths.Length != other.element_paths.Length)
                    return false;
                for (int __i__=0; __i__ < element_paths.Length; __i__++)
                {
                    ret &= element_paths[__i__] == other.element_paths[__i__];
                }
                ret &= success == other.success;
                ret &= error == other.error;
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
