using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For AIState", "Animal Controller/Look/Look For AIState", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForAIState : ObserverDecorator
    {
        [Header("Node")]
        public Color debugColor = new Color(0, 0, 0.7f, 0.3f);
        [Range(0, 1)]
        [Tooltip("Shorten the Look Ray to not found the ground by mistake")]
        public float lookMultiplier = 0.9f;
        [Tooltip("Range for Looking forward and Finding something")]
        public float lookRange = 15;
        [Range(0, 360)]
        [Tooltip("Angle of Vision of the Animal")]
        public float lookAngle = 120;
        [Tooltip("Layers that can block the Animal Eyes")]
        public LayerReference obstacleLayer = new LayerReference(1);

        [Tooltip("AIState to look for")]
        public AIStateID AIStateToLookFor;

        [Tooltip("Select randomly one of the potential targets, not the first one found")]
        public bool chooseRandomly = false;
        [Space(20), Tooltip("If the AIState is found then Assign it as a new Target")]
        public bool assignTarget = false;
        [Tooltip("If the AIState is found then also start moving")]
        public bool moveToTarget = false;

        AIBrain AIBrain;
        public bool debug;

        public override event Action OnValueChange;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AIBrain = GetOwner().GetComponent<AIBrain>();
        }

        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }

        public override bool CalculateResult()
        {
            return LookForAIState(AIBrain);
        }

        public bool LookForAIState(AIBrain AIBrain)
        {
            if (AIBrain == null || AIStateToLookFor == null)
            {
                return false;
            }

            float minDistance = float.MaxValue;
            AIBrain closestAI = null;

            foreach (var ai in FindObjectsByType<AIBrain>(FindObjectsSortMode.None)) // Find all AIBrains in the scene
            {
                if (ai != AIBrain) // Exclude self
                {
                    if (AIUtility.IsInFieldOfView(AIBrain, ai.transform.position, lookAngle, lookRange, lookMultiplier, obstacleLayer, out float distance)) // Check if the AI is within field of view
                    {
                        if (distance < minDistance) // If it's the closest AI found so far
                        {
                            minDistance = distance;
                            closestAI = ai;
                        }
                    }
                }
            }

            if (closestAI != null && closestAI.CurrentAIState == AIStateToLookFor) // If a closest AI with the desired state is found
            {
                if (assignTarget)
                {
                    // Assign target or perform other actions here
                }
                return true;
            }
            return false;
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            if (debug)
            AIUtility.DrawFieldOfViewGizmos(AIBrain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}
