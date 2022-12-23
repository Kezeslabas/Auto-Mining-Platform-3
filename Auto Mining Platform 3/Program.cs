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

        readonly PlatformConfig config;
        readonly IniStateManager configManager;

        bool indicator = false;

        public Program()
        {
            Debugger.Init(DEBUG_ENABLED, Echo);

            runManager = new RunManager(Runtime);

            config = new PlatformConfig();
            configManager = new PartialCustomDataIniStateManager(SaveConfig, LoadConfig, config.StatesToImmutableDictionary());

            router = new Router(Echo, new Dictionary<string, Action<MyCommandLine>>(){
                { "test", p => Echo("Test") },
                { "pause", p => { runManager.Paused = true; }},
                { "start", p => { runManager.Paused = false; }},
                { "load", p => { configManager.LoadStates(); }}
            });

            configManager.SaveStates();
            configManager.LoadStates();
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

            Debugger.Debug(config.MainTag.Value);
            runManager.ApplySchedule();
        }

        public void SaveConfig(string customData) => Me.CustomData = customData;

        public string LoadConfig() => Me.CustomData;

        public void SaveToStorage(string storageString) => Storage = storageString;

        public string LoadFromStorage() => Storage;
    }
}
