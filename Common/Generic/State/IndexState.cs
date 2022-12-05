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

        public interface IState
        {
            void parseAndSet(string valueString);
            string ToState();
        }

        public class Int32State : IState
        {
            public Int32 Value;

            public void parseAndSet(string valueString)
            {
                Int32.TryParse(valueString, out Value);
            }

            public string ToState()
            {
                return "" + Value;
            }
        }

        /// <summary>
        /// <b>Experimental, use with caution.</b>
        /// <para/>
        /// Uses an array and stores the values as string separated by a ';' in the Storage String.
        /// </summary>
        public abstract class StateManager
        {
            private readonly int origianlStateVersion;
            private readonly Int32State stateVersion;
            private readonly IState[] states;
            private readonly Action<string> setStorage;
            private readonly Func<string> getStorage;

            public StateManager(Action<string> setStorage, Func<string> getStorage, int origianlStateVersion, IState[] states)
            {
                this.origianlStateVersion = origianlStateVersion;
                stateVersion = new Int32State() { Value = origianlStateVersion };

                this.setStorage = setStorage;
                this.getStorage = getStorage;

                this.states = new IState[] { stateVersion }.Concat(states).ToArray();
            }

            public void SaveStates()
            {
                setStorage.Invoke(string.Join(";", states.Select(p => p.ToState())));   
            }

            public void LoadStates()
            {
                string[] stateStrings = getStorage.Invoke().Split(';');

                if(stateStrings.Length == 0)
                {
                    return;
                }

                stateVersion.parseAndSet(stateStrings[0]);

                if(stateVersion.Value == origianlStateVersion)
                {
                    for (int i = 1; i < stateStrings.Length; i++)
                    {
                        states[i].parseAndSet(stateStrings[i]);
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
