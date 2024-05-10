using MalbersAnimations;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Unequip Weapon", "Animal Controller/Weapon/Unequip Weapon", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MUnequipWeapon : MTaskNode
    {
        [Header("Node")]
        [Tooltip("Play the mode only when the animal has arrived at the target.")]
        public bool near = false;

        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            base.OnEntry();

            if (near && !AIBrain.AIControl.HasArrived)
            {
                return; // Don't play if 'Play on target' is true but we are not near the target.
            }
            AIBrain.weaponManager.UnEquip();
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            return State.Success;
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
        }
    }
}