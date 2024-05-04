using MalbersAnimations;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using System.ComponentModel;
using UnityEngine;
using State = RenownedGames.AITree.State;


namespace Malbers.Integration.AITree
{
    [NodeContent("In Zone", "Animal Controller/Animal/In Zone", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MInZoneNode : TaskNode
    {
        [Header("Node")]
        [HelpBox]
        public string description = "Specifies whether to activate a Zone when the Animal(Self) or the Target(Target) is within that zone.";


        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        AIBrain aiBrain;
        bool Done;

        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
            DoTask(aiBrain);
        }

        protected override State OnUpdate()
        {
            DoTask(aiBrain);

            if (Done)
            {
                return State.Success;

            }
            else
            {
                return State.Running;
            }
        }


        private void DoTask(AIBrain aiBrain)
        {
            Done = false;

            switch (affect)
            {
                case Affected.Self: Done = InZone(aiBrain.Animal); break;
                case Affected.Target: Done = InZone(aiBrain.TargetAnimal); break;
            }
        }

        public bool InZone(MAnimal animal)
        {
            if (animal && animal.InZone)
            {
                animal.Zone.ActivateZone(animal);
                return true;
            }
            return false;
        }

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

            return description;
        }
    }
}

