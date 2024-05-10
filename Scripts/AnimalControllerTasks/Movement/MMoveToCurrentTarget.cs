using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Move To Current Target", "Animal Controller/ACMovement/Move To Current Target", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MMoveToCurrentTarget : MTaskNode
    {
        [Header("Node")]
        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);
        public float stoppingDistance =1.5f;
        public float additiveStopDistance = 0.5f;

        public bool LookAtTarget = false;
        [Tooltip("The AI will stop if it arrives to the current target")]
        public bool StopOnArrive = true;
        bool arrived;
        bool failed;

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
            AIBrain.AIControl.StoppingDistance = stoppingDistance;
            AIBrain.AIControl.AdditiveStopDistance = additiveStopDistance;
            AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;
            if (AIBrain.AIControl.Target)
            {
                AIBrain.AIControl.SetTarget(AIBrain.AIControl.Target, true); //Reset the Target
                AIBrain.AIControl.UpdateDestinationPosition = true;          //Check if the target has moved
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
            if (AIBrain.AIControl.HasArrived)
            {
                if (StopOnArrive)
                {
                    AIBrain.AIControl.Stop();
                }
                AIBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;
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
            AIBrain.AIControl.UpdateDestinationPosition = false;
            AIBrain.AIControl.ResetStoppingDistance();
        }
    }
}