using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For GameObjectSet", "Animal Controller/Look/Look For GameObjectSet", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForGameObjectSet : ObserverDecorator
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

        [RequiredField, Tooltip("GameObjectSet. Search for all  GameObjects Set in the Set")]
        public RuntimeGameObjects gameObjectSet;
        [Space(20), Tooltip("Select randomly one of the potential targets, not the first one found")]
        public bool chooseRandomly = false;

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
            return LookForGoSet();
        }

        public bool LookForGoSet()
        {
            if (gameObjectSet == null || gameObjectSet.Count == 0 || brain == null)
            {
                return false;
            }

            if (chooseRandomly)
            {
                return ChooseRandomObject();
            }

            return ClosestGameObject();
        }

        private bool ClosestGameObject()
        {
            if (gameObjectSet != null && gameObjectSet.Count > 0)
            {
                var allGameObjects = gameObjectSet.Items;

                if (allGameObjects == null || allGameObjects.Count == 0)
                {
                    return false;
                }

                float minDistance = float.MaxValue;
                GameObject closestGameObject = null;

                foreach (var go in allGameObjects)
                {
                    if (go != null)
                    {
                        Vector3 center = go.transform.position;

                        if (AIUtility.IsInFieldOfView(brain, center, lookAngle, lookRange, lookMultiplier, obstacleLayer, out float distance))
                        {
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestGameObject = go;
                            }
                        }
                    }
                }

                if (closestGameObject != null)
                {
                    if (assignTarget)
                    {
                        brain.AIControl.SetTarget(closestGameObject.transform, moveToTarget);
                    }
                    return true;
                }
            }
            return false;
        }


        public bool ChooseRandomObject()
        {
            var All = new List<GameObject>();
            if (gameObjectSet != null && gameObjectSet.Items != null)
            {
                All.AddRange(gameObjectSet.Items); // Add all the saved gameobjects to the list
            }

            if (All.Count == 0)
            {
                return false;
            }

            while (All.Count != 0)
            {
                int newIndex = Random.Range(0, All.Count);
                if (All[newIndex] != null)
                {
                    var center = All[newIndex].transform.position + new Vector3(0, brain.Animal.Height, 0);

                    var renderer = All[newIndex].GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        center = renderer.bounds.center;
                    }

                    if (AIUtility.IsInFieldOfView(brain, center, lookAngle, lookRange, lookMultiplier, obstacleLayer, out float distance))
                    {
                        if (assignTarget)
                        {
                            brain.AIControl.SetTarget(All[newIndex].transform, moveToTarget);
                        }
                        return true;
                    }
                }
                All.RemoveAt(newIndex);
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
