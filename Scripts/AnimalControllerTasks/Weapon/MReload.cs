using MalbersAnimations;
using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{
    [NodeContent("Reload", "Animal Controller/Weapon/Reload", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MReload : TaskNode
    {
        bool taskDone;
        MWeaponManager WeaponManager;
        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            WeaponManager = GetOwner().GetComponentInParent<MWeaponManager>();
        }

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            base.OnEntry();
            taskDone = false;
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            WeaponManager.ReloadWeapon(); // Reloads if no ammo is in chamber
            if (!WeaponManager.IsReloading)
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