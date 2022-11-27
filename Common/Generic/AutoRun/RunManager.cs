using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public interface IRunScheduler
        {
            bool CheckForFrequency(UpdateFrequency frequency);
            bool CheckForFrequency(params UpdateFrequency[] frequencies);
            void ScheduleRunFrequency(UpdateFrequency frequency);
        }

        public class RunManager : IRunScheduler
        {
            private readonly IMyGridProgramRuntimeInfo runtime;
            private byte weigth1to10 = 0;
            private byte weigth10to100 = 0;

            public bool Paused { get; set; }
            private UpdateFrequency scheduledFrequency = UpdateFrequency.None;
            private UpdateFrequency currentFrequency = UpdateFrequency.None;

            public RunManager(IMyGridProgramRuntimeInfo runtime)
            {
                this.runtime = runtime;
            }

            public void AnalyzeUpdateType(UpdateType updateType)
            {
                if (Paused)
                {
                    return;
                }

                currentFrequency = UpdateFrequency.None;

                if (updateType.HasFlag(UpdateType.Update1))
                {
                    Debugger.Log("Is1");
                    weigth1to10++;
                    currentFrequency |= UpdateFrequency.Update1;
                }

                if (updateType.HasFlag(UpdateType.Update10) || weigth1to10 >= 10)
                {
                    Debugger.Log("Is10");
                    weigth1to10 = 0;
                    weigth10to100++;
                    currentFrequency |= UpdateFrequency.Update10;
                }

                if (updateType.HasFlag(UpdateType.Update100) || weigth10to100 >= 10)
                {
                    weigth10to100 = 0;
                    Debugger.Log("Is100");
                    currentFrequency |= UpdateFrequency.Update100;
                }

                if (updateType.HasFlag(UpdateType.Once))
                {
                    weigth1to10++;
                    Debugger.Log("IsOnce");
                    currentFrequency |= UpdateFrequency.Once;
                }

                Debugger.Log("Analyzed As: " + currentFrequency);
            }

            public bool CheckForFrequency(UpdateFrequency frequency)
            {
                return currentFrequency.HasFlag(frequency);
            }

            public bool CheckForFrequency(params UpdateFrequency[] frequencies)
            {
                return frequencies.Any(frequency => currentFrequency.HasFlag(frequency));
            }

            public void ScheduleRunFrequency(UpdateFrequency frequency)
            {
                if (!scheduledFrequency.HasFlag(frequency))
                {
                    scheduledFrequency |= frequency;
                }
            }

            public bool IsAutoRun()
            {
                Debugger.Log("CurrentFreq: " + currentFrequency);
                Debugger.Log("NoFreq: " + UpdateFrequency.None);
                return currentFrequency > UpdateFrequency.None;
            }

            public void ApplySchedule()
            {
                if (Paused)
                {
                    runtime.UpdateFrequency = UpdateFrequency.None;
                }
                else
                {
                    runtime.UpdateFrequency = scheduledFrequency;
                    Debugger.Log("Pre-End Actual Freq: " + runtime.UpdateFrequency);
                    Debugger.Log("Pre-End Scheduled: " + (int)scheduledFrequency);
                    scheduledFrequency = UpdateFrequency.None;
                }

                Debugger.Log("End Scheduled: " + scheduledFrequency);
                Debugger.Log("End Actual Freq: " + (int)runtime.UpdateFrequency);
            }
        }
    }
}
