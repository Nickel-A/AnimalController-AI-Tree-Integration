using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using System;


namespace Malbers.Integration.AITree
{
    [NodeContent("Target Too High", "Animal Controller/MObserverDecorator/Target Too High", IconPath = "Icons/AIDecision_Icon.png")]
    public class MTargetTooHigh : ObserverDecorator
    {
        bool previousTargetTooHigh;

        AIBrain AIBrain;

        public override event Action OnValueChange;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AIBrain = GetOwner().GetComponent<AIBrain>();
            if (AIBrain.AIControl != null && AIBrain.AIControl is MAnimalAIControl)
            {
                MAnimalAIControl animalAIControl = (MAnimalAIControl)AIBrain.AIControl;
                previousTargetTooHigh = animalAIControl.TargetTooHigh;
            }
        }

        // Override the Evaluate method or else your environment will throw an error
        public override bool CalculateResult()
        {
            if (AIBrain != null)
            {

                if (AIBrain.AIControl != null && AIBrain.AIControl is MAnimalAIControl)
                {
                    MAnimalAIControl animalAIControl = (MAnimalAIControl)AIBrain.AIControl;
                    bool currentTargetTooHigh = animalAIControl.TargetTooHigh;

                    if (currentTargetTooHigh != previousTargetTooHigh)
                    {
                        previousTargetTooHigh = currentTargetTooHigh;
                        OnValueChange?.Invoke();
                    }

                    return currentTargetTooHigh;
                }
            }
            return false;
        }
        public override string GetDescription()
        {
            string description = base.GetDescription();

            if (!string.IsNullOrEmpty(description))
            {
                description += "\n";
            }

            description += "Target is too high: ";
            description += $"{(CalculateResult() ? "Yes" : "No")}";

            return description;
        }

    }





}