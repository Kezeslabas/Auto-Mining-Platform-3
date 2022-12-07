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
        public class IniStateManager
        {
            protected readonly MyIni _ini = new MyIni();

            private readonly Action<string> setStateString;
            private readonly Func<string> getStateString;
            protected readonly IniState[] states;

            public IniStateManager(Action<string> setStateString, Func<string> getStateString, IniState[] states)
            {
                this.setStateString = setStateString;
                this.getStateString = getStateString;
                this.states = states;
            }

            public void SaveStates()
            {
                _ini.Clear();
                Array.ForEach(states, p => _ini.Set(p.GetIniKey(), p.ToState()));
                setStateString(_ini.ToString());
            }

            public MyIniParseResult LoadStates()
            {
                MyIniParseResult result;
                if (!_ini.TryParse(getStateString(), out result))
                {
                    return result;
                }

                SetStates();

                return result;
            }

            protected virtual void SetStates()
            {
                Array.ForEach(states, p => p.ParseAndSet(_ini));
            }
        }

        public class SectionBasedIniStateManager : IniStateManager
        {
            private readonly ImmutableHashSet<string> sections;
            private readonly ImmutableDictionary<MyIniKey, IniState> statesByKeys;

            private readonly List<MyIniKey> _tempKeys;

            public SectionBasedIniStateManager(Action<string> setStateString, Func<string> getStateString, IniState[] states) : base(setStateString, getStateString, states)
            {
                sections = states.Select(p => p.GetIniKey().Section).ToImmutableHashSet();
                statesByKeys = states.ToImmutableDictionary(key => key.GetIniKey(), val => val);

                _tempKeys = new List<MyIniKey>(sections.Count);
            }

            protected override void SetStates()
            {
                if(sections.Count > 0)
                {
                    foreach (var section in sections)
                    {
                        _ini.GetKeys(section, _tempKeys);

                        _tempKeys.ForEach(k => {
                            if (statesByKeys.ContainsKey(k))
                            {
                                statesByKeys[k].ParseAndSet(_ini);
                            }
                            else
                            {
                                Debugger.Warn(k.Name + " is not a valid config property!");
                            }
                        });
                    }
                }
                else
                {
                    base.SetStates();
                }
            }
        }
    }
}
