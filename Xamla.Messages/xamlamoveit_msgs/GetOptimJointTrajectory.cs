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
    public class GetOptimJointTrajectory : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/GetOptimJointTrajectory"; } }
        public override string ServiceDefinition() { return @"string[] joint_names
JointPathPoint[] waypoints
float64[] max_velocity
float64[] max_acceleration
float64 max_deviation
float64 dt
---
trajectory_msgs/JointTrajectory solution
moveit_msgs/MoveItErrorCodes error_code"; }
        public override string MD5Sum() { return "bfe060903fecad0a3d1c73e984016453"; }

        public GetOptimJointTrajectory()
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
				public string[] joint_names;
				public Messages.xamlamoveit_msgs.JointPathPoint[] waypoints;
				public double[] max_velocity;
				public double[] max_acceleration;
				public double max_deviation;
				public double dt;


            public override string MD5Sum() { return "151c01222bc646940d24780e754204b3"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"string[] joint_names
JointPathPoint[] waypoints
float64[] max_velocity
float64[] max_acceleration
float64 max_deviation
float64 dt"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetOptimJointTrajectory__Request"; } }
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
                //waypoints
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (waypoints == null)
                    waypoints = new Messages.xamlamoveit_msgs.JointPathPoint[arraylength];
                else
                    Array.Resize(ref waypoints, arraylength);
                for (int i=0;i<waypoints.Length; i++) {
                    //waypoints[i]
                    waypoints[i] = new Messages.xamlamoveit_msgs.JointPathPoint(serializedMessage, ref currentIndex);
                }
                //max_velocity
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (max_velocity == null)
                    max_velocity = new double[arraylength];
                else
                    Array.Resize(ref max_velocity, arraylength);
// Start Xamla
                    //max_velocity
                    piecesize = Marshal.SizeOf(typeof(double)) * max_velocity.Length;
                    if (currentIndex + piecesize > serializedMessage.Length) {
                        throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                    }
                    Buffer.BlockCopy(serializedMessage, currentIndex, max_velocity, 0, piecesize);
                    currentIndex += piecesize;
// End Xamla

                //max_acceleration
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (max_acceleration == null)
                    max_acceleration = new double[arraylength];
                else
                    Array.Resize(ref max_acceleration, arraylength);
// Start Xamla
                    //max_acceleration
                    piecesize = Marshal.SizeOf(typeof(double)) * max_acceleration.Length;
                    if (currentIndex + piecesize > serializedMessage.Length) {
                        throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                    }
                    Buffer.BlockCopy(serializedMessage, currentIndex, max_acceleration, 0, piecesize);
                    currentIndex += piecesize;
// End Xamla

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
                //waypoints
                hasmetacomponents |= true;
                if (waypoints == null)
                    waypoints = new Messages.xamlamoveit_msgs.JointPathPoint[0];
                pieces.Add(BitConverter.GetBytes(waypoints.Length));
                for (int i=0;i<waypoints.Length; i++) {
                    //waypoints[i]
                    if (waypoints[i] == null)
                        waypoints[i] = new Messages.xamlamoveit_msgs.JointPathPoint();
                    pieces.Add(waypoints[i].Serialize(true));
                }
                //max_velocity
                hasmetacomponents |= false;
                if (max_velocity == null)
                    max_velocity = new double[0];
                pieces.Add(BitConverter.GetBytes(max_velocity.Length));
// Start Xamla
                    //max_velocity
                    x__size = Marshal.SizeOf(typeof(double)) * max_velocity.Length;
                    scratch1 = new byte[x__size];
                    Buffer.BlockCopy(max_velocity, 0, scratch1, 0, x__size);
                    pieces.Add(scratch1);
// End Xamla

                //max_acceleration
                hasmetacomponents |= false;
                if (max_acceleration == null)
                    max_acceleration = new double[0];
                pieces.Add(BitConverter.GetBytes(max_acceleration.Length));
