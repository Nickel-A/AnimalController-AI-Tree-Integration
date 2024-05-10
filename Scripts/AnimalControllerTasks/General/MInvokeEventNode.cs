using MalbersAnimations.Controller.AI;
using MalbersAnimations.Events;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Invoke Event", "Animal Controller/General/Invoke Event", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MInvokeEventNode : MTaskNode
    {
        [Header("Node")]

        [Tooltip("Send the Animal as the Event Parameter or the Target")]
        public Affected send = Affected.Self;
        public GameObjectEvent Raise = new GameObjectEvent();

        protected override void OnEntry()
        {
            switch (send)
            {
                case Affected.Self:
                    Raise.Invoke(AIBrain.Animal.gameObject);
                    break;
                case Affected.Target:
                    Raise.Invoke(AIBrain.Target.gameObject);
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
