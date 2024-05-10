using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.HAP;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{

    [NodeContent("Dismount", "Animal Controller/Mount/Dismount")]
    public class MDismount : TaskNode
    {
        private AIBrain AIBrain;
        private Faction faction;
        private MRider mRider;
        private Mount mount;
        protected override void OnEntry()
        {
            faction = GetOwner().gameObject.GetComponent<Faction>();
            faction.inFormation = false;
            AIBrain = GetOwner().gameObject.GetComponent<AIBrain>();
            mRider = faction.mRider;
            faction.taken = false;
            if (mRider != null)
            {
                mount = mRider.Montura.gameObject.GetComponent<Mount>();
            }
            else
            {
                mount = GetOwner().transform.root.gameObject.GetComponentInChildren<Mount>();
            }

            if (mRider == null)
            {
                //Dismount called by the mount
                mRider = mount.Rider;
                mount.Rider.DismountAnimal();
                mount.Set_InputMount.Value = true;
                AIBrain.AIControl.SetActive(false);
                AIBrain.AIControl.SetTarget((Transform)null,false);
                GameObject cam = mount.transform.Find("On Mount/CM Mount Camera State").gameObject;
                cam.SetActive(true);
                Faction factionRider = mount.Rider.transform.gameObject.GetComponentInChildren<Faction>();
                if (faction.whenDismountingClearGroup || factionRider.whenDismountingClearGroup)
                {
                    faction.groupName = "";
                }
                mount.transform.root.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = true;
            }
            else
            {
                mRider.DismountAnimal();
                mRider.Montura.Set_InputMount.Value = true;
                GameObject cam = mRider.Montura.transform.Find("On Mount/CM Mount Camera State").gameObject;
                cam.SetActive(true);
                if (faction.whenDismountingClearGroup)
                {
                    Faction aiMontura = mRider.Montura.transform.root.gameObject.GetComponent<Faction>();
                    aiMontura.groupName = "";
                }
                mRider.Montura.transform.root.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = true;
            }
        }
        protected override State OnUpdate()
        {

            if (!mRider.IsRiding)
            {
                return State.Success;
            }
           else
            {
                return State.Running;
            }
        }
    }
}

