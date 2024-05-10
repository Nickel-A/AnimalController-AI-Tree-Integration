//using MalbersAnimations.Controller.AI;
//using RenownedGames.AITree;
//using UnityEngine;

//namespace Malbers.Integration.AITree
//{

//    [NodeContent("New Task", "Animal Controller/New Task", IconPath = "Icons/AnimalAI_Icon.png")]
//    public class MMoveToGameObject : TaskNode
//    {
//        // public GameObject target;
//        private AIBrain AIBrain;
//        private MAnimalAIControl mAnimalAIControl;
//        public BlackboardEntrySelector<GameObject> target;
//        protected override void OnEntry()
//        {
//            AIBrain =  GetOwner().gameObject.GetComponent<AIBrain>();
//            mAnimalAIControl = AIBrain.mAnimalAIControl;
//            mAnimalAIControl.SetTarget(target.value);
//        }

//        protected override State OnUpdate()
//        {
//            if (target == null)
//            {
//                return State.Failure;
//            }

//            if (mAnimalAIControl.HasArrived)
//            {
//                return State.Success;
//            }
//            else
//            {
//                return State.Running;
//            }
//        }
//    }
//}

