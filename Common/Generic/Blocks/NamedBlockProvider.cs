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

        public class NamedBlockProvider : Debuggable
        {
            private readonly Action<string, List<IMyTerminalBlock>> loadBlocksAction;
            private readonly ImmutableList<IBlockConsumer> consumers;

            private readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

            public NamedBlockProvider(Debugger debugger, Action<string, List<IMyTerminalBlock>> loadBlocksAction, ImmutableList<IBlockConsumer> consumers) : base(debugger)
            {
                this.loadBlocksAction = loadBlocksAction;
                this.consumers = consumers;
            }

            /// <summary>
            /// Re-Loads all blocks from teh GridTerminalSystem, and consumes all.
            /// </summary>
            public void LoadBlocks(string name)
            {
                blocks.Clear();
                loadBlocksAction.Invoke(name, blocks);

                blocks.ForEach(block => {
                    consumers.ForEach(consumer => consumer.ConsumeBlock(block));
                });

                debugger.Debug("Blocks found: " + blocks.Count);
            }
        }
    }
}
