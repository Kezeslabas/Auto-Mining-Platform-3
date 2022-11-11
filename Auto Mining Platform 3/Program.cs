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
        const bool DEBUG_ENABLED = true;

        readonly Debugger debugger;
        readonly Config config;
        readonly ConfigBlockProvider blockProvider;

        public Program()
        {
            debugger = new Debugger(Echo, DEBUG_ENABLED);

            config = new Config(debugger);
            blockProvider = new ConfigBlockProvider(debugger, GetBlocksWithName, config, ImmutableList.Create<IBlockConsumer>());
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            blockProvider.LoadBlocks();
        }

        /// <summary>
        /// Wrapper around <see cref="IMyGridTerminalSystem.SearchBlocksOfName(string, List{IMyTerminalBlock}, Func{IMyTerminalBlock, bool})"/>,
        /// to access all blocks with the provided name.
        /// </summary>
        public void GetBlocksWithName(string name, List<IMyTerminalBlock> blocks)
        {
            GridTerminalSystem.SearchBlocksOfName(name, blocks);
        }
    }
}
