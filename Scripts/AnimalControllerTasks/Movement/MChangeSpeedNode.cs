using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;
using MalbersAnimations.HAP;

namespace Malbers.Integration.AITree
{
    [NodeContent("Change Speed", "Animal Controller/Change Speed", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MChangeSpeedNode : TaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        private Faction faction;
        public string SpeedSet = "Ground";
        public IntReference SpeedIndex = new(3);
        private AIBrain aiBrain;
        public bool matchLeaderSpeed;
        private MAnimal targetAnimal;
        protected override void OnEntry()
        {
            faction = GetOwner().gameObject.GetComponent<Faction>();
            aiBrain = GetOwner().GetComponent<AIBrain>();
            if (matchLeaderSpeed)
            {
                targetAnimal = faction.FindLeaderAnimal(faction.groupName) ;
                if (targetAnimal.GetComponent<MRider>().IsRiding)
                {
                    targetAnimal = targetAnimal.GetComponent<MRider>().Montura.Animal;
                }
            }
        }

        protected override State OnUpdate()
        {
            if (matchLeaderSpeed)
            {
                aiBrain.Animal.SetSprint(targetAnimal.Sprint);
                SpeedSet = targetAnimal.CurrentSpeedSet.name;
                SpeedIndex = targetAnimal.CurrentSpeedIndex;
                ChangeSpeed(aiBrain.Animal);
            }
            else
            {
                switch (affect)
                {
                    case Affected.Self: ChangeSpeed(aiBrain.Animal); break;
                    case Affected.Target: ChangeSpeed(aiBrain.TargetAnimal); break;
                }
            }

            return State.Success;
        }

        public void ChangeSpeed(MAnimal animal) => animal?.SpeedSet_Set_Active(SpeedSet, SpeedIndex);

        public override string GetDescription()
        {
            string description = base.GetDescription();

            string checkType;
            if (affect == Affected.Self)
            {
                checkType = "Self";
            }
            else
            {
                checkType = "Target";
            }
            description += $"Affect: {checkType}\n";
            description += $"SpeedSet: {SpeedSet}\n";
            description += $"SpeedIndex: {SpeedIndex.Value}\n";
            description += $"Match Leader Speed: {matchLeaderSpeed}\n";
            return description;
        }
    }
}