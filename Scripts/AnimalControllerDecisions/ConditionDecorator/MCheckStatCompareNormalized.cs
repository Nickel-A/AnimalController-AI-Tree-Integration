using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Stat Compare Normalized", "Animal Controller/MConditionDecorator/Check Stat Compare Normalized", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckStatCompareNormalized : MConditionDecorator
    {
        [Header("Node")]
        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected checkOn = Affected.Self;

        [Tooltip("Stat you want to find")]
        public StatID Stat;

        [Tooltip("(Option CompareNormalized Only) Type of the comparation")]
        public ComparerInt StatIs = ComparerInt.Less;

        [Tooltip("(Option CompareNormalized Only) Value to Compare the Stat")]
        public float NormalizedValue;

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
                            result = CompareNormalizedWithValue(statS.NormalizedValue);
                        }
                    }
                    break;

                case Affected.Target:
                    if (AIBrain != null && AIBrain.TargetHasStats && AIBrain.TargetStats != null)
                    {
                        if (AIBrain.TargetStats.TryGetValue(Stat.ID, out Stat statT))
                        {
                            result = CompareNormalizedWithValue(statT.NormalizedValue);
                        }
                    }
                    break;
            }

            return result;
        }

        private bool CompareNormalizedWithValue(float stat)
        {
            switch (StatIs)
            {
                case ComparerInt.Equal:
                    return stat == NormalizedValue;
                case ComparerInt.Greater:
                    return stat > NormalizedValue;
                case ComparerInt.Less:
                    return stat < NormalizedValue;
                case ComparerInt.NotEqual:
                    return stat != NormalizedValue;
                default:
                    return false;
            }
        }
    }
}
