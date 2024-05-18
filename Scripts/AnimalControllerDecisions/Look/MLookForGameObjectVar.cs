using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For GameObjectVar", "Animal Controller/Look/Look For GameObjectVar", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForGameObjectVar : ObserverDecorator
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
        [Space(20), NotNull, RequiredField, Tooltip("GameObject Reference value. This value should be set by a GameObject Hook Component")]
        public GameObjectVar gameObject;
        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
        public bool assignTarget = false;
        [Tooltip("If the what we are looking for is found then also start moving")]
        public bool moveToTarget = false;

        AIBrain AIBrain;

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

        /// <summary>
        /// Calculates the result of the condition.
        /// </summary>
        public override bool CalculateResult()
        {
            bool result = LookForGoVar();
            if (assignTarget && result)
            {
                AIBrain.AIControl.SetTarget(gameObject.Value.transform, moveToTarget);
            }
            return result;
        }

        public bool LookForGoVar()
        {
            if (gameObject == null && gameObject.Value && !gameObject.Value.IsPrefab())
            {
                return false;
            }
            var Center =
                gameObject.Value.transform == AIBrain.Target && AIBrain.AIControl.IsAITarget != null ?
                AIBrain.AIControl.IsAITarget.GetCenterY() :
                gameObject.Value.transform.position;

            return AIUtility.IsInFieldOfView(AIBrain, Center,lookAngle,lookRange,lookMultiplier,obstacleLayer, out _);
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            AIUtility.DrawFieldOfViewGizmos(AIBrain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}