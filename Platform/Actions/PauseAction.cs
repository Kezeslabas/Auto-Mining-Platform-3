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
        /// <summary>
        /// The Action that should be taken when the "pause" command is used.
        /// </summary>
        public class PauseAction : IPlatformAction
        {
            private readonly PlatformRunner platformRunner;

            public PauseAction(PlatformRunner platformRunner)
            {
                this.platformRunner = platformRunner;
            }

            /// <summary>
            /// Pauses the Automatic Running.
            /// </summary>
            /// <param name="cl">Command Line to get extra argument from.</param>
            public void DoAction(MyCommandLine cl)
            {
                platformRunner.Pause();
            }
        }
    }
}
