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
        /// It also validates the value after parsing.
        /// </summary>
        /// <typeparam name="T">Type of the State's value.</typeparam>
        public class ValidatedTypedIniState<T> : TypedIniState<T>
        {
            private readonly Func<T, bool> validator;
            private readonly string validationFailedMessage;

            protected ValidatedTypedIniState(string section, string label, T defaultValue, StateValueExtractor valueExtractor, Func<T, bool> validator, string validationFailedMessage = null) : base(section, label, defaultValue, valueExtractor)
            {
                this.validator = validator;
                this.validationFailedMessage = validationFailedMessage;
            }

            /// <summary>
            /// <inheritdoc/>
            /// <para/>
            /// Also validates the states using the provided valdiation schema.
            /// When the validation failed, then the default value is used for the state, 
            /// and the provided fail message is logged.
            /// If there is no fail message, then a default message is logged.
            /// </summary>
            /// <param name="ini">The ini to check the state for.</param>
            /// <returns>If the state was changed, then true, otherwise false.</returns>
            public override bool ParseAndSet(MyIni ini)
            {
                bool parseResult = base.ParseAndSet(ini);

                if (validator != null && !validator.Invoke(Value))
                {
                    Debugger.Warn(GetIniKey().Name + " => " + (validationFailedMessage ?? "Validation Failed!"));
                    Debugger.Warn(GetIniKey().Name + " falling back to default (" + DefaultValue + ")");
                    return Set(DefaultValue);
                }

                return parseResult;
            }

            /// <summary>
            /// Creates a <see cref="bool"/> based State, that is also validated.
            /// </summary>
            /// <param name="section">The section used for the state.</param>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <param name="validator">The validator function.</param>
            /// <param name="validationFailedMessage">Optional. The message to log when the validation failed.</param>
            /// <returns>New instance.</returns>
            public static ValidatedTypedIniState<bool> OfBool(string section, string label, bool defaultValue, Func<bool, bool> validator, string validationFailedMessage = null)
            {
                return new ValidatedTypedIniState<bool>(section, label, defaultValue, BoolExtractor, validator, validationFailedMessage);
            }

            /// <summary>
            /// Creates a <see cref="int"/> based State, that is also validated.
            /// </summary>
            /// <param name="section">The section used for the state.</param>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <param name="validator">The validator function.</param>
            /// <param name="validationFailedMessage">Optional. The message to log when the validation failed.</param>
            /// <returns>New instance.</returns>
            public static ValidatedTypedIniState<int> OfInt(string section, string label, int defaultValue, Func<int, bool> validator, string validationFailedMessage = null)
            {
                return new ValidatedTypedIniState<int>(section, label, defaultValue, IntExtractor, validator, validationFailedMessage);
            }

            /// <summary>
            /// Creates a <see cref="string"/> based State, that is also validated.
            /// </summary>
            /// <param name="section">The section used for the state.</param>
            /// <param name="label">The label of the state.</param>
            /// <param name="defaultValue">The default value to use.</param>
            /// <param name="validator">The validator function.</param>
            /// <param name="validationFailedMessage">Optional. The message to log when the validation failed.</param>
            /// <returns>New instance.</returns>
            public static ValidatedTypedIniState<string> OfString(string section, string label, string defaultValue, Func<string, bool> validator, string validationFailedMessage = null)
            {
                return new ValidatedTypedIniState<string>(section, label, defaultValue, StringExtractor, validator, validationFailedMessage);
            }
        }
    }
}
