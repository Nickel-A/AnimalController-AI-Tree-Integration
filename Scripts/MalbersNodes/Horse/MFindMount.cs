//using MalbersAnimations.HAP;
//using RenownedGames.AITree;
//using UnityEngine;

//namespace Malbers.Integration.AITree
//{

//    [NodeContent("New Task", "Animal Controller/New Task")]
//    public class MFindMount : TaskNode
//    {
//        public enum MountType
//        {
//            Horse,
//            Wolf,
//            Dragon,
//            Any
//        }
//        private AIBrain aiBrain;
//        public MountType mountType;
//        public float maxDistance;
//        public BlackboardEntrySelector<GameObject> gameObject;

//        private GameObject mount;
//        protected override void OnEntry()
//        {
//            aiBrain =  GetOwner().gameObject.GetComponent<AIBrain>();
//            //mount=aiBrain.FindMount(mountType.ToString(), maxDistance);
//            AIBrain[] schemaBrains = FindObjectsOfType<AIBrain>();
//            foreach (AIBrain aiBrain in schemaBrains)
//            {
//                if (!aiBrain.mountable || aiBrain.transform.root.GetComponentInChildren<Mount>().Rider != null)
//                {
//                    continue;
//                }

//                if (mountType.ToString() != "Any" && mountType.ToString() != this.aiBrain.mountType.ToString())
//                {
//                    continue;
//                }

//                if (Vector3.Distance( GetOwner().transform.position, aiBrain.transform.position) <= maxDistance)
//                {
//                    mount = aiBrain.gameObject;
//                }
//            }
//        }

//        protected override State OnUpdate()
//        {
//            if (mount != null)
//            {
//                gameObject.value = mount;
//                return State.Success;
//            }
//            else
//            {
//                return State.Failure;
//            }
//        }
//    }
//}

