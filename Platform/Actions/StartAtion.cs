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
        /// The Action that should be taken when the "start" command is used.
        /// </summary>
        public class StartAtion : IPlatformAction
        {
            private readonly PlatformState state;
            private readonly PlatformRunner platformRunner;

            public StartAtion(PlatformState state, PlatformRunner platformRunner)
            {
                this.state = state;
                this.platformRunner = platformRunner;
            }

            /// <summary>
            /// If the Platform is ready to be started, then it starts the mining sequence and the atutomatic running.
            /// When a mining is already in progress, then just restarts the running.
            /// </summary>
            /// <param name="cl">Command Line to get extra argument from.</param>
            public void DoAction(MyCommandLine cl)
            {
                if (!state.IsValidPlatform.Value)
                {
                    Debugger.Error("Platform is Not Valid!");
                    return;
                }

                //TODO Start Mining Components

                state.IsMining.Set(true);

                platformRunner.StartPlatform();
            }
        }
    }
}
