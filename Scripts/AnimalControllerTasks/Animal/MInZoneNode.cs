using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using System.ComponentModel;
using UnityEngine;
using State = RenownedGames.AITree.State;


namespace Malbers.Integration.AITree
{
    [NodeContent("In Zone", "Animal Controller/Animal/In Zone", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MInZoneNode : MTaskNode
    {
        [Header("Node")]
        [HelpBox]
        public string description = "Specifies whether to activate a Zone when the Animal(Self) or the Target(Target) is within that zone.";

        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        bool done;

        protected override void OnEntry()
        {
            DoTask(AIBrain);
        }

        protected override State OnUpdate()
        {
            DoTask(AIBrain);

            if (done)
            {
                return State.Success;

            }
            else
            {
                return State.Running;
            }
        }


        private void DoTask(AIBrain AIBrain)
        {
            done = false;

            switch (affect)
            {
                case Affected.Self: done = InZone(AIBrain.Animal); break;
                case Affected.Target: done = InZone(AIBrain.TargetAnimal); break;
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

