using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using UnityEngine;
using MalbersAnimations.HAP;

namespace Malbers.Integration.AITree
{

    [NodeContent("FollowLeader", "Animal Controller/Follow Leader", IconPath = "Icons/AIDecision_Icon.png")]
    public class MFollowLeader : TaskNode
    {


        private Faction faction;
        private GameObject leader;
        private AIBrain aiBrain;

        public bool inFormation;
        public bool stopFollowing;
        public float stoppingDistance=1;
        public float additiveStopDistance = 0;
        protected override void OnEntry()
        {
            faction = GetOwner().gameObject.GetComponent<Faction>();
            aiBrain = GetOwner().gameObject.GetComponent<AIBrain>();
            leader = faction.FindLeader(faction.groupName);
            if (stopFollowing)
            {
                faction.followingLeader = false;
                faction.inFormation = false;
                aiBrain.AIControl.Target = null;
            }
            else
            {
                aiBrain.AIControl.StoppingDistance = stoppingDistance;
                aiBrain.AIControl.AdditiveStopDistance = additiveStopDistance;
                faction.followingLeader = true;
                if (inFormation && faction.inFormation == false)
                {
                    faction.inFormation = true;
                    leader.GetComponent<Faction>().SetFormation();
                }
                else if (!inFormation)
                {
                    if (leader.GetComponent<MRider>().IsRiding)
                    {
                        aiBrain.AIControl.SetTarget(leader.GetComponent<MRider>().Montura.Animal.transform, true);
                    }
                    else
                    {
                        aiBrain.AIControl.SetTarget(leader.transform, true);
                    }
                }
            }
        }
        // Override the Evaluate method or else your environment will throw an error
        protected override RenownedGames.AITree.State OnUpdate()
        {
            if (leader != null)
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
