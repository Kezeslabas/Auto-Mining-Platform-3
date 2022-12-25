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

        public class PlatformState
        {
            public const string DEFAULT_SECTION = "PlatformState";

            private readonly IniState[] allStates;

            public TypedIniState<bool> IsDig { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsDig", false);
            public TypedIniState<bool> IsValidPlatform { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsValidPlatform", false);
            public TypedIniState<bool> IsStartable { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsStartable", false);
            public TypedIniState<bool> IsMining { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsRunning", false);

            public PlatformState()
            {
                allStates = new IniState[]
                {
                    IsDig,
                    IsValidPlatform,
                    IsStartable,
                    IsMining
                };
            }

            public IniState[] AllStates()
            {
                return allStates;
            }
        }
    }
}
