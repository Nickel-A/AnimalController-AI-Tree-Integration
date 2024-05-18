using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Remaining Distance", "Animal Controller/MConditionDecorator/Remaining Distance", IconPath = "Icons/AIDecision_Icon.png")]
    public class MRemainingDistance : MConditionDecorator
    {
        [Header("Node")]
        public float minDistance;
        public float maxDistance;

        /// <summary>
        /// Calculates the result of the condition.
        /// </summary>
        protected override bool CalculateResult()
        {
            return CalculateDistance();
        }

        bool CalculateDistance()
        {
            if (AIBrain == null || AIBrain.AIControl.Target == null)
            {
                return false;
            }

            float remainingDistance = Vector3.Distance(AIBrain.AIControl.Target.transform.position, AIBrain.transform.position);
            return remainingDistance >= minDistance && remainingDistance <= maxDistance;
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();
            if (AIBrain != null && AIBrain.AIControl != null && AIBrain.AIControl.Target != null)
                description += "Remaining Distance: " + Vector3.Distance(AIBrain.AIControl.Target.transform.position, AIBrain.transform.position);

            return description;
        }
    }
}
