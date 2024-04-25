//using MalbersAnimations.Controller;
//using MalbersAnimations.Controller.AI;
//using RenownedGames.AITree;
//using UnityEngine;
//using State = RenownedGames.AITree.State;

//namespace Malbers.Integration.AITree
//{
//    [NodeContent("New Task", "Animal Controller/New Task", IconPath = "Icons/AnimalAI_Icon.png")]
//    public class MFollowLeader : TaskNode
//    {
//        private MAnimalAIControl mAnimalAIControl;
//        private MAnimal mAnimal;
//        private AIBrain aiBrain;
//        private GameObject leader;
//        private MAnimal mAnimalLeader;

//        protected override void OnEntry()
//        {
//            aiBrain =  GetOwner().gameObject.GetComponent<AIBrain>();
//            mAnimal = aiBrain.mAnimal;
//            mAnimalAIControl = aiBrain.mAnimalAIControl;
//            leader = aiBrain.FindLeader(aiBrain.groupName);
//            mAnimalAIControl.SetTarget(leader.transform);
//            mAnimalLeader = leader.GetComponent<MAnimal>();
//        }

//        protected override State OnUpdate()
//        {
//            mAnimal.Sprint_Set(mAnimalLeader.Sprint);
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

