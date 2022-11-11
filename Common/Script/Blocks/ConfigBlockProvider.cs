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

        public class ConfigBlockProvider : NamedBlockProvider
        {
            private readonly IMainTagConfig mainTagConfig;

            public ConfigBlockProvider(Debugger debugger, Action<string, List<IMyTerminalBlock>> loadBlocksAction, IMainTagConfig mainTagConfig, ImmutableList<IBlockConsumer> consumers)
                : base(debugger, loadBlocksAction, consumers)
            {
                this.mainTagConfig = mainTagConfig;
            }

            /// <summary>
            /// Re-Loads all blocks from teh GridTerminalSystem, and consumes all.
            /// </summary>
            public void LoadBlocks()
            {
                LoadBlocks(mainTagConfig.MainTag);
            }
        }
    }
}
