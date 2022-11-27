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

        readonly Debugger debugger = new Debugger();

        public Program()
        {
            Debugger.Log("Before Init");

            Debugger.Init(DEBUG_ENABLED, Echo);

            Debugger.Log("After Init");
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            MyClass asd = new MyClass();
            asd.DoStaff();
        }

        class MyClass
        {
            public void DoStaff()
            {
                Debugger.Log("Inside Class");
            }
        }

        /// <summary>
        /// Wrapper around <see cref="IMyGridTerminalSystem.SearchBlocksOfName(string, List{IMyTerminalBlock}, Func{IMyTerminalBlock, bool})"/>,
        /// to access all blocks with the provided name.
        /// </summary>
        public void GetBlocksWithName(string name, List<IMyTerminalBlock> blocks)
        {
            GridTerminalSystem.GetBlocksOfType(blocks, block => block.CustomName.Contains(name));
        }
    }
}
