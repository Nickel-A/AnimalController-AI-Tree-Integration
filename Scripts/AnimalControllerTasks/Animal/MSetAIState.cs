using MalbersAnimations;
using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set AI State", "Animal Controller/Animal/Set AI State", IconPath = "Icons/AIDecision_Icon.png")]
    public class MSetAIState : MTaskNode
    {
        public AIStateID setAIState;

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
            AIBrain.SetAIState(setAIState);
            base.OnEntry();
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