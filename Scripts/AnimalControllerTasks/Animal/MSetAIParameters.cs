using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using Unity.VisualScripting;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set AI Parameters", "Animal Controller/Set AI Parameters", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetAIParameters : TaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        public enum Parameters { StoppingDistance, AdditiveStopDistance };
        public Parameters Parameter;
        public float value;


        AIBrain aiBrain;
        MAnimalAIControl mAIControl;

        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
            if (affect == Affected.Self)
            {
                mAIControl = (MAnimalAIControl)aiBrain.AIControl;
            }
            else
            {
                mAIControl = aiBrain.AIControl.Target.GetComponent<MAnimalAIControl>();
            }

            switch (Parameter)
            {
                case Parameters.StoppingDistance:
                    mAIControl.StoppingDistance = value;
                    break;
                case Parameters.AdditiveStopDistance:
                    mAIControl.AdditiveStopDistance = value;
                    break;
            }
        }
        protected override State OnUpdate()
        {
            return State.Success;
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();

            description += "Affect: ";
            if (affect == Affected.Self)
            {
                description += "Self\n";
            }
            else
            {
                description += "Target\n";
            }
            return description;
        }
    }
}
