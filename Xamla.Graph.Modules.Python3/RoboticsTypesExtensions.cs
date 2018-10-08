using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Numerics;
using Xamla.Types;
using Xamla.Robotics.Types;


namespace Xamla.Graph.Modules.Python3
{
    public class XamlaMotionException
        : ApplicationException
    {
        public XamlaMotionException()
            : base("NumpyException")
        {
        }

        public XamlaMotionException(string message)
            : base(message)
        {
        }

        public XamlaMotionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public static class RoboticsTypesExtensions
    {
        internal static dynamic pyXamlaMotionTypes;
        internal static dynamic pyPyQuaternion;
        internal static dynamic pyDateTime;
        internal static PyObject deepcopy;

        public static bool Initialize()
        {
            using (Py.GIL())
            {
                pyXamlaMotionTypes = Py.Import("xamla_motion.data_types");
                pyPyQuaternion = Py.Import("pyquaternion");
                pyDateTime = Py.Import("datetime");
                //PyTimeDelta = time.GetAttr("timedelta");


                var copy = Py.Import("copy");
                deepcopy = copy.GetAttr("deepcopy");
            }

            return true;
        }

        // extenstions to convert C# Xamla.Robotics.Types to Python Xamla.Robotics.Types
        public static PyObject ToPython(this JointSet obj)
        {
            PyObject jointNames = PyConvert.ToPyObject(obj.ToArray());
            using (Py.GIL())
            {
                return pyXamlaMotionTypes.JointSet(jointNames);
            }
        }

        public static PyObject ToPython(this JointValues obj)
        {
            PyObject jointSet = obj.JointSet.ToPython();
            PyObject values = PyConvert.ToPyObject(obj.Values);
            using (Py.GIL())
            {
                return pyXamlaMotionTypes.JointValues(jointSet, values);
            }
        }

        public static PyObject ToPython(this Pose obj)
        {
            using (Py.GIL())
            {
                var translation = new PyList();
                translation.Append(obj.Translation.X.ToPython());
                translation.Append(obj.Translation.Y.ToPython());
                translation.Append(obj.Translation.Z.ToPython());

                var rotation = new PyList();
                rotation.Append(obj.Rotation.W.ToPython());
                rotation.Append(obj.Rotation.X.ToPython());
                rotation.Append(obj.Rotation.Y.ToPython());
                rotation.Append(obj.Rotation.Z.ToPython());

                var frame = new PyString(obj.Frame);

                var quaternion = pyPyQuaternion.Quaternion(rotation);
                return pyXamlaMotionTypes.Pose(translation, quaternion, frame);
            }
        }

        public static PyObject ToPython(this TimeSpan obj)
        {
            var days = PyConvert.ToPyObject(obj.Days);
            var seconds = PyConvert.ToPyObject(obj.Hours * 3600 + obj.Minutes * 60 + obj.Seconds);
            var microseconds = PyConvert.ToPyObject(obj.Milliseconds * 1000);

            using (Py.GIL())
            {
                return pyDateTime.timedelta(days: days, seconds: seconds, microseconds: microseconds);
            }
        }

        public static PyObject ToPython(this JointTrajectoryPoint obj)
        {
            var timeFromStart = PyConvert.ToPyObject(obj.TimeFromStart);
            var positions = PyConvert.ToPyObject(obj.Positions);
            var velocities = PyConvert.ToPyObject(obj.Velocities);
            var accelerations = PyConvert.ToPyObject(obj.Accelerations);
            var efforts = PyConvert.ToPyObject(obj.Effort);

            using (Py.GIL())
            {
                return pyXamlaMotionTypes.JointTrajectoryPoint(timeFromStart, positions, velocities, accelerations, efforts);
            }
        }

        public static PyObject ToPython(this JointTrajectory obj)
        {
            var jointSet = PyConvert.ToPyObject(obj.JointSet);
            using (Py.GIL())
            {
                var points = new PyList();
                foreach (var p in obj)
                {
                    points.Append(PyConvert.ToPyObject(p));
                }
                return pyXamlaMotionTypes.JointTrajectory(jointSet, points);
            }
        }

        public static PyObject ToPython(this JointStates obj)
        {
            var positions = PyConvert.ToPyObject(obj.Positions);
            var velocities = PyConvert.ToPyObject(obj.Velocities);
            var efforts = PyConvert.ToPyObject(obj.Efforts);
            using (Py.GIL())
            {
                return pyXamlaMotionTypes.JointStates(positions, velocities, efforts);
            }
        }

        public static PyObject ToPython(this JointPath obj)
        {
            var jointSet = PyConvert.ToPyObject(obj.JointSet);
            using (Py.GIL())
            {
                var jointValues = new PyList();
                foreach (var v in obj)
                {
                    jointValues.Append(PyConvert.ToPyObject(v));
                }
                return pyXamlaMotionTypes.JointPath(jointSet, jointValues);
            }
        }

        public static PyObject ToPython(this CartesianPath obj)
        {
            using (Py.GIL())
            {
                var poses = new PyList();
                foreach (var p in obj)
                {
                    poses.Append(PyConvert.ToPyObject(p));
                }
                return pyXamlaMotionTypes.CartesianPath(poses);
            }
        }

        public static PyObject ToPython(this PlanParameters obj)
        {
            var nullObject = new object();
            nullObject = null;
            var none = nullObject.ToPython();
            var moveGroupName = PyConvert.ToPyObject(obj.MoveGroupName);
            var jointSet = PyConvert.ToPyObject(obj.JointSet);
            var maxVelocity = PyConvert.ToPyObject(obj.MaxVelocity);
            var maxAcceleration = PyConvert.ToPyObject(obj.MaxAcceleration);
            var sampleResolution = PyConvert.ToPyObject(obj.SampleResolution);
            var collisionCheck = PyConvert.ToPyObject(obj.CollisionCheck);
            var maxDeviation = PyConvert.ToPyObject(obj.MaxDeviation);
            using (Py.GIL())
            {
                return pyXamlaMotionTypes.PlanParameters.from_arguments(moveGroupName, jointSet, maxVelocity, maxAcceleration, none, none,
                                                     sample_resolution: sampleResolution, collisionCheck: collisionCheck,
                                                     max_devitation: maxDeviation);
            }
        }

        public static PyObject ToPython(this CollisionPrimitive obj)
        {
            var kindInt = PyConvert.ToPyObject((int)obj.Kind);
            var parameters = PyConvert.ToPyObject(obj.Parameters);
            var pose = PyConvert.ToPyObject(obj.Pose);
            using (Py.GIL())
            {
                var kind = pyXamlaMotionTypes.CollisionPrimitiveKind(kindInt);
                return pyXamlaMotionTypes.CollisionPrimitive(kind, parameters, pose);
            }
        }

        public static PyObject ToPython(this CollisionObject obj)
        {
            var frame = PyConvert.ToPyObject(obj.Frame);
            using (Py.GIL())
            {
                var primitives = new PyList();
                foreach (var p in obj.Primitives)
                {
                    primitives.Append(PyConvert.ToPyObject(p));
                }
                return pyXamlaMotionTypes.CollisionObject(primitives, frame);
            }
        }

        // methods to convert Python Xamla.Robotics.Types to C# Xamla.Robotics.Types

        public static JointSet FromPyJointSet(PyObject obj)
        {
            var names = (List<string>)PyConvert.ToClrObject(obj.GetAttr("names"), typeof(List<string>));
            return new JointSet(names);
        }

        public static JointValues FromPyJointValues(PyObject obj)
        {
            var jointSet = FromPyJointSet(obj.GetAttr("joint_set"));
            var values = (A<double>)PyConvert.ToClrObject(obj.GetAttr("values"), typeof(A<double>));
            return new JointValues(jointSet, values);
        }

        public static Pose FromPyPose(PyObject obj)
        {
            var t = (A<double>)PyConvert.ToClrObject(obj.GetAttr("translation"), typeof(A<double>));
            var translationVector = new Vector3((float)t[0], (float)t[1], (float)t[2]);
            var pyQuaternion = obj.GetAttr("quaternion");
            var q = (A<double>)PyConvert.ToClrObject(pyQuaternion.GetAttr("elements"), typeof(A<double>));
            var quaternion = new Quaternion((float)q[1], (float)q[2], (float)q[3], (float)q[0]);
            var frame = (string)PyConvert.ToClrObject(obj.GetAttr("frame_id"), typeof(string));
            return new Pose(translationVector, quaternion, frame);
        }

        public static TimeSpan FromPyTimeDelta(PyObject obj)
        {
            var days = (int)PyConvert.ToClrObject(obj.GetAttr("days"), typeof(int));
            var seconds = (int)PyConvert.ToClrObject(obj.GetAttr("seconds"), typeof(int));
            var microseconds = (int)PyConvert.ToClrObject(obj.GetAttr("microseconds"), typeof(int));
            return new TimeSpan((long)days * 864000000000 + (long)seconds * 10000000 + (long)microseconds * 10);
        }

        public static JointTrajectoryPoint FromPyJoinTrajectoryPoint(PyObject obj)
        {
            var timeFromStart = (TimeSpan)PyConvert.ToClrObject(obj.GetAttr("time_from_start"), typeof(TimeSpan));
            var positions = (JointValues)PyConvert.ToClrObject(obj.GetAttr("positions"), typeof(JointValues));
            var velocities = (JointValues)PyConvert.ToClrObject(obj.GetAttr("velocities"), typeof(JointValues));
            var accelerations = (JointValues)PyConvert.ToClrObject(obj.GetAttr("accelerations"), typeof(JointValues));
            var efforts = (JointValues)PyConvert.ToClrObject(obj.GetAttr("efforts"), typeof(JointValues));
            return new JointTrajectoryPoint(timeFromStart, positions, velocities, accelerations, efforts);
        }

        public static JointTrajectory FromPyJointTrajectory(PyObject obj)
        {
            var jointSet = (JointSet)PyConvert.ToClrObject(obj.GetAttr("joint_set"), typeof(JointSet));
            var points = (List<JointTrajectoryPoint>)PyConvert.ToClrObject(obj.GetAttr("points"), typeof(List<JointTrajectoryPoint>));
            var valid = (bool)PyConvert.ToClrObject(obj.GetAttr("is_valid"), typeof(bool));
            return new JointTrajectory(jointSet, points, valid);
        }

        public static JointStates FromPyJointStates(PyObject obj)
        {
            var positions = (JointValues)PyConvert.ToClrObject(obj.GetAttr("positions"), typeof(JointValues));
            var velocities = (JointValues)PyConvert.ToClrObject(obj.GetAttr("velocities"), typeof(JointValues));
            var efforts = (JointValues)PyConvert.ToClrObject(obj.GetAttr("efforts"), typeof(JointValues));
            return new JointStates(positions, velocities, efforts);
        }

        public static JointPath FromPyJointPath(PyObject obj)
        {
            var jointSet = (JointSet)PyConvert.ToClrObject(obj.GetAttr("joint_set"), typeof(JointSet));
            var points = (List<JointValues>)PyConvert.ToClrObject(obj.GetAttr("points"), typeof(List<JointValues>));
            return new JointPath(jointSet, points);
        }

        public static CartesianPath FromPyCartesianPath(PyObject obj)
        {
            var poses = (List<Pose>)PyConvert.ToClrObject(obj.GetAttr("points"), typeof(List<Pose>));
            return new CartesianPath(poses);
        }

        public static PlanParameters FromPyPlanParameters(PyObject obj)
        {
            var moveGroupName = (string)PyConvert.ToClrObject(obj.GetAttr("move_group_name"), typeof(string));
            var jointSet = (JointSet)PyConvert.ToClrObject(obj.GetAttr("joint_set"), typeof(JointSet));
            var maxVelocity = (A<double>)PyConvert.ToClrObject(obj.GetAttr("max_velocity"), typeof(A<double>));
            var maxAcceleration = (A<double>)PyConvert.ToClrObject(obj.GetAttr("max_acceleration"), typeof(A<double>));
            var sampleResolution = (double)PyConvert.ToClrObject(obj.GetAttr("sample_resolution"), typeof(double));
            var collisionCheck = (bool)PyConvert.ToClrObject(obj.GetAttr("collision_check"), typeof(bool));
            var maxDeviation = (double)PyConvert.ToClrObject(obj.GetAttr("max_deviation"), typeof(double));
            return new PlanParameters(moveGroupName, jointSet, maxVelocity.ToArray(), maxAcceleration.ToArray(),
                                      sampleResolution, collisionCheck, maxDeviation);
        }

        public static CollisionPrimitive FromPyCollisionPrimitive(PyObject obj)
        {
            var pyKind = obj.GetAttr("kind");
            var kind = (CollisionPrimitiveKind)PyConvert.ToClrObject(pyKind.GetAttr("value"), typeof(int));
            var parameters = (A<double>)PyConvert.ToClrObject(obj.GetAttr("parameters"), typeof(A<double>));
            var pose = (Pose)PyConvert.ToClrObject(obj.GetAttr("pose"), typeof(Pose));
            return new CollisionPrimitive(kind, parameters.ToArray(), pose);
        }

        public static CollisionObject FromPyCollisionObject(PyObject obj)
        {
            var frame = (string)PyConvert.ToClrObject(obj.GetAttr("frame_id"), typeof(string));
            var primitives = (List<CollisionPrimitive>)PyConvert.ToClrObject(obj.GetAttr("primitives"), typeof(List<CollisionPrimitive>));
            return new CollisionObject(frame, primitives);
        }
    }
}
