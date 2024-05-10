using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("PickUp Drop", "Animal Controller/Animal/PickUp Drop", IconPath = "Icons/AnimalAI_Icon.png")]

    public class MPickUpDropNode : MTaskNode
    {
        [Header("Node")]
        [Message("Pick up or drop the Pickable")]
        public bool pickUp;


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




            if (pickUp)
            {
                AIBrain.pickUpDrop.TryPickUp();
            }
            else
            {
                AIBrain.pickUpDrop.TryDrop();
            }
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