using Malbers.Integration.AITree;
using RenownedGames.AITree;
using UnityEngine;

[NodeContent("Lock Animal Movement", "Animal Controller/ACMovement/Lock Animal Movement", IconPath = "Icons/AnimalAI_Icon.png")]
public class MLockAnimalMovement : TaskNode
{
    [Header("Node")]

    AIBrain aiBrain;
    public bool lockMovement;
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
        base.OnEntry();
        aiBrain.Animal.LockMovement = lockMovement;
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