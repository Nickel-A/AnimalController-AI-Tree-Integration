using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Attack", "Animal Controller/Weapon/Attack", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MAttackNode : MTaskNode
    {
        [Header("Node")]
        [Tooltip("Play the mode only when the animal has arrived to the target")]
        public bool near = false;

        [Tooltip("Set to true to perform the attack once.")]
        public bool attackOnce;
        [Tooltip("Set to true to use the combo manager for attacks. If using with combomanager the branch will play once.")]
        public bool useComboManager = false;
        [ShowIf("useComboManager")]
        public bool useRandomBranch;

        private bool inspectorSwitcher;

        [ShowIf("inspectorSwitcher"), Tooltip("Specify the branch number for combo attacks.")]
        public int branchNumber;
        [ShowIf("useRandomBranch")]
        public int branchMinNumber;
        [ShowIf("useRandomBranch")]
        public int branchMaxNumber;

        private void OnValidate()
        {
            if (useComboManager == false)
            {
                useRandomBranch = false;
                inspectorSwitcher = false;
            }
            else
            {
                inspectorSwitcher = true;
            }

            if (useRandomBranch)
            {
                inspectorSwitcher = false;
            }
        }

        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (near)
            {
                AIBrain.AIControl.UpdateDestinationPosition = true;
                if (!AIBrain.AIControl.HasArrived)
                {
                    AIBrain.weaponManager.MainAttackReleased();
                    AIBrain.weaponManager.Weapon.Input = false;
                    return State.Failure;
                }
            }

            if (AIBrain.weaponManager.Weapon)
            {
                if (AIBrain.weaponManager.Weapon is MMelee)
                {

                    if (useComboManager)
                    {
                        if (!AIBrain.comboManager)
                        {
                            Debug.LogError("Combo Manager is not assigned to " + AIBrain.name);
                            return State.Failure;
                        }

                        if (useRandomBranch)
                        {
                            branchNumber = Random.Range(branchMinNumber, branchMaxNumber);
                        }
                        AIBrain.comboManager.Play(branchNumber);

                        if (attackOnce && !AIBrain.comboManager.PlayingCombo)
                        {
                            return State.Success;
                        }
                    }
                    else
                    {
                        AIBrain.weaponManager.MainAttack();
                        if (attackOnce)
                        {
                            AIBrain.weaponManager.MainAttackReleased();
                            return State.Success;
                        }
                    }
                }
                else
                {
                    if (!AIBrain.weaponManager.Weapon.Input || (AIBrain.weaponManager.Weapon as MShootable).releaseProjectile == MShootable.Release_Projectile.OnAttackStart)
                    {

                        AIBrain.weaponManager.MainAttack();
                        if (attackOnce)
                        {
                            return State.Success;                            
                        }
                    }
                    else
                    {
                        AIBrain.weaponManager.MainAttackReleased();
                        
                        return State.Success;
                    }
                }
            }
            
            return State.Running;
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            if(AIBrain.weaponManager.Weapon != null)
            AIBrain.weaponManager.Weapon.Input = false;
            base.OnExit();
        }
    }
}