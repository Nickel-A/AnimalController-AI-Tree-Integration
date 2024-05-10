using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Arrive Decision", "Animal Controller/MConditionDecorator/Arrive Desicion", IconPath = "Icons/AIDecision_Icon.png")]
    public class MArriveDecision : MConditionDecorator
    {
        [Header("Node")]
        [Tooltip("(OPTIONAL)Use it if you want to know if we have arrived to a specific Target")]
        public string TargetName = string.Empty;

        bool arrived;
        float remainingdistance;

        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            bool Result;

            remainingdistance = AIBrain.AIControl.RemainingDistance;

            if (string.IsNullOrEmpty(TargetName))
            {
                Result =
                    AIBrain.AIControl.HasArrived;
            }
            else
            {
                Result = AIBrain.AIControl.HasArrived && 
                    (AIBrain.Target.name == TargetName || AIBrain.Target.root.name == TargetName); //If we are looking for an specific Target
            }
            arrived = Result;
            return Result;
        }

        public override string GetDescription()
        {
            string description = "";
            if (!string.IsNullOrEmpty(TargetName))
            {
                description += $"Specific Target: {TargetName}\n";
            }
            description += $"Remaining Distance: {remainingdistance}\n";
            description += $"Has Arrived: {arrived}\n";

            return description;
        }
    }
}
