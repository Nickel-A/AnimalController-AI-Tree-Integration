using MalbersAnimations;
using RenownedGames.AITree;
using UnityEngine;
using RenownedGames.Apex;
namespace Malbers.Integration.AITree
{
    [NodeContent("Set Behaviour Tree", "Animal Controller/General/Set Behaviour Tree", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetBehaviourTree : MTaskNode
    {
        [Header("Node")]
        [Title("Blackboard")]
        [SerializeField]
        [KeyTypes(typeof(Transform), typeof(Vector3))]
        private Key key;

        public BehaviourTree behaviourTree;
        private Faction faction;

        protected override void OnEntry()
        {
            //behaviourRunner = affect.gameObject.GetComponent<BehaviourRunner>();
            if (behaviourTree != null)
            {
                if (key.TryGetTransform(out Transform transform))
                {
                    faction = transform.gameObject.GetComponentInChildren<Faction>();
                    faction.behaviourRunner.SetSharedBehaviourTree(behaviourTree);
                    faction.behaviourRunner.enabled = true;
                }
            }
            else
            {
                if (key.TryGetTransform(out Transform transform))
                {
                    faction = transform.gameObject.GetComponentInChildren<Faction>();
                    faction.behaviourRunner.enabled = false;
                    faction.behaviourRunner.SetSharedBehaviourTree((BehaviourTree)null);
                }
            }

        }

        protected override State OnUpdate()
        {
                return State.Success;
        }
    }
}