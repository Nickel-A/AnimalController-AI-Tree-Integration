using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;


namespace Malbers.Integration.AITree
{
    [NodeContent("Change Speed", "Animal Controller/Change Speed", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MChangeSpeedNode : TaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        public string SpeedSet = "Ground";
        public IntReference SpeedIndex = new(3);
        private AIBrain aiBrain;
        public bool matchTargetSpeed;
        private MAnimal targetAnimal;
        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
            if (matchTargetSpeed)
            {
                if (aiBrain.AIControl.Target != null) targetAnimal = aiBrain.AIControl.Target.GetComponent<MAnimal>();
            }
        }

        protected override State OnUpdate()
        {
            if (matchTargetSpeed)
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
            description += $"Match Target Speed: {matchTargetSpeed}\n";
            return description;
        }
    }
}