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
        /// A simple Context that can register updates, and reset it's state upon consuming the update.
        /// </summary>
        public class StateContext
        {
            private bool wasUpdated = false;

            /// <summary>
            /// Updates the internal state to signals that there was an update for the context.
            /// </summary>
            public void Update()
            {
                wasUpdated = true;
            }

            /// <summary>
            /// Checks if there was an update to the context, then resets it.
            /// </summary>
            /// <returns>True if there was an update, false otherwise.</returns>
            public bool ConsumeUpdates()
            {
                if (!wasUpdated)
                {
                    return false;
                }

                wasUpdated = false;
                return true;
            }
        }
    }
}
