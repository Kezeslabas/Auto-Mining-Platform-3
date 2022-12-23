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
        /// Basic State template that uses a <see cref="MyIniKey"/> with the default section of [X].
        /// It also expects a <see cref="MyIni"/> that can be used to acquire the value.
        /// </summary>
        public abstract class IniState
        {
            public static readonly string DEFAULT_SECTION = "X";

            protected readonly MyIniKey iniKey;

            public MyIniKey GetIniKey()
            {
                return iniKey;
            }

            /// <summary>
            /// Checks if this state is part of the provided <see cref="MyIni"/>.
            /// </summary>
            /// <param name="ini">The ini to check the state for.</param>
            public abstract void ParseAndSet(MyIni ini);

            /// <summary>
            /// Stringifies this state.
            /// </summary>
            /// <returns></returns>
            public abstract override string ToString();

            public IniState(string label)
            {
                iniKey = new MyIniKey(DEFAULT_SECTION, label);
            }

            public IniState(string section, string label)
            {
                iniKey = new MyIniKey(section, label);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// <para/>
        /// Capable of storing any state value. 
        /// Can both Update the internal value using <see cref="MyIni"/>, and stringify it.
        /// <para/>
        /// The parsing is soft, so when a state is not found, then a default is used instead.
        /// <para/>
        /// You may use the static 'Of' functions to create an instance of this class.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        public class TypedIniState<T> : IniState
        {
            /// <summary>
            /// Template used internally to parse a string to a value.
            /// </summary>
            /// <typeparam name="K">Target Type</typeparam>
            /// <param name="value">Value to Parse</param>
            /// <param name="tempValue">Value to Set</param>
            /// <returns></returns>
            public delegate bool StateValueExtractor(MyIniValue value, out T tempValue);

            public T Value { get; set; }
            protected readonly T DefaultValue;
            private readonly StateValueExtractor valueExtractor;
            

            public TypedIniState(string section, string label, T defaultValue, StateValueExtractor valueExtractor) : base(section, label)
            {
                DefaultValue = defaultValue;
                Value = defaultValue;
                this.valueExtractor = valueExtractor;
            }

            /// <summary>
            /// <inheritdoc/>
            /// <para/>
            /// If the state is in the provided ini, then the value will be updated, otherwise the default value is used.
            /// <para/>
            /// When the value can't be parsed form the ini, then a warning is logged.
            /// </summary>
            /// <param name="ini">The ini to check the state for.</param>
            public override void ParseAndSet(MyIni ini)
            {
                MyIniValue myIniValue = ini.Get(iniKey);

                if (myIniValue.IsEmpty)
                {
                    Value = DefaultValue;
                    return;
                }

                T temp;
                if (!valueExtractor(myIniValue, out temp))
                {
                    Debugger.Warn(iniKey.Name + " can't be parsed: " + myIniValue.ToString());
                    return;
                }

                Value = temp == null ? DefaultValue : temp;
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <returns>The state's value as string.</returns>
            public override string ToString()
            {
                return Value.ToString();
            }

            public KeyValuePair<MyIniKey, IniState> ToKeyValuePair()
            {
                return new KeyValuePair<MyIniKey, IniState>(GetIniKey(), this);
            }

            private static bool BoolExtractor(MyIniValue p, out bool k) => p.TryGetBoolean(out k);

            /// <summary>
            /// Creates a <see cref="bool"/> based State.
            /// </summary>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <returns>New instance.</returns>
            public static TypedIniState<bool> OfBool(string section, string label, bool defaultValue, Func<bool, bool> validator = null, string validationFailedMessage = null)
            {

                return validator == null ?
                    new TypedIniState<bool>(section, label, defaultValue, BoolExtractor) : 
                    new ValidatedTypedIniState<bool>(section, label, defaultValue, BoolExtractor, validator, validationFailedMessage);
            }

            private static bool IntExtractor(MyIniValue p, out int k) => p.TryGetInt32(out k);

            /// <summary>
            /// Creates a <see cref="int"/> based State.
            /// </summary>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <returns>New instance.</returns>
            public static TypedIniState<int> OfInt(string section, string label, int defaultValue, Func<int, bool> validator = null, string validationFailedMessage = null)
            {
                return validator == null ? 
                    new TypedIniState<int>(section, label, defaultValue, IntExtractor) :
                    new ValidatedTypedIniState<int>(section, label, defaultValue, IntExtractor, validator, validationFailedMessage);
            }

            private static bool StringExtractor(MyIniValue p, out string k) => p.TryGetString(out k);

            /// <summary>
            /// Creates a <see cref="string"/> based State.
            /// </summary>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <returns>New instance.</returns>
            public static TypedIniState<string> OfString(string section, string label, string defaultValue, Func<string, bool> validator = null, string validationFailedMessage = null)
            {
                return validator == null ? 
                    new TypedIniState<string>(section, label, defaultValue, StringExtractor) :
                    new ValidatedTypedIniState<string>(section, label, defaultValue, StringExtractor, validator, validationFailedMessage);
            }
        }

        public class ValidatedTypedIniState<T> : TypedIniState<T>
        {
            private readonly Func<T, bool> validator;
            private readonly string validationFailedMessage;

            public ValidatedTypedIniState(string section, string label, T defaultValue, StateValueExtractor valueExtractor, Func<T, bool> validator, string validationFailedMessage) : base(section, label, defaultValue, valueExtractor)
            {
                this.validator = validator;
                this.validationFailedMessage = validationFailedMessage;
            }

            public override void ParseAndSet(MyIni ini)
            {
                base.ParseAndSet(ini);

                if (validator != null && !validator.Invoke(Value))
                {
                    Value = DefaultValue;
                    Debugger.Warn(GetIniKey().Name + " => " + (validationFailedMessage ?? "Validation Failed!"));
                    Debugger.Warn(GetIniKey().Name + " falling back to default (" + Value + ")");
                }
            }
        }
    }
}
