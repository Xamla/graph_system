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
    public class GetFKSolution : RosService
    {
        public override string ServiceType { get { return "xamlamoveit_msgs/GetFKSolution"; } }
        public override string ServiceDefinition() { return @"string group_name
string end_effector_link
string[] joint_names
JointPathPoint[] points
---
geometry_msgs/PoseStamped[] solutions
moveit_msgs/MoveItErrorCodes[] error_codes
string[] error_msgs"; }
        public override string MD5Sum() { return "06f497445c440bfa9a85b74b714ac9ad"; }

        public GetFKSolution()
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
				public Messages.xamlamoveit_msgs.JointPathPoint[] points;


            public override string MD5Sum() { return "02ba7b25e0c5154f809c49023f33ec32"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"string group_name
string end_effector_link
string[] joint_names
JointPathPoint[] points"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetFKSolution__Request"; } }
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
                xamlamoveit_msgs.GetFKSolution.Request other = (Messages.xamlamoveit_msgs.GetFKSolution.Request)____other;

                ret &= group_name == other.group_name;
                ret &= end_effector_link == other.end_effector_link;
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
				public Messages.geometry_msgs.PoseStamped[] solutions;
				public Messages.moveit_msgs.MoveItErrorCodes[] error_codes;
				public string[] error_msgs;



            public override string MD5Sum() { return "02ba7b25e0c5154f809c49023f33ec32"; }
            public override bool HasHeader() { return false; }
            public override bool IsMetaType() { return true; }
            public override string MessageDefinition() { return @"geometry_msgs/PoseStamped[] solutions
moveit_msgs/MoveItErrorCodes[] error_codes
string[] error_msgs"; }
			public override string MessageType { get { return "xamlamoveit_msgs/GetFKSolution__Response"; } }
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
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (solutions == null)
                    solutions = new Messages.geometry_msgs.PoseStamped[arraylength];
                else
                    Array.Resize(ref solutions, arraylength);
                for (int i=0;i<solutions.Length; i++) {
                    //solutions[i]
                    solutions[i] = new Messages.geometry_msgs.PoseStamped(serializedMessage, ref currentIndex);
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
                //error_msgs
                hasmetacomponents |= false;
                arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
                currentIndex += Marshal.SizeOf(typeof(System.Int32));
                if (error_msgs == null)
                    error_msgs = new string[arraylength];
                else
                    Array.Resize(ref error_msgs, arraylength);
                for (int i=0;i<error_msgs.Length; i++) {
                    //error_msgs[i]
                    error_msgs[i] = "";
                    piecesize = BitConverter.ToInt32(serializedMessage, currentIndex);
                    currentIndex += 4;
                    error_msgs[i] = Encoding.ASCII.GetString(serializedMessage, currentIndex, piecesize);
                    currentIndex += piecesize;
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
                hasmetacomponents |= false;
                if (solutions == null)
                    solutions = new Messages.geometry_msgs.PoseStamped[0];
                pieces.Add(BitConverter.GetBytes(solutions.Length));
                for (int i=0;i<solutions.Length; i++) {
                    //solutions[i]
                    if (solutions[i] == null)
                        solutions[i] = new Messages.geometry_msgs.PoseStamped();
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
                //error_msgs
                hasmetacomponents |= false;
                if (error_msgs == null)
                    error_msgs = new string[0];
                pieces.Add(BitConverter.GetBytes(error_msgs.Length));
                for (int i=0;i<error_msgs.Length; i++) {
                    //error_msgs[i]
                    if (error_msgs[i] == null)
                        error_msgs[i] = "";
                    scratch1 = Encoding.ASCII.GetBytes((string)error_msgs[i]);
                    thischunk = new byte[scratch1.Length + 4];
                    scratch2 = BitConverter.GetBytes(scratch1.Length);
                    Array.Copy(scratch1, 0, thischunk, 4, scratch1.Length);
                    Array.Copy(scratch2, thischunk, 4);
                    pieces.Add(thischunk);
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
                    solutions = new Messages.geometry_msgs.PoseStamped[arraylength];
                else
                    Array.Resize(ref solutions, arraylength);
                for (int i=0;i<solutions.Length; i++) {
                    //solutions[i]
                    solutions[i] = new Messages.geometry_msgs.PoseStamped();
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
                //error_msgs
                arraylength = rand.Next(10);
                if (error_msgs == null)
                    error_msgs = new string[arraylength];
                else
                    Array.Resize(ref error_msgs, arraylength);
                for (int i=0;i<error_msgs.Length; i++) {
                    //error_msgs[i]
                    strlength = rand.Next(100) + 1;
                    strbuf = new byte[strlength];
                    rand.NextBytes(strbuf);  //fill the whole buffer with random bytes
                    for (int __x__ = 0; __x__ < strlength; __x__++)
                        if (strbuf[__x__] == 0) //replace null chars with non-null random ones
                            strbuf[__x__] = (byte)(rand.Next(254) + 1);
                    strbuf[strlength - 1] = 0; //null terminate
                    error_msgs[i] = Encoding.ASCII.GetString(strbuf);
                }
            }

            public override bool Equals(RosMessage ____other)
            {
                if (____other == null)
					return false;

                bool ret = true;
                xamlamoveit_msgs.GetFKSolution.Response other = (Messages.xamlamoveit_msgs.GetFKSolution.Response)____other;

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
                if (error_msgs.Length != other.error_msgs.Length)
                    return false;
                for (int __i__=0; __i__ < error_msgs.Length; __i__++)
                {
                    ret &= error_msgs[__i__] == other.error_msgs[__i__];
                }
                // for each SingleType st:
                //    ret &= {st.Name} == other.{st.Name};
                return ret;
            }
        }
    }
}
