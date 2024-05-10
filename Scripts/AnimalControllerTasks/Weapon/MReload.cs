using MalbersAnimations;
using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{
    [NodeContent("Reload", "Animal Controller/Weapon/Reload", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MReload : MTaskNode
    {
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
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            AIBrain.weaponManager.ReloadWeapon(); // Reloads if no ammo is in chamber
            if (!AIBrain.weaponManager.IsReloading)
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