using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Wait", "Animal Controller/MConditionDecorator/Wait", IconPath = "Icons/AIDecision_Icon.png")]
    public class MWait : MConditionDecorator
    {
        [Header("Node")]
        /// <summary>Range for Looking forward and Finding something</summary>
        public FloatReference WaitMinTime = new FloatReference(5);
        public FloatReference WaitMaxTime = new FloatReference(5);
        private float WaitTime;
        private float randomTime;
        private bool chkresult;
        public float StateLastTime { get; set; }
        protected override void OnInitialize()
        {
            //Store the time we want to wait on the Local Decision Float var
            randomTime = UnityEngine.Random.Range(WaitMinTime, WaitMaxTime);
        }
        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            WaitTime = randomTime;

            bool timepassed = MTools.ElapsedTime(StateLastTime, WaitTime);
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