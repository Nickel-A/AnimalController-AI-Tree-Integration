using Malbers.Integration.AITree;
using RenownedGames.AITree;

[NodeContent("Rotate In Place", "Animal Controller/ACMovement/Rotate In Place", IconPath = "Icons/AnimalAI_Icon.png")]
public class MRotateInPlace : MTaskNode
{
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
        AIBrain.AIControl.RemainingDistance = 0;
        AIBrain.AIControl.DestinationPosition = AIBrain.AIControl.Transform.position;//Set yourself as the Destination Pos
        AIBrain.AIControl.LookAtTargetOnArrival = true;          //Set the Animal to look Forward to the Target
        AIBrain.AIControl.UpdateDestinationPosition = false;          //Set the Animal to look Forward to the Target
        AIBrain.AIControl.HasArrived = true;      //Set the Stopping Distance to almost nothing that way the animal keeps trying to go towards the target
        AIBrain.AIControl.Stop();
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