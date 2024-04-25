using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{

    [NodeContent("Clear Target", "Animal Controller/Clear Target", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MClearTarget : TaskNode
    {
        // public GameObject target;
        private AIBrain aiBrain;
        protected override void OnEntry()
        {
            aiBrain = GetOwner().gameObject.GetComponent<AIBrain>();
            aiBrain.AIControl.SetTarget(null, false);
        }

        protected override State OnUpdate()
        {

            return State.Success;


        }
    }
}

