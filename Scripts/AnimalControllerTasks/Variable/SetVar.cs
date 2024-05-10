using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Var", "Animal Controller/Variable/Set Var", IconPath = "Icons/AnimalAI_Icon.png")]
    public class SetVar : TaskNode
    {
        [Header("Node")]
        public VarType varType;

        [ShowIf("varType", VarType.Float)]
        public FloatVar floatVariable;
        [ShowIf("varType", VarType.Float)]
        public float floatValueToSet;

        [ShowIf("varType", VarType.Int)]
        public IntVar intVariable;
        [ShowIf("varType", VarType.Int)]
        public int intValueToSet;

        [ShowIf("varType", VarType.Bool)]
        public BoolVar boolVariable;
        [ShowIf("varType", VarType.Bool)]
        public bool boolValueToSet;

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
            switch (varType)
            {
                case VarType.Int:
                    intVariable.Value = intValueToSet;
                    break;
                case VarType.Float:
                    floatVariable.Value = floatValueToSet;
                    break;
                case VarType.Bool:
                    boolVariable.Value = boolValueToSet;
                    break;
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