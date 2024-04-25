
//using MalbersAnimations.HAP;
//using RenownedGames.AITree;
//using UnityEngine;

//namespace Malbers.Integration.AITree
//{

//    [NodeContent("New Task", "Animal Controller/New Task")]
//    public class MCheckLeaderDismounted : TaskNode
//    {
//        private AIBrain aiBrain;
//        private GameObject leader;
//        protected override void OnEntry()
//        {
//            aiBrain =  GetOwner().gameObject.GetComponent<AIBrain>();
//            leader = aiBrain.FindLeader(aiBrain.groupName);
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

