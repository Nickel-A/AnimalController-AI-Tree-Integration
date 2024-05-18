using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For Blackboard Key", "Animal Controller/Look/Look For Blackboard Key", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForBlackboardKey : ObserverDecorator
    {
        [Header("Node")]

        public Color debugColor = new(0, 0, 0.7f, 0.3f);
        [Range(0, 1)]
        /// <summary>Angle of Vision of the Animal</summary>
        [Tooltip("Shorten the Look Ray to not found the ground by mistake")]
        public float lookMultiplier = 0.9f;

        /// <summary>Range for Looking forward and Finding something</summary>
        [Tooltip("Range for Looking forward and Finding something")]
        public float lookRange = 15;
        [Range(0, 360)]
        /// <summary>Angle of Vision of the Animal</summary>
        [Tooltip("Angle of Vision of the Animal")]
        public float lookAngle = 120;

        [Tooltip("Layers that can block the Animal Eyes")]
        public LayerReference obstacleLayer = new(1);

        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
        public bool assignTarget = false;
        [Tooltip("If the what we are looking for is found then also start moving")]
        public bool moveToTarget = false;

        [KeyTypes(typeof(Transform), typeof(Vector3))]
        public Key key;
        Transform targetTransfrom;

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
            bool result = LookForBlackboardKey();
            if (result && assignTarget && AIBrain.Target != MAnimal.MainAnimal.transform)
            {
                AIBrain.AIControl.SetTarget(targetTransfrom, moveToTarget);
            }
            return result;
        }

        private bool LookForBlackboardKey()
        {
            if (key == null) return false;

            if (key is TransformKey transformKey)
            {
                Transform transform = transformKey.GetValue();
                if (transform != null)
                {
                    targetTransfrom = transform;
                }
            }
            else if (key is Vector3Key vector3Key)
            {
                targetTransfrom.position = vector3Key.GetValue();
            }
            if (targetTransfrom == null) return false;
            return AIUtility.IsInFieldOfView(AIBrain, targetTransfrom.position, lookAngle, lookRange, lookMultiplier, obstacleLayer, out _);

        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            AIUtility.DrawFieldOfViewGizmos(AIBrain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}