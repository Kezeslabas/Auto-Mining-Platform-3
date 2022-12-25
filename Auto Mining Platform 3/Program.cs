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

        readonly PlatformState state;
        readonly IniStateManager stateManager;

        bool indicator = false;

        public Program()
        {
            Debugger.Init(DEBUG_ENABLED, Echo);

            runManager = new RunManager(Runtime);

            config = new PlatformConfig();
            configManager = new PartialCustomDataIniStateManager(SaveConfig, LoadConfig, config.StatesToImmutableDictionary());
            configManager.SaveStates();//Save to make sure that the Custom Data is initialized
            configManager.LoadStates();//Load To make sure that previously set config values are applied

            state = new PlatformState();
            stateManager = new AllStateIniStateManager(SaveToStorage, LoadFromStorage, state.AllStates());
            stateManager.LoadStates();//Load the states, so any previous platform states are continued

            router = new Router(Echo, new Dictionary<string, Action<MyCommandLine>> {
                { "set", new SetAction(configManager, stateManager, state).DoAction },
                { "refresh", p => { return; } },
                { "start", p => { return; } },
                { "pause", p => { return; } },
                { "reset", p => { return; } }
            });
        }

        public void Save()
        {
            stateManager.SaveStates();
        }

        public void Main(string argument, UpdateType updateSource)
        {

            Echo(indicator ? "[/-/-/-]" : "[-/-/-/]");
            indicator = !indicator;

            runManager.AnalyzeUpdateType(updateSource);

            if (runManager.IsAutoRun())
            {
                //TODO
            }
            else
            {
                router.ParseAndRoute(argument);
            }

            runManager.ApplySchedule();
        }

        public void SaveConfig(string customData) => Me.CustomData = customData;

        public string LoadConfig() => Me.CustomData;

        public void SaveToStorage(string storageString) => Storage = storageString;

        public string LoadFromStorage() => Storage;
    }
}
