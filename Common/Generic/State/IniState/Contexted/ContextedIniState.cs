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
        /// Basic State template that uses a <see cref="MyIniKey"/>.
        /// It also expects a <see cref="MyIni"/> that can be used to acquire the value.
        /// It can hold a <see cref="StateContext"/> to signal changes to it when implemented.
        /// </summary>
        public abstract class ContextedIniState
        {
            protected StateContext context;
            protected readonly MyIniKey iniKey;

            public ContextedIniState(string section, string label)
            {
                iniKey = new MyIniKey(section, label);
            }

            /// <summary>
            /// The <see cref="MyIniKey"/> used for by the instance.
            /// </summary>
            public MyIniKey GetIniKey()
            {
                return iniKey;
            }

            /// <summary>
            /// Tries to load this state using it's key from the <see cref="MyIni"/>.
            /// </summary>
            /// <param name="ini">The ini to check the state for.</param>
            /// <returns>If the state was changed, then true, otherwise false.</returns>
            public abstract bool ParseAndSet(MyIni ini);

            /// <summary>
            /// Stringifies this state.
            /// </summary>
            public abstract override string ToString();

            /// <summary>
            /// Sets the context for the State.
            /// <para/>
            /// The context may only be set once, and any subsequencts attempts will not update the context.
            /// </summary>
            /// <param name="context">Context to use</param>
            public void SetContext(StateContext context)
            {
                if (this.context != null)
                {
                    Debugger.Debug("Context already Set! (" + GetIniKey().Name + ")");
                    return;
                }

                this.context = context;
            }
        }
    }
}
