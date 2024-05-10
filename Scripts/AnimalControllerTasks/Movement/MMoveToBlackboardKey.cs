using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
namespace Malbers.Integration.AITree
{

    [NodeContent("Move To BlackboardKey", "Animal Controller/ACMovement/Move To BlackboardKey", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MMoveToBlackboardKey : MTaskNode
    {
        [Header("Node")]
        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);
        public bool LookAtTarget = false;
        [Tooltip("The AI will stop if it arrives to the current target")]
        public bool StopOnArrive = true;
        bool arrived;
        bool failed;
        public TransformKey transformKey;
        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {
            AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;
            if (transformKey.GetValue())
            {
                AIBrain.AIControl.SetTarget(transformKey.GetValue(), true); //Reset the Target
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
            //arrived = false;
            AIBrain.AIControl.UpdateDestinationPosition = false;
        }
    }
}
