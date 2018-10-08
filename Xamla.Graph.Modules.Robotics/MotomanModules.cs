using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rosvita.RestApi;
using Rosvita.RosGardener.Contracts;
using Rosvita.RosMonitor;
using Xamla.Graph;
using Xamla.Graph.MethodModule;
using Xamla.MessageRouter.Client;
using Xamla.MessageRouter.Common;
using Xamla.Robotics.Types;
using Xamla.Types.Converters;
using Xamla.Utilities;
using xamlamoveit = Messages.xamlamoveit_msgs;
using Motoman = Xamla.Robotics.Motoman;
using Xamla.Robotics.Motoman;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Xamla.Graph.Modules.Robotics
{
    public static partial class StaticModules
    {
        const string DEFAULT_MOTOMAN_SDA10F_BASE_NAME = "sda10f";

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.ReadIO", Flow = true)]
        public static uint ReadIO(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int address)
        {
            using (var motomanIo = new Motoman.CommIO(rosClient.GlobalNodeHandle, robotName))
            {
                return motomanIo.Read(address);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.WriteIO", Flow = true)]
        public static void WriteIO(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int address,
            [InputPin(PropertyMode = PropertyMode.Default)] uint value)
        {
            using (var motomanIo = new Motoman.CommIO(rosClient.GlobalNodeHandle, robotName))
            {
                motomanIo.Write(address, value);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.ListJobs", Flow = true)]
        public static string[] ListJobs(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                return jobControl.ListJobs();
            }
        }

        public class StartJobAndWaitDisplayName
            : IDynamicDisplayName
        {
            public string Format(IModule module)
            {
                string jobName = module.Properties.Get<string>("jobName");
                if (string.IsNullOrEmpty(jobName))
                    return null;    // use default name

                return $"RunJob: {jobName}";
            }
        }

        /// <summary>
        /// Executes a Job and waits until it is finished.
        /// </summary>
        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.StartJobAndWaitForEnd", Flow = true, DynamicDisplayName = typeof(StartJobAndWaitDisplayName))]
        public async static Task StartJobAndWaitForEnd(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] string jobName,
            [InputPin(PropertyMode = PropertyMode.Default)] short taskNo = 0,
            CancellationToken cancel = default(CancellationToken)
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                await jobControl.StartJobAndWaitForEnd(jobName, taskNo, cancel);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.StartJob", Flow = true)]
        public static void StartJob(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] string jobName,
            [InputPin(PropertyMode = PropertyMode.Default)] short taskNo = 0
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.StartJob(jobName, taskNo);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.Hold", Flow = true)]
        public static void Hold(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] bool hold = true
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.Hold(hold);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.DeleteJob", Flow = true)]
        public static void DeleteJob(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] string jobName
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.DeleteJob(jobName);
            }
        }

        /// <summary>
        /// Retrieves the name, line number, step number of the current job.
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="taskNo">Task number</param>
        /// <returns>
        /// <return name="jobName">Job name (up to 32 characters for a job name)</return>
        /// <return name="step">Step</return>
        /// <return name="line">Line</return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.GetCurJob", Flow = true)]
        public static Tuple<string, int, int> GetCurJob(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] short taskNo = 0
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                var info = jobControl.GetCurJob(taskNo);
                return Tuple.Create<string, int, int>(info.JobName, info.Step, info.Line);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.SetCurJob", Flow = true)]
        public static void SetCurJob(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] string jobName,
            [InputPin(PropertyMode = PropertyMode.Default)] int line = 0
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.SetCurJob(jobName, line);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.GetMasterJob", Flow = true)]
        public static string GetMasterJob(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] short taskNo = 0
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                return jobControl.GetMasterJob(taskNo);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.SetMasterJob", Flow = true)]
        public static void SetMasterJob(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] string jobName,
            [InputPin(PropertyMode = PropertyMode.Default)] short taskNo = 0
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.SetMasterJob(jobName, taskNo);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.WaitForJobEnd", Flow = true)]
        public static void WaitForJobEnd(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] short taskNo = 0,
            [InputPin(PropertyMode = PropertyMode.Default)] short timeSeconds = 1
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.WaitForJobEnd(taskNo, timeSeconds);
            }
        }

        /// <summary>
        /// Generates an application alarm.
        /// </summary>
        /// <param name="robotName"></param>
        /// <param name="code">Alarm code: 8000 to 8999</param>
        /// <param name="message">Alarm message (up to 32 characters)</param>
        /// <param name="subCode">Alarm sub code</param>
        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.SetAlarm", Flow = true)]
        public static void SetAlarm(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] short code = 8000,
            [InputPin(PropertyMode = PropertyMode.Default)] string message = "Application Alarm",
            [InputPin(PropertyMode = PropertyMode.Default)] byte subCode = 0
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.SetAlarm(code, message, subCode);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.ResetAlarm", Flow = true)]
        public static void ResetAlarm(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.ResetAlarm();
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.CancelError", Flow = true)]
        public static void CancelError(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.CancelError();
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.GetUserVarB", Flow = true)]
        public static int GetUserVarB(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                return jobControl.GetUserVarB(no);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.GetUserVarI", Flow = true)]
        public static int GetUserVarI(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                return jobControl.GetUserVarI(no);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.GetUserVarD", Flow = true)]
        public static int GetUserVarD(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                return jobControl.GetUserVarD(no);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.GetUserVarR", Flow = true)]
        public static double GetUserVarR(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                return jobControl.GetUserVarR(no);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.GetUserVarS", Flow = true)]
        public static string GetUserVarS(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                return jobControl.GetUserVarS(no);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.PutUserVarB", Flow = true)]
        public static void PutUserVarB(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no,
            [InputPin(PropertyMode = PropertyMode.Default)] int value
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.PutUserVarB(no, value);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.PutUserVarI", Flow = true)]
        public static void PutUserVarI(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no,
            [InputPin(PropertyMode = PropertyMode.Default)] int value
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.PutUserVarI(no, value);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.PutUserVarD", Flow = true)]
        public static void PutUserVarD(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no,
            [InputPin(PropertyMode = PropertyMode.Default)] int value
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.PutUserVarD(no, value);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.PutUserVarR", Flow = true)]
        public static void PutUserVarR(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no,
            [InputPin(PropertyMode = PropertyMode.Default)] double value
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.PutUserVarR(no, value);
            }
        }

        [StaticModule(ModuleType = "Xamla.Robotics.Motoman.PutUserVarS", Flow = true)]
        public static void PutUserVarS(
            [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_MOTOMAN_SDA10F_BASE_NAME)] string robotName,
            [InputPin(PropertyMode = PropertyMode.Default)] int no,
            [InputPin(PropertyMode = PropertyMode.Default)] string value
        )
        {
            using (var jobControl = new JobControl(rosClient.GlobalNodeHandle, robotName))
            {
                jobControl.PutUserVarS(no, value);
            }
        }
    }
}