using MalbersAnimations;
using RenownedGames.AITree;




namespace Malbers.Integration.AITree
{
    [NodeContent("Check AI State", "Animal Controller/MObserverDecorator/Check AI State", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckAIState : MConditionDecorator
    {
        public AIStateID AIStateToCheck;


        /// <summary>
        /// Calculates the result of the condition.
        /// </summary>
        protected override bool CalculateResult()
        {
            bool result = AIBrain.CurrentAIState == AIStateToCheck;

            return result;

        }
    }
}