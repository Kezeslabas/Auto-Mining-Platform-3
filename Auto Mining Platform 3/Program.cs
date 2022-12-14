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
    partial class Program : MyGridProgram
    {
        readonly bool DEBUG_ENABLED = true;

        readonly Router router;
        readonly RunManager runManager;

        bool indicator = false;

        int run1 = 0;
        int run1Target = 0;
        int run10 = 0;
        int run10Target = 0;
        int run100 = 0;
        int run100Target = 0;

        public Program()
        {
            Debugger.Init(DEBUG_ENABLED, Echo);

            runManager = new RunManager(Runtime);

            router = new Router(Echo, new Dictionary<string, Action<MyCommandLine>>(){
                { "test", p => Echo("Test") },
                { "run1", p => {
                    int.TryParse(p.Argument(1), out run1Target);
                    runManager.ScheduleRunFrequency(UpdateFrequency.Update1);
                }},
                { "run10", p => {
                    int.TryParse(p.Argument(1), out run10Target);
                    runManager.ScheduleRunFrequency(UpdateFrequency.Update10);
                }},
                { "run100", p => {
                    int.TryParse(p.Argument(1), out run100Target);
                    runManager.ScheduleRunFrequency(UpdateFrequency.Update100);
                }},
                { "pause", p => { runManager.Paused = true; }},
                { "start", p => { runManager.Paused = false; }},
                { "reset1", p => { run1 = 0; run1Target = 0; }},
                { "reset10", p => { run10 = 0; run10Target = 0; }},
                { "reset100", p => { run100 = 0; run100Target = 0; }}
            });
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {

            Echo(indicator ? "[/-/-/-]" : "[-/-/-/]");
            indicator = !indicator;

            runManager.AnalyzeUpdateType(updateSource);

            if (runManager.IsAutoRun())
            {
                //DoSomething
            }
            else
            {
                router.ParseAndRoute(argument);
            }


            runManager.ApplySchedule();
        }

        public void SaveToStorage(string storageString)
        {
            Storage = storageString;
        }

        public string LoadFromStorage()
        {
            return Storage;
        }
    }
}
