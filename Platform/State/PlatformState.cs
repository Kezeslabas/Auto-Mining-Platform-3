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

        public class PlatformState : StateContext
        {
            public const string DEFAULT_SECTION = "PlatformState";

            private readonly IniState[] allStates;

            /// <summary>
            /// Indicates if the Mining Sequence is a digging or a mining.
            /// </summary>
            public TypedIniState<bool> IsDig { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsDig", false);

            /// <summary>
            /// Indicates if the detected blocks create a valid mining platform.
            /// </summary>
            public TypedIniState<bool> IsValidPlatform { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsValidPlatform", false);

            /// <summary>
            /// Indicates if a mining is In Progress at the moment and the platform should be running or not.
            /// </summary>
            public TypedIniState<bool> IsRunning { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsRunning", false);

            public PlatformState()
            {
                allStates = new IniState[]
                {
                    IsDig,
                    IsValidPlatform,
                    IsRunning
                };
            }

            public IniState[] AllStates()
            {
                return allStates;
            }
        }
    }
}
