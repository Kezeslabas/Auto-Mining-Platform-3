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
        /// <b>Experimental, use with caution.</b>
        /// <para/>
        /// Uses an array and stores the values as string separated by a ';' using the provided getters/setters
        /// </summary>
        public abstract class IndexStateManager
        {
            private readonly int origianlStateVersion;
            private readonly Int32State stateVersion;
            private readonly IState[] states;
            private readonly Action<string> setStateString;
            private readonly Func<string> getStateString;

            public IndexStateManager(Action<string> setStateString, Func<string> getStateString, int origianlStateVersion, IState[] states)
            {
                this.origianlStateVersion = origianlStateVersion;
                stateVersion = new Int32State() { Value = origianlStateVersion };

                this.setStateString = setStateString;
                this.getStateString = getStateString;

                this.states = new IState[] { stateVersion }.Concat(states).ToArray();
            }

            public void SaveStates()
            {
                setStateString.Invoke(string.Join(";", states.Select(p => p.ToState())));   
            }

            public void LoadStates()
            {
                string[] stateStrings = getStateString.Invoke().Split(';');

                if(stateStrings.Length == 0)
                {
                    return;
                }

                stateVersion.ParseAndSet(stateStrings[0]);

                if(stateVersion.Value == origianlStateVersion)
                {
                    for (int i = 1; i < stateStrings.Length; i++)
                    {
                        states[i].ParseAndSet(stateStrings[i]);
                    }

                    return;
                }

                while (stateVersion.Value < origianlStateVersion)
                {
                    stateVersion.Value =  ConvertStateToNext();
                }
            }

            protected abstract int ConvertStateToNext();
        }
    }
}
