using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Move To Current Target", "Animal Controller/ACMovement/Move To Current Target", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MMoveToCurrentTarget : TaskNode
    {
        [Header("Node")]
        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);
        public bool LookAtTarget = false;
        [Tooltip("The AI will stop if it arrives to the current target")]
        public bool StopOnArrive = true;
        AIBrain aiBrain;
        bool arrived;
        bool failed;

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
            aiBrain.AIControl.CurrentSlowingDistance = slowingDistance;
            if (aiBrain.AIControl.Target)
            {
                aiBrain.AIControl.SetTarget(aiBrain.AIControl.Target, true); //Reset the Target
                aiBrain.AIControl.UpdateDestinationPosition = true;          //Check if the target has moved
            }
            else
            {
                Debug.LogWarning("The Animal does not have a current Target", this);
                failed = true;
            }
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (failed)
            {
                return State.Failure;
            }
            StopOnArrived();
            return arrived ? State.Success : State.Running;
        }

        private void StopOnArrived()
        {
            if (aiBrain.AIControl.HasArrived)
            {
                if (StopOnArrive)
                {
                    aiBrain.AIControl.Stop();
                }
                aiBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;
                arrived = true;
            }
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
            arrived = false;
            aiBrain.AIControl.UpdateDestinationPosition = false;
        }
    }
}