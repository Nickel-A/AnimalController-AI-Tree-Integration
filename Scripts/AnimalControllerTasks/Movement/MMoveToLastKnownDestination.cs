using Malbers.Integration.AITree;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Last Known Destination", "Animal Controller/ACMovement/Move To Last Known Destination", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MMoveToLastKnownDestination : MTaskNode
    {
        [Header("Node")]

        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);

        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            var LastDestination = AIBrain.AIControl.DestinationPosition; //Store the Last Destination
            Debug.DrawRay(AIBrain.Position, Vector3.up, Color.white, 1);
            AIBrain.AIControl.DestinationPosition = Vector3.zero;
            AIBrain.AIControl.SetDestination(LastDestination, true); //Go to the last Destination position
            AIBrain.AIControl.UpdateDestinationPosition = false;          //Set the Animal to look Forward to the Target
            AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target

            base.OnEntry();
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (AIBrain.AIControl.HasArrived)
            {
                AIBrain.AIControl.Stop();
                return State.Success;
            }
            else return State.Running;
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
        }
    }
}