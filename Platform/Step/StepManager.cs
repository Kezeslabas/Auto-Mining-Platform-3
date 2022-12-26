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
        /// The nature of the different steps.
        /// </summary>
        public enum StepType
        {
            ALIGNMENT,
            FINAL,
            ROTATION,
            HORIZONTAL,
            VERTICAL
        }

        /// <summary>
        /// Handles the Steps of the Mining Sequence.
        /// <para/>
        /// There are always at least two steps, 
        /// where the first step is always the aligning to the starting position,
        /// and the last one is the retraction at the end.
        /// </summary>
        public class StepManager
        {
            private readonly PlatformState state;

            public int MaxSteps { get; private set; } = 0;
            public StepType CurrentStep { get; private set; } = StepType.ALIGNMENT;
            public int CurrentStepNumber
            {
                get { return state.StepNumber.Value; }
                private set { state.StepNumber.Set(value); }
            }

            private readonly List<StepType> _steps = new List<StepType>();

            private IEnumerator<StepType> stepSequence;

            public StepManager(PlatformState state)
            {
                this.state = state;
                Init(2, 2, true, false);//TODO Load values dynamically
            }

            /// <summary>
            /// Initalizes a new step sequence based on the number of horizontal/vertical extensions,
            /// and based if there are rotations as well or not.
            /// <para/>
            /// Steps [S] Are:
            /// <list type="bullet">
            ///     <item>[S]Align to Starting Position</item>
            ///     <item>[S]Rotation</item>
            ///     <item>For Each Vertical Step:
            ///     <list type="bullet">
            ///         <item>For Each Horizontal Step:
            ///         <list type="bullet">
            ///             <item>[S]Horizontal Extension</item>
            ///             <item>[S]Rotation</item>
            ///         </list>
            ///         </item>  
            ///         <item>[S]Vertical Extension</item>
            ///         <item>[S]Rotation</item>
            ///     </list>
            ///     </item>
            ///     <item>For Each Horizontal Step:
            ///     <list type="bullet">
            ///         <item>[S]Horizontal Extension</item>
            ///         <item>[S]Rotation</item>
            ///     </list>
            ///     </item>  
            ///     <item>[S]Retract all when finished</item>
            /// </list>
            /// If there is no vertical, horizontal or rotation indicated, then those steps are not added to the sequence.
            /// </summary>
            /// <param name="horizontalExtensions">The number of horizontal extensions.</param>
            /// <param name="verticalExtensions">The number of vertical extensions.</param>
            /// <param name="hasRotation">Indicates if there should be rotations.</param>
            public void Init(int horizontalExtensions, int verticalExtensions, bool hasRotation, bool resetStepNumber = true)
            {
                _steps.Clear();


                _steps.Add(StepType.ALIGNMENT);//Aligning Starting Position

                //Build steps of mining
                PopulateDynamicSteps(horizontalExtensions, verticalExtensions, hasRotation);

                _steps.Add(StepType.FINAL);//Final Step
                MaxSteps = _steps.Count();
                stepSequence = _steps.GetEnumerator();

                if (resetStepNumber)
                {
                    CurrentStepNumber = 0;
                }
                else
                {
                    for (int i = 0; i < CurrentStepNumber; i++)
                    {
                        stepSequence.MoveNext();
                    }
                }

                CurrentStep = stepSequence.Current;
            }

            /// <summary>
            /// Moves the Step Sequence to the next Step.
            /// </summary>
            public void NextStep()
            {
                if (stepSequence.MoveNext())
                {
                    CurrentStep = stepSequence.Current;
                    CurrentStepNumber++;
                }
            }

            /// <summary>
            /// When the Step sequence reaches it's last step, the returns true, otherwise false.
            /// </summary>
            /// <returns></returns>
            public bool IsFinalStep()
            {
                return CurrentStep == StepType.FINAL;
            }

            /// <summary>
            /// Fast Forwards the Step Sequence to the specified step number.
            /// </summary>
            /// <param name="stepNumber">The step number to forward to.</param>
            public void MoveToStep(int stepNumber)
            {
                while (stepNumber > CurrentStepNumber)
                {
                    NextStep();
                }
            }

            /// <summary>
            /// Populates the internal step list with the correct steps.
            /// </summary>
            /// <param name="horizontalExtensions"></param>
            /// <param name="verticalExtensions"></param>
            /// <param name="hasRotation"></param>
            private void PopulateDynamicSteps(int horizontalExtensions, int verticalExtensions, bool hasRotation)
            {
                //TODO Figure out a way to make this nicer

                if (!hasRotation)//No Rotation
                {
                    if (verticalExtensions == 0)//Only Horizontal
                    {
                        for (int j = 0; j < horizontalExtensions; j++)
                        {
                            _steps.Add(StepType.HORIZONTAL);
                        }
                        return;
                    }

                    for (int i = 0; i < verticalExtensions; i++)//Horizontal + Vertical
                    {
                        for (int j = 0; j < horizontalExtensions; j++)
                        {
                            _steps.Add(StepType.HORIZONTAL);
                        }
                        _steps.Add(StepType.VERTICAL);
                    }

                    for (int j = 0; j < horizontalExtensions; j++)//+1 Full Horizontal at the End
                    {
                        _steps.Add(StepType.HORIZONTAL);
                        _steps.Add(StepType.ROTATION);
                    }
                    return;
                }

                _steps.Add(StepType.ROTATION); //With Rotation

                if (verticalExtensions == 0)//Only Horizontal
                {
                    for (int j = 0; j < horizontalExtensions; j++)
                    {
                        _steps.Add(StepType.HORIZONTAL);
                        _steps.Add(StepType.ROTATION);
                    }

                    return;
                }

                for (int i = 0; i < verticalExtensions; i++)//Horizontal + Vertical
                {
                    for (int j = 0; j < horizontalExtensions; j++)
                    {
                        _steps.Add(StepType.HORIZONTAL);
                        _steps.Add(StepType.ROTATION);
                    }
                    _steps.Add(StepType.VERTICAL);
                    _steps.Add(StepType.ROTATION);
                }

                for (int j = 0; j < horizontalExtensions; j++)//+1 Full Horizontal at the End
                {
                    _steps.Add(StepType.HORIZONTAL);
                    _steps.Add(StepType.ROTATION);
                }
            } 
        }
    }
}
