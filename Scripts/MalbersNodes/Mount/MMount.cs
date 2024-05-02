using MalbersAnimations.HAP;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{

    [NodeContent("Mount", "Tasks/Faction/Mount")]
    public class MMount : TaskNode
    {
        private Faction faction;
        private MRider mRider;
        private Mount mount;
        private AIBrain aiBrain;
        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
            faction = GetOwner().gameObject.GetComponent<Faction>();
            mRider =  faction.mRider;
            if (mRider.Montura != null)
            {
                mount = mRider.Montura.gameObject.GetComponent<Mount>();
                GameObject cam = mRider.Montura.transform.Find("On Mount/CM Mount Camera State").gameObject;
                cam.SetActive(false);
                mount.Set_InputMount.Value = false;
                mRider.MountAnimal();
            }

        }

        protected override State OnUpdate()
        {
            if (mRider.Mounted)
            {
                if (faction.whenMountingAssignGroup)
                {
                    Faction aiMontura = mRider.Montura.transform.parent.gameObject.GetComponentInChildren<Faction>();
                    aiMontura.groupName = faction.groupName;
                }
                if (faction.followingLeader)
                {
                    faction.followingLeader = false;
                }
                aiBrain.AIControl.SetTarget((Transform)null, false);
                mRider.Montura.transform.root.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }
    }
}

