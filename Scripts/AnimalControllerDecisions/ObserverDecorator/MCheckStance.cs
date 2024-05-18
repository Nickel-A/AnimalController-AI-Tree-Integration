using MalbersAnimations;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using System;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Check Stance", "Animal Controller/MObserverDecorator/Check Stance", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckStance : ObserverDecorator
    {
        public enum Affected { Self, Target, Leader };
        [Header("Node")]
        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected check = Affected.Self;
        [Tooltip("Identifier for the stance")]
        public StanceID stanceID;

        [Tooltip("Check if the State is Entering or Exiting")]
        public EEnterExit when = EEnterExit.Enter;

        [Tooltip("If true, inverts the result of the check")]
        public bool invertResult = false;

        private bool checkResult;
        private Faction faction;
        AIBrain AIBrain;

        public override event Action OnValueChange;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AIBrain = GetOwner().GetComponent<AIBrain>();
            faction = GetOwner().gameObject.GetComponent<Faction>();
        }


        /// <summary>
        /// Called every tick regardless of the node execution.
        /// </summary>
        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }
        public override bool CalculateResult()
        {
            bool result = CalculateStateResult();

            if (invertResult)
            {
                result = !result;
            }

            return result;
        }

        private bool CalculateStateResult()
        {
            switch (check)
            {
                case Affected.Self:
                    if (AIBrain != null && AIBrain.Animal != null)
                    {
                        return CheckState(AIBrain.Animal);
                    }
                    else
                    {
                        return false;
                    }
                case Affected.Target:
                    if (AIBrain != null && AIBrain.TargetAnimal != null)
                    {
                        return CheckState(AIBrain.TargetAnimal);
                    }
                    else
                    {
                        return false;
                    }
                case Affected.Leader:
                    if (faction != null)
                    {
                        return CheckState(faction.FindLeader(faction.groupName).GetComponent<MAnimal>());
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }
        private bool CheckState(MAnimal animal)
        {
            switch (when)
            {
                case EEnterExit.Enter:
                    checkResult = animal.Stance == stanceID.ID;
                    return checkResult;
                case EEnterExit.Exit:
                    checkResult = animal.LastStanceID == stanceID.ID;
                    return checkResult;
                default:
                    checkResult = false;
                    return false;
            }
        }

        public override string GetDescription()
        {
            string description = $"Enter or Exit: {when} \n";
            if (stanceID != null)
            {
                description += $"Stance ID: {stanceID.DisplayName} \n";
            }
            description += $"Result: {checkResult} \n";

            return description;
        }
    }
}