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
        // Gizmo color
        public Color gizmoColor = Color.yellow;
        public bool debug;

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
            // Get the last known position from AIBrain component
            var lastDestination = AIBrain.GetLastTargetPosition();
            Debug.DrawRay(AIBrain.Position, Vector3.up, Color.white, 1);
            // Set the AI's destination to the last known position
            AIBrain.AIControl.SetDestination(lastDestination, true);
            // Set other properties
            AIBrain.AIControl.UpdateDestinationPosition = false;
            AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;

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

        public override void OnDrawGizmos()
        {
            if (debug)
            {

            base.OnDrawGizmos();
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(AIBrain.AIControl.DestinationPosition, 1f);
            }
        }
    }
}