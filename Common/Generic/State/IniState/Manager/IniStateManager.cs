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

            protected readonly Action<string> setStateHolder;
            protected readonly Func<string> getStateHolder;

            /// <summary>
            /// </summary>
            /// <param name="setStateHolder">Method that updates the state holder.</param>
            /// <param name="getStateHolder">Method that loads the current state from the holder.</param>
            protected IniStateManager(Action<string> setStateHolder, Func<string> getStateHolder)
            {
                this.setStateHolder = setStateHolder;
                this.getStateHolder = getStateHolder;
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
                if (!_ini.TryParse(getStateHolder(), out result))
                {
                    Debugger.Error("Loading the config failed!");
                    Debugger.Error(result.Error);
                    return result;
                }

                SetStates(_ini);

                return result;
            }

            /// <summary>
            /// Clears the StateHolder by setting it to an empty string.
            /// </summary>
            public void ResetHolder()
            {
                setStateHolder("");
            }
        }
    }
}
