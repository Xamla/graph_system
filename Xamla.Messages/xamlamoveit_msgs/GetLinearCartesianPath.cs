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
    public class GetLinearCartesianPath : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/GetLinearCartesianPath"; } }
        public override string ServiceDefinition() { return @"geometry_msgs/PoseStamped[] waypoints
uint32 num_steps
---
geometry_msgs/PoseStamped[] path
moveit_msgs/MoveItErrorCodes error_code"; }
        public override string MD5Sum() { return "07554dabd174085fe1702a2d11f228ac"; }

        public GetLinearCartesianPath()
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
				public Messages.geometry_msgs.PoseStamped[] waypoints;
				public uint num_steps;


            public override string MD5Sum() { return "8be1e38eff30a1bf03963c9c16ce75ea"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"geometry_msgs/PoseStamped[] waypoints
uint32 num_steps"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetLinearCartesianPath__Request"; } }
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
                //num_steps
                scratch1 = new byte[Marshal.SizeOf(typeof(uint))];
                h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
                Marshal.StructureToPtr(num_steps, h.AddrOfPinnedObject(), false);
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
                //num_steps
                num_steps = (uint)rand.Next();
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetLinearCartesianPath.Request other = (Messages.xamlamoveit_msgs.GetLinearCartesianPath.Request)____other;

                if (waypoints.Length != other.waypoints.Length)
                    return false;
                for (int __i__=0; __i__ < waypoints.Length; __i__++)
                {
                    ret &= waypoints[__i__].Equals(other.waypoints[__i__]);
                }
                ret &= num_steps == other.num_steps;
                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.geometry_msgs.PoseStamped[] path;
				public Messages.moveit_msgs.MoveItErrorCodes error_code = new Messages.moveit_msgs.MoveItErrorCodes();



            public override string MD5Sum() { return "8be1e38eff30a1bf03963c9c16ce75ea"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"geometry_msgs/PoseStamped[] path
moveit_msgs/MoveItErrorCodes error_code"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetLinearCartesianPath__Response"; } }
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
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (path == null)
                    path = new Messages.geometry_msgs.PoseStamped[arraylength];
                else
                    Array.Resize(ref path, arraylength);
                for (int i=0;i<path.Length; i++) {
                    //path[i]
                    path[i] = new Messages.geometry_msgs.PoseStamped(serializedMessage, ref currentIndex);
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
                hasmetacomponents |= false;
                if (path == null)
                    path = new Messages.geometry_msgs.PoseStamped[0];
                pieces.Add(BitConverter.GetBytes(path.Length));
                for (int i=0;i<path.Length; i++) {
                    //path[i]
                    if (path[i] == null)
                        path[i] = new Messages.geometry_msgs.PoseStamped();
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
                    path = new Messages.geometry_msgs.PoseStamped[arraylength];
                else
                    Array.Resize(ref path, arraylength);
                for (int i=0;i<path.Length; i++) {
                    //path[i]
                    path[i] = new Messages.geometry_msgs.PoseStamped();
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
                xamlamoveit_msgs.GetLinearCartesianPath.Response other = (Messages.xamlamoveit_msgs.GetLinearCartesianPath.Response)____other;

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
