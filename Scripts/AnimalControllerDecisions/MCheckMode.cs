using MalbersAnimations.Scriptables;
using MalbersAnimations;
using RenownedGames.AITree;
using UnityEngine;
using MalbersAnimations.Controller;
using System;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Mode", "Animal Controller/Check Mode", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckMode : ObserverDecorator
    {
        [Header("Node")]

        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected checkOn = Affected.Self;
        [Tooltip("Identifier for the mode")]
        public ModeID ModeID;
        [Tooltip("Which ability is playing in the Mode. If is set to less or equal to zero; then it will return true if the Mode Playing")]
        public IntReference Ability = new IntReference();
        [Tooltip("Check if the Mode is Entering or Exiting")]
        public EEnterExit ModeState = EEnterExit.Enter;
        [Tooltip("Toggle to invert the result")]
        public bool invertResult = false;
        AIBrain aiBrain;

        public override event Action OnValueChange;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();
        }

        /// <summary>
        /// Called every tick regardless of the node execution.
        /// </summary>
        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }

        // Override the Evaluate method or else your environment will throw an error
        public override bool CalculateResult()
        {
            if (aiBrain == null) return false;
            bool result = checkOn switch
            {
                Affected.Self => AnimalMode(aiBrain.Animal),
                Affected.Target => AnimalMode(aiBrain.TargetAnimal),
                _ => false,
            };

            if (invertResult)
            {
                result = !result;
            }

            return result;
        }

        private bool AnimalMode(MAnimal animal)
        {

            if (aiBrain == null) return false;
            return ModeState switch
            {
                EEnterExit.Enter => OnEnterMode(animal),
                EEnterExit.Exit => OnExitMode(animal),
                _ => false,
            };
        }

        private bool OnEnterMode(MAnimal animal)
        {
            if (animal == null) return false;
            if (animal.ActiveModeID == ModeID)
            {
                if (Ability <= 0)
                    return true; //Means that Is playing a random mode does not mater which one
                else
                    return Ability == (animal.ModeAbility % 1000); //Return if the Ability is playing 
            }
            return false;
        }

        private bool OnExitMode(MAnimal animal)
        {
            if (animal.LastModeID != 0)
            {
                animal.LastModeID = 0;
                animal.LastAbilityIndex = 0;
                return true;
            }
            return false;
        }

        public override string GetDescription()
        {
            string description = $"Enter or Exit: {ModeState} \n";
            if (ModeID != null)
            {
                description += $"Mode ID: {ModeID.DisplayName} \n";
            }
            description += $"Ability: {Ability.Value} \n";

            return description;
        }
    }
}
