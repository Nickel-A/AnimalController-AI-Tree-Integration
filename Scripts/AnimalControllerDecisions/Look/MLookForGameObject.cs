using Malbers.Integration.AITree;
using MalbersAnimations.Conditions;
using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For GameObject", "Animal Controller/Look/Look For GameObject", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForGameObject : MObserverDecorator
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

        [Space]
        [Tooltip("Look for an Specific GameObject by its name")]
        public string gameObjectName = string.Empty;
        GameObject targetByName;
        [Tooltip("Buffer for storing colliders from Physics.OverlapSphereNonAlloc. Pre-allocated for performance optimization.")]
        [SerializeField] Collider[] collidersBuffer = new Collider[100];

        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
        public bool assignTarget = false;
        [Tooltip("If the what we are looking for is found then also start moving")]
        public bool moveToTarget = false;

        public override event Action OnValueChange;

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }

        protected override void OnEntry()
        {
            base.OnEntry();
            AIBrain.AIControl.UpdateDestinationPosition = false;
        }

        /// <summary>
        /// Calculates the result of the condition.
        /// </summary>
        public override bool CalculateResult()
        {
            bool isGameObjectFound = LookForGameObjectByName();

            if (isGameObjectFound && assignTarget)
            {
                AIBrain.AIControl.SetTarget(targetByName.transform, moveToTarget);
            }

            return isGameObjectFound;
        }

        private bool LookForGameObjectByName()
        {
            if (string.IsNullOrEmpty(gameObjectName) || !AIBrain)
            {
                return false;
            }

            targetByName = GameObject.Find(gameObjectName);

            if (targetByName == null)
            {
                return false;
            }

            Vector3 targetPosition = targetByName.transform.position;

            int numColliders = Physics.OverlapSphereNonAlloc(AIBrain.transform.position, lookRange, collidersBuffer);

            for (int i = 0; i < numColliders; i++)
            {
                Collider collider = collidersBuffer[i];

                if (IsInFieldOfView(AIBrain, targetPosition, lookAngle, lookRange, lookMultiplier, obstacleLayer, out _))
                {
                    return true;
                }
            }

            return false;
        }
#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            if(AIBrain.debug)
            DrawFieldOfViewGizmos(AIBrain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}

