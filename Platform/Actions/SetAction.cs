﻿using Sandbox.Game.EntityComponents;
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
        /// <summary>
        /// The Action that should be taken when the "set" command is used.
        /// </summary>
        public class SetAction : IPlatformAction
        {
            private readonly IniStateManager configManager;
            private readonly IniStateManager stateManager;
            private readonly PlatformState state;
            private readonly PlatformRunner platformRunner;

            private readonly StepManager stepManager;

            private const string DIG_FLAG = "dig";
            private const string METER_FLAG = "m";

            public SetAction(IniStateManager configManager, IniStateManager stateManager, PlatformState state, PlatformRunner platformRunner, StepManager stepManager)
            {
                this.configManager = configManager;
                this.stateManager = stateManager;
                this.state = state;
                this.platformRunner = platformRunner;
                this.stepManager = stepManager;
            }

            /// <summary>
            /// Resets all states in the platform, and loads the config. (If config can't be loaded, then exits.)
            /// <para/>
            /// TODO Work In Progress
            /// <para/>
            /// Uses the provided command line to parse additional arguments and flags.
            /// <para/>
            /// Arguments:
            /// <list type="bullet">
            ///     <item>arg[1] Optional. After the Set, the platform will be aligned to the provided step number.</item>
            /// </list>
            /// Flags:
            /// <list type="bullet">
            ///     <item>-m Interprets arg[1] as depth in meters, then finds and applies the appropriate stepNumber for that depth.</item>
            ///     <item>-dig Instructs the platform to setup for a digging instead of mining.</item>
            /// </list>
            /// </summary>
            /// <param name="cl">Command Line to get extra argument from.</param>
            public void DoAction(MyCommandLine cl)
            {
                platformRunner.StopPlatform();//Stop any progress

                stateManager.ResetHolder();//Reset All states
                stateManager.LoadStates();

                if (!configManager.LoadStates().Success)
                {
                    return;
                }

                state.IsDig.Set(cl.Switch(DIG_FLAG));

                //TODO Check if Platform Is Valid
                state.IsValidPlatform.Set(true);

                //TODO Use Actual Params
                stepManager.Init(2, 2, true);

                int stepNumber = 0;
                if (cl.ArgumentCount > 1)
                {
                    string arg1 = cl.Argument(1);
                    if (!int.TryParse(arg1, out stepNumber))
                    {
                        Debugger.Error("Can't Parse arg[1] of SET: " + arg1);
                    }

                    if (cl.Switch(METER_FLAG))
                    {
                        //TODO Find and Set StepNumber for Meter
                    }
                }

                stepManager.MoveToStep(stepNumber);

                stateManager.SaveStates();
            }
        }
    }
}