using MalbersAnimations;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using UnityEngine;
using MalbersAnimations.HAP;

namespace Malbers.Integration.AITree
{

    [NodeContent("Check Leader Mount", "Animal Controller/Faction/Check Leader Mount", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckLeaderIsMounted : ConditionDecorator
    {
        private Faction faction;
        private GameObject leader;
        [Space, Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected check = Affected.Self;
        public bool checkForMounted;
        private bool result;
        protected override void OnInitialize()
        {
            faction = GetOwner().gameObject.GetComponent<Faction>();
            leader = faction.FindLeader(faction.groupName);
        }
        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            if (leader.GetComponent<MRider>().IsRiding)
            {
                if (checkForMounted)
                {
                    result = true;
                    return true;
                }
                else
                {
                    result = false;
                    return false;
                }
            }
            else
            {
                if (checkForMounted)
                {
                    result = false;
                    return false;
                }
                else
                {
                    result = true;
                    return true;
                }
            }
        }
        public override string GetDescription()
        {
            string description = "";
            description += $"Result: {result}\n";

            return description;
        }
    }
}
