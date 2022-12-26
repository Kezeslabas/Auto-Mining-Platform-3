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
        public enum StepType
        {
            ALIGNMENT,
            FINAL,
            ROTATION,
            HORIZONTAL,
            VERTICAL
        }

        public class StepManager
        {
            public int MaxSteps { get; private set; } = 0;
            public StepType CurrentStep { get; private set; }

            public int CurrentStepNumber { get; private set; }

            private readonly List<StepType> steps = new List<StepType>();

            private IEnumerator<StepType> stepSequence;

            public StepManager()
            {
                Init(0, 0, false);
            }

            public void Init(int horizontalExtensions, int verticalExtensions, bool hasRotation)
            {
                steps.Clear();

                steps.Add(StepType.ALIGNMENT);//Aligning Starting Position

                //Build steps of mining
                if (hasRotation)
                {
                    if(horizontalExtensions > 0)
                    {
                        for (int i = 0; i < horizontalExtensions; i++)
                        {
                            steps.Add(StepType.ROTATION);
                            steps.Add(StepType.HORIZONTAL);
                            for (int j = 0; j < verticalExtensions; j++)
                            {
                                steps.Add(StepType.ROTATION);
                                steps.Add(StepType.VERTICAL);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < verticalExtensions; j++)
                        {
                            steps.Add(StepType.ROTATION);
                            steps.Add(StepType.VERTICAL);
                        }
                    }
                }
                else
                {
                    if (horizontalExtensions > 0)
                    {
                        for (int i = 0; i < horizontalExtensions; i++)
                        {
                            steps.Add(StepType.HORIZONTAL);
                            for (int j = 0; j < verticalExtensions; j++)
                            {
                                steps.Add(StepType.ROTATION);
                                steps.Add(StepType.VERTICAL);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < verticalExtensions; j++)
                        {
                            steps.Add(StepType.VERTICAL);
                        }
                    }
                }

                steps.Add(StepType.FINAL);//Final Step
                MaxSteps = steps.Count();
                stepSequence = steps.GetEnumerator();
                CurrentStep = stepSequence.Current;
                CurrentStepNumber = 0;
            }

            public void NextStep()
            {
                if (stepSequence.MoveNext())
                {
                    CurrentStep = stepSequence.Current;
                    CurrentStepNumber++;
                }
            }

            public bool IsFinalStep()
            {
                return CurrentStep == StepType.FINAL;
            }

            public void MoveToStep(int stepNumber)
            {
                while (CurrentStepNumber > stepNumber)
                {
                    NextStep();
                }
            }
        }
    }
}
