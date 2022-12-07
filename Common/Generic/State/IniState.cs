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
        public abstract class IniState
        {
            public static readonly string DEFAULT_SECTION = "X";

            protected readonly MyIniKey iniKey;

            public MyIniKey GetIniKey()
            {
                return iniKey;
            }

            public abstract void ParseAndSet(MyIni ini);
            public abstract string ToState();

            public IniState(string label)
            {
                iniKey = new MyIniKey(DEFAULT_SECTION, label);
            }
        }

        public class TypedIniState<T> : IniState
        {
            private delegate bool StateValueExtractor<K>(MyIniValue value, K tempValue);

            private static readonly StateValueExtractor<bool> BOOL_EXTRACTOR = (p, k) => p.TryGetBoolean(out k);
            private static readonly StateValueExtractor<int> INT_EXTRACTOR = (p, k) => p.TryGetInt32(out k);
            private static readonly StateValueExtractor<string> STRING_EXTRACTOR = (p, k) => p.TryGetString(out k);

            public T Value { get; set; }
            private readonly T DefaultValue;
            private readonly StateValueExtractor<T> valueExtractor;

            private TypedIniState(string label, T defaultValue, StateValueExtractor<T> valueExtractor) : base(label)
            {
                DefaultValue = defaultValue;
                Value = defaultValue;
                this.valueExtractor = valueExtractor;
            }

            public override void ParseAndSet(MyIni ini)
            {
                MyIniValue myIniValue = ini.Get(iniKey);

                T temp = DefaultValue;
                if (!myIniValue.IsEmpty)
                {
                    if (valueExtractor(myIniValue, temp))
                    {
                        Value = temp;
                    }
                    else
                    {
                        Debugger.Warn(iniKey.Name + " can't be parsed: " + myIniValue.ToString());
                    }
                }
                else
                {
                    Value = DefaultValue;
                }
            }

            public override string ToState()
            {
                return "" + Value;
            }

            public static TypedIniState<bool> OfBool(string label, bool defaultValue)
            {
                return new TypedIniState<bool>(label, defaultValue, TypedIniState<bool>.BOOL_EXTRACTOR);
            }

            public static TypedIniState<int> OfInt(string label, int defaultValue)
            {
                return new TypedIniState<int>(label, defaultValue, TypedIniState<int>.INT_EXTRACTOR);
            }

            public static TypedIniState<string> OfString(string label, string defaultValue)
            {
                return new TypedIniState<string>(label, defaultValue, TypedIniState<string>.STRING_EXTRACTOR);
            }
        }
    }
}
