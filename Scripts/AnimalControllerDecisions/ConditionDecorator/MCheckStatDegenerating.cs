using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Stat Degenerating", "Animal Controller/MConditionDecorator/Check Stat Degenerating", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckStatDegenerating : MConditionDecorator
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
                            result = statS.IsDegenerating;
                        }
                    }
                    break;

                case Affected.Target:
                    if (AIBrain != null && AIBrain.TargetHasStats && AIBrain.TargetStats != null)
                    {
                        if (AIBrain.TargetStats.TryGetValue(Stat.ID, out Stat statT))
                        {
                            result = statT.IsDegenerating;
                        }
                    }
                    break;
            }

            return result;
        }
    }
}
