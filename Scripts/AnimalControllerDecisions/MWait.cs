using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Wait", "Animal Controller/Wait", IconPath = "Icons/AIDecision_Icon.png")]
    public class MWait : ConditionDecorator
    {
        [Header("Node")]
        /// <summary>Range for Looking forward and Finding something</summary>
        public FloatReference WaitMinTime = new FloatReference(5);
        public FloatReference WaitMaxTime = new FloatReference(5);
        private float WaitTime;
        private bool chkresult;
        AIBrain aiBrain;

        protected override void OnInitialize()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
            //Store the time we want to wait on the Local Decision Float var
            aiBrain.DecisionsVars.floatValue = UnityEngine.Random.Range(WaitMinTime, WaitMaxTime);
        }
        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            WaitTime = aiBrain.DecisionsVars.floatValue;

            bool timepassed = MTools.ElapsedTime(aiBrain.StateLastTime, WaitTime);
            chkresult = timepassed;
            return timepassed;
        }

        public override string GetDescription()
        {
            string description = $"Wait Min Time: {WaitMinTime.Value} \n";
            description += $"Wait Max Time: {WaitMaxTime.Value} \n";
            description += $"Wait Time: {WaitTime} \n";
            description += $"Result: {chkresult} \n";
            return description;
        }
    }
}