using Malbers.Integration.AITree;
using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For GameObject", "Animal Controller/Look/Look For GameObject", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForGameObject : ObserverDecorator
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

        [Tooltip("Buffer for storing colliders from Physics.OverlapSphereNonAlloc. Pre-allocated for performance optimization.")]
        [SerializeField] Collider[] collidersBuffer = new Collider[100];

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
            bool isGameObjectFound = LookForGameObjectByName();

            if (isGameObjectFound && assignTarget)
            {
                brain.AIControl.SetTarget(MAnimal.MainAnimal.transform, moveToTarget);
            }

            return isGameObjectFound;
        }

        private bool LookForGameObjectByName()
        {
            if (string.IsNullOrEmpty(gameObjectName) || !brain)
            {
                return false;
            }

            GameObject targetObject = GameObject.Find(gameObjectName);

            if (targetObject == null)
            {
                return false;
            }

            Vector3 targetPosition = targetObject.transform.position;

            int numColliders = Physics.OverlapSphereNonAlloc(brain.transform.position, lookRange, collidersBuffer);

            for (int i = 0; i < numColliders; i++)
            {
                Collider collider = collidersBuffer[i];

                if (AIUtility.IsInFieldOfView(brain, targetPosition, lookAngle, lookRange, lookMultiplier, obstacleLayer, out _))
                {
                    return true;
                }
            }

            return false;
        }
#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            AIUtility.DrawFieldOfViewGizmos(brain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}

