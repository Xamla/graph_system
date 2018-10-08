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
    public class GetLinearCartesianTrajectory : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/GetLinearCartesianTrajectory"; } }
        public override string ServiceDefinition() { return @"string end_effector_name
geometry_msgs/PoseStamped[] waypoints
float64 max_xyz_velocity
float64 max_xyz_acceleration
float64 max_angular_velocity
float64 max_angular_acceleration
float64 dt
float64 ik_jump_threshold
float64 max_deviation
string[] joint_names
JointPathPoint seed
bool collision_check
---
trajectory_msgs/JointTrajectory solution
moveit_msgs/MoveItErrorCodes error_code"; }
        public override string MD5Sum() { return "c89575223f23b28b8631e60bc3e998dd"; }

        public GetLinearCartesianTrajectory()
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
				public string end_effector_name = "";
				public Messages.geometry_msgs.PoseStamped[] waypoints;
				public double max_xyz_velocity;
				public double max_xyz_acceleration;
				public double max_angular_velocity;
				public double max_angular_acceleration;
				public double dt;
				public double ik_jump_threshold;
				public double max_deviation;
				public string[] joint_names;
				public Messages.xamlamoveit_msgs.JointPathPoint seed = new Messages.xamlamoveit_msgs.JointPathPoint();
				public bool collision_check;


            public override string MD5Sum() { return "e9176b5d8294bdb2b40fc88d6a3633e2"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"string end_effector_name
geometry_msgs/PoseStamped[] waypoints
float64 max_xyz_velocity
float64 max_xyz_acceleration
float64 max_angular_velocity
float64 max_angular_acceleration
float64 dt
float64 ik_jump_threshold
float64 max_deviation
string[] joint_names
JointPathPoint seed
bool collision_check"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetLinearCartesianTrajectory__Request"; } }
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
                
                //end_effector_name
                end_effector_name = "";
                piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += 4;
                end_effector_name = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                currentIndex += piecesize;
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
                //max_xyz_velocity
                piecesize = Marshal.SizeOf(typeof(double));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                max_xyz_velocity = (double)Marshal.PtrToStructure(h, typeof(double));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //max_xyz_acceleration
                piecesize = Marshal.SizeOf(typeof(double));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                max_xyz_acceleration = (double)Marshal.PtrToStructure(h, typeof(double));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //max_angular_velocity
                piecesize = Marshal.SizeOf(typeof(double));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                max_angular_velocity = (double)Marshal.PtrToStructure(h, typeof(double));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //max_angular_acceleration
                piecesize = Marshal.SizeOf(typeof(double));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                max_angular_acceleration = (double)Marshal.PtrToStructure(h, typeof(double));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //dt
                piecesize = Marshal.SizeOf(typeof(double));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                dt = (double)Marshal.PtrToStructure(h, typeof(double));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
                //ik_jump_threshold
                piecesize = Marshal.SizeOf(typeof(double));
                h = IntPtr.Zero;
                if (serializedMessage.Length - currentIndex != 0)
                {
                    h = Marshal.AllocHGlobal(piecesize);
                    Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
                }
                if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
                ik_jump_threshold = (double)Marshal.PtrToStructure(h, typeof(double));
                Marshal.FreeHGlobal(h);
                currentIndex+= piecesize;
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
                
                //end_effector_name
                if (end_effector_name == null)
                    end_effector_name = "";
                scratch1 = Encoding.ASCII.GetBytes((string)end_effector_name);
                thischunk = new byte[scratch1.Length + 4];
                scratch2 = BitConverter.GetBytes(scratch1.Length);
                Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                Array.Copy(scratch2, thischunk, 4);
                pieces.Add(thischunk);
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
                //max_xyz_velocity
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(max_xyz_velocity, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //max_xyz_acceleration
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(max_xyz_acceleration, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //max_angular_velocity
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(max_angular_velocity, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //max_angular_acceleration
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(max_angular_acceleration, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //dt
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(dt, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //ik_jump_threshold
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(ik_jump_threshold, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //max_deviation
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(max_deviation, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
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
                
                //end_effector_name
                strlength = rand.Next(100) + 1;
                strbuf = new byte[strlength];
                rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                for (int __x__ = 0; __x__ < strlength; __x__++)
                    if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                        strbuf[__x__] = (byte)(rand.Next(254) + 1);
                strbuf[strlength - 1] = 0; //null terminate
                end_effector_name = Encoding.ASCII.GetString(strbuf);
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
                //max_xyz_velocity
                max_xyz_velocity = (rand.Next() + rand.NextDouble());
                //max_xyz_acceleration
                max_xyz_acceleration = (rand.Next() + rand.NextDouble());
                //max_angular_velocity
                max_angular_velocity = (rand.Next() + rand.NextDouble());
                //max_angular_acceleration
                max_angular_acceleration = (rand.Next() + rand.NextDouble());
                //dt
                dt = (rand.Next() + rand.NextDouble());
                //ik_jump_threshold
                ik_jump_threshold = (rand.Next() + rand.NextDouble());
                //max_deviation
                max_deviation = (rand.Next() + rand.NextDouble());
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
                //collision_check
                collision_check = rand.Next(2) == 1;
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetLinearCartesianTrajectory.Request other = (Messages.xamlamoveit_msgs.GetLinearCartesianTrajectory.Request)____other;

                ret &= end_effector_name == other.end_effector_name;
                if (waypoints.Length != other.waypoints.Length)
                    return false;
                for (int __i__=0; __i__ < waypoints.Length; __i__++)
                {
                    ret &= waypoints[__i__].Equals(other.waypoints[__i__]);
                }
                ret &= max_xyz_velocity == other.max_xyz_velocity;
                ret &= max_xyz_acceleration == other.max_xyz_acceleration;
                ret &= max_angular_velocity == other.max_angular_velocity;
                ret &= max_angular_acceleration == other.max_angular_acceleration;
                ret &= dt == other.dt;
                ret &= ik_jump_threshold == other.ik_jump_threshold;
                ret &= max_deviation == other.max_deviation;
                if (joint_names.Length != other.joint_names.Length)
                    return false;
                for (int __i__=0; __i__ < joint_names.Length; __i__++)
                {
                    ret &= joint_names[__i__] == other.joint_names[__i__];
                }
                ret &= seed.Equals(other.seed);
                ret &= collision_check == other.collision_check;
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.trajectory_msgs.JointTrajectory solution = new Messages.trajectory_msgs.JointTrajectory();
				public Messages.moveit_msgs.MoveItErrorCodes error_code = new Messages.moveit_msgs.MoveItErrorCodes();



            public override string MD5Sum() { return "e9176b5d8294bdb2b40fc88d6a3633e2"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"trajectory_msgs/JointTrajectory solution
moveit_msgs/MoveItErrorCodes error_code"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetLinearCartesianTrajectory__Response"; } }
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
                
                //solution
                solution = new Messages.trajectory_msgs.JointTrajectory(serializedMessage, ref currentIndex);
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
                
                //solution
                if (solution == null)
                    solution = new Messages.trajectory_msgs.JointTrajectory();
                pieces.Add(solution.Serialize(true));
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
                
                //solution
                solution = new Messages.trajectory_msgs.JointTrajectory();
                solution.Randomize();
                //error_code
                error_code = new Messages.moveit_msgs.MoveItErrorCodes();
                error_code.Randomize();
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetLinearCartesianTrajectory.Response other = (Messages.xamlamoveit_msgs.GetLinearCartesianTrajectory.Response)____other;

                ret &= solution.Equals(other.solution);
                ret &= error_code.Equals(other.error_code);
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
