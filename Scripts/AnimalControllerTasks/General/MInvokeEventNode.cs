using MalbersAnimations.Events;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Invoke Event", "Animal Controller/Invoke Event", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MInvokeEventNode : TaskNode
    {
        [Header("Node")]

        [Tooltip("Send the Animal as the Event Parameter or the Target")]
        public Affected send = Affected.Self;
        public GameObjectEvent Raise = new GameObjectEvent();
        AIBrain aiBrain;

        protected override void OnEntry()
        {
             GetOwner().GetComponent<AIBrain>();
            switch (send)
            {
                case Affected.Self:
                    Raise.Invoke(aiBrain.Animal.gameObject);
                    break;
                case Affected.Target:
                    Raise.Invoke(aiBrain.Target.gameObject);
                    break;
                default:
                    break;
            }
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();

            string checkType;
            if (send == Affected.Self)
            {
                checkType = "Self";
            }
            else
            {
                checkType = "Target";
            }
            description += $"Send: {checkType}\n";

            return description;
        }

        protected override State OnUpdate()
        {


            return State.Success;
        }
    }
}
