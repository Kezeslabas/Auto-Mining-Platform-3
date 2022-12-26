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
        readonly AllStateIniStateManager stateManager;

        readonly PlatformRunner platformRunner;

        readonly StepManager stepManager;

        bool indicator = false;

        public Program()
        {
            Debugger.Init(DEBUG_ENABLED, Echo);

            runManager = new RunManager(Runtime);

            config = new PlatformConfig();
            configManager = new PartialCustomDataIniStateManager(SaveConfig, LoadConfig, config.StatesToImmutableDictionary());
            configManager.SaveStates();//Save to make sure that the Custom Data is initialized
            configManager.LoadStates();//Load to make sure that previously set config values are applied

            state = new PlatformState();
            
            stateManager = new AllStateIniStateManager(SaveToStorage, LoadFromStorage, state.AllStates());

            stateManager.LoadStates();//Load the states, so any previous platform states are continued

            platformRunner = new PlatformRunner(runManager, state, config);

            stepManager = new StepManager(state);

            Echo("---After Step Manager Init");
            Debugger.Debug("States: \n" + state.AllStates().Select(p => p.GetIniKey().Name + "=" + p.ToString()).Aggregate((p, k) => string.Join("\n", p, k)));

            router = new Router(Echo, new Dictionary<string, Action<MyCommandLine>> {
                { "set", new SetAction(configManager, stateManager, state, platformRunner, stepManager).DoAction },
                { "refresh", p => { return; } },
                { "start", new StartAtion(state, platformRunner).DoAction },
                { "pause", new PauseAction(platformRunner).DoAction },
                { "reset", new ResetAction(configManager, stateManager, platformRunner).DoAction }
            });

            //TODO Apply States

            stateManager.UpdateStates();
        }

        public void Save()
        {
            stateManager.SaveStates();
        }

        public void Main(string argument, UpdateType updateSource)
        {

            Echo(indicator ? "[/-/-/-]" : "[-/-/-/]");
            indicator = !indicator;

            if (platformRunner.IsAutoRun(updateSource))
            {
                //TEMP Step Controller
                stepManager.NextStep();
                if (stepManager.IsFinalStep())
                {
                    platformRunner.StopPlatform();
                }
                //---
            }
            else
            {
                router.ParseAndRoute(argument);
            }

            Echo("---");
            Debugger.Debug("States: \n" + state.AllStates().Select(p => p.GetIniKey().Name + "=" + p.ToString()).Aggregate((p, k) => string.Join("\n", p, k)));
            Echo("---");
            Debugger.Debug("Step: " + stepManager.CurrentStep);
            Debugger.Debug("StepNumber: " + stepManager.CurrentStepNumber);
            Debugger.Debug("MaxStep: " + stepManager.MaxSteps);
            Debugger.Debug("Final: " + stepManager.IsFinalStep());
            Echo("---");

            stateManager.UpdateStates();
            platformRunner.Apply();
        }

        public void SaveConfig(string customData) => Me.CustomData = customData;

        public string LoadConfig() => Me.CustomData;

        public void SaveToStorage(string storageString) => Storage = storageString;

        public string LoadFromStorage() => Storage;
    }
}
