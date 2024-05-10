using Malbers.Integration.AITree;
using MalbersAnimations;
using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Equip Weapon", "Animal Controller/Weapon/Equip Weapon", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MEquipWeapon : MTaskNode
    {
        [Header("Node")]
        [Tooltip("Play the mode only when the animal has arrived at the target.")]
        public bool near = false;


        [NotNull,Tooltip("The weapon to equip.")]
        public MWeapon Weapon;
        bool taskDone;

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
            taskDone = false;

            if (near && !AIBrain.AIControl.HasArrived)
            {
                return; // Don't play if 'Play on target' is true but we are not near the target.
            }

            if (AIBrain.weaponManager.WeaponIsActive == Weapon)
            {
                taskDone = true;
            }
            AIBrain.weaponManager.Equip_External(Weapon);
            taskDone = true;
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            return taskDone ? State.Success : State.Running;
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
            taskDone = false;
        }
    }
}