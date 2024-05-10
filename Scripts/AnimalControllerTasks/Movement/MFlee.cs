using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Flee", "Animal Controller/ACMovement/Flee", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MFlee : MTaskNode
    {
        [Header("Node")]
        /// <summary> Distance for the Flee, Circle Around and keep Distance Task</summary>
        public FloatReference distance = new(10f);
        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);
        /// <summary> Animal Controller Stopping Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference stoppingDistance = new(0.5f);
        public bool LookAtTarget = false;
        [Tooltip("It will flee from the Target forever. If this value is false it will flee once it has reached a safe distance and the Task will end.")]
        public bool FleeForever = true;

        bool arrived;

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
            base.OnEntry();
            AIBrain.AIControl.UpdateDestinationPosition = false;

            AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
            Flee(AIBrain);
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            Flee(AIBrain);

            if (arrived)
            {
                return State.Success;
            }
            return State.Running;
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
            arrived = false;
        }

        private void Flee(AIBrain AIBrain)
        {
            if (AIBrain.Target)
            {
                //Animal wont Update the Destination Position with the Target position. We need to go to the opposite side
                AIBrain.AIControl.UpdateDestinationPosition = false;

                //We can look at the target on arrival, we are fleeing from the target!
                // AIBrain.AIControl.LookAtTargetOnArrival = false;

                var CurrentPos = AIBrain.Animal.transform.position;

                var AgentDistance = Vector3.Distance(AIBrain.Animal.transform.position, AIBrain.Position);
                var TargetDirection = CurrentPos - AIBrain.Target.position;

                float TargetDistance = TargetDirection.magnitude;

                var distance = this.distance * AIBrain.Animal.ScaleFactor; //Remember to use the scale

                if (TargetDistance < distance)
                {
                    //player is too close from us, pick a point diametrically oppossite at twice that distance and try to move there.
                    Vector3 fleePoint = AIBrain.Target.position + (TargetDirection.normalized * (distance + (AgentDistance * 2f)));

                    AIBrain.AIControl.StoppingDistance = stoppingDistance;

                    Debug.DrawRay(fleePoint, Vector3.up * 3, Color.blue, 2f);

                    //If the New flee Point is not in the Stopping distance radius then set a new Flee Point
                    if (Vector3.Distance(CurrentPos, fleePoint) > stoppingDistance)
                    {
                        AIBrain.AIControl.UpdateDestinationPosition = false; //Means the Animal wont Update the Destination Position with the Target position.
                        AIBrain.AIControl.SetDestination(fleePoint, true);

                        if (AIBrain.debug)
                        {
                            Debug.DrawRay(fleePoint, AIBrain.transform.up, Color.blue, 2f);
                        }
                    }
                }
                else
                {
                    AIBrain.AIControl.Stop();
                    AIBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;

                    if (!FleeForever)
                    {
                        arrived = true; //If flee forever is false then flee is finished
                    }
                }
            }
        }
    }
}