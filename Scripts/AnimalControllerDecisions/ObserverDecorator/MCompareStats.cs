using MalbersAnimations;
using RenownedGames.AITree;
using System;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Compare Stats", "Animal Controller/MObserverDecorator/Compare Stats", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCompareStats : ObserverDecorator
    {
        [Header("Node")]

        [Tooltip("Stats you want to find on the AI Animal")]
        public StatID OwnStat;
        [Tooltip("Compare values of the Stat")]
        public ComparerInt compare = ComparerInt.Less;
        [Tooltip("Stats you want to find on the Target")]
        public StatID TargetStat;

        AIBrain AIBrain;

        public override event Action OnValueChange;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AIBrain = GetOwner().GetComponent<AIBrain>();
        }
        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }
        public override bool CalculateResult()
        {
            bool result = false;

            var OwnStats = AIBrain?.AnimalStats;
            var TargetStats = AIBrain?.TargetStats;

            if (OwnStats != null && TargetStats != null)
            {
                if (OwnStats.TryGetValue(OwnStat, out Stat own) &&
                    TargetStats.TryGetValue(TargetStat, out Stat target))
                {
                    return own.Value.CompareFloat(target.value, compare);
                }
            }
            return result;
        }

        public override string GetDescription()
        {
            string description = "";
            
            if (OwnStat != null)
            {
                description += $"Stat: {OwnStat.DisplayName} \n";
            }
            description += $"compare: {compare} \n";
            if (TargetStat != null)
            {
                description += $"Stat: {TargetStat.DisplayName} \n";
            }
            
            return description;
        }
    }
}