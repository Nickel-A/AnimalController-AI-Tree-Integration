
//using MalbersAnimations.HAP;
//using RenownedGames.AITree;
//using UnityEngine;

//namespace Malbers.Integration.AITree
//{

//    [NodeContent("New Task", "Animal Controller/New Task")]
//    public class MCheckLeaderDismounted : TaskNode
//    {
//        private AIBrain AIBrain;
//        private GameObject leader;
//        protected override void OnEntry()
//        {
//            AIBrain =  GetOwner().gameObject.GetComponent<AIBrain>();
//            leader = AIBrain.FindLeader(AIBrain.groupName);
//        }

//        protected override State OnUpdate()
//        {
//            if (!leader.GetComponent<MRider>().IsRiding)
//            {
//                return State.Success;
//            }
//            else
//            {
//                return State.Failure;
//            }

//        }
//    }
//}

