using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEditor;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Target From BB", "Animal Controller/Set Target From BB", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetTargetFromBB : TaskNode
    {

        public TransformKey target;
        public bool MoveToTarget = true;
        private AIBrain aiBrain;

        protected override void OnEntry()
        {
            aiBrain =  GetOwner().GetComponent<AIBrain>();

            if (MoveToTarget)
            {
                aiBrain.AIControl.UpdateDestinationPosition = true;          //Check if the target has moved
            }
            else
            {
                if (aiBrain.AIControl.IsMoving) { aiBrain.AIControl.Stop(); } //Stop if the animal is already moving
            }
            aiBrain.AIControl.SetTarget(target.GetValue(), MoveToTarget);

        }

        protected override State OnUpdate()
        {

                return RenownedGames.AITree.State.Success;

            
        }
 

    }
}
