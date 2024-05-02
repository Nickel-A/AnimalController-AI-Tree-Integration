using MalbersAnimations.Controller;
using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Random Chance", "Animal Controller/Random Chance", IconPath = "Icons/AIDecision_Icon.png")]
    public class MRandomChance : ConditionDecorator
    {
        [Title("Decorator")]
        [Tooltip("Chance at which the decision will apply")]
        public FloatKey chance;


        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            return Random.value < chance.GetValue(); 
        }
    }
}