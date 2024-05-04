using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Move To Next Target", "Animal Controller/ACMovement/Move To Next Target", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MMoveToNextTarget : TaskNode
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
            base.OnEntry();

            if (aiBrain.AIControl.NextTarget)
            {
                aiBrain.AIControl.SetTarget(aiBrain.AIControl.NextTarget, true);
            }
            else
            {
                Debug.LogWarning("The Animal does not have a next Target", this);
                failed = true;
            }
            aiBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
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

            IsArrivedOnNextTarget();

            if (arrived)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }
        private void IsArrivedOnNextTarget()
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
            failed = false;

        }
    }
}