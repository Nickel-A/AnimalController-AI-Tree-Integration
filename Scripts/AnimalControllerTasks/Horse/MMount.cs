//using MalbersAnimations.HAP;
//using RenownedGames.AITree;
//using UnityEngine;

//namespace Malbers.Integration.AITree
//{

//    [NodeContent("New Task", "Animal Controller/New Task")]
//    public class MMount : TaskNode
//    {
//        private AIBrain aiBrain;
//        private MRider mRider;
//        private Mount mount;
//        protected override void OnEntry()
//        {
//            aiBrain =  GetOwner().gameObject.GetComponent<AIBrain>();
//            mRider =  GetOwner().gameObject.transform.root.GetComponentInChildren<MRider>();
//            mount = mRider.Montura.gameObject.GetComponent<Mount>();
//            GameObject cam = mRider.Montura.transform.Find("On Mount/CM Mount Camera State").gameObject;
//            cam.SetActive(false);
//            mount.Set_InputMount.Value = false;
//            mRider.MountAnimal();
//            if (aiBrain.whenMountingAssignGroup)
//            {
//                AIBrain aiMontura = mRider.Montura.transform.root.gameObject.GetComponent<AIBrain>();
//                aiMontura.groupName = aiBrain.groupName;
//            }
//        }

//        protected override State OnUpdate()
//        {
//            if (mRider.IsRiding)
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

