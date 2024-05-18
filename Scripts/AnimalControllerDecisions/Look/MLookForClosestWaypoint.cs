using Malbers.Integration.AITree;
using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For Closest Waypoint", "Animal Controller/Look/Look For Closest Waypoint", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForClosestWaypoint : ObserverDecorator
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
            return LookForClosestWaypoint();
        }

        public bool LookForClosestWaypoint()
        {
            var allWaypoints = MWayPoint.WayPoints;
            if (allWaypoints == null || allWaypoints.Count == 0)
            {
                return false;  //There's no waypoints  around here
            }

            float minDistance = float.MaxValue;

            MWayPoint closestWayPoint = null;

            foreach (var way in allWaypoints)
            {
                var center = way.GetCenterY();
                if (AIUtility.IsInFieldOfView(AIBrain, center,lookAngle,lookRange,lookMultiplier,obstacleLayer, out float Distance))
                {
                    if (Distance < minDistance)
                    {
                        minDistance = Distance;
                        closestWayPoint = way;
                    }
                }
            }

            if (closestWayPoint)
            {
                if (assignTarget)
                {
                    AIBrain.AIControl.SetTarget(closestWayPoint.transform, moveToTarget);
                }
                return true; //Find if is inside the Field of view
            }
            return false;
        }
#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            AIUtility.DrawFieldOfViewGizmos(AIBrain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}