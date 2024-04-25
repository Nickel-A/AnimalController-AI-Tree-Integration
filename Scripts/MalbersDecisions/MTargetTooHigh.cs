using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using System;


namespace Malbers.Integration.AITree
{
    [NodeContent("Target Too High", "Animal Controller/Target Too High", IconPath = "Icons/AIDecision_Icon.png")]
    public class MTargetTooHigh : ObserverDecorator
    {
        AIBrain aiBrain;
        bool previousTargetTooHigh;

        public override event Action OnValueChange;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();
            if (aiBrain.AIControl != null && aiBrain.AIControl is MAnimalAIControl)
            {
                MAnimalAIControl animalAIControl = (MAnimalAIControl)aiBrain.AIControl;
                previousTargetTooHigh = animalAIControl.TargetTooHigh;
            }
        }

        // Override the Evaluate method or else your environment will throw an error
        public override bool CalculateResult()
        {
            if (aiBrain != null)
            {

                if (aiBrain.AIControl != null && aiBrain.AIControl is MAnimalAIControl)
                {
                    MAnimalAIControl animalAIControl = (MAnimalAIControl)aiBrain.AIControl;
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