using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set AI Parameters", "Animal Controller/Animal/Set AI Parameters", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetAIParameters : MTaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        public enum Parameters { StoppingDistance, AdditiveStopDistance };
        public Parameters Parameter;
        public float value;
        IAIControl mAIControl;

        protected override void OnEntry()
        {
            if (affect == Affected.Self)
            {
                switch (Parameter)
                {
                    case Parameters.StoppingDistance:
                        AIBrain.AIControl.StoppingDistance = value;
                        break;
                    case Parameters.AdditiveStopDistance:
                        //AIBrain.AIControl.AdditiveStopDistance = value;
                        break;
                }
            }
            else if (affect == Affected.Target && AIBrain.AIControl.Target != null)
            {
                mAIControl = AIBrain.AIControl.Target.GetComponent<MAnimalAIControl>();
                switch (Parameter)
                {
                    case Parameters.StoppingDistance:
                        mAIControl.StoppingDistance = value;
                        break;
                    case Parameters.AdditiveStopDistance:
                        //mAIControl.AdditiveStopDistance = value;
                        break;
                }
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
