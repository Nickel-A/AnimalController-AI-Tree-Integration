using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Weapon", "Animal Controller/Weapon/Weapon", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MWeaponNode : MTaskNode
    {
        [Header("Node")]
        [Tooltip("Play the mode only when the animal has arrived at the target.")]
        public bool near = false;
        [Tooltip("Specify the action to perform with the weapon.")]
        public BrainWeaponActions Actions = BrainWeaponActions.Attack;

        [Hide("Actions", (int)BrainWeaponActions.Equip_Weapon)]
        [Tooltip("The weapon to equip.")]
        public MWeapon Weapon;
        [Hide("Actions", (int)BrainWeaponActions.Draw_Holster)]
        [Tooltip("The holster ID to use for drawing or holstering the weapon.")]
        public HolsterID HolsterID;
        [Hide("Actions", (int)BrainWeaponActions.Aim)]
        [Tooltip("Specifies whether to aim with the weapon.")]
        public bool AimValue = true;

        [Hide("Actions", (int)BrainWeaponActions.Draw_Holster, (int)BrainWeaponActions.Store_Weapon)]
        [Tooltip("Ignore draw and store weapon animations.")]
        public bool IgnoreDrawStore = false;

        [Hide("Actions", (int)BrainWeaponActions.Attack)]
        [Tooltip("Set to true to perform the attack once.")]
        public bool attackOnce;
        private bool taskDone;

        [Hide("Actions", (int)BrainWeaponActions.Attack)]
        [Tooltip("Set to true to use the combo manager for attacks.")]
        public bool useComboManager = false;

        [Hide("Actions", (int)BrainWeaponActions.Attack)]
        [Tooltip("Specify the branch number for combo attacks.")]
        public int branchNumber;


        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {

            if (near && !AIBrain.AIControl.HasArrived)
            {
                return; // Don't play if 'Play on target' is true but we are not near the target.
            }

            if (AIBrain.weaponManager)
            {
                switch (Actions)
                {
                    case BrainWeaponActions.Equip_Weapon:
                        if (AIBrain.weaponManager.WeaponIsActive == Weapon)
                        {
                            taskDone = true;
                            break;
                        }
                        AIBrain.weaponManager.Equip_External(Weapon);
                        taskDone = true;
                        break;
                    case BrainWeaponActions.Draw_Holster:
                        if (AIBrain.weaponManager.Weapon && AIBrain.weaponManager.Weapon.HolsterID == HolsterID)
                        {
                            taskDone = true;
                            break;
                        }
                        AIBrain.weaponManager.UnEquip_Fast();
                        AIBrain.weaponManager.IgnoreDraw = IgnoreDrawStore;
                        AIBrain.weaponManager.Holster_Equip(HolsterID);
                        taskDone = true;
                        break;
                    case BrainWeaponActions.Aim:
                        AIBrain.weaponManager.Aim_Set(AimValue);
                        break;
                    case BrainWeaponActions.Attack:
                        break;
                    case BrainWeaponActions.Store_Weapon:
                        AIBrain.weaponManager.IgnoreStore = IgnoreDrawStore;
                        AIBrain.weaponManager.Aim_Set(false);
                        AIBrain.weaponManager.Store_Weapon();
                        taskDone = true;
                        break;
                    case BrainWeaponActions.Reload:
                        if (AIBrain.weaponManager.Weapon as MShootable)
                        {
                            AIBrain.weaponManager.ReloadWeapon();
                        }
                        break;
                    case BrainWeaponActions.Unequip_Weapon:
                        AIBrain.weaponManager.UnEquip();
                        taskDone = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                taskDone = true;
                Debug.Log("No Weapon Manager Found on the Animal", AIBrain.Animal);
            }
        }

        protected override State OnUpdate()
        {
            if (near && !AIBrain.AIControl.HasArrived)
            {
                if (Actions != BrainWeaponActions.Attack)
                {
                    return State.Failure; // Don't play if 'Play on target' is true but we are not near the target.
                }
            }

            if (AIBrain.weaponManager)
            {
                switch (Actions)
                {
                    case BrainWeaponActions.Draw_Holster:
                        if (AIBrain.weaponManager.DrawWeapon && AIBrain.weaponManager.WeaponAction == Weapon_Action.None)
                        {
                            AIBrain.weaponManager.UnEquip_Fast();
                            AIBrain.weaponManager.IgnoreDraw = IgnoreDrawStore;
                            AIBrain.weaponManager.Holster_Equip(HolsterID);
                        }

                        if (AIBrain.weaponManager.ActiveHolster == HolsterID && AIBrain.weaponManager.WeaponAction == Weapon_Action.Idle)
                        {
                            taskDone = true;
                        }
                        break;
                    case BrainWeaponActions.Store_Weapon:
                        if (AIBrain.weaponManager.WeaponAction == Weapon_Action.None)
                        {
                            taskDone = true;
                        }
                        break;
                    case BrainWeaponActions.Aim:
                        if (AIBrain.weaponManager.Weapon)
                        {
                            taskDone = true;
                        }
                        break;
                    case BrainWeaponActions.Attack:
                        if (near && !AIBrain.AIControl.HasArrived)
                        {
                            AIBrain.weaponManager.MainAttackReleased();
                            AIBrain.weaponManager.Weapon.Input = false;
                            return State.Running;
                        }

                        if (AIBrain.weaponManager.Weapon)
                        {
                            if (AIBrain.weaponManager.Weapon is MMelee)
                            {
                                if (useComboManager)
                                {
                                    AIBrain.comboManager.Play(branchNumber);
                                    if (attackOnce && !AIBrain.comboManager.PlayingCombo)
                                    {
                                        taskDone = true;
                                    }
                                }
                                else
                                {
                                    AIBrain.weaponManager.MainAttack();
                                    if (attackOnce)
                                    {
                                        AIBrain.weaponManager.MainAttackReleased();
                                        taskDone = true;
                                    }
                                }
                            }
                            else
                            {
                                if (!AIBrain.weaponManager.Weapon.Input || (AIBrain.weaponManager.Weapon as MShootable).releaseProjectile == MShootable.Release_Projectile.OnAttackStart)
                                {
                                    AIBrain.weaponManager.MainAttack();
                                }
                                else
                                {
                                    AIBrain.weaponManager.MainAttackReleased();
                                }
                            }
                        }
                        break;
                    case BrainWeaponActions.Reload:
                        if (AIBrain.weaponManager.Weapon as MShootable)
                        {
                            if (!AIBrain.weaponManager.IsReloading)
                            {
                                taskDone = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //return taskDone || GetState().Equals(State.Aborted) ? State.Success : State.Running;
            return State.Success;
        }

        protected override void OnExit()
        {
            base.OnExit();
            taskDone = false;
        }



        public override string GetDescription()
        {
            string description = base.GetDescription();


            switch (Actions)
            {
                case BrainWeaponActions.Draw_Holster:
                    description += "Draw Holster\n";
                    description += $"Near target: {near}\n";
                    description += $"HolsterID: {HolsterID.DisplayName}\n";
                    description += $"Ignore draw store: {IgnoreDrawStore}\n";
                    break;
                case BrainWeaponActions.Store_Weapon:
                    description += "Store Weapon\n";
                    description += $"Near target: {near}\n";
                    description += $"Ignore draw store: {IgnoreDrawStore}\n";
                    break;
                case BrainWeaponActions.Equip_Weapon:
                    description += "Equip Weapon\n";
                    description += $"Near target: {near}\n";
                    description += $"{Weapon.WeaponType.DisplayName}\n";
                    break;
                case BrainWeaponActions.Unequip_Weapon:
                    description += "Unequip_Weapon\n";
                    description += $"Near target: {near}\n";
                    break;
                case BrainWeaponActions.Aim:
                    description += "Aim\n";
                    description += $"Near target: {near}\n";
                    description += $"Aim value: {AimValue}\n";
                    break;
                case BrainWeaponActions.Attack:
                    description += "Attack\n";
                    description += $"Near target: {near}\n";
                    break;
                case BrainWeaponActions.Reload:
                    description += "Reload\n";
                    description += $"Near target: {near}\n";
                    break;
            }
            return description;
        }


    }
}
