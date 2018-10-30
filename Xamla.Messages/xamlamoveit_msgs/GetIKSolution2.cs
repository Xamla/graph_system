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
    public class GetIKSolution2 : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/GetIKSolution2"; } }
        public override string ServiceDefinition() { return @"string group_name
string[] joint_names
JointValuesPoint seed
bool const_seed
EndEffectorPoses[] points
bool collision_check
int32 attempts
duration timeout
---
JointValuesPoint[] solutions
moveit_msgs/MoveItErrorCodes[] error_codes"; }
        public override string MD5Sum() { return "8077a0f08c0609ce4db100ba19b3c347"; }

        public GetIKSolution2()
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
				public string group_name = "";
				public string[] joint_names;
				public Messages.xamlamoveit_msgs.JointValuesPoint seed = new Messages.xamlamoveit_msgs.JointValuesPoint();
				public bool const_seed;
				public Messages.xamlamoveit_msgs.EndEffectorPoses[] points;
				public bool collision_check;
				public int attempts;
				public Duration timeout = new Duration();


            public override string MD5Sum() { return "198486961b24d9162719e71a5147534f"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"string group_name
string[] joint_names
JointValuesPoint seed
bool const_seed
EndEffectorPoses[] points
bool collision_check
int32 attempts
duration timeout"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetIKSolution2__Request"; } }
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
                
                //group_name
                group_name = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                group_name = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
                //seed
                seed = new Messages.xamlamoveit_msgs.JointValuesPoint(serializedMessage, ref currentIndex);
                //const_seed
                const_seed = serializedMessage[currentIndex++]==1;
                //points
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.EndEffectorPoses[arraylength];
                else
                    Array.Resize(ref points, arraylength);
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    points[i] = new Messages.xamlamoveit_msgs.EndEffectorPoses(serializedMessage, ref currentIndex);
                }
                //collision_check
                collision_check = serializedMessage[currentIndex++]==1;
                //attempts
                piecesize = Marshal.SizeOf(typeof(int));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                attempts = (int)Marshal.PtrToStructure(h, typeof(int));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //timeout
                timeout = new Duration(new TimeData(
                        BitConverter.ToInt32(serializedMessage, currentIndex),
                        BitConverter.ToInt32(serializedMessage, currentIndex+Marshal.SizeOf(typeof(System.Int32)))));
                currentIndex += 2*Marshal.SizeOf(typeof(System.Int32));
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
                
                //group_name
                if (group_name == null)
                    group_name = "";
                scratch1 = Encoding.ASCII.GetBytes((string)group_name);
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
                //seed
                if (seed == null)
                    seed = new Messages.xamlamoveit_msgs.JointValuesPoint();
                pieces.Add(seed.Serialize(true));
                //const_seed
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)const_seed ? 1 : 0 );
                pieces.Add(thischunk);
                //points
                hasmetacomponents |= true;
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.EndEffectorPoses[0];
                pieces.Add(BitConverter.GetBytes(points.Length));
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    if (points[i] == null)
                        points[i] = new Messages.xamlamoveit_msgs.EndEffectorPoses();
                    pieces.Add(points[i].Serialize(true));
                }
                //collision_check
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)collision_check ? 1 : 0 );
                pieces.Add(thischunk);
                //attempts
                scratch1 = new byte[Marshal.SizeOf(typeof(int))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(attempts, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //timeout
                pieces.Add(BitConverter.GetBytes(timeout.data.sec));
                pieces.Add(BitConverter.GetBytes(timeout.data.nsec));
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
                
                //group_name
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                group_name = Encoding.ASCII.GetString(strbuf);
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
                //seed
                seed = new Messages.xamlamoveit_msgs.JointValuesPoint();
                seed.Randomize();
                //const_seed
                const_seed = rand.Next(2) == 1;
                //points
                arraylength = rand.Next(10);
                if (points == null)
                    points = new Messages.xamlamoveit_msgs.EndEffectorPoses[arraylength];
                else
                    Array.Resize(ref points, arraylength);
                for (int i=0;i<points.Length; i++) {
                    //points[i]
                    points[i] = new Messages.xamlamoveit_msgs.EndEffectorPoses();
                    points[i].Randomize();
                }
                //collision_check
                collision_check = rand.Next(2) == 1;
                //attempts
                attempts = rand.Next();
                //timeout
                timeout = new Duration(new TimeData(
                        Convert.ToInt32(rand.Next()),
                        Convert.ToInt32(rand.Next())));
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetIKSolution2.Request other = (Messages.xamlamoveit_msgs.GetIKSolution2.Request)____other;

                ret &= group_name == other.group_name;
                if (joint_names.Length != other.joint_names.Length)
                    return false;
                for (int __i__=0; __i__ < joint_names.Length; __i__++)
                {
                    ret &= joint_names[__i__] == other.joint_names[__i__];
                }
                ret &= seed.Equals(other.seed);
                ret &= const_seed == other.const_seed;
                if (points.Length != other.points.Length)
                    return false;
                for (int __i__=0; __i__ < points.Length; __i__++)
                {
                    ret &= points[__i__].Equals(other.points[__i__]);
                }
                ret &= collision_check == other.collision_check;
                ret &= attempts == other.attempts;
                ret &= timeout.data.Equals(other.timeout.data);
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.xamlamoveit_msgs.JointValuesPoint[] solutions;
				public Messages.moveit_msgs.MoveItErrorCodes[] error_codes;



            public override string MD5Sum() { return "198486961b24d9162719e71a5147534f"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"JointValuesPoint[] solutions
moveit_msgs/MoveItErrorCodes[] error_codes"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetIKSolution2__Response"; } }
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
                
                //solutions
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (solutions == null)
                    solutions = new Messages.xamlamoveit_msgs.JointValuesPoint[arraylength];
                else
                    Array.Resize(ref solutions, arraylength);
                for (int i=0;i<solutions.Length; i++) {
                    //solutions[i]
                    solutions[i] = new Messages.xamlamoveit_msgs.JointValuesPoint(serializedMessage, ref currentIndex);
                }
                //error_codes
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (error_codes == null)
                    error_codes = new Messages.moveit_msgs.MoveItErrorCodes[arraylength];
                else
                    Array.Resize(ref error_codes, arraylength);
                for (int i=0;i<error_codes.Length; i++) {
                    //error_codes[i]
                    error_codes[i] = new Messages.moveit_msgs.MoveItErrorCodes(serializedMessage, ref currentIndex);
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
                
                //solutions
                hasmetacomponents |= true;
                if (solutions == null)
                    solutions = new Messages.xamlamoveit_msgs.JointValuesPoint[0];
                pieces.Add(BitConverter.GetBytes(solutions.Length));
                for (int i=0;i<solutions.Length; i++) {
                    //solutions[i]
                    if (solutions[i] == null)
                        solutions[i] = new Messages.xamlamoveit_msgs.JointValuesPoint();
                    pieces.Add(solutions[i].Serialize(true));
                }
                //error_codes
                hasmetacomponents |= false;
                if (error_codes == null)
                    error_codes = new Messages.moveit_msgs.MoveItErrorCodes[0];
                pieces.Add(BitConverter.GetBytes(error_codes.Length));
                for (int i=0;i<error_codes.Length; i++) {
                    //error_codes[i]
                    if (error_codes[i] == null)
                        error_codes[i] = new Messages.moveit_msgs.MoveItErrorCodes();
                    pieces.Add(error_codes[i].Serialize(true));
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
                
                //solutions
                arraylength = rand.Next(10);
                if (solutions == null)
                    solutions = new Messages.xamlamoveit_msgs.JointValuesPoint[arraylength];
                else
                    Array.Resize(ref solutions, arraylength);
                for (int i=0;i<solutions.Length; i++) {
                    //solutions[i]
                    solutions[i] = new Messages.xamlamoveit_msgs.JointValuesPoint();
                    solutions[i].Randomize();
                }
                //error_codes
                arraylength = rand.Next(10);
                if (error_codes == null)
                    error_codes = new Messages.moveit_msgs.MoveItErrorCodes[arraylength];
                else
                    Array.Resize(ref error_codes, arraylength);
                for (int i=0;i<error_codes.Length; i++) {
                    //error_codes[i]
                    error_codes[i] = new Messages.moveit_msgs.MoveItErrorCodes();
                    error_codes[i].Randomize();
                }
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetIKSolution2.Response other = (Messages.xamlamoveit_msgs.GetIKSolution2.Response)____other;

                if (solutions.Length != other.solutions.Length)
                    return false;
                for (int __i__=0; __i__ < solutions.Length; __i__++)
                {
                    ret &= solutions[__i__].Equals(other.solutions[__i__]);
                }
                if (error_codes.Length != other.error_codes.Length)
                    return false;
                for (int __i__=0; __i__ < error_codes.Length; __i__++)
                {
                    ret &= error_codes[__i__].Equals(other.error_codes[__i__]);
                }
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
