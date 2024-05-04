using MalbersAnimations;
using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Draw Holster", "Animal Controller/Weapon/Draw Holster", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MDrawHolster : TaskNode
    {
        [Header("Node")]
        [Tooltip("The holster ID to use for drawing or holstering the weapon.")]
        public HolsterID HolsterID;
        [Tooltip("Ignore draw and store weapon animations.")]
        public bool IgnoreDrawStore = false;
        [Tooltip("Play the mode only when the animal has arrived at the target.")]
        public bool near = false;
        bool taskDone;
        AIBrain aiBrain;
        MWeaponManager WeaponManager;

        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();
            WeaponManager = aiBrain.GetComponentInParent<MWeaponManager>();
        }

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            base.OnEntry();
            taskDone = false;

            if (WeaponManager)
            {
                if (near && !aiBrain.AIControl.HasArrived)
                {
                    return; // Don't play if 'Play on target' is true but we are not near the target.
                }

                WeaponManager.DrawWeapon = true;
                //if (WeaponManager.Weapon && WeaponManager.Weapon.HolsterID == HolsterID)
                //{
                //    // If already equiped
                //    taskDone = true;
                //}
                //else
                //{
                //    WeaponManager.UnEquip_Fast();
                //    WeaponManager.IgnoreDraw = IgnoreDrawStore;
                //    WeaponManager.Holster_Equip(HolsterID);
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
            if (WeaponManager.DrawWeapon && WeaponManager.WeaponAction == Weapon_Action.None)
            {
                WeaponManager.UnEquip_Fast();
                WeaponManager.IgnoreDraw = IgnoreDrawStore;
                WeaponManager.Holster_Equip(HolsterID);
            }

            if (WeaponManager.ActiveHolster == HolsterID && WeaponManager.WeaponAction == Weapon_Action.Idle)
            {
                // If already equiped
                taskDone = true;
            }

            return taskDone ? State.Success : State.Running;


        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
            WeaponManager.DrawWeapon = false;
            taskDone = false;
        }
    }
}