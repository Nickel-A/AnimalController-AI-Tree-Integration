using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Flee", "Animal Controller/ACMovement/Flee", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MFlee : TaskNode
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
            base.OnEntry();
            aiBrain.AIControl.UpdateDestinationPosition = false;

            aiBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
            Flee(aiBrain);
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            Flee(aiBrain);

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

        private void Flee(AIBrain aiBrain)
        {
            if (aiBrain.Target)
            {
                //Animal wont Update the Destination Position with the Target position. We need to go to the opposite side
                aiBrain.AIControl.UpdateDestinationPosition = false;

                //We can look at the target on arrival, we are fleeing from the target!
                // brain.AIControl.LookAtTargetOnArrival = false;

                var CurrentPos = aiBrain.Animal.transform.position;

                var AgentDistance = Vector3.Distance(aiBrain.Animal.transform.position, aiBrain.Position);
                var TargetDirection = CurrentPos - aiBrain.Target.position;

                float TargetDistance = TargetDirection.magnitude;

                var distance = this.distance * aiBrain.Animal.ScaleFactor; //Remember to use the scale

                if (TargetDistance < distance)
                {
                    //player is too close from us, pick a point diametrically oppossite at twice that distance and try to move there.
                    Vector3 fleePoint = aiBrain.Target.position + (TargetDirection.normalized * (distance + (AgentDistance * 2f)));

                    aiBrain.AIControl.StoppingDistance = stoppingDistance;

                    Debug.DrawRay(fleePoint, Vector3.up * 3, Color.blue, 2f);

                    //If the New flee Point is not in the Stopping distance radius then set a new Flee Point
                    if (Vector3.Distance(CurrentPos, fleePoint) > stoppingDistance)
                    {
                        aiBrain.AIControl.UpdateDestinationPosition = false; //Means the Animal wont Update the Destination Position with the Target position.
                        aiBrain.AIControl.SetDestination(fleePoint, true);

                        if (aiBrain.debug)
                        {
                            Debug.DrawRay(fleePoint, aiBrain.transform.up, Color.blue, 2f);
                        }
                    }
                }
                else
                {
                    aiBrain.AIControl.Stop();
                    aiBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;

                    if (!FleeForever)
                    {
                        arrived = true; //If flee forever is false then flee is finished
                    }
                }
            }
        }
    }
}