using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Weapon", "Animal Controller/Weapon", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MWeaponNode : TaskNode
    {
        public enum BrainWeaponActions { Draw_Holster, Store_Weapon, Equip_Weapon, Unequip_Weapon, Aim, Attack, Reload }

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

        bool taskDone;
        AIBrain aiBrain;
        MWeaponManager WeaponManager;
        ComboManager comboManager;

        [Hide("Actions", (int)BrainWeaponActions.Attack)]
        [Tooltip("Set to true to use the combo manager for attacks.")]
        public bool useComboManager = false;

        [Hide("Actions", (int)BrainWeaponActions.Attack)]
        [Tooltip("Specify the branch number for combo attacks.")]
        public int branchNumber;


        protected override void OnInitialize()
        {
            base.OnInitialize(); 
            aiBrain = GetOwner().GetComponent<AIBrain>();

            WeaponManager = aiBrain.GetComponentInParent<MWeaponManager>();
            comboManager = aiBrain.GetComponentInParent<ComboManager>(); 
        }

        protected override void OnEntry()
        {

            if (near && !aiBrain.AIControl.HasArrived)
            {
                return; // Don't play if 'Play on target' is true but we are not near the target.
            }

            if (WeaponManager)
            {
                switch (Actions)
                {
                    case BrainWeaponActions.Equip_Weapon:
                        if (WeaponManager.WeaponIsActive == Weapon)
                        {
                            taskDone = true;
                            break;
                        }
                        WeaponManager.Equip_External(Weapon);
                        taskDone = true;
                        break;
                    case BrainWeaponActions.Draw_Holster:
                        if (WeaponManager.Weapon && WeaponManager.Weapon.HolsterID == HolsterID)
                        {
                            taskDone = true;
                            break;
                        }
                        WeaponManager.UnEquip_Fast();
                        WeaponManager.IgnoreDraw = IgnoreDrawStore;
                        WeaponManager.Holster_Equip(HolsterID);
                        taskDone = true;
                        break;
                    case BrainWeaponActions.Aim:
                        WeaponManager.Aim_Set(AimValue);
                        break;
                    case BrainWeaponActions.Attack:
                        break;
                    case BrainWeaponActions.Store_Weapon:
                        if (WeaponManager.Weapon == null)
                        {
                            taskDone = true;
                        }
                        WeaponManager.IgnoreStore = IgnoreDrawStore;
                        WeaponManager.Aim_Set(false);
                        WeaponManager.Store_Weapon();
                        taskDone = true;
                        break;
                    case BrainWeaponActions.Reload:
                        if (WeaponManager.Weapon as MShootable)
                        {
                            WeaponManager.ReloadWeapon();
                        }
                        break;
                    case BrainWeaponActions.Unequip_Weapon:
                        if (WeaponManager.Weapon == null)
                        {
                            taskDone = true;
                        }
                        WeaponManager.UnEquip();
                        taskDone = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                taskDone = true;
                Debug.Log("No Weapon Manager Found on the Animal", aiBrain.Animal);
            }
        }

        protected override State OnUpdate()
        {
            if (near && !aiBrain.AIControl.HasArrived)
            {
                if (Actions != BrainWeaponActions.Attack)
                {
                    return State.Failure; // Don't play if 'Play on target' is true but we are not near the target.
                }
            }

            if (WeaponManager)
            {
                switch (Actions)
                {
                    case BrainWeaponActions.Draw_Holster:
                        if (WeaponManager.DrawWeapon && WeaponManager.WeaponAction == Weapon_Action.None)
                        {
                            WeaponManager.UnEquip_Fast();
                            WeaponManager.IgnoreDraw = IgnoreDrawStore;
                            WeaponManager.Holster_Equip(HolsterID);
                        }

                        if (WeaponManager.ActiveHolster == HolsterID && WeaponManager.WeaponAction == Weapon_Action.Idle)
                        {
                            taskDone = true;
                        }
                        break;
                    case BrainWeaponActions.Store_Weapon:
                        if (WeaponManager.WeaponAction == Weapon_Action.None)
                        {
                            taskDone = true;
                        }
                        break;
                    case BrainWeaponActions.Aim:
                        if (WeaponManager.Weapon)
                        {
                            taskDone = true;
                        }
                        break;
                    case BrainWeaponActions.Attack:
                        if (near && !aiBrain.AIControl.HasArrived)
                        {
                            WeaponManager.MainAttackReleased();
                            if (WeaponManager.Weapon) WeaponManager.Weapon.Input = false;
                            return State.Running;
                        }

                        if (WeaponManager.Weapon)
                        {
                            if (WeaponManager.Weapon is MMelee)
                            {
                                if (useComboManager)
                                {
                                    comboManager.Play(branchNumber);
                                    if (attackOnce && !comboManager.PlayingCombo)
                                    {
                                        taskDone = true;
                                    }
                                }
                                else
                                {
                                    WeaponManager.MainAttack();
                                    if (attackOnce)
                                    {
                                        taskDone = true;
                                    }
                                }
                            }
                            else
                            {
                                if (!WeaponManager.Weapon.Input || (WeaponManager.Weapon as MShootable).releaseProjectile == MShootable.Release_Projectile.OnAttackStart)
                                {
                                    WeaponManager.MainAttack();
                                    //taskDone = true;
                                }
                                else
                                {
                                    WeaponManager.MainAttackReleased();
                                    taskDone = true;
                                }
                            }
                        }
                        break;
                    case BrainWeaponActions.Reload:
                        if (WeaponManager.Weapon as MShootable)
                        {
                            if (!WeaponManager.IsReloading)
                            {
                                taskDone = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return taskDone || GetState().Equals(State.Aborted) ? State.Success : State.Running;//
        }

        protected override void OnExit()
        {
            base.OnExit();
            if (WeaponManager.Weapon)
            {
            WeaponManager.Weapon.Input = false;

            }
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
