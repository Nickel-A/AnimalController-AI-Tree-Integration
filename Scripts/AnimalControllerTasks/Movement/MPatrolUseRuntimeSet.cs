using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Patrol Use RuntimeSet", "Animal Controller/ACMovement/Patrol Use RuntimeSet", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MPatrolUseRuntimeSet : MTaskNode
    {

        [Tooltip("The Animal will Rotate/Look at the Target when he arrives to it")]
        public bool LookAtOnArrival = false;

        [Tooltip("Use a Runtime GameObjects Set to find the Next waypoint")]
        public RuntimeGameObjects RuntimeSet;
        [Label("Get")]public RuntimeSetTypeGameObject rtype = RuntimeSetTypeGameObject.Random;
        [HideInInspector] public IntReference RTIndex = new();
        [HideInInspector] public StringReference RTName = new();

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

            AIBrain.AIControl.AutoNextTarget = true; //When Patrolling make sure AutoTarget is set to true... 

            if (RuntimeSet != null)                                             //If we had a last Waypoint then move to it
            {
                AIBrain.TargetAnimal = null;                                      //Clean the Animal Target in case it was one
                GameObject go = RuntimeSet.GetItem(rtype, RTIndex, RTName, AIBrain.Animal.gameObject);
                if (go)
                {
                    AIBrain.AIControl.SetTarget(go.transform, true);
                }
            }
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            OnTargetArrived();

            if (AIBrain.AIControl.HasArrived)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
        }

        void OnTargetArrived()
        {
            AIBrain.AIControl.AutoNextTarget = true; //When Patrolling make sure AutoTarget is set to true... 

            GameObject NextTarget = RuntimeSet.GetItem(rtype, RTIndex, RTName, AIBrain.Animal.gameObject);
            if (NextTarget && AIBrain.AIControl.NextTarget == null)
            {
                AIBrain.AIControl.SetNextTarget(NextTarget);
                AIBrain.AIControl.MovetoNextTarget();
            }

        }
    }
}
