using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Range", "Animal Controller/MObserverDecorator/Check Range", IconPath = "Icons/AIDecision_Icon.png")]

    public class MCheckRange : ObserverDecorator
    {
        [Header("Node")]

        [Tooltip("The minimum range to check against")]
        public float minRange = 0f;

        [Tooltip("The maximum range to check against")]
        public float maxRange = 5f;

        public TransformVar target;
        private float distance;

        private bool checkResult;

        AIBrain AIBrain;

        public override event Action OnValueChange;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AIBrain = GetOwner().GetComponent<AIBrain>();
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
                return IsInRange(target.Value, minRange, maxRange);
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

            distance = Vector3.Distance(AIBrain.transform.position, target.position);

            return distance >= minRange && distance <= maxRange;
        }

        public override string GetDescription()
        {
            string description = $"Min Range: {minRange} \n";
            description += $"Max Range: {maxRange} \n";
            description += $"Distance: {distance} \n";
            if (AIBrain != null && AIBrain.TargetAnimal != null)
            {
                description += $"Result: {AIBrain.TargetAnimal.name} \n";
            }

            description += $"Result: {checkResult} \n";

            return description;
        }
    }

}
