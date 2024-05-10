using MalbersAnimations.Conditions;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For Unity Tags", "Animal Controller/Look/Look For Unity Tags", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForUnityTags : MObserverDecorator
    {
        [Header("Node")]
        public Color debugColor = new Color(0, 0, 0.7f, 0.3f);
        [Range(0, 1)]
        [Tooltip("Shorten the Look Ray to not found the ground by mistake")]
        public float lookMultiplier = 0.9f;
        [Tooltip("Range for Looking forward and Finding something")]
        public float lookRange =15;
        [Range(0, 360)]
        [Tooltip("Angle of Vision of the Animal")]
        public float lookAngle = 120;
        [Tooltip("Layers that can block the Animal Eyes")]
        public LayerReference obstacleLayer = new LayerReference(1);

        [Space]
        [Tooltip("Look for this Unity Tag on an Object")]
        public string unityTag = string.Empty;
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

        public override bool CalculateResult()
        {
            return LookForUnityTags();
        }

        public bool LookForUnityTags()
        {
            if (string.IsNullOrEmpty(unityTag) || AIBrain == null)
            {
                return false;
            }

            int numColliders = Physics.OverlapSphereNonAlloc(AIBrain.transform.position, lookRange, collidersBuffer);

            for (int i = 0; i < numColliders; i++)
            {
                Collider collider = collidersBuffer[i];

                if (collider.CompareTag(unityTag))
                {
                    if (assignTarget)
                    {
                        AIBrain.AIControl.SetTarget(collider.transform, moveToTarget);
                    }
                    return true;
                }
            }

            return false;
        }


#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            DrawFieldOfViewGizmos(AIBrain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}
