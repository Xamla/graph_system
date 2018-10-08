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
    public class QueryJointStateCollisions : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/QueryJointStateCollisions"; } }
        public override string ServiceDefinition() { return @"string move_group_name
string[] joint_names
JointPathPoint[] points
---
bool success
bool[] in_collision
string[] messages
int32[] error_codes
int32 STATE_VALID=1
int32 STATE_SELF_COLLISION=-1
int32 STATE_SCENE_COLLISION =-2
int32 INVALID_JOINTS=-11
int32 INVALID_MOVE_GROUP=-12"; }
        public override string MD5Sum() { return "8fdc633e4bb0e8e2f0d4061fa00754ed"; }

        public QueryJointStateCollisions()
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
				public string  move_group_name = "";
				public string[] joint_names;
				public Messages.xamlamoveit_msgs.JointPathPoint[] points;


            public override string MD5Sum() { return "d37bd709cf1d7b852923929cb0212ccd"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"string move_group_name
string[] joint_names
JointPathPoint[] points"; }
			public override string MessageType { get { return "xamlamoveit_msgs/QueryJointStateCollisions__Request"; } }
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
                
                //move_group_name
                move_group_name = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                move_group_name = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
                //joint_names
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (joint_names == null)
                    joint_names = new string[arraylength];
                else
                    Array.Resize(ref joint_names, arraylength);
                for (int i=0;i<joint_names.Length; i++) {
                    //joint_names[i]
                    joint_names[i] = "";
                    piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                    currentIndex += 4;
                    joint_names[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                    currentIndex += piecesize;
                }
                //points
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.JointPathPoint[arraylength];
                else
                    Array.Resize(ref points, arraylength);
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    points[i] = new Messages.xamlamoveit_msgs.JointPathPoint(serializedMessage, ref currentIndex);
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
                
                //move_group_name
                if (move_group_name == null)
                    move_group_name = "";
                scratch1 = Encoding.ASCII.GetBytes((string)move_group_name);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
                //joint_names
                hasmetacomponents |= false;
                if (joint_names == null)
                    joint_names = new string[0];
                pieces.Add(BitConverter.GetBytes(joint_names.Length));
                for (int i=0;i<joint_names.Length; i++) {
                    //joint_names[i]
                    if (joint_names[i] == null)
                        joint_names[i] = "";
                    scratch1 = Encoding.ASCII.GetBytes((string)joint_names[i]);
                    thischunk = new byte[scratch1.Length + 4];
                    scratch2 = BitConverter.GetBytes(scratch1.Length);
                    Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                    Array.Copy(scratch2, thischunk, 4);
                    pieces.Add(thischunk);
                }
                //points
                hasmetacomponents |= true;
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.JointPathPoint[0];
                pieces.Add(BitConverter.GetBytes(points.Length));
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    if (points[i] == null)
                        points[i] = new Messages.xamlamoveit_msgs.JointPathPoint();
                    pieces.Add(points[i].Serialize(true));
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
                
                //move_group_name
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                move_group_name = Encoding.ASCII.GetString(strbuf);
                //joint_names
                arraylength = rand.Next(10);
                if (joint_names == null)
                    joint_names = new string[arraylength];
                else
                    Array.Resize(ref joint_names, arraylength);
                for (int i=0;i<joint_names.Length; i++) {
                    //joint_names[i]
                    strlength = rand.Next(100) + 1;
                    strbuf = new byte[strlength];
                    rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                    for (int __x__ = 0; __x__ < strlength; __x__++)
                        if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                            strbuf[__x__] = (byte)(rand.Next(254) + 1);
                    strbuf[strlength - 1] = 0; //null terminate
                    joint_names[i] = Encoding.ASCII.GetString(strbuf);
                }
                //points
                arraylength = rand.Next(10);
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.JointPathPoint[arraylength];
                else
                    Array.Resize(ref points, arraylength);
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    points[i] = new Messages.xamlamoveit_msgs.JointPathPoint();
                    points[i].Randomize();
                }
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.QueryJointStateCollisions.Request other = (Messages.xamlamoveit_msgs.QueryJointStateCollisions.Request)____other;

                ret &= move_group_name == other.move_group_name;
                if (joint_names.Length != other.joint_names.Length)
                    return false;
                for (int __i__=0; __i__ < joint_names.Length; __i__++)
                {
                    ret &= joint_names[__i__] == other.joint_names[__i__];
                }
                if (points.Length != other.points.Length)
                    return false;
                for (int __i__=0; __i__ < points.Length; __i__++)
                {
                    ret &= points[__i__].Equals(other.points[__i__]);
                }
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public bool success;
				public bool[] in_collision;
				public string[] messages;
				public int[] error_codes;
				public const int STATE_VALID = 1;
				public const int STATE_SELF_COLLISION = -1;
				public const int STATE_SCENE_COLLISION =-2;
				public const int INVALID_JOINTS = -11;
				public const int INVALID_MOVE_GROUP = -12;



            public override string MD5Sum() { return "d37bd709cf1d7b852923929cb0212ccd"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return false; }
            public override string MessageDefinition() { return @"bool success
bool[] in_collision
string[] messages
int32[] error_codes
int32 STATE_VALID=1
int32 STATE_SELF_COLLISION=-1
int32 STATE_SCENE_COLLISION =-2
int32 INVALID_JOINTS=-11
int32 INVALID_MOVE_GROUP=-12"; }
			public override string MessageType { get { return "xamlamoveit_msgs/QueryJointStateCollisions__Response"; } }
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
                //in_collision
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (in_collision == null)
                    in_collision = new bool[arraylength];
                else
                    Array.Resize(ref in_collision, arraylength);
                for (int i=0;i<in_collision.Length; i++) {
                    //in_collision[i]
                    in_collision[i] = serializedMessage[currentIndex++]==1;
                }
                //messages
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (messages == null)
                    messages = new string[arraylength];
                else
                    Array.Resize(ref messages, arraylength);
                for (int i=0;i<messages.Length; i++) {
                    //messages[i]
                    messages[i] = "";
                    piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                    currentIndex += 4;
                    messages[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                    currentIndex += piecesize;
                }
                //error_codes
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (error_codes == null)
                    error_codes = new int[arraylength];
                else
                    Array.Resize(ref error_codes, arraylength);
// Start Xamla
                    //error_codes
                    piecesize = Marshal.SizeOf(typeof(int)) * error_codes.Length;
                    if (currentIndex + piecesize > serializedMessage.Length) {
                        throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                    }
                    Buffer.BlockCopy(serializedMessage, currentIndex, error_codes, 0, piecesize);
                    currentIndex += piecesize;
// End Xamla

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
                //in_collision
                hasmetacomponents |= false;
                if (in_collision == null)
                    in_collision = new bool[0];
                pieces.Add(BitConverter.GetBytes(in_collision.Length));
                for (int i=0;i<in_collision.Length; i++) {
                    //in_collision[i]
                    thischunk = new byte[1];
                    thischunk[0] = (byte) ((bool)in_collision[i] ? 1 : 0 );
                    pieces.Add(thischunk);
                }
                //messages
                hasmetacomponents |= false;
                if (messages == null)
                    messages = new string[0];
                pieces.Add(BitConverter.GetBytes(messages.Length));
                for (int i=0;i<messages.Length; i++) {
                    //messages[i]
                    if (messages[i] == null)
                        messages[i] = "";
                    scratch1 = Encoding.ASCII.GetBytes((string)messages[i]);
                    thischunk = new byte[scratch1.Length + 4];
                    scratch2 = BitConverter.GetBytes(scratch1.Length);
                    Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                    Array.Copy(scratch2, thischunk, 4);
                    pieces.Add(thischunk);
                }
                //error_codes
                hasmetacomponents |= false;
                if (error_codes == null)
                    error_codes = new int[0];
                pieces.Add(BitConverter.GetBytes(error_codes.Length));
// Start Xamla
                    //error_codes
                    x__size = Marshal.SizeOf(typeof(int)) * error_codes.Length;
                    scratch1 = new byte[x__size];
                    Buffer.BlockCopy(error_codes, 0, scratch1, 0, x__size);
                    pieces.Add(scratch1);
// End Xamla

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
                //in_collision
                arraylength = rand.Next(10);
                if (in_collision == null)
                    in_collision = new bool[arraylength];
                else
                    Array.Resize(ref in_collision, arraylength);
                for (int i=0;i<in_collision.Length; i++) {
                    //in_collision[i]
                    in_collision[i] = rand.Next(2) == 1;
                }
                //messages
                arraylength = rand.Next(10);
                if (messages == null)
                    messages = new string[arraylength];
                else
                    Array.Resize(ref messages, arraylength);
                for (int i=0;i<messages.Length; i++) {
                    //messages[i]
                    strlength = rand.Next(100) + 1;
                    strbuf = new byte[strlength];
                    rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                    for (int __x__ = 0; __x__ < strlength; __x__++)
                        if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                            strbuf[__x__] = (byte)(rand.Next(254) + 1);
                    strbuf[strlength - 1] = 0; //null terminate
                    messages[i] = Encoding.ASCII.GetString(strbuf);
                }
                //error_codes
                arraylength = rand.Next(10);
                if (error_codes == null)
                    error_codes = new int[arraylength];
                else
                    Array.Resize(ref error_codes, arraylength);
                for (int i=0;i<error_codes.Length; i++) {
                    //error_codes[i]
                    error_codes[i] = rand.Next();
                }
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.QueryJointStateCollisions.Response other = (Messages.xamlamoveit_msgs.QueryJointStateCollisions.Response)____other;

                ret &= success == other.success;
                if (in_collision.Length != other.in_collision.Length)
                    return false;
                for (int __i__=0; __i__ < in_collision.Length; __i__++)
                {
                    ret &= in_collision[__i__] == other.in_collision[__i__];
                }
                if (messages.Length != other.messages.Length)
                    return false;
                for (int __i__=0; __i__ < messages.Length; __i__++)
                {
                    ret &= messages[__i__] == other.messages[__i__];
                }
                if (error_codes.Length != other.error_codes.Length)
                    return false;
                for (int __i__=0; __i__ < error_codes.Length; __i__++)
                {
                    ret &= error_codes[__i__] == other.error_codes[__i__];
                }
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
