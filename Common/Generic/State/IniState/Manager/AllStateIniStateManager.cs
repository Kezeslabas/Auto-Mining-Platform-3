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
        /// Assumes, that all states should be maintained always.
        /// <para/>
        /// Uses an Internal context to determine if there was a state updated or not. 
        /// By calling <see cref="UpdateStates"/> at the end of a run,
        /// then if there was a change, then the stateholder will be updated.
        /// </summary>
        public class AllStateIniStateManager : IniStateManager
        {
            protected readonly ContextedIniState[] states;
            protected readonly StateContext _context = new StateContext();

            /// <summary>
            /// Makes a new State Manager with the initial states.
            /// <para/>
            /// The provided states will be updated with the context of this State Manager
            /// </summary>
            /// <param name="setStateHolder">Method that updates the state holder.</param>
            /// <param name="getStateHolder">Method that loads the current state from the holder.</param>
            /// <param name="states">The array of states that should be handled.</param>
            public AllStateIniStateManager(Action<string> setStateHolder, Func<string> getStateHolder, ContextedIniState[] states) : base(setStateHolder, getStateHolder)
            {
                this.states = states;
                Array.ForEach(states, p => p.SetContext(_context));
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void SaveStates()
            {
                _ini.Clear();
                Array.ForEach(states, p => _ini.Set(p.GetIniKey(), p.ToString()));
                setStateHolder(_ini.ToString());
            }

            /// <summary>
            /// If based on the context there was any change recorded, then the stateholder will be updated.
            /// </summary>
            public void UpdateStates()
            {
                if (_context.ConsumeUpdates())
                {
                    SaveStates();
                }
            }


            /// <summary>
            /// Use the internal <see cref="MyIni"/> to parse and update all internal <see cref="ContextedIniState"/>
            /// </summary>
            protected override void SetStates(MyIni ini)
            {
                Array.ForEach(states, p => p.ParseAndSet(ini));
            }
        }
    }
}
