using System;
using System.Diagnostics;
using Xamla.Graph.MethodModule;

namespace Xamla.Graph.Modules
{
    [Module(ModuleType = "Xamla.Diagnostics.StartTimer")]
    public class StartTimer
        : SingleInstanceMethodModule
    {
        public StartTimer(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "Stopwatch", Description = "Stopwatch")]
        public Stopwatch StartTiming(
            [InputPin(Description = "", PropertyMode = PropertyMode.Never)] object trigger
        )
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            return stopWatch;
        }
    }

    [Module(ModuleType = "Xamla.Diagnostics.StopTimer")]
    public class StopTimer
        : SingleInstanceMethodModule
    {
        public enum StopBehaviour
        {
            KeepRunning,
            Stop
        }

        public StopTimer(IGraphRuntime runtime)
            : base(runtime, ModuleKind.Module, DisplayMode.Expanded)
        {
        }

        [ModuleMethod]
        [OutputPin(Name = "Time", Description = "Timespan")]
        public TimeSpan StopTiming(
            [InputPin(Description = "", PropertyMode = PropertyMode.Never)] Stopwatch stopwatch,
            [InputPin(Description = "", PropertyMode = PropertyMode.Never)] object trigger,
            [InputPin(Description = "", PropertyMode = PropertyMode.Default, Editor = WellKnownEditors.CheckBox)] StopBehaviour behaviour = StopBehaviour.Stop
        )
        {
            if (behaviour == StopBehaviour.Stop)
                stopwatch.Stop();

            return stopwatch.Elapsed;
        }
    }
}