// Start Xamla
                    //max_acceleration
                    x__size = Marshal.SizeOf(typeof(double)) * max_acceleration.Length;
                    scratch1 = new byte[x__size];
                    Buffer.BlockCopy(max_acceleration, 0, scratch1, 0, x__size);
                    pieces.Add(scratch1);
// End Xamla

                //max_deviation
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(max_deviation, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
                //dt
                scratch1 = new byte[Marshal.SizeOf(typeof(double))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(dt, h.AddrOfPinnedObject(), false);
                h.Free();
                pieces.Add(scratch1);
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
                //waypoints
                arraylength = rand.Next(10);
                if (waypoints == null)
                    waypoints = new Messages.xamlamoveit_msgs.JointPathPoint[arraylength];
                else
                    Array.Resize(ref waypoints, arraylength);
                for (int i=0;i<waypoints.Length; i++) {
                    //waypoints[i]
                    waypoints[i] = new Messages.xamlamoveit_msgs.JointPathPoint();
                    waypoints[i].Randomize();
                }
                //max_velocity
                arraylength = rand.Next(10);
                if (max_velocity == null)
                    max_velocity = new double[arraylength];
                else
                    Array.Resize(ref max_velocity, arraylength);
                for (int i=0;i<max_velocity.Length; i++) {
                    //max_velocity[i]
                    max_velocity[i] = (rand.Next() + rand.NextDouble());
                }
                //max_acceleration
                arraylength = rand.Next(10);
                if (max_acceleration == null)
                    max_acceleration = new double[arraylength];
                else
                    Array.Resize(ref max_acceleration, arraylength);
                for (int i=0;i<max_acceleration.Length; i++) {
                    //max_acceleration[i]
                    max_acceleration[i] = (rand.Next() + rand.NextDouble());
                }
                //max_deviation
                max_deviation = (rand.Next() + rand.NextDouble());
                //dt
                dt = (rand.Next() + rand.NextDouble());
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetOptimJointTrajectory.Request other = (Messages.xamlamoveit_msgs.GetOptimJointTrajectory.Request)____other;

                if (joint_names.Length != other.joint_names.Length)
                    return false;
                for (int __i__=0; __i__ < joint_names.Length; __i__++)
                {
                    ret &= joint_names[__i__] == other.joint_names[__i__];
                }
                if (waypoints.Length != other.waypoints.Length)
                    return false;
                for (int __i__=0; __i__ < waypoints.Length; __i__++)
                {
                    ret &= waypoints[__i__].Equals(other.waypoints[__i__]);
                }
                if (max_velocity.Length != other.max_velocity.Length)
                    return false;
                for (int __i__=0; __i__ < max_velocity.Length; __i__++)
                {
                    ret &= max_velocity[__i__] == other.max_velocity[__i__];
                }
                if (max_acceleration.Length != other.max_acceleration.Length)
                    return false;
                for (int __i__=0; __i__ < max_acceleration.Length; __i__++)
                {
                    ret &= max_acceleration[__i__] == other.max_acceleration[__i__];
                }
                ret &= max_deviation == other.max_deviation;
                ret &= dt == other.dt;
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.trajectory_msgs.JointTrajectory solution = new Messages.trajectory_msgs.JointTrajectory();
				public Messages.moveit_msgs.MoveItErrorCodes error_code = new Messages.moveit_msgs.MoveItErrorCodes();



            public override string MD5Sum() { return "151c01222bc646940d24780e754204b3"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"trajectory_msgs/JointTrajectory solution
moveit_msgs/MoveItErrorCodes error_code"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetOptimJointTrajectory__Response"; } }
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
                xamlamoveit_msgs.GetOptimJointTrajectory.Response other = (Messages.xamlamoveit_msgs.GetOptimJointTrajectory.Response)____other;

                ret &= solution.Equals(other.solution);
                ret &= error_code.Equals(other.error_code);
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
