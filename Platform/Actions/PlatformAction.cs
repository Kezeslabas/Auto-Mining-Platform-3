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
                        //Find and Set StepNumber for Meter
                    }
                }

                //Set Platform To Step

                stateManager.SaveStates();
            }
        }
    }
}
