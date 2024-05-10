using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Stat", "Animal Controller/General/Set Stat", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetStatNode : MTaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;
        public StatModifier stat;

        bool taskDone;

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {
            if (affect == Affected.Self)
            {
                if (AIBrain.AnimalStats != null)
                {
                    if (AIBrain.AnimalStats.TryGetValue(stat.ID, out Stat statS))
                    {
                        stat.ModifyStat(statS);
                    }
                }
            }
            else
            {
                if (AIBrain.TargetStats != null)
                {
                    if (AIBrain.TargetStats.TryGetValue(stat.ID, out Stat statS))
                    {
                        stat.ModifyStat(statS);
                    }
                }
            }
            taskDone = true;
        }

        protected override State OnUpdate()
        {
            if (taskDone)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }

        protected override void OnExit()
        {
            base.OnExit();
            taskDone = false;
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();
            description += "Affect: ";
            if (AIBrain != null)
            {
                if (affect == Affected.Self)
                {
                    description += "Self\n";
                    description += $"Mode ID: {(AIBrain.AnimalStats != null ? stat.ID.name : "null")}\n";
                    description += $"Modify: {(AIBrain.AnimalStats != null ? stat.modify : "null")}\n";
                }
                else
                {
                    description += "Target\n";
                    description += $"Mode ID: {(AIBrain.TargetStats != null ? stat.ID.DisplayName : "null")}\n";
                    description += $"Modify: {(AIBrain.TargetStats != null ? stat.modify : "null")}\n";
                }
            }
            return description;
        }
    }
}