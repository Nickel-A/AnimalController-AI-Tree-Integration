using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;
using MalbersAnimations.HAP;
using MalbersAnimations.Controller.AI;

namespace Malbers.Integration.AITree
{
    [NodeContent("Change Speed", "Animal Controller/ACMovement/Change Speed", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MChangeSpeedNode : MTaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        private Faction faction;
        public string SpeedSet = "Ground";
        public IntReference SpeedIndex = new(3);
        public bool matchLeaderSpeed;
        private MAnimal targetAnimal;
        protected override void OnEntry()
        {
            faction = GetOwner().gameObject.GetComponent<Faction>();
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
                AIBrain.Animal.SetSprint(targetAnimal.Sprint);
                SpeedSet = targetAnimal.CurrentSpeedSet.name;
                SpeedIndex = targetAnimal.CurrentSpeedIndex;
                ChangeSpeed(AIBrain.Animal);
            }
            else
            {
                switch (affect)
                {
                    case Affected.Self: ChangeSpeed(AIBrain.Animal); break;
                    case Affected.Target: ChangeSpeed(AIBrain.TargetAnimal); break;
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