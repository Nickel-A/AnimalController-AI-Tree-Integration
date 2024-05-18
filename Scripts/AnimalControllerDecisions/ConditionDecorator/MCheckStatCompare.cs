using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Stat Compare", "Animal Controller/MConditionDecorator/Check Stat Compare", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckStatCompare : MConditionDecorator
    {
        [Header("Node")]
        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected checkOn = Affected.Self;

        [Tooltip("Stat you want to find")]
        public StatID Stat;

        [Tooltip("(Option Compare Only) Type of the comparation")]
        public ComparerInt StatIs = ComparerInt.Less;

        [Tooltip("(Option Compare Only) Value to Compare the Stat")]
        public float Value;

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
                            result = CompareWithValue(statS.Value);
                        }
                    }
                    break;

                case Affected.Target:
                    if (AIBrain != null && AIBrain.TargetHasStats && AIBrain.TargetStats != null)
                    {
                        if (AIBrain.TargetStats.TryGetValue(Stat.ID, out Stat statT))
                        {
                            result = CompareWithValue(statT.Value);
                        }
                    }
                    break;
            }

            return result;
        }

        private bool CompareWithValue(float stat)
        {
            switch (StatIs)
            {
                case ComparerInt.Equal:
                    return stat == Value;
                case ComparerInt.Greater:
                    return stat > Value;
                case ComparerInt.Less:
                    return stat < Value;
                case ComparerInt.NotEqual:
                    return stat != Value;
                default:
                    return false;
            }
        }
    }
}
