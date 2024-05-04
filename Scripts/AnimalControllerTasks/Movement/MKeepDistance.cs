using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Keep Distance", "Animal Controller/ACMovement/Keep Distance", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MKeepDistance : TaskNode
    {
        [Header("Node")]
        /// <summary> Distance for the Flee, Circle Around and keep Distance Task</summary>
        public FloatReference distance = new(10f);
        /// <summary> Distance Threshold for the Keep Distance Task</summary>
        public FloatReference distanceThreshold = new(1f);
        /// <summary> Animal Controller Stopping Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference stoppingDistance = new(0.5f);
        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);
        public bool LookAtTarget = false;

        AIBrain aiBrain;
        bool arrived;
        public bool keepDistanceForever;
        public bool useStrafe;

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
            aiBrain.Animal.Strafe = useStrafe;
            arrived = false;
            aiBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            KeepDistance(aiBrain);

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

        private void KeepDistance(AIBrain aiBrain)
        {
            if (aiBrain.Target)
            {
                //aiBrain.AIControl.UpdateDestinationPosition = true;

                aiBrain.AIControl.StoppingDistance = stoppingDistance;
                //aiBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;

                Vector3 KeepDistPoint = aiBrain.Animal.transform.position;

                var DirFromTarget = KeepDistPoint - aiBrain.Target.position;

                float halThreshold = distanceThreshold * 0.5f;
                float TargetDist = DirFromTarget.magnitude;

                var distance = this.distance * aiBrain.Animal.ScaleFactor; //Remember to use the scale

                if (TargetDist < distance - distanceThreshold) //Flee 
                {
                    float DistanceDiff = distance - TargetDist;
                    KeepDistPoint = CalculateDistance(aiBrain, DirFromTarget, DistanceDiff, halThreshold);


                }
                else if (TargetDist > distance + distanceThreshold) //Go to Target
                {
                    float DistanceDiff = TargetDist - distance;
                    KeepDistPoint = CalculateDistance(aiBrain, -DirFromTarget, DistanceDiff, -halThreshold);
                }
                else
                {
                    if (!aiBrain.AIControl.HasArrived)
                    {
                        aiBrain.AIControl.Stop(); //It need to stop
                    }
                    else
                    {
                        aiBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;

                    }


                    aiBrain.AIControl.HasArrived = true;
                    aiBrain.AIControl.StoppingDistance = distance + distanceThreshold; //Force to have a greater Stopping Distance so the animal can rotate around the target
                    aiBrain.AIControl.RemainingDistance = 0; //Force the remaining distance to be 0
                    if (!keepDistanceForever) arrived = true;
                }
            }
        }
        private Vector3 CalculateDistance(AIBrain brain, Vector3 DirFromTarget, float DistanceDiff, float halThreshold)
        {
            Vector3 KeepDistPoint = brain.transform.position + (DirFromTarget.normalized * (DistanceDiff + halThreshold));
            brain.AIControl.UpdateDestinationPosition = false; //Means the Animal Wont Update the Destination Position with the Target position.
            brain.AIControl.StoppingDistance = stoppingDistance;
            brain.AIControl.SetDestination(KeepDistPoint, true);
            return KeepDistPoint;
        }
    }
}