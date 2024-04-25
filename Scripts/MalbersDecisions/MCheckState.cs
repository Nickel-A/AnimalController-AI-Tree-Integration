using MalbersAnimations;
using MalbersAnimations.Controller;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System;
using UnityEditor;
using UnityEngine;

namespace Malbers.Integration.AITree
{

    [NodeContent("Check State", "Animal Controller/Check State", IconPath = "Icons/AIDecision_Icon.png")]


    public class MCheckState : ObserverDecorator
    {
        public override event Action OnValueChange;

        [Header("Node")]

        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected check = Affected.Self;

        [Tooltip("Identifier for the state")]
        public StateID StateID;

        [Tooltip("Check if the State is Entering or Exiting")]
        public EEnterExit when = EEnterExit.Enter;

        [Tooltip("Toggle to invert the result")]
        public bool invertResult = false;

        AIBrain aiBrain;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();
        }

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
                    if (aiBrain != null && aiBrain.Animal != null)
                    {
                        return CheckState(aiBrain.Animal);
                    }
                    else
                    {
                        return false;
                    }
                case Affected.Target:
                    if (aiBrain != null && aiBrain.TargetAnimal != null)
                    {
                        return CheckState(aiBrain.TargetAnimal);
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
                    return animal.ActiveStateID == StateID.ID;
                case EEnterExit.Exit:
                    return animal.LastState.ID == StateID.ID;
                default:
                    return false;
            }
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();

            string checkType = check == Affected.Self ? "Self" : "Target";
            string activeState = string.Empty;
            string lastState = string.Empty;

            if (aiBrain != null)
            {
                switch (check)
                {
                    case Affected.Self:
                        if (aiBrain.Animal != null)
                        {
                            activeState = aiBrain.Animal.ActiveStateID.DisplayName;
                            lastState = aiBrain.Animal.LastState.ID.DisplayName;
                        }
                        break;

                    case Affected.Target:
                        if (aiBrain.TargetAnimal != null)
                        {
                            activeState = aiBrain.TargetAnimal.ActiveStateID.DisplayName;
                            lastState = aiBrain.TargetAnimal.LastState.ID.DisplayName;
                        }
                        break;
                }

                description += $"\nCheck: {checkType}\n";
                description += $"StateID: {StateID.DisplayName}\n";
                if (when == EEnterExit.Exit)
                {
                    description += $"Last State ID: {lastState}\n";
                }
                else
                {
                    description += $"Active State ID: {activeState}\n";
                }
            }   
            return description;
        }
    }
}

