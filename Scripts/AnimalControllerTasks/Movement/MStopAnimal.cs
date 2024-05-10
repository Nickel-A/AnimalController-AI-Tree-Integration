using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{

    [NodeContent("Stop Animal", "Animal Controller/ACMovement/Stop Animal", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MStopAnimal : MTaskNode
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
            AIBrain.AIControl.Stop();
            AIBrain.AIControl.UpdateDestinationPosition = false;         //IMPORTANT or the animal will try to Move if the Target moves
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
}