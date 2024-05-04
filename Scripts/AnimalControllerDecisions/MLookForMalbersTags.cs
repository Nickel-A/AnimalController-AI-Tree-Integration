using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For Malbers Tags", "Animal Controller/Look/Look For Malbers Tags", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForMalbersTags : ObserverDecorator
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

        AIBrain brain;

        /// <summary>Custom Tags you want to find</summary>
        [Tooltip("Custom Tags you want to find")]
        public Tag[] tags;

        [Tooltip("Select randomly one of the potential targets, not the first one found")]
        public bool chooseRandomly = false;
        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
        public bool assignTarget = false;
        [Tooltip("If the what we are looking for is found then also start moving")]
        public bool moveToTarget = false;

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

        public override bool CalculateResult()
        {
            return LookForMalbersTags(brain);
        }

        public bool LookForMalbersTags(AIBrain brain)
        {
            if (Tags.TagsHolders == null || tags == null || tags.Length == 0)
            {
                return false;
            }

            float minDistance = float.MaxValue;
            Transform closest = null;

            var filteredTags = Tags.GambeObjectbyTag(tags);
            if (filteredTags == null)
            {
                return false;
            }

            if (chooseRandomly)
            {
                while (filteredTags.Count != 0)
                {
                    int newIndex = Random.Range(0, filteredTags.Count);
                    var go = filteredTags[newIndex].transform;

                    if (go != null)
                    {
                        if (AIUtility.IsInFieldOfView(brain, go.position, lookAngle, lookRange, lookMultiplier, obstacleLayer, out _))
                        {
                            if (assignTarget)
                            {
                                brain.AIControl.SetTarget(go, moveToTarget);
                            }
                            return true;
                        }
                    }
                    filteredTags.RemoveAt(newIndex);
                }
            }
            else
            {
                foreach (var tagHolder in filteredTags)
                {
                    var go = tagHolder.transform;

                    if (go != null)
                    {
                        if (AIUtility.IsInFieldOfView(brain, go.position, lookAngle, lookRange, lookMultiplier, obstacleLayer, out float distance))
                        {
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closest = go;
                            }
                        }
                    }
                }
            }

            if (closest)
            {
                if (assignTarget)
                {
                    brain.AIControl.SetTarget(closest.transform, moveToTarget);
                }
                return true;
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
