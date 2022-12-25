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
        public interface IPlatformAction
        {
            void DoAction(MyCommandLine cl);
        }

        /// <summary>
        /// Initializes the Platform and sets it to a starting position.
        /// </summary>
        public class SetAction : IPlatformAction
        {
            private readonly IniStateManager configManager;
            private readonly IniStateManager stateManager;
            private readonly PlatformState state;

            private const string DIG_FLAG = "dig";
            private const string METER_FLAG = "m";

            public SetAction(IniStateManager configManager, IniStateManager stateManager, PlatformState state)
            {
                this.configManager = configManager;
                this.stateManager = stateManager;
                this.state = state;
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
            /// <param name="cl">command line that parsed the main argument</param>
            public void DoAction(MyCommandLine cl)
            {
                stateManager.ResetHolder();
                stateManager.LoadStates();

                if (!configManager.LoadStates().Success)
                {
                    return;
                }

                state.IsDig.Set(cl.Switch(DIG_FLAG));

                //TODO Check if Valid
                state.IsValidPlatform.Set(true);


                int stepNumber = 0;
                if(cl.ArgumentCount > 1)
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

                //TODO Set Platform To Step

                stateManager.SaveStates();
            }
        }
    }
}
