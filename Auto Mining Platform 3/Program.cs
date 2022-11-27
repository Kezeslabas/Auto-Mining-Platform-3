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

        readonly MyCommandLine CL = new MyCommandLine();
        readonly Router router;

        public Program()
        {
            Debugger.Init(DEBUG_ENABLED, Echo);
            router = new Router(Echo, new Dictionary<string, Action<MyCommandLine>>{
                { "test", p => Echo("Test") }
            });
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            router.ParseAndRoute(argument);   
        }
    }
}
