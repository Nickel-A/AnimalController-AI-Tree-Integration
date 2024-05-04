using Malbers.Integration.AITree;
using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For TransformVar", "Animal Controller/Look/Look For TransformVar", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForTransformVar : ObserverDecorator
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

        [Space(20), NotNull, RequiredField, Tooltip("Transform Reference value. This value should be set by a Transform Hook Component")]
        public TransformVar transform;

        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
        public bool assignTarget = false;
        [Tooltip("If the what we are looking for is found then also start moving")]
        public bool moveToTarget = false;

        private AIBrain brain;

        public override event Action OnValueChange;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            brain = GetOwner().GetComponent<AIBrain>();
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
            bool result = LookForTransformVar();
            if (assignTarget && result)
            {
                brain.AIControl.SetTarget(transform.Value, moveToTarget);
            }
            return result;
        }

        public bool LookForTransformVar()
        {
            if (transform == null || transform.Value == null || brain == null)
            {
                return false;
            }
            var Center =
                transform.Value == brain.Target && brain.AIControl.IsAITarget != null ?
                brain.AIControl.IsAITarget.GetCenterY() :
                transform.Value.position;

            return AIUtility.IsInFieldOfView(brain, Center,lookAngle,lookRange,lookMultiplier,obstacleLayer, out _);
        }
#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            AIUtility.DrawFieldOfViewGizmos(brain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}