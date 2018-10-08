using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using uint8 = System.Byte;
using Uml.Robotics.Ros;
using Messages.geometry_msgs;
using Messages.sensor_msgs;
using Messages.actionlib_msgs;

using Messages.std_msgs;
using String=System.String;

namespace Messages.xamlamoveit_msgs
{
    public class ControllerState : RosMessage
    {

			public double[] joint_distance;
			public double[] cartesian_distance;
			public const long OK = 1L;
			public const long INVALID_IK = -1L;
			public const long SELF_COLLISION = -2L;
			public const long SCENE_COLLISION = -3L;
			public const long FRAME_TRANSFORM_FAILURE = -4L;
			public const long IK_JUMP_DETECTED = -5L;
			public const long CLOSE_TO_SINGULARITY = -6L;
			public const long JOINT_LIMITS_VIOLATED = -7L;
			public const long INVALID_LINK_NAME = -8L;
			public const long TASK_SPACE_JUMP_DETECTED = -9L;
			public long error_code;
			public bool converged;
			public bool self_collision_check_enabled;
			public bool scene_collision_check_enabled;
			public bool joint_limits_check_enabled;


        public override string MD5Sum() { return "01ea75095076027795b9fb3cadd5e7bc"; }
        public override bool HasHeader() { return false; }
        public override bool IsMetaType() { return false; }
        public override string MessageDefinition() { return @"float64[] joint_distance
float64[] cartesian_distance
int64 OK=1
int64 INVALID_IK=-1
int64 SELF_COLLISION=-2
int64 SCENE_COLLISION=-3
int64 FRAME_TRANSFORM_FAILURE=-4
int64 IK_JUMP_DETECTED=-5
int64 CLOSE_TO_SINGULARITY=-6
int64 JOINT_LIMITS_VIOLATED=-7
int64 INVALID_LINK_NAME=-8
int64 TASK_SPACE_JUMP_DETECTED=-9
int64 error_code
bool converged
bool self_collision_check_enabled
bool scene_collision_check_enabled
bool joint_limits_check_enabled"; }
        public override string MessageType { get { return "xamlamoveit_msgs/ControllerState"; } }
        public override bool IsServiceComponent() { return false; }

        public ControllerState()
        {
            
        }

        public ControllerState(byte[] serializedMessage)
        {
            Deserialize(serializedMessage);
        }

        public ControllerState(byte[] serializedMessage, ref int currentIndex)
        {
            Deserialize(serializedMessage, ref currentIndex);
        }



        public override void Deserialize(byte[] serializedMessage, ref int currentIndex)
        {
            int arraylength = -1;
            bool hasmetacomponents = false;
            object __thing;
            int piecesize = 0;
            byte[] thischunk, scratch1, scratch2;
            IntPtr h;
            
            //joint_distance
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (joint_distance == null)
                joint_distance = new double[arraylength];
            else
                Array.Resize(ref joint_distance, arraylength);
// Start Xamla
                //joint_distance
                piecesize = Marshal.SizeOf(typeof(double)) * joint_distance.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, joint_distance, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //cartesian_distance
            hasmetacomponents |= false;
            arraylength = BitConverter.ToInt32(serializedMessage, currentIndex);
            currentIndex += Marshal.SizeOf(typeof(System.Int32));
            if (cartesian_distance == null)
                cartesian_distance = new double[arraylength];
            else
                Array.Resize(ref cartesian_distance, arraylength);
// Start Xamla
                //cartesian_distance
                piecesize = Marshal.SizeOf(typeof(double)) * cartesian_distance.Length;
                if (currentIndex + piecesize > serializedMessage.Length) {
                    throw new Exception("Memory allocation failed: Ran out of bytes to read.");
                }
                Buffer.BlockCopy(serializedMessage, currentIndex, cartesian_distance, 0, piecesize);
                currentIndex += piecesize;
// End Xamla

            //error_code
            piecesize = Marshal.SizeOf(typeof(long));
            h = IntPtr.Zero;
            if (serializedMessage.Length - currentIndex != 0)
            {
                h = Marshal.AllocHGlobal(piecesize);
                Marshal.Copy(serializedMessage, currentIndex, h, piecesize);
            }
            if (h == IntPtr.Zero) throw new Exception("Memory allocation failed");
            error_code = (long)Marshal.PtrToStructure(h, typeof(long));
            Marshal.FreeHGlobal(h);
            currentIndex+= piecesize;
            //converged
            converged = serializedMessage[currentIndex++]==1;
            //self_collision_check_enabled
            self_collision_check_enabled = serializedMessage[currentIndex++]==1;
            //scene_collision_check_enabled
            scene_collision_check_enabled = serializedMessage[currentIndex++]==1;
            //joint_limits_check_enabled
            joint_limits_check_enabled = serializedMessage[currentIndex++]==1;
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
            
            //joint_distance
            hasmetacomponents |= false;
            if (joint_distance == null)
                joint_distance = new double[0];
            pieces.Add(BitConverter.GetBytes(joint_distance.Length));
// Start Xamla
                //joint_distance
                x__size = Marshal.SizeOf(typeof(double)) * joint_distance.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(joint_distance, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //cartesian_distance
            hasmetacomponents |= false;
            if (cartesian_distance == null)
                cartesian_distance = new double[0];
            pieces.Add(BitConverter.GetBytes(cartesian_distance.Length));
// Start Xamla
                //cartesian_distance
                x__size = Marshal.SizeOf(typeof(double)) * cartesian_distance.Length;
                scratch1 = new byte[x__size];
                Buffer.BlockCopy(cartesian_distance, 0, scratch1, 0, x__size);
                pieces.Add(scratch1);
// End Xamla

            //error_code
            scratch1 = new byte[Marshal.SizeOf(typeof(long))];
            h = GCHandle.Alloc(scratch1, GCHandleType.Pinned);
            Marshal.StructureToPtr(error_code, h.AddrOfPinnedObject(), false);
            h.Free();
            pieces.Add(scratch1);
            //converged
            thischunk = new byte[1];
            thischunk[0] = (byte) ((bool)converged ? 1 : 0 );
            pieces.Add(thischunk);
            //self_collision_check_enabled
            thischunk = new byte[1];
            thischunk[0] = (byte) ((bool)self_collision_check_enabled ? 1 : 0 );
            pieces.Add(thischunk);
            //scene_collision_check_enabled
            thischunk = new byte[1];
            thischunk[0] = (byte) ((bool)scene_collision_check_enabled ? 1 : 0 );
            pieces.Add(thischunk);
            //joint_limits_check_enabled
            thischunk = new byte[1];
            thischunk[0] = (byte) ((bool)joint_limits_check_enabled ? 1 : 0 );
            pieces.Add(thischunk);
            // combine every array in pieces into one array and return it
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
            
            //joint_distance
            arraylength = rand.Next(10);
            if (joint_distance == null)
                joint_distance = new double[arraylength];
            else
                Array.Resize(ref joint_distance, arraylength);
            for (int i=0;i<joint_distance.Length; i++) {
                //joint_distance[i]
                joint_distance[i] = (rand.Next() + rand.NextDouble());
            }
            //cartesian_distance
            arraylength = rand.Next(10);
            if (cartesian_distance == null)
                cartesian_distance = new double[arraylength];
            else
                Array.Resize(ref cartesian_distance, arraylength);
            for (int i=0;i<cartesian_distance.Length; i++) {
                //cartesian_distance[i]
                cartesian_distance[i] = (rand.Next() + rand.NextDouble());
            }
            //error_code
            error_code = (System.Int64)(rand.Next() << 32) | rand.Next();
            //converged
            converged = rand.Next(2) == 1;
            //self_collision_check_enabled
            self_collision_check_enabled = rand.Next(2) == 1;
            //scene_collision_check_enabled
            scene_collision_check_enabled = rand.Next(2) == 1;
            //joint_limits_check_enabled
            joint_limits_check_enabled = rand.Next(2) == 1;
        }

        public override bool Equals(RosMessage ____other)
        {
            if (____other == null)
				return false;
            bool ret = true;
            var other = ____other as Messages.xamlamoveit_msgs.ControllerState;
            if (other == null)
                return false;
            if (joint_distance.Length != other.joint_distance.Length)
                return false;
            for (int __i__=0; __i__ < joint_distance.Length; __i__++)
            {
                ret &= joint_distance[__i__] == other.joint_distance[__i__];
            }
            if (cartesian_distance.Length != other.cartesian_distance.Length)
                return false;
            for (int __i__=0; __i__ < cartesian_distance.Length; __i__++)
            {
                ret &= cartesian_distance[__i__] == other.cartesian_distance[__i__];
            }
            ret &= error_code == other.error_code;
            ret &= converged == other.converged;
            ret &= self_collision_check_enabled == other.self_collision_check_enabled;
            ret &= scene_collision_check_enabled == other.scene_collision_check_enabled;
            ret &= joint_limits_check_enabled == other.joint_limits_check_enabled;
            // for each SingleType st:
            //    ret &= {st.Name} == other.{st.Name};
            return ret;
        }
    }
}
