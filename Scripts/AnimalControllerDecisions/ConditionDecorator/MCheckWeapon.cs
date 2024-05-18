using Malbers.Integration.AITree;
using MalbersAnimations;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("CheckWeapon", "Animal Controller/MConditionDecorator/CheckWeapon", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckWeapon : MConditionDecorator
    {
        [Header("Node")]
        [Tooltip("The holster ID to use for drawing or holstering the weapon.")]
        public HolsterID HolsterID;

        /// <summary>
        /// Calculates the result of the condition.
        /// </summary>
        protected override bool CalculateResult()
        {
            if (AIBrain.weaponManager.Weapon && AIBrain.weaponManager.ActiveHolster == HolsterID)
            {
                return true;
            }
            else return false;
        }
    }
}