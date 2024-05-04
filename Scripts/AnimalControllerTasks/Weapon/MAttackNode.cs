using MalbersAnimations;
using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Attack", "Animal Controller/Weapon/Attack", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MAttackNode : TaskNode
    {
        [Header("Node")]
        [Tooltip("Play the mode only when the animal has arrived to the target")]

        public bool near = false;
        public bool AimValue = true;
        bool taskDone;
        AIBrain aiBrain;
        MWeaponManager WeaponManager;
        [Tooltip("Set to true to perform the attack once.")]
        public bool attackOnce;
        [Tooltip("Set to true to use the combo manager for attacks.")]
        public bool useComboManager = false;
        [Tooltip("Specify the branch number for combo attacks.")]
        public int branchNumber;

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
            if (near && !aiBrain.AIControl.HasArrived)
            {
                return; //Dont play if Play on target is true but we are not near the target.
            }
            taskDone = false;
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (near && !aiBrain.AIControl.HasArrived)
            {
                WeaponManager.MainAttackReleased();
                WeaponManager.Weapon.Input = false;
                return State.Running;
            }

            if (WeaponManager.Weapon)
            {
                if (WeaponManager.Weapon is MMelee)
                {
                    if (useComboManager)
                    {
                        if (!aiBrain.comboManager)
                        {
                            Debug.LogError("Combo Manager is not assigned to " + aiBrain.name);
                            return State.Failure;
                        }

                        aiBrain.comboManager.Play(branchNumber);
                        if (attackOnce && !aiBrain.comboManager.PlayingCombo)
                        {
                            taskDone = true;
                        }
                    }
                    else
                    {
                        WeaponManager.MainAttack();
                        if (attackOnce)
                        {
                            WeaponManager.MainAttackReleased();
                            taskDone = true;
                        }
                    }
                }
                else
                {
                    if (!WeaponManager.Weapon.Input || (WeaponManager.Weapon as MShootable).releaseProjectile == MShootable.Release_Projectile.OnAttackStart)
                    {
                        WeaponManager.MainAttack();
                        if (attackOnce)
                        {
                            taskDone = true;
                        }
                    }
                    else
                    {
                        WeaponManager.MainAttackReleased();
                        taskDone = true;
                    }
                }
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