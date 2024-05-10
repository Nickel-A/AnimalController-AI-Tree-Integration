using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Keep Distance", "Animal Controller/ACMovement/Keep Distance", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MKeepDistance : MTaskNode
    {
        [Header("Node Settings")]
        public FloatReference distance = new FloatReference(10f);
        public FloatReference distanceThreshold = new FloatReference(1f);
        public FloatReference stoppingDistance = new FloatReference(0.5f);
        public FloatReference slowingDistance = new FloatReference(0f);
        public bool lookAtTarget = false;
        public bool keepDistanceForever;
        public bool useStrafe;

        private bool arrived;

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {
            base.OnEntry();
            AIBrain.Animal.Strafe = useStrafe;
            arrived = false;
            AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;
        }

        protected override State OnUpdate()
        {
            if (AIBrain.Target)
            {
                KeepDistance();
                if (arrived)
                {
                    return State.Success;
                }
                return State.Running;
            }
            return State.Failure; // No target found
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void KeepDistance()
        {
            Vector3 keepDistPoint = AIBrain.Animal.transform.position;
            var dirFromTarget = keepDistPoint - AIBrain.Target.position;
            float halThreshold = distanceThreshold * 0.5f;
            float targetDist = dirFromTarget.magnitude;

            float targetDistance = distance * AIBrain.Animal.ScaleFactor;

            if (targetDist < targetDistance - distanceThreshold) // Flee 
            {
                float distanceDiff = targetDistance - targetDist;
                keepDistPoint = CalculateDistance(AIBrain, dirFromTarget, distanceDiff, halThreshold);
            }
            else if (targetDist > targetDistance + distanceThreshold) // Go to Target
            {
                float distanceDiff = targetDist - targetDistance;
                keepDistPoint = CalculateDistance(AIBrain, -dirFromTarget, distanceDiff, -halThreshold);
            }
            else // Maintain distance
            {
                if (!AIBrain.AIControl.HasArrived)
                {
                    AIBrain.AIControl.Stop();
                }
                AIBrain.AIControl.LookAtTargetOnArrival = lookAtTarget;
                AIBrain.AIControl.HasArrived = true;
                AIBrain.AIControl.StoppingDistance = targetDistance + distanceThreshold;
                AIBrain.AIControl.RemainingDistance = 0;
                if (!keepDistanceForever)
                {
                    arrived = true;
                }
            }
        }

        private Vector3 CalculateDistance(AIBrain AIBrain, Vector3 dirFromTarget, float distanceDiff, float halThreshold)
        {
            Vector3 keepDistPoint = AIBrain.transform.position + (dirFromTarget.normalized * (distanceDiff + halThreshold));
            AIBrain.AIControl.UpdateDestinationPosition = false;
            AIBrain.AIControl.StoppingDistance = stoppingDistance;
            AIBrain.AIControl.SetDestination(keepDistPoint, true);
            return keepDistPoint;
        }
    }
}
