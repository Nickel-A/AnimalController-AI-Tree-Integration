using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Cooldown", "Animal Controller/Cooldown", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCooldown : ConditionDecorator
    {
        [Header("Decorator Settings")]
        [Tooltip("Cooldown period in seconds")]
        public float cooldownDuration = 5f;

        // Cache the last evaluation time for performance optimization
        private float lastEvaluationTime;

        // Initialize any required variables
        protected override void OnInitialize()
        {
            base.OnInitialize();
            lastEvaluationTime = -cooldownDuration; // Initialize last evaluation time to allow immediate evaluation
        }

        // Override the Evaluate method for custom behavior
        protected override bool CalculateResult()
        {
            // Check if the cooldown period has elapsed
            if (Time.time - lastEvaluationTime >= cooldownDuration)
            {
                lastEvaluationTime = Time.time;
                return true;
            }

            return false;
        }

        // Method to manually reset the cooldown
        public void ResetCooldown()
        {
            lastEvaluationTime = -cooldownDuration; // Set the last evaluation time to allow immediate evaluation
        }

        // Calculate the progress of the cooldown
        protected override float? GetProgress()
        {
            if (lastEvaluationTime < 0)
            {
                return null; // Not on cooldown
            }

            return Mathf.Clamp01((Time.time - lastEvaluationTime) / cooldownDuration);
        }

        // Override the GetDescription method to provide information about the cooldown duration
        public override string GetDescription()
        {
            string description = base.GetDescription();
            description += $"Cooldown Duration: {cooldownDuration} seconds\n";

            // Calculate remaining time until cooldown ends
            float remainingTime = Mathf.Max(0f, cooldownDuration - (Time.time - lastEvaluationTime));
            description += $"Remaining Cooldown Time: {remainingTime:F2} seconds\n";

            return description;
        }
    }
}
