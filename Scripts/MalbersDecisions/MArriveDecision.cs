using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Arrive Decision", "Animal Controller/Arrive Desicion", IconPath = "Icons/AIDecision_Icon.png")]
    public class MArriveDecision : ConditionDecorator
    {
        [Header("Node")]
        [Tooltip("(OPTIONAL)Use it if you want to know if we have arrived to a specific Target")]
        public string TargetName = string.Empty;

        AIBrain aiBrain;
        bool arrived;
        float remainingdistance;

        protected override void OnInitialize()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
        }

        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            bool Result;

            remainingdistance = aiBrain.AIControl.RemainingDistance;

            if (string.IsNullOrEmpty(TargetName))
            {
                Result =
                    aiBrain.AIControl.HasArrived;
            }
            else
            {
                Result = aiBrain.AIControl.HasArrived && 
                    (aiBrain.Target.name == TargetName || aiBrain.Target.root.name == TargetName); //If we are looking for an specific Target
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
