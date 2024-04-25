using RenownedGames.AITree;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Range", "Animal Controller/Check Range", IconPath = "Icons/AIDecision_Icon.png")]

    public class MCheckRange : ObserverDecorator
    {
        [Header("Node")]

        [Tooltip("The minimum range to check against")]
        public float minRange = 0f;

        [Tooltip("The maximum range to check against")]
        public float maxRange = 5f;

        AIBrain aiBrain;

        private float distance;

        private bool checkResult;

        public override event Action OnValueChange;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();
            checkResult = false;
        }

        /// <summary>
        /// Called every tick regardless of the node execution.
        /// </summary>
        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }

        public override bool CalculateResult()
        {
            if (aiBrain != null && aiBrain.TargetAnimal != null)
                IsInRange(aiBrain.TargetAnimal.transform, minRange, maxRange);

                return checkResult;
        }

        bool IsInRange(Transform target, float minRange, float maxRange)
        {
            if (minRange > maxRange)
            {
                // Swap minRange and maxRange if minRange is greater than maxRange
                float temp = minRange;
                minRange = maxRange;
                maxRange = temp;
            }

            distance = Vector3.Distance(aiBrain.transform.position, target.position);
            checkResult = distance >= minRange && distance <= maxRange;
            return checkResult;
        }

        public override string GetDescription()
        {
            string description = $"Min Range: {minRange} \n";
            description += $"Max Range: {maxRange} \n";
            description += $"Distance: {distance} \n";
            if (aiBrain != null && aiBrain.TargetAnimal != null)
            {
                description += $"Result: {aiBrain.TargetAnimal.name} \n";
            }

            description += $"Result: {checkResult} \n";

            return description;
        }
    }

}
