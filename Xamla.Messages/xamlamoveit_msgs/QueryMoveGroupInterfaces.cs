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
    public class QueryMoveGroupInterfaces : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/QueryMoveGroupInterfaces"; } }
        public override string ServiceDefinition() { return @"---
MoveGroupInterfaceDescription[] move_group_interfaces"; }
        public override string MD5Sum() { return "70efd3165cc1e0028f6c0e1bbcbbd09e"; }

        public QueryMoveGroupInterfaces()
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
			public override string MessageType { get { return "xamlamoveit_msgs/QueryMoveGroupInterfaces__Request"; } }
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
                xamlamoveit_msgs.QueryMoveGroupInterfaces.Request other = (Messages.xamlamoveit_msgs.QueryMoveGroupInterfaces.Request)____other;

                return ret;
            }
        }

        public class Response : RosMessage
        {
				public Messages.xamlamoveit_msgs.MoveGroupInterfaceDescription[] move_group_interfaces;



            public override string MD5Sum() { return "d41d8cd98f00b204e9800998ecf8427e"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"MoveGroupInterfaceDescription[] move_group_interfaces"; }
			public override string MessageType { get { return "xamlamoveit_msgs/QueryMoveGroupInterfaces__Response"; } }
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
                
                //move_group_interfaces
                hasmetacomponents |= true;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (move_group_interfaces == null)
                    move_group_interfaces = new Messages.xamlamoveit_msgs.MoveGroupInterfaceDescription[arraylength];
                else
                    Array.Resize(ref move_group_interfaces, arraylength);
                for (int i=0;i<move_group_interfaces.Length; i++) {
                    //move_group_interfaces[i]
                    move_group_interfaces[i] = new Messages.xamlamoveit_msgs.MoveGroupInterfaceDescription(serializedMessage, ref currentIndex);
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
                
                //move_group_interfaces
                hasmetacomponents |= true;
                if (move_group_interfaces == null)
                    move_group_interfaces = new Messages.xamlamoveit_msgs.MoveGroupInterfaceDescription[0];
                pieces.Add(BitConverter.GetBytes(move_group_interfaces.Length));
                for (int i=0;i<move_group_interfaces.Length; i++) {
                    //move_group_interfaces[i]
                    if (move_group_interfaces[i] == null)
                        move_group_interfaces[i] = new Messages.xamlamoveit_msgs.MoveGroupInterfaceDescription();
                    pieces.Add(move_group_interfaces[i].Serialize(true));
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
                
                //move_group_interfaces
                arraylength = rand.Next(10);
                if (move_group_interfaces == null)
                    move_group_interfaces = new Messages.xamlamoveit_msgs.MoveGroupInterfaceDescription[arraylength];
                else
                    Array.Resize(ref move_group_interfaces, arraylength);
                for (int i=0;i<move_group_interfaces.Length; i++) {
                    //move_group_interfaces[i]
                    move_group_interfaces[i] = new Messages.xamlamoveit_msgs.MoveGroupInterfaceDescription();
                    move_group_interfaces[i].Randomize();
                }
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.QueryMoveGroupInterfaces.Response other = (Messages.xamlamoveit_msgs.QueryMoveGroupInterfaces.Response)____other;

                if (move_group_interfaces.Length != other.move_group_interfaces.Length)
                    return false;
                for (int __i__=0; __i__ < move_group_interfaces.Length; __i__++)
                {
                    ret &= move_group_interfaces[__i__].Equals(other.move_group_interfaces[__i__]);
                }
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
