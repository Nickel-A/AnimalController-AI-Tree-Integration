using MalbersAnimations;
using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Draw Holster", "Animal Controller/Weapon/Draw Holster", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MDrawHolster : MTaskNode
    {
        [Header("Node")]
        [Tooltip("The holster ID to use for drawing or holstering the weapon.")]
        public HolsterID HolsterID;
        [Tooltip("Ignore draw and store weapon animations.")]
        public bool IgnoreDrawStore = false;
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

            if (AIBrain.weaponManager)
            {
                if (near && !AIBrain.AIControl.HasArrived)
                {
                    return; // Don't play if 'Play on target' is true but we are not near the target.
                }

                //AIBrain.weaponManager.DrawWeapon = true;
                //if (AIBrain.weaponManager.Weapon && AIBrain.weaponManager.Weapon.HolsterID == HolsterID)
                //{
                //    // If already equiped
                //    taskDone = true;
                //}
                //else
                //{
                //    AIBrain.weaponManager.UnEquip_Fast();
                //    AIBrain.weaponManager.IgnoreDraw = IgnoreDrawStore;
                //    AIBrain.weaponManager.Holster_Equip(HolsterID);
                //    taskDone = true;
                //}
            }
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (AIBrain.weaponManager.Weapon && AIBrain.weaponManager.ActiveHolster == HolsterID)
            {
                return State.Success;
            }
            else if (AIBrain.weaponManager.WeaponAction == Weapon_Action.None || AIBrain.weaponManager.WeaponAction == Weapon_Action.Idle || AIBrain.weaponManager.WeaponAction == Weapon_Action.Aim)
            {
                AIBrain.weaponManager.UnEquip_Fast();
                AIBrain.weaponManager.IgnoreDraw = IgnoreDrawStore;
                AIBrain.weaponManager.Holster_Equip(HolsterID);
                return State.Success;
            }

            return State.Running;


        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
            AIBrain.weaponManager.DrawWeapon = false;
        }
    }
}