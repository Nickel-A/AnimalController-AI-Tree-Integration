using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Move To Cover", "Animal Controller/ACMovement/Move To Cover", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MMoveToCover : MTaskNode
    {
        public TransformVar player; // Assign the player GameObject in the Unity Editor
        public float searchRadius = 10f; // Radius to search for cover positions
        public float hideDistance = 5f; // Distance at which the AI should start hiding

        private Vector3 initialPosition;
        private bool isHiding = false;

        protected override State OnUpdate()
        {
            if (!isHiding)
            {
                // Find cover position
                Vector3 coverPosition = FindCoverPosition();

                if (coverPosition != Vector3.zero)
                {
                    // Move towards cover position
                    AIBrain.AIControl.SetDestination(coverPosition, true);
                    return State.Running;
                }
            }

            float distanceToPlayer = Vector3.Distance(AIBrain.transform.position, player.Value.position);

            // Check if the player is within hiding distance
            if (distanceToPlayer <= hideDistance)
            {
                // AI is hiding
                isHiding = true;
                Hide();
            }
            else
            {
                // Player is not within hiding distance
                isHiding = false;
                ReturnToInitialPosition();
            }

            if (AIBrain.AIControl.HasArrived)
                return State.Success;
            else
                return State.Running;
        }

        Vector3 FindCoverPosition()
        {
            Collider[] colliders = Physics.OverlapSphere(AIBrain.transform.position, searchRadius);

            foreach (Collider collider in colliders)
            {
                // Check if the collider is not the player and is not part of the AI itself
                if (collider.transform != player.Value && collider.transform != AIBrain.transform)
                {
                    // Check if there's enough space to hide behind this cover (you might need to adjust this condition)
                    if (!Physics.Linecast(AIBrain.transform.position, collider.transform.position))
                    {
                        return collider.transform.position;
                    }
                }
            }

            return Vector3.zero; // No cover position found
        }

        void Hide()
        {
            // Calculate direction from AI to player
            Vector3 directionToPlayer = AIBrain.transform.position - player.Value.position;
            // Normalize the direction to have unit length
            directionToPlayer.Normalize();
            // Calculate the position where AI should hide from the player
            Vector3 hidePosition = player.Value.position + directionToPlayer * hideDistance;
            // Move AI to the calculated hide position
            AIBrain.AIControl.SetDestination(hidePosition, true);
        }

        void ReturnToInitialPosition()
        {
            // If AI was hiding, return it to its initial position
            if (isHiding)
            {
                AIBrain.AIControl.SetDestination(initialPosition, true);
            }
        }

        public override void OnDrawGizmos()
        {
            // Draw Gizmos if needed
        }
    }
}
