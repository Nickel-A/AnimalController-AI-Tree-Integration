using MalbersAnimations;
using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;
using static MalbersAnimations.AIWanderArea;

namespace Malbers.Integration.AITree
{
    [NodeContent("Change Wander Area", "Animal Controller/ACMovement/Change Wander Area", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MChangeWanderArea : MTaskNode
    {
        AIWanderArea AIWanderArea;

        [Group("Node")]
        public WayPointType pointType = WayPointType.Ground;
        
        public float wanderAreaY;

        [Group("Node")]
        [Tooltip("Distance for AI driven animals to stop when arriving to this gameobject. When is set as the AI Target.")]
        [Min(0)] public float stoppingDistance = 1f;

        [Group("Node")]
        [Tooltip("Distance for AI driven animals to start slowing its speed when arriving to this gameobject. If its set to zero or lesser than the Stopping distance, the Slowing Movement Logic will be ignored")]
        [Min(0)] public float slowingDistance = 0;
        
        [Group("Node")]
        [MinMaxRange(0, 60), Tooltip("Waytime range to go to the next destination")]
        public RangedFloat m_WaitTime = new RangedFloat(1, 5);

        [Group("")]
        [Tooltip("Type of Area to wander")]
        public AreaType m_AreaType = AreaType.Circle;

        [Group("")]
        [ShowIf("m_AreaType", AreaType.Circle)]
        [Min(0)] public float radius = 5;

        [Group("")]
        [ShowIf("m_AreaType", AreaType.Box)]
        public Vector3 BoxArea = new Vector3(10, 1, 10);

        [Group("Node")]
        [Range(0, 1), Tooltip("Probability of keep wandering on this WayPoint Area")]
        public float WanderWeight = 1f;

        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            base.OnEntry();
            AIWanderArea = AIBrain.Target.GetComponent<AIWanderArea>();

            Vector3 newPosition = new Vector3(AIWanderArea.transform.position.x, 0, AIWanderArea.transform.position.z);
            newPosition.y = wanderAreaY;
            AIWanderArea.transform.position = newPosition;

            AIWanderArea.pointType = pointType;
            AIWanderArea.stoppingDistance = stoppingDistance;
            AIWanderArea.slowingDistance = slowingDistance;
            AIWanderArea.m_WaitTime = m_WaitTime;
            AIWanderArea.m_AreaType = m_AreaType;
            AIWanderArea.radius = radius;
            AIWanderArea.BoxArea = BoxArea;
            AIWanderArea.WanderWeight = WanderWeight;
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (AIWanderArea != null)
            {

                return State.Success;
            }
            else
            {
                Debug.Log("Target is not AI Wander Area");
                return State.Failure;
            }
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
        }
    }
}