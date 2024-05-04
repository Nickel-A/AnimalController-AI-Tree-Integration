using MalbersAnimations;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Stat", "Animal Controller/General/Set Stat", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetStatNode : TaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;
        public StatModifier stat;

        AIBrain aiBrain;

        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();

            if (affect == Affected.Self)
            {
                if (aiBrain.AnimalStats != null)
                {
                    if (aiBrain.AnimalStats.TryGetValue(stat.ID, out Stat statS))
                    {
                        stat.ModifyStat(statS);
                    }
                }
            }
            else
            {
                if (aiBrain.TargetStats != null)
                {
                    if (aiBrain.TargetStats.TryGetValue(stat.ID, out Stat statS))
                    {
                        stat.ModifyStat(statS);
                    }
                }
            }
            aiBrain.TasksDone = true;
        }

        protected override State OnUpdate()
        {
            if (aiBrain.TasksDone)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }
        public override string GetDescription()
        {
            string description = base.GetDescription();
            description += "Affect: ";
            if (aiBrain != null)
            {

                if (affect == Affected.Self)
                {
                    description += "Self\n";
                    description += $"Mode ID: {(aiBrain.AnimalStats != null ? stat.ID.name : "null")}\n";
                    description += $"Modify: {(aiBrain.AnimalStats != null ? stat.modify : "null")}\n";
                }
                else
                {
                    description += "Target\n";
                    description += $"Mode ID: {(aiBrain.TargetStats != null ? stat.ID.DisplayName : "null")}\n";
                    description += $"Modify: {(aiBrain.TargetStats != null ? stat.modify : "null")}\n";

                }
                
            }
            return description;
        }
    }
}