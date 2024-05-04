using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Var Listener", "Animal Controller/Variable/Set Var Listener", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetVarListenerNode : TaskNode
    {

        public enum VarType
        {
            Bool,
            Int,
            Float
        }

        [Header("Node")]
        [Tooltip("Check the Variable Listener ID Value, when this value is Zero, the ID is ignored")]
        public IntReference ListenerID = 0;

        /// <summary>Range for Looking forward and Finding something</summary>
        [Space, Tooltip("Check the Decision on the Animal(Self) or the Target(Target), or on an object with a tag")]
        public Affected checkOn = Affected.Self;

        [Space,
            Tooltip("Check on the Target or Self if it has a Listener Variable Component <Int><Bool><Float> and compares it with the local variable)")]
        public VarType varType = VarType.Bool;


        [Hide("varType", (int)VarType.Bool)] public bool boolValue = true;
        [Hide("varType", (int)VarType.Int)] public int intValue = 0;
        [Hide("varType", (int)VarType.Float)] public float floatValue = 0f;

        AIBrain aiBrain;


        protected override void OnEntry()
        {

            aiBrain = GetOwner().GetComponent<AIBrain>();
            switch (checkOn)
            {
                case Affected.Self:
                    Set_VarListener(aiBrain.Animal);
                    break;
                case Affected.Target:
                    Set_VarListener(aiBrain.Target);
                    break;
                default:
                    break;
            }
            aiBrain.TasksDone = true;
        }

        protected override State OnUpdate()
        {
            if (aiBrain.TasksDone)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }

        public void Set_VarListener(Component comp)
        {
            var AllListeners = comp.GetComponentsInChildren<VarListener>();

            foreach (var listener in AllListeners)
            {
                if (ListenerID == 0 || listener.ID.Value == ListenerID.Value)
                {
                    switch (varType)
                    {
                        case VarType.Bool:
                            if (listener is BoolVarListener)
                            {
                                (listener as BoolVarListener).value.Value = boolValue;
                            }

                            break;
                        case VarType.Int:
                            if (listener is IntVarListener)
                            {
                                (listener as IntVarListener).value.Value = intValue;
                            }

                            break;
                        case VarType.Float:
                            if (listener is FloatVarListener)
                            {
                                (listener as FloatVarListener).value.Value = floatValue;
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();
             description += "Target: ";
            if (checkOn == Affected.Self)
            {
                description += "Self\n";
                switch (varType)
                {
                    case VarType.Bool:
                        description += $"Bool: {boolValue}";
                        break;
                    case VarType.Float:
                        description += $"Float: {floatValue}";
                        break;
                    case VarType.Int:
                        description += $"Int: {intValue}";
                        break;
                }
            }
            else
            {
                description += "Target\n";
                switch (varType)
                {
                    case VarType.Bool:
                        description += $"Bool: {boolValue}";
                        break;
                    case VarType.Float:
                        description += $"Float: {floatValue}";
                        break;
                    case VarType.Int:
                        description += $"Int: {intValue}";
                        break;
                }
            }
            return description;
        }
    }
}
