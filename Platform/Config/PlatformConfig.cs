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
        /// Contains the Configurable values of the Mining Platform.
        /// Specifically built for usage with <see cref="PartialCustomDataIniStateManager"/>
        /// </summary>
        public class PlatformConfig
        {
            private const string DEFAULT_SECTION = PartialCustomDataIniStateManager.DEFAULT_SECTION;

            public readonly UpdateFrequency DEFAULT_UPDATE_FREQUENCY = UpdateFrequency.Update100;

            /// <summary>
            /// The Main Idenitfier of the Mining Platform. Only blocks are considered for usage, that has this tag in their name.
            /// </summary>
            public readonly TypedIniState<string> MainTag = ValidatedTypedIniState<string>.OfString(DEFAULT_SECTION, "MAIN_TAG", "/Mine 01/", p => !string.IsNullOrEmpty(p), "Can't be empty!");

            /// <summary>
            /// Returns all configurable config fields as an Immutable Dictionary with the labels as Key
            /// </summary>
            /// <returns></returns>
            public ImmutableDictionary<MyIniKey, ContextedIniState> StatesToImmutableDictionary()
            {
                return new ContextedIniState[] 
                { 
                    MainTag

                }.ToImmutableDictionary(k => k.GetIniKey(), v => v);
            }
        }
    }
}
