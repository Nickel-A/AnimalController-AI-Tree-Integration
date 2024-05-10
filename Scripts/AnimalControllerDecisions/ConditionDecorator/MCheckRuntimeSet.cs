using MalbersAnimations.Scriptables;
using MalbersAnimations;
using RenownedGames.AITree;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Check Runtime Set", "Animal Controller/MConditionDecorator/Check Runtime Set", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckRuntimeSet : MConditionDecorator
    {
        public enum CheckSetSize { Empty, Equal, Greater, Less }
        [Header("Node")]

        [RequiredField] public RuntimeGameObjects Set;

        [Tooltip("Check options of the Set")]
        public CheckSetSize CheckSize = CheckSetSize.Equal;

        [Tooltip("Size of the Current Set")]
        [Hide("CheckSize", true, 0)]
        public int Size = 0;
        private bool chkresult;
        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            chkresult= (CheckSize) switch
            {
                CheckSetSize.Empty => Set.IsEmpty,
                CheckSetSize.Equal => Set.Count == Size,
                CheckSetSize.Greater => Set.Count > Size,
                CheckSetSize.Less => Set.Count < Size,
                _ => false
            };
            return chkresult;

        }

        public override string GetDescription()
        {
            string description = "";
            if (Set !=null)
            {
                description += $"Set: {Set.name} \n";
            }
            description += $"Check Set Size: {CheckSize} \n";
            description += $"Size: {Size} \n";
            description += $"Result: {chkresult} \n";
            return description;
        }
    }
}