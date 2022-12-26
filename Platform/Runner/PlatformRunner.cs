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
        /// Script runner for specific use in the Mining Platform Script, built on <see cref="RunManager"/>.
        /// </summary>
        public class PlatformRunner
        {
            private readonly RunManager runManager;
            private readonly PlatformState state;
            private readonly PlatformConfig config;

            private UpdateFrequency currentFrequency = UpdateFrequency.None;

            /// <summary>
            /// New Instnace. 
            /// If based on the injected state the script should be running, 
            /// then it it is started and applied.
            /// </summary>
            public PlatformRunner(RunManager runManager, PlatformState state, PlatformConfig config)
            {
                this.runManager = runManager;
                this.state = state;
                this.config = config;

                if (state.IsRunning.Value)
                {
                    StartPlatform();
                    Apply();
                }
            }

            /// <summary>
            /// Starts the Autonomous running of the platform and updates the corresponding states.
            /// </summary>
            public void StartPlatform()
            {
                state.IsRunning.Set(true);
                runManager.Paused = false;
                currentFrequency = config.DEFAULT_UPDATE_FREQUENCY;
            }

            /// <summary>
            /// Pauses the Autonomous running of the platform.
            /// </summary>
            public void Pause()
            {
                runManager.Paused = true;
                state.IsRunning.Set(false);
            }

            /// <summary>
            /// Stops the Autonomous running of the platform, and updates the appropriate states.
            /// </summary>
            public void StopPlatform()
            {
                state.IsRunning.Set(false);
                currentFrequency = UpdateFrequency.None;
            }

            /// <summary>
            /// Apply any change made to the run manager.
            /// </summary>
            public void Apply()
            {
                runManager.ScheduleRunFrequency(currentFrequency);
                runManager.ApplySchedule();
            }

            /// <summary>
            /// Analyzes the current running and determines if this is an automatic run or not.
            /// </summary>
            /// <param name="updateSource">The updatetype to use for determining the run's nature.</param>
            /// <returns>True if it's an automatic run, false otherwise.</returns>
            public bool IsAutoRun(UpdateType updateSource)
            {
                runManager.AnalyzeUpdateType(updateSource);
                return runManager.IsAutoRun();
            }
        }
    }
}
