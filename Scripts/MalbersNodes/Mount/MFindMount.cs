using MalbersAnimations.HAP;
using RenownedGames.AITree;
using UnityEngine;
using RenownedGames.Apex;

namespace Malbers.Integration.AITree
{

    [NodeContent("Find Mount", "Tasks/Mount/Find Mount")]
    public class MFindMount : TaskNode
    {
        public enum MountType
        {
            Horse,
            Wolf,
            Dragon,
            Any
        }
        private Faction faction;
        public MountType mountType;
        public float maxDistance;
        private GameObject mount;
        private GameObject tmpmount;
        private float distance;
        private float tmpdistance;
        private Faction tmpfaction;
        [Title("Blackboard")]
        [SerializeField]
        [HideSelfKey]
        [NonLocal]
        private TransformKey storeResult;

        private void FindClosest()
        {
            distance = 100000f;
            faction = GetOwner().gameObject.GetComponent<Faction>();
            faction.inFormation = false;
            Faction[] factions = FindObjectsOfType<Faction>();
            foreach (Faction faction in factions)
            {
                if (mount == null)
                {
                    if (!faction.mountable || faction.transform.root.GetComponentInChildren<Mount>().Rider != null)
                    {
                        continue;
                    }

                    if (mountType.ToString() != "Any" && mountType.ToString() != this.faction.mountType.ToString())
                    {
                        continue;
                    }

                    tmpdistance = Vector3.Distance(GetOwner().transform.position, faction.transform.position);
                    if (tmpdistance <= maxDistance && tmpdistance < distance)
                    {
                        tmpmount = faction.gameObject;
                        distance = tmpdistance;
                        tmpfaction = faction;
                    }
                }
            }
        }

        protected override void OnEntry()
        {
            FindClosest();
            if (tmpmount != null)
            {
                if (tmpfaction.taken == false)
                {
                    tmpfaction.taken = true;
                    mount = tmpmount;
                    storeResult.SetValue(mount.transform);
                }
            }
        }

        protected override State OnUpdate()
        {
            if (mount != null)
            {
                
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }
    }
}

