using MalbersAnimations;
using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Store Weapon", "Animal Controller/Weapon/Store Weapon", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MStoreWeapon : TaskNode
    {
        [Header("Node")]
        bool taskDone;
        AIBrain aiBrain;
        MWeaponManager WeaponManager;
        [Tooltip("Ignore draw and store weapon animations.")]
        public bool IgnoreDrawStore = false;

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

            WeaponManager.IgnoreStore = IgnoreDrawStore;
            WeaponManager.Aim_Set(false);
            WeaponManager.Store_Weapon();
            taskDone = true;
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (WeaponManager.WeaponAction == Weapon_Action.None)
            {
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
            taskDone = false;
        }
    }
}