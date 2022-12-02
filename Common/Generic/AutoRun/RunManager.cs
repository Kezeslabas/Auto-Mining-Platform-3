using Sandbox.ModAPI.Ingame;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Gives access to check for the currently active frequencies,
        /// and to shedule a new frequency.
        /// </summary>
        public interface IRunScheduler
        {
            /// <summary>
            ///  Check if the provided frequency is amongst the currently evaluated frequencies.
            /// </summary>
            /// <param name="frequency">Frequency to check for.</param>
            /// <returns>True when frequency is matched, false otherwise.</returns>
            bool CheckForFrequency(UpdateFrequency frequency);

            /// <summary>
            ///  Check if any of the provided frequencies are amongst the currently evaluated frequencies.
            /// </summary>
            /// <param name="frequencies">Frequencies to check for.</param>
            /// <returns>True when any of the frequencies are matched, false otherwise.</returns>
            bool CheckForFrequency(params UpdateFrequency[] frequencies);

            /// <summary>
            /// Add the provided frequency to the schedule.
            /// </summary>
            /// <param name="frequency">Frequency to schedule.</param>
            void ScheduleRunFrequency(UpdateFrequency frequency);
        }

        /// <summary>
        /// Handles the automatic running of the PB.
        /// <para/>
        /// By calling the <see cref="AnalyzeUpdateType(UpdateType)"/> at the beginning of the main method, 
        /// the currently running frequency is evaulated, and it can be checked for specific frequencies 
        /// trough the <see cref="IRunScheduler"/> inteface.
        /// <para/>
        /// Using the <see cref="IRunScheduler"/> interface new schedules can be added
        /// as <see cref="UpdateFrequency"/>, and it can be applied to the PB 
        /// if <see cref="ApplySchedule"/> is called at the end of the main method.
        /// <para/>
        /// As of now, during an automatic run, only one <see cref="UpdateType"/> 
        /// is passed to the PB by the game, and it's always represent's the highest frequency.
        /// This is handled internally by this class. 
        /// Higher frequencies also evaluate to lower frequencies,
        /// when the appropriate number of runs were achieved relative to each other.
        /// </summary>
        public class RunManager : IRunScheduler
        {
            private readonly IMyGridProgramRuntimeInfo runtime;

            /// <summary>
            /// The number of times <see cref="UpdateFrequency.Update1"/> or <see cref="UpdateFrequency.Once"/>
            /// was evaluated relative to <see cref="UpdateFrequency.Update10"/>
            /// <para/>
            /// When <see cref="UpdateFrequency.Update10"/> is evaluated, it resets to 0.
            /// </summary>
            private byte weightOf1 = 0;

            /// <summary>
            /// The number of times <see cref="UpdateFrequency.Update10"/> 
            /// was evaluated relative to <see cref="UpdateFrequency.Update100"/>
            /// <para/>
            /// When <see cref="UpdateFrequency.Update10"/> is evaluated, it resets to 0.
            /// </summary>
            private byte weightOf10 = 0;

            /// <summary>
            /// By setting this flag the automatic running can be disabled/enabled.
            /// The previously scheduled frequencies are not lost.
            /// </summary>
            public bool Paused { get; set; } = false;

            /// <summary>
            /// The combination of currently scheduled frequencies.
            /// </summary>
            public UpdateFrequency ScheduledFrequency { get; private set; } = UpdateFrequency.None;

            /// <summary>
            /// The combination of the last evaluated frequencies.
            /// </summary>
            public UpdateFrequency CurrentFrequency { get; private set; } = UpdateFrequency.None;

            /// <summary>
            /// New instance with the injected Runtime.
            /// </summary>
            /// <param name="runtime">Runtime to apply the schedule to.</param>
            public RunManager(IMyGridProgramRuntimeInfo runtime)
            {
                this.runtime = runtime;
            }

            /// <summary>
            /// Checks the provided <see cref="UpdateType"/> and derives which <see cref="UpdateFrequency"/>
            /// it can be associated with.
            /// <br/>
            /// If none, then it's interpreted as <see cref="UpdateFrequency.None"/>.
            /// </summary>
            /// <param name="updateType">The type to analyze.</param>
            /// <returns>The evaluated frequencies.</returns>
            public UpdateFrequency AnalyzeUpdateType(UpdateType updateType)
            {
                CurrentFrequency = UpdateFrequency.None;

                //Just to improve performance, as when paused then no automatic run is allowed.
                if (Paused)
                {
                    return CurrentFrequency;
                }

                if (updateType.HasFlag(UpdateType.Once))
                {
                    CurrentFrequency |= UpdateFrequency.Once;
                    weightOf1++;
                }

                if (updateType.HasFlag(UpdateType.Update1))
                {
                    CurrentFrequency |= UpdateFrequency.Update1;
                    weightOf1++;
                }

                if (weightOf1 >= 10 || updateType.HasFlag(UpdateType.Update10))
                {
                    CurrentFrequency |= UpdateFrequency.Update10;
                    weightOf1 = 0;
                    weightOf10++;
                }

                if (weightOf10 >= 10 || updateType.HasFlag(UpdateType.Update100))
                {
                    CurrentFrequency |= UpdateFrequency.Update100;
                    weightOf10 = 0;
                }

                return CurrentFrequency;
            }

            /// <inheritdoc/>
            public bool CheckForFrequency(UpdateFrequency frequency)
            {
                return CurrentFrequency.HasFlag(frequency);
            }

            /// <inheritdoc/>
            public bool CheckForFrequency(params UpdateFrequency[] frequencies)
            {
                return frequencies.Any(frequency => CheckForFrequency(frequency));
            }

            /// <inheritdoc/>
            public void ScheduleRunFrequency(UpdateFrequency frequency)
            {
                if (!ScheduledFrequency.HasFlag(frequency))
                {
                    ScheduledFrequency |= frequency;
                }
            }

            /// <summary>
            /// Check if the currently evaulated frequencies represent an automatic run or not.
            /// </summary>
            /// <returns>True if it's an automatic run, false otherwise.</returns>
            public bool IsAutoRun()
            {
                return CurrentFrequency != UpdateFrequency.None;
            }

            /// <summary>
            /// Applies the schedule frequency to the Runtime, then clears the schedule.
            /// <para/>
            /// When <see cref="Paused"/> is true, then <see cref="UpdateFrequency.None"/> is applied instead,
            /// and the schedule is kept.
            /// </summary>
            public void ApplySchedule()
            {
                if (Paused)
                {
                    runtime.UpdateFrequency = UpdateFrequency.None;
                }
                else
                {
                    runtime.UpdateFrequency = ScheduledFrequency;
                    ScheduledFrequency = UpdateFrequency.None;
                }
            }
        }
    }
}
