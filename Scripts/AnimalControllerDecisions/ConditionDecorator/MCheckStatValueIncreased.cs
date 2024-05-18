using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Stat Value Increased", "Animal Controller/MConditionDecorator/Check Stat Value Increased", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckStatValueIncreased : MConditionDecorator
    {
        [Header("Node")]
        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected checkOn = Affected.Self;

        [Tooltip("Stat you want to find")]
        public StatID Stat;

        protected override bool CalculateResult()
        {
            bool result = false;

            switch (checkOn)
            {
                case Affected.Self:
                    if (AIBrain != null && AIBrain.AnimalStats != null)
                    {
                        if (AIBrain.AnimalStats.TryGetValue(Stat.ID, out Stat statS))
                        {
                            result = CheckStat(statS, AIBrain);
                        }
                    }
                    break;

                case Affected.Target:
                    if (AIBrain != null && AIBrain.TargetHasStats && AIBrain.TargetStats != null)
                    {
                        if (AIBrain.TargetStats.TryGetValue(Stat.ID, out Stat statT))
                        {
                            result = CheckStat(statT, AIBrain);
                        }
                    }
                    break;
            }

            return result;
        }

        private bool CheckStat(Stat stat, AIBrain AIBrain)
        {
            float previousValue = AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.PreviousValue ?? 0f;

            // Get the previous value of the stat from the ParameterInfo stored in AIBrain
            previousValue = AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.PreviousValue ?? 0f;

            // Compare the current value of the stat with its previous value
            bool valueIncreased = stat.Value > previousValue;

            // Set the current value as the previous value if increased
            if (valueIncreased)
            {
                AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.UpdatePreviousValue(stat.Value);
            }

            return valueIncreased;
        }
    }
}
