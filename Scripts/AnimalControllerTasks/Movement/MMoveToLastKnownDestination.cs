using Malbers.Integration.AITree;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Move To Last Known Destination", "Animal Controller/ACMovement/Last Known Destination", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MMoveToLastKnownDestination : TaskNode
    {
        [Header("Node")]

        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);

        AIBrain aiBrain;
        bool arrived;

        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();

        }

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            var LastDestination = aiBrain.AIControl.DestinationPosition; //Store the Last Destination
            Debug.DrawRay(aiBrain.Position, Vector3.up, Color.white, 1);
            aiBrain.AIControl.DestinationPosition = Vector3.zero;
            aiBrain.AIControl.SetDestination(LastDestination, true); //Go to the last Destination position
            aiBrain.AIControl.UpdateDestinationPosition = false;          //Set the Animal to look Forward to the Target
            aiBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target

            base.OnEntry();
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (aiBrain.AIControl.HasArrived)
            {
                aiBrain.AIControl.Stop();
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