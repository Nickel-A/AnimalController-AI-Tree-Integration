using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For Zones", "Animal Controller/Look/Look For Zones", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForZones : ObserverDecorator
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


        [Space(20),Tooltip("Search for all zones")]
        public bool AllZones = true;

        [HideIf("AllZones")]
        [Tooltip("Type of Zone we want to find")]
        public ZoneType zoneType;

        [HideIf("AllZones")]
        [HelpBox("If ID is (-1). It will search for any zone", Style = MessageStyle.None)]
        [Tooltip("ID value of the Zone we want to find")]
        [Min(-1)] public int ZoneID = -1;

        [ShowIf("zoneType", ZoneType.Mode)]
        [Tooltip("Mode Zone Index")]
        [HelpBox("Zone Index is (-1), it will search for any Ability on the Mode Zone", Style = MessageStyle.None)]
        [Min(-1)] public int ZoneIndex = -1;

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
            return LookForZones();
        }

        public bool LookForZones()
        {
            var zones = Zone.Zones;
            if (zones == null || zones.Count == 0)
            {
                return false;  //There's no zone around here
            }

            float minDistance = float.PositiveInfinity;

            Zone FoundZone = null;
            foreach (var zone in zones)
            {
                if (AllZones ||
                    (zone && zone.zoneType == zoneType &&                      //Check the same Zone Types
                    ZoneID == -1) ||                                             //Check First if its Any Zone
                    zone.ZoneID == ZoneID ||                                    //Check Zone has the same ID
                    zone.zoneType != ZoneType.Mode ||                           //Check if its not a Zone Mode
                    (zone.zoneType == ZoneType.Mode && ZoneIndex == -1) ||  //Check if it's a Zone Mode but the Ability its any
                    zone.ModeAbilityIndex == ZoneIndex)                   //Check if it's a Zone Mode AND Ability Match
                {
                    if (AIUtility.IsInFieldOfView(brain, zone.ZoneCollider.bounds.center, lookAngle, lookRange, lookMultiplier, obstacleLayer, out float Distance) && Distance < minDistance)
                    {
                        minDistance = Distance;
                        FoundZone = zone;
                    }
                }
            }

            if (FoundZone)
            {
                if (assignTarget)
                {
                    brain.AIControl.SetTarget(FoundZone.transform, moveToTarget);
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
