using MalbersAnimations;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using UnityEngine;
using MalbersAnimations.HAP;

namespace Malbers.Integration.AITree
{

    [NodeContent("Find Leader", "Animal Controller/Faction/FindLeader", IconPath = "Icons/AIDecision_Icon.png")]
    public class MFindLeader : TaskNode
    {
        private Faction faction;
        public TransformKey leader;
        public bool findMountOfLeader;
        public bool flagAsFollowLeader;

        protected override void OnInitialize()
        {
            faction = GetOwner().gameObject.GetComponent<Faction>();
            if (findMountOfLeader)
            {
                leader.SetValue(faction.FindLeader(faction.groupName).GetComponent<MRider>().Montura.Animal.gameObject.transform);
            }else
            {
                leader.SetValue(faction.FindLeader(faction.groupName).transform);
            }
            faction.followingLeader = false;
            if (flagAsFollowLeader)
            {
                faction.followingLeader = true;
            }
        }
        // Override the Evaluate method or else your environment will throw an error
        protected override RenownedGames.AITree.State OnUpdate()
        {
            if (leader.GetValue() != null)
            {
                return RenownedGames.AITree.State.Success;
            }
            else
            {
                return RenownedGames.AITree.State.Running;
            }

        }
    }
}
