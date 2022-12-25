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
        /// An <see cref="MyIni"/> based manager handling basic states.
        /// </summary>
        public abstract class IniStateManager
        {
            protected readonly MyIni _ini = new MyIni();

            protected readonly Action<string> setStateString;
            protected readonly Func<string> getStateString;

            /// <summary>
            /// </summary>
            /// <param name="setStateString">Method that updates the state holder.</param>
            /// <param name="getStateString">Method that loads the current state from the holder.</param>
            protected IniStateManager(Action<string> setStateString, Func<string> getStateString)
            {
                this.setStateString = setStateString;
                this.getStateString = getStateString;
            }

            /// <summary>
            /// Formats and saves the current state to the state holder.
            /// </summary>
            public abstract void SaveStates();

            /// <summary>
            /// Updates the internal states.
            /// </summary>
            /// <param name="ini">The ini holding the parsed state.</param>
            protected abstract void SetStates(MyIni ini);

            /// <summary>
            /// Parses the state string and tries to update all internal states.
            /// </summary>
            /// <returns>The result of the parsing.</returns>
            public MyIniParseResult LoadStates()
            {
                MyIniParseResult result;
                if (!_ini.TryParse(getStateString(), out result))
                {
                    Debugger.Error("Loading the config failed!");
                    Debugger.Error(result.Error);
                    return result;
                }

                SetStates(_ini);

                return result;
            }

            public void ResetHolder()
            {
                setStateString("");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// Assumes, that all states should be maintained always.
        /// </summary>
        public class AllStateIniStateManager : IniStateManager
        {
            protected readonly IniState[] states;

            /// <summary>
            /// Makes a new State Manager with the initial states.
            /// </summary>
            /// <param name="setStateString">Method that updates the state holder.</param>
            /// <param name="getStateString">Method that loads the current state from the holder.</param>
            /// <param name="states">The array of states that should be handled.</param>
            public AllStateIniStateManager(Action<string> setStateString, Func<string> getStateString, IniState[] states) : base(setStateString, getStateString)
            {
                this.states = states;
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void SaveStates()
            {
                _ini.Clear();
                Array.ForEach(states, p => _ini.Set(p.GetIniKey(), p.ToString()));
                setStateString(_ini.ToString());
            }



            /// <summary>
            /// Use the internal <see cref="MyIni"/> to parse and set all internal <see cref="IniState"/>
            /// </summary>
            protected override void SetStates(MyIni ini)
            {
                Array.ForEach(states, p => p.ParseAndSet(ini));
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// It only updates the states that are parsed and ignores the rest.
        /// <para/>
        /// It is designed to be used with a Custom Data of a Terminal Block.
        /// </summary>
        public class PartialCustomDataIniStateManager : IniStateManager
        {
            public const string DEFAULT_SECTION = "Config-Start";
            private const string CONFIG_END_SECTION = "Config-End";
            private const string CONFIG_START = "[" + DEFAULT_SECTION + "]";
            private const string CONFIG_END = "[" + CONFIG_END_SECTION + "]";

            private const string INI_END = "---";

            private const string DESCRIPTION = 
                "You may add config VALUES to the script\n" +
                "between " + CONFIG_START + " and " + CONFIG_END + " in any order.\n" +
                "You can check the possible config VALUES\n" +
                "in the script's description.\n" +
                "\n" +
                "Please DO NOT change the content here,\n" +
                "except between " + CONFIG_START + " and " + CONFIG_END + ",\n" +
                "otherwise your config values may be lost,\n" +
                "or not applied correctly!\n";

            private const string FINISHER = "///";

            private const string DEFAULT_CONTENT = 
                CONFIG_START + "\n" +
                "\n" +
                "\n" +
                "\n" +
                CONFIG_END + "\n" +
                INI_END + "\n" +
                DESCRIPTION +
                FINISHER;

            private readonly List<MyIniKey> _tempIniKeys = new List<MyIniKey>();

            private readonly ImmutableDictionary<MyIniKey, IniState> statesByKeys;

            /// <summary>
            /// Makes a new State Manager with the initial states.
            /// </summary>
            /// <param name="setStateString">Method that updates the state holder.</param>
            /// <param name="getStateString">Method that loads the current state from the holder.</param>
            /// <param name="statesByKeys">The dictionary containing the states.</param>
            public PartialCustomDataIniStateManager(Action<string> setStateString, Func<string> getStateString, ImmutableDictionary<MyIniKey, IniState> statesByKeys) : base(setStateString, getStateString)
            {
                this.statesByKeys = statesByKeys;
            }

            /// <summary>
            /// It checks if the state holder has the corect structure for reading, and if it doesn't then set's it with a default.
            /// The state holder is not updated otherwise.
            /// <para/>
            /// The structure that is accepted:
            /// <code>
            /// [Config-Start]
            /// ---
            /// </code>
            /// </summary>
            public override void SaveStates()
            {
                string stateString = getStateString();

                if (!stateString.StartsWith(CONFIG_START) || !stateString.EndsWith(FINISHER))
                {
                    setStateString(DEFAULT_CONTENT);
                }
            }

            /// <summary>
            /// <inheritdoc/>
            /// <para/>
            /// It only parses config values that belong to the <see cref="DEFAULT_SECTION"/> section of the <see cref="MyIni"/>
            /// </summary>
            /// <param name="ini">Ini to get the values.</param>
            protected override void SetStates(MyIni ini)
            {
                ini.GetKeys(DEFAULT_SECTION, _tempIniKeys);

                _tempIniKeys.ForEach(k => {
                    if (!statesByKeys.ContainsKey(k))
                    {
                        Debugger.Warn(k.Name + " is not a valid config property!");
                        return;
                    }

                    statesByKeys[k].ParseAndSet(ini);
                });

            }

        }

    }
}
