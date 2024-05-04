using MalbersAnimations;
using RenownedGames.AITree;
using UnityEngine;
using UnityEngine.UIElements;

namespace Malbers.Integration.AITree
{
    [NodeContent("Aim", "Animal Controller/Weapon/Aim", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MAim : TaskNode
    {
        [Header("Node")]
        [Tooltip("Specifies whether to aim with the weapon.")]
        public bool AimValue = true;
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
            WeaponManager.Aim_Set(AimValue);
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