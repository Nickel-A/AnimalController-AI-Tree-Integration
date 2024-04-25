//using MalbersAnimations.Controller;
//using MalbersAnimations.Controller.AI;
//using MalbersAnimations.HAP;
//using RenownedGames.AITree;
//using UnityEngine;
//using State = RenownedGames.AITree.State;

//namespace Malbers.Integration.AITree
//{

//    [NodeContent("New Task", "Animal Controller/New Task")]
//    public class MDismount : TaskNode
//    {
//        private AIBrain aiBrain;
//        private MRider mRider;
//        private Mount mount;
//        private MAnimalAIControl mAnimalAIControl;
//        public string newTree;
//        protected override void OnEntry()
//        {
//            aiBrain = GetOwner().gameObject.GetComponent<AIBrain>();
//            mRider = GetOwner().gameObject.transform.root.GetComponentInChildren<MRider>();
//            mAnimalAIControl = aiBrain.mAnimalAIControl;
//            if (mRider != null)
//            {
//                mount = mRider.Montura.gameObject.GetComponent<Mount>();
//            }
//            else
//            {
//                mount = GetOwner().transform.root.gameObject.GetComponentInChildren<Mount>();

//            }

//            if (mRider == null)
//            {
//                //Dismount called by the mount
//                mRider = mount.Rider;
//                mount.Rider.DismountAnimal();
//                mount.Set_InputMount.Value = true;
//                mAnimalAIControl.SetActive(false);
//                GameObject cam = mount.transform.Find("On Mount/CM Mount Camera State").gameObject;
//                cam.SetActive(true);
//                AIBrain aiRider = mount.Rider.transform.root.gameObject.GetComponent<AIBrain>();
//                if (aiBrain.whenDismountingClearGroup || aiRider.whenDismountingClearGroup)
//                {
//                    aiBrain.groupName = "";
//                }
//                if (newTree != "")
//                {
//                    aiBrain.ChangeTree(mount.Rider.gameObject, newTree, false);
//                }
//            }
//            else
//            {
//                mRider.DismountAnimal();
//                mRider.Montura.Set_InputMount.Value = true;
//                GameObject cam = mRider.Montura.transform.Find("On Mount/CM Mount Camera State").gameObject;
//                cam.SetActive(true);
//                if (aiBrain.whenDismountingClearGroup)
//                {
//                    AIBrain aiMontura = mRider.Montura.transform.root.gameObject.GetComponent<AIBrain>();
//                    aiMontura.groupName = "";
//                }
//            }

//        }

//        protected override State OnUpdate()
//        {

//            if (!mRider.IsRiding)
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

