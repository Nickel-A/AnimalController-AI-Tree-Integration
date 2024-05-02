using Malbers.Integration.AITree;
using MalbersAnimations;
using MalbersAnimations.Weapons;
using RenownedGames.AITree;
using UnityEngine;

[NodeContent("Attack", "Animal Controller/Attack", IconPath = "Icons/AnimalAI_Icon.png")]
public class MAttackNode : TaskNode
{
    [Header("Node")]
    [Tooltip("Play the mode only when the animal has arrived to the target")]

    public bool near = false;
    public bool AimValue = true;
    bool taskDone = false;
    AIBrain aiBrain;

    /// <summary>
    /// Called on behaviour tree is awake.
    /// </summary>
    protected override void OnInitialize()
    {
        base.OnInitialize();
        aiBrain = GetOwner().GetComponent<AIBrain>();
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
        //if (near && !aiBrain.AIControl.HasArrived)
        //{
        //    return State.Failure;
        //}
        var WeaponManager = aiBrain.TasksVars.mono as MWeaponManager;

        if (near && !aiBrain.AIControl.HasArrived) //meaning the target has gone far
        {
            WeaponManager.MainAttackReleased();
            //WeaponManager.Weapon.Input = false;
            return State.Running;
        }

        if (WeaponManager.Weapon)
        {
            if (WeaponManager.Weapon is MMelee)
            {
                WeaponManager.MainAttack();
                taskDone = true;
            }
            else //
            {
                if (!WeaponManager.Weapon.Input ||
                    (WeaponManager.Weapon as MShootable).releaseProjectile == MShootable.Release_Projectile.OnAttackStart)
                {
                    WeaponManager.MainAttack();
                    taskDone = true;
                }
                else
                {
                    WeaponManager.MainAttackReleased();
                    taskDone = true;
                }
            }
        }


        //Debug.Log("ATTACK1");

        return taskDone ? State.Success : State.Running;

    }


}