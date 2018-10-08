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
    public class GetCartesianPath : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/GetCartesianPath"; } }
        public override string ServiceDefinition() { return @"string group_name
string end_effector_link
string[] joint_names
JointPathPoint seed
geometry_msgs/PoseStamped[] waypoints
float64 max_deviation
uint32 num_steps
bool collision_check
---
JointPathPoint[] path
moveit_msgs/MoveItErrorCodes error_code"; }
        public override string MD5Sum() { return "7f1225579e989f10ec94a6b23daad5f2"; }

        public GetCartesianPath()
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
				public string end_effector_link = "";
				public string[] joint_names;
				public Messages.xamlamoveit_msgs.JointPathPoint seed = new Messages.xamlamoveit_msgs.JointPathPoint();
				public Messages.geometry_msgs.PoseStamped[] waypoints;
				public double max_deviation;
				public uint num_steps;
				public bool collision_check;


            public override string MD5Sum() { return "7403c0907314f10195279ded05d421a3"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"string group_name
string end_effector_link
string[] joint_names
JointPathPoint seed
geometry_msgs/PoseStamped[] waypoints
float64 max_deviation
uint32 num_steps
bool collision_check"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetCartesianPath__Request"; } }
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
                //end_effector_link
                end_effector_link = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                end_effector_link = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
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
                seed = new Messages.xamlamoveit_msgs.JointPathPoint(serializedMessage, ref currentIndex);
                //waypoints
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (waypoints == null)
                    waypoints = new Messages.geometry_msgs.PoseStamped[arraylength];
                else
                    Array.Resize(ref waypoints, arraylength);
                for (int i=0;i<waypoints.Length; i++) {
                    //waypoints[i]
                    waypoints[i] = new Messages.geometry_msgs.PoseStamped(serializedMessage, ref currentIndex);
                }
                //max_deviation
                piecesize = Marshal.SizeOf(typeof(double));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                max_deviation = (double)Marshal.PtrToStructure(h, typeof(double));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //num_steps
                piecesize = Marshal.SizeOf(typeof(uint));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                num_steps = (uint)Marshal.PtrToStructure(h, typeof(uint));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //collision_check
                collision_check = serializedMessage[currentIndex++]==1;
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
                //end_effector_link
                if (end_effector_link == null)
                    end_effector_link = "";
                scratch1 = Encoding.ASCII.GetBytes((string)end_effector_link);
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
                    seed = new Messages.xamlamoveit_msgs.JointPathPoint();
                pieces.Add(seed.Serialize(true));
                //waypoints
                hasmetacomponents |= true;
                if (waypoints == null)
                    waypoints = new Messages.geometry_msgs.PoseStamped[0];
                pieces.Add(BitConverter.GetBytes(waypoints.Length));
                for (int i=0;i<waypoints.Length; i++) {
                    //waypoints[i]
                    if (waypoints[i] == null)
                        waypoints[i] = new Messages.geometry_msgs.PoseStamped();
                    pieces.Add(waypoints[i].Serialize(true));
                }
                //max_deviation
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(max_deviation, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //num_steps
                scratch1 = new byte[Marshal.SizeOf(typeof(uint))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(num_steps, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //collision_check
                thischunk = new byte[1];
                thischunk[0] = (byte) ((bool)collision_check ? 1 : 0 );
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
                
                //group_name
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                group_name = Encoding.ASCII.GetString(strbuf);
                //end_effector_link
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                end_effector_link = Encoding.ASCII.GetString(strbuf);
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
                seed = new Messages.xamlamoveit_msgs.JointPathPoint();
                seed.Randomize();
                //waypoints
                arraylength = rand.Next(10);
                if (waypoints == null)
                    waypoints = new Messages.geometry_msgs.PoseStamped[arraylength];
                else
                    Array.Resize(ref waypoints, arraylength);
                for (int i=0;i<waypoints.Length; i++) {
                    //waypoints[i]
                    waypoints[i] = new Messages.geometry_msgs.PoseStamped();
                    waypoints[i].Randomize();
                }
                //max_deviation
                max_deviation = (rand.Next() + rand.NextDouble());
                //num_steps
                num_steps = (uint)rand.Next();
                //collision_check
                collision_check = rand.Next(2) == 1;
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetCartesianPath.Request other = (Messages.xamlamoveit_msgs.GetCartesianPath.Request)____other;

                ret &= group_name == other.group_name;
                ret &= end_effector_link == other.end_effector_link;
                if (joint_names.Length != other.joint_names.Length)
                    return false;
                for (int __i__=0; __i__ < joint_names.Length; __i__++)
                {
                    ret &= joint_names[__i__] == other.joint_names[__i__];
                }
                ret &= seed.Equals(other.seed);
                if (waypoints.Length != other.waypoints.Length)
                    return false;
                for (int __i__=0; __i__ < waypoints.Length; __i__++)
                {
                    ret &= waypoints[__i__].Equals(other.waypoints[__i__]);
                }
                ret &= max_deviation == other.max_deviation;
                ret &= num_steps == other.num_steps;
                ret &= collision_check == other.collision_check;
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.xamlamoveit_msgs.JointPathPoint[] path;
				public Messages.moveit_msgs.MoveItErrorCodes error_code = new Messages.moveit_msgs.MoveItErrorCodes();



            public override string MD5Sum() { return "7403c0907314f10195279ded05d421a3"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"JointPathPoint[] path
moveit_msgs/MoveItErrorCodes error_code"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetCartesianPath__Response"; } }
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
                
                //path
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (path == null)
                    path = new Messages.xamlamoveit_msgs.JointPathPoint[arraylength];
                else
                    Array.Resize(ref path, arraylength);
                for (int i=0;i<path.Length; i++) {
                    //path[i]
                    path[i] = new Messages.xamlamoveit_msgs.JointPathPoint(serializedMessage, ref currentIndex);
                }
                //error_code
                error_code = new Messages.moveit_msgs.MoveItErrorCodes(serializedMessage, ref currentIndex);
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
                
                //path
                hasmetacomponents |= true;
                if (path == null)
                    path = new Messages.xamlamoveit_msgs.JointPathPoint[0];
                pieces.Add(BitConverter.GetBytes(path.Length));
                for (int i=0;i<path.Length; i++) {
                    //path[i]
                    if (path[i] == null)
                        path[i] = new Messages.xamlamoveit_msgs.JointPathPoint();
                    pieces.Add(path[i].Serialize(true));
                }
                //error_code
                if (error_code == null)
                    error_code = new Messages.moveit_msgs.MoveItErrorCodes();
                pieces.Add(error_code.Serialize(true));
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
                
                //path
                arraylength = rand.Next(10);
                if (path == null)
                    path = new Messages.xamlamoveit_msgs.JointPathPoint[arraylength];
                else
                    Array.Resize(ref path, arraylength);
                for (int i=0;i<path.Length; i++) {
                    //path[i]
                    path[i] = new Messages.xamlamoveit_msgs.JointPathPoint();
                    path[i].Randomize();
                }
                //error_code
                error_code = new Messages.moveit_msgs.MoveItErrorCodes();
                error_code.Randomize();
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetCartesianPath.Response other = (Messages.xamlamoveit_msgs.GetCartesianPath.Response)____other;

                if (path.Length != other.path.Length)
                    return false;
                for (int __i__=0; __i__ < path.Length; __i__++)
                {
                    ret &= path[__i__].Equals(other.path[__i__]);
                }
                ret &= error_code.Equals(other.error_code);
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
