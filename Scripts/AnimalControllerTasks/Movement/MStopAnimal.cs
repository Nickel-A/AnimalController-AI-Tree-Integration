using Malbers.Integration.AITree;
using RenownedGames.AITree;
using UnityEngine;

[NodeContent("Stop Animal", "Animal Controller/ACMovement/Stop Animal", IconPath = "Icons/AnimalAI_Icon.png")]
public class MStopAnimal : TaskNode
{  
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
        base.OnEntry();
        aiBrain.AIControl.Stop();
        aiBrain.AIControl.UpdateDestinationPosition = false;         //IMPORTANT or the animal will try to Move if the Target moves
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