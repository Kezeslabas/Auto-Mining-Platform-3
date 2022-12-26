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
        /// The specific states of the Mining Platform.
        /// </summary>
        public class PlatformState
        {
            public const string DEFAULT_SECTION = "PlatformState";

            private readonly ContextedIniState[] allStates;

            /// <summary>
            /// Indicates if the Mining Sequence is a digging or a mining.
            /// </summary>
            public TypedIniState<bool> IsDig { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsDig", false);

            /// <summary>
            /// Indicates if the detected blocks create a valid mining platform.
            /// </summary>
            public TypedIniState<bool> IsValidPlatform { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsValidPlatform", false);

            /// <summary>
            /// Indicates if a mining sequence was started with the current configuration.
            /// </summary>
            public TypedIniState<bool> IsMining { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsMining", false);

            /// <summary>
            /// Indicates if an autonomous running is in progress.
            /// </summary>
            public TypedIniState<bool> IsRunning { get; private set; } = TypedIniState<bool>.OfBool(DEFAULT_SECTION, "IsRunning", false);

            /// <summary>
            /// Contains the current step number of the mining sequence
            /// </summary>
            public TypedIniState<int> StepNumber { get; private set; } = TypedIniState<int>.OfInt(DEFAULT_SECTION, "StepNumber", 0);


            public PlatformState()
            {
                //Add All states that should be handled
                //TODO Write test to check this.
                allStates = new ContextedIniState[]
                {
                    IsDig,
                    IsMining,
                    IsValidPlatform,
                    IsRunning,
                    StepNumber
                };
            }

            /// <summary>
            /// The array containing all relevant states.
            /// </summary>
            public ContextedIniState[] AllStates()
            {
                return allStates;
            }
        }
    }
}
