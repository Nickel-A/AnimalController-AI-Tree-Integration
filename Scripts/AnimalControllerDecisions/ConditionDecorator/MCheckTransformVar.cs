using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    public enum CompareTransformVar { IsNull, IsCurrentTarget, IsInRuntimeSet }

    [NodeContent("Check Transform Var", "Animal Controller/MConditionDecorator/Check Transform Var", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckTransformVar : MConditionDecorator
    {
        [Header("Node")]

        public TransformVar Var;
        public CompareTransformVar compare;
        [Hide("compare", 2)]
        public RuntimeGameObjects Set;
        private bool checkResult;

        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            if (Var)
            {
                switch (compare)
                {
                    case CompareTransformVar.IsNull:
                        checkResult = false;
                        return Var.Value == null;
                    case CompareTransformVar.IsCurrentTarget:
                        checkResult = true;
                        return Var.Value == AIBrain.Target;
                    case CompareTransformVar.IsInRuntimeSet:
                        if (Set)
                        {
                            checkResult = true;
                            return Set.items.Contains(Var.Value.gameObject);
                        }
                        else
                        {
                            checkResult = false;
                            return false;
                        }

                    default:
                        break;
                }
            }
            checkResult = false;
            return false;
        }

        public override string GetDescription()
        {
            string description = "";
            if (Var != null)
            {
                description += $"Var: {Var.name} \n";
            }
            description += $"Compare: {compare} \n";
            description += $"Result: {checkResult} \n";
            return description;
        }

        [SerializeField, HideInInspector] private bool showSet;
        private void OnValidate()
        {
            showSet = compare == CompareTransformVar.IsInRuntimeSet;

        }
    }
}