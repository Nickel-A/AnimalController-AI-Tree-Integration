using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System;
using UnityEditor;
using UnityEngine;

namespace Malbers.Integration.AITree
{

    [NodeContent("Check State", "Animal Controller/MObserverDecorator/Check State", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckState : MObserverDecorator
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

        protected override void OnInitialize()
        {
            base.OnInitialize();
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

            if (AIBrain != null)
            {
                switch (check)
                {
                    case Affected.Self:
                        if (AIBrain.Animal != null)
                        {
                            activeState = AIBrain.Animal.ActiveStateID.DisplayName;
                            lastState = AIBrain.Animal.LastState.ID.DisplayName;
                        }
                        break;

                    case Affected.Target:
                        if (AIBrain.TargetAnimal != null)
                        {
                            activeState = AIBrain.TargetAnimal.ActiveStateID.DisplayName;
                            lastState = AIBrain.TargetAnimal.LastState.ID.DisplayName;
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

