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
        /// <inheritdoc/>
        /// <para/>
        /// Capable of storing any state value. 
        /// Can update the internal value using <see cref="MyIni"/>.
        /// <para/>
        /// When stringified then only the value is returned.
        /// <para/>
        /// The parsing is soft, so when a state is not found, then a default is used instead.
        /// <para/>
        /// You may use the static 'Of' functions to create an instance of this class.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        public class TypedIniState<T> : ContextedIniState
        {
            /// <summary>
            /// Template used internally to parse a string to a value.
            /// </summary>
            /// <typeparam name="K">Target Type</typeparam>
            /// <param name="value">Value to Parse</param>
            /// <param name="tempValue">Value to Set</param>
            /// <returns></returns>
            public delegate bool StateValueExtractor(MyIniValue value, out T tempValue);

            public T Value { get; private set; }
            protected readonly T DefaultValue;
            private readonly StateValueExtractor valueExtractor;

            protected TypedIniState(string section, string label, T defaultValue, StateValueExtractor valueExtractor) : base(section, label)
            {
                DefaultValue = defaultValue;
                Value = defaultValue;
                this.valueExtractor = valueExtractor;
            }

            /// <summary>
            /// Sets the Value of the State if it changed.
            /// <para/>
            /// If the value changed, and a context is set, then the context is updated.
            /// </summary>
            /// <param name="val">The value to set</param>
            /// <returns>If the value was changed, then true, otherwise false.</returns>
            public bool Set(T val)
            {
                if (val == null ? Value == null : val.Equals(Value))
                {
                    return false;
                }

                Value = val;
                context?.Update();
                return true;
            }

            /// <summary>
            /// <inheritdoc/>
            /// <para/>
            /// If the state is in the provided ini, then the value will be updated, otherwise the default value is used.
            /// <para/>
            /// When the value can't be parsed from the ini, then a warning is logged.
            /// </summary>
            /// <param name="ini">The ini to check the state for.</param>
            /// <returns>If the state was changed, then true, otherwise false.</returns>
            public override bool ParseAndSet(MyIni ini)
            {
                MyIniValue myIniValue = ini.Get(iniKey);

                if (myIniValue.IsEmpty)
                {
                    return Set(DefaultValue);
                }

                T temp;
                if (!valueExtractor(myIniValue, out temp))
                {
                    Debugger.Warn(iniKey.Name + " can't be parsed: " + myIniValue.ToString());
                    return false;
                }

                return Set(temp == null ? DefaultValue : temp);
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <returns>The state's value as string.</returns>
            public override string ToString()
            {
                return Value.ToString();
            }

            protected static bool BoolExtractor(MyIniValue p, out bool k) => p.TryGetBoolean(out k);

            /// <summary>
            /// Creates a <see cref="bool"/> based State.
            /// </summary>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <returns>New instance.</returns>
            public static TypedIniState<bool> OfBool(string section, string label, bool defaultValue)
            {
                return new TypedIniState<bool>(section, label, defaultValue, BoolExtractor);
            }

            protected static bool IntExtractor(MyIniValue p, out int k) => p.TryGetInt32(out k);

            /// <summary>
            /// Creates a <see cref="int"/> based State.
            /// </summary>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <returns>New instance.</returns>
            public static TypedIniState<int> OfInt(string section, string label, int defaultValue)
            {
                return new TypedIniState<int>(section, label, defaultValue, IntExtractor);
            }

            protected static bool StringExtractor(MyIniValue p, out string k) => p.TryGetString(out k);

            /// <summary>
            /// Creates a <see cref="string"/> based State.
            /// </summary>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <returns>New instance.</returns>
            public static TypedIniState<string> OfString(string section, string label, string defaultValue)
            {
                return new TypedIniState<string>(section, label, defaultValue, StringExtractor);
            }
        }
    }
}
