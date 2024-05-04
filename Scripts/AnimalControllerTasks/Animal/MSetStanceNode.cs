using MalbersAnimations;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using UnityEngine;
using UnityEngine.Serialization;
using State = RenownedGames.AITree.State;


namespace Malbers.Integration.AITree
{
    [NodeContent("Set Stance", "Animal Controller/Animal/Set Stance", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetStanceNode : TaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;
        [Tooltip("Stance to Set on Task Enter. Leave empty to ignore it"), FormerlySerializedAs("stance")]
        public StanceID stanceOnEnter;

        [Hide(nameof(stanceOnEnter), true)]
        [Tooltip("If enabled, restores the stance to its default value upon entering the task.")]
        public bool restoreDefaultOnEnter = false;

        [Tooltip("If enabled, resets the stance.")]
        public bool resetStance = false;


        AIBrain aiBrain;

        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();

            switch (affect)
            {
                case Affected.Self:
                    Stance_Set(aiBrain.Animal, stanceOnEnter, true);
                    break;
                case Affected.Target:
                    Stance_Set(aiBrain.TargetAnimal, stanceOnEnter, true);
                    break;
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


        public void Stance_Set(MAnimal animal, StanceID stance, bool onEnter)
        {
            if (animal != null)
            {
                if (stance != null)
                {
                    animal.Stance_Set(stance);
                }
                else if (onEnter && restoreDefaultOnEnter)
                {
                    animal.Stance_RestoreDefault();
                }
                else if (resetStance)
                {
                    animal.Stance_Reset();
                }
            }
        }
        public override string GetDescription()
        {
            string description = base.GetDescription();

            if (affect == Affected.Self)
            {
                description += "Self\n";

                if (stanceOnEnter != null)
                {
                    description += "Stance on Enter ID: ";
                    description += stanceOnEnter.DisplayName + "\n";
                }
                else
                {
                    description += $"Restore default on Enter: {restoreDefaultOnEnter}\n";
                }
            }
            else
            {
                description += "Target\n";

                if (stanceOnEnter != null)
                {
                    description += "Stance on Enter ID: ";
                    description += stanceOnEnter.DisplayName + "\n";
                }
                else
                {

                    description += $"Restore default on Enter: {restoreDefaultOnEnter}\n";
                }
            }
            return description;
        }
    }
}
