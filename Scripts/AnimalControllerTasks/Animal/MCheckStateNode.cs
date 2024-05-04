using MalbersAnimations;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check State", "Animal Controller/Animal/Check State", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MCheckStateNode : TaskNode
    {
        [Header("Node")]

        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected check = Affected.Self;
        public StateID StateID;
        [Tooltip("Check if the State is Entering or Exiting")]
        public EEnterExit when = EEnterExit.Enter;
        AIBrain aiBrain;

        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
        }

        protected override State OnUpdate()
        {
            switch (check)
            {

                case Affected.Self:
                    if (CheckState(aiBrain.Animal))
                    {
                        return State.Success;
                    }
                    else
                    {
                        return State.Failure;
                    }

                case Affected.Target:
                    if (aiBrain.TargetAnimal != null && CheckState(aiBrain.TargetAnimal))
                    {
                        return State.Success;
                    }
                    else
                    {
                        return State.Failure;
                    }

                default:
                    return State.Failure;
            }
        }

        private bool CheckState(MAnimal animal)
        {
            switch (when)
            {
                case EEnterExit.Enter:
                    Debug.Log("Enter " + (animal.ActiveStateID == StateID.ID));
                    return animal.ActiveStateID == StateID.ID;
                case EEnterExit.Exit:
                    return animal.LastState.ID == StateID.ID;
                default:
                    return false;
            }
        }
    }
}
