using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    public enum Affect
    {
        Self,
        CurrentTarget,
        Tag,
        TransformHook,
        GameObjectHook,
        RuntimeGameObjectSet
    }

    public enum ComponentPlace
    {
        SameHierarchy,
        Parent,
        Children
    }


    [NodeContent("Check Var Listener", "Animal Controller/Check Var Listener", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckVarListener : ObserverDecorator
    {
        [Header("Node")]

        [Tooltip("Check the Variable Listener ID Value, when this value is Zero, the ID is ignored")]
        public IntReference ListenerID = 0;
        /// <summary>Range for Looking forward and Finding something</summary>
        [Space, Tooltip("Find the VarListener component on:\n\n" +
            "-Self: \nCheck on the Animal Gameobject using the Brain\n" +
            "-Target: \ncurrent AI Target\n" +
            "-Tag: \nAll the gameobjects using a Malbers Tag\n" +
            "-Transform Hook: \na in a Transform Hook\n" +
            "-GameObject Hook: \na in a GameObject Hook\n" +
            "-Runtime GameObject Set: \na in all the GameObject in a Runtime Set")]
        public Affect checkOn = Affect.Self;

        [Tooltip("Check if the Var Listener component its placed on:\n\n" +
            "-SameHierarchy: \nsame hierarchy level as the gameobject(s) in the [CheckOn] Option\n" +
            "-Parent: \nany of the parents of the gameobject(s) in the [CheckOn] Option\n" +
            "-Children: \nany of the children of the gameobject(s) in the [CheckOn] Option")]
        public ComponentPlace PlacedOn = ComponentPlace.SameHierarchy;
        [Hide("checkOn", (int)Affect.Tag)] public Tag tag;
        [Hide("checkOn", (int)Affect.TransformHook)] public TransformVar Transform;
        [Hide("checkOn", (int)Affect.GameObjectHook)] public GameObjectVar GameObject;
        [Hide("checkOn", (int)Affect.RuntimeGameObjectSet)] public RuntimeGameObjects GameObjectSet;


        [Space,
            Tooltip("Check on the Target or Self if it has a Listener Variable Component <Int><Bool><Float> and compares it with the local variable)")]
        public VarType varType = VarType.Bool;

        [Hide("varType", (int)VarType.Int, (int)VarType.Float)] public ComparerInt comparer;
        [Hide("varType", (int)VarType.Bool)] public bool boolValue = true;
        [Hide("varType", (int)VarType.Int)] public int intValue = 0;
        [Hide("varType", (int)VarType.Float)] public float floatValue = 0f;

        public bool debug = false;

        AIBrain aiBrain;

        public override event Action OnValueChange;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();

            MonoBehaviour[] monoValue = null;

            var objectives = GetObjective(aiBrain);

            if (objectives == null)
            {
                if (debug)
                {
                    Debug.LogWarning("Check Var Listener Objectives is Null, Please check your Decisions", this);
                }

                return;
            }

            if (objectives != null && objectives.Length > 0)
            {
                foreach (var target in objectives)
                {
                    if (target == null)
                    {
                        if (debug)
                        {
                            Debug.LogWarning($"Check Var Listener Checking on [{checkOn}]. Objective is Null", this);
                        }

                        return;
                    }
                    switch (varType)
                    {
                        case VarType.Bool:
                            monoValue = GetComponents<BoolVarListener>(target.gameObject);
                            break;
                        case VarType.Int:
                            monoValue = GetComponents<IntVarListener>(target.gameObject);
                            break;
                        case VarType.Float:
                            monoValue = GetComponents<FloatVarListener>(target.gameObject);
                            break;
                    }
                }
            }

            aiBrain.DecisionsVars.AddComponents(monoValue);
        }
        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }
        public override bool CalculateResult()
        {

                var listeners = aiBrain?.DecisionsVars.Components;

                if (listeners == null || listeners.Length == 0)
                {
                    return false;
                }

                bool result = false;

                foreach (var varListener in listeners)
                {
                    if (varListener is VarListener)
                    {
                        switch (varType)
                        {
                            case VarType.Bool:
                                var LB = varListener as BoolVarListener;
                                result = LB.Value == boolValue;
                                if (debug)
                                {
                                    Debug.Log($"{aiBrain.Animal.name}: <B>[{name}]</B> ListenerBool<{LB.transform.name}> ID<{LB.ID.Value}> Value<{LB.Value}>  <B>Result[{result}]</B>");
                                }

                                break;
                            case VarType.Int:
                                var LI = varListener as IntVarListener;
                                result = CompareInteger(LI.Value);
                                if (debug)
                                {
                                    Debug.Log($"{aiBrain.Animal.name}: <B>[{name}]</B> ListenerInt<{LI.transform.name}> ID<{LI.ID.Value}> Value<{LI.Value}>  <B>Result[{result}]</B>");
                                }

                                break;
                            case VarType.Float:
                                var LF = varListener as FloatVarListener;
                                result = CompareFloat(LF.Value);
                                if (debug)
                                {
                                    Debug.Log($"{aiBrain.Animal.name}: <B>[{name}]</B> ListenerInt<{LF.transform.name}> ID<{LF.ID.Value}> Value<{LF.Value}>  <B>Result[{result}]</B>");
                                }

                                break;
                            default:
                                return false;
                        }
                    }
                }
                return result;
     
        }

        private Transform[] GetObjective(AIBrain aiBrain)
        {
            switch (checkOn)
            {
                case Affect.Self:
                    return new Transform[1] { aiBrain.Animal.transform };
                case Affect.CurrentTarget:
                    return new Transform[1] { aiBrain.Target };
                case Affect.Tag:
                    var tagH = Tags.TagsHolders.FindAll(X => X.HasTag(tag));
                    if (tagH != null)
                    {
                        var TArray = new List<Transform>();
                        foreach (var t in tagH)
                        {
                            TArray.Add(t.transform);
                        }

                        return TArray.ToArray();
                    }
                    return null;
                case Affect.TransformHook:
                    {
                        if (Transform == null || Transform.Value == null)
                        {
                            return null;
                        }

                        return new Transform[1] { Transform.Value };
                    }
                case Affect.GameObjectHook:
                    if (!GameObject.Value.IsPrefab())
                    {
                        return new Transform[1] { GameObject.Value.transform };
                    }
                    else
                    {
                        Debug.LogWarning("The GameObject Hook is a Prefab. Is not in the scene.", GameObject.Value);

                        return null;
                    }
                case Affect.RuntimeGameObjectSet:
                    var TGOS = new List<Transform>();
                    foreach (var t in GameObjectSet.Items)
                    {
                        TGOS.Add(t.transform);
                    }

                    return TGOS.ToArray();
                default:
                    return null;
            }
        }



        private TVarListener[] GetComponents<TVarListener>(GameObject gameObject)
            where TVarListener : VarListener
        {
            TVarListener[] list;

            switch (PlacedOn)
            {
                case ComponentPlace.Children:
                    list = gameObject.GetComponentsInChildren<TVarListener>();
                    break;
                case ComponentPlace.Parent:
                    list = gameObject.GetComponentsInParent<TVarListener>();
                    break;
                case ComponentPlace.SameHierarchy:
                    list = gameObject.GetComponents<TVarListener>();
                    break;
                default:
                    list = gameObject.GetComponents<TVarListener>();
                    break;
            }

            list = list.ToList().FindAll(x => ListenerID.Value == 0 || x.ID == ListenerID.Value).ToArray();

            return list;
        }

        public enum VarType
        {
            Bool,
            Int,
            Float
        }

        public enum BoolType
        {
            True,
            False
        }


        public bool CompareInteger(int IntValue)
        {
            switch (comparer)
            {
                case ComparerInt.Equal:
                    return IntValue == intValue;
                case ComparerInt.Greater:
                    return IntValue > intValue;
                case ComparerInt.Less:
                    return IntValue < intValue;
                case ComparerInt.NotEqual:
                    return IntValue != intValue;
                default:
                    return false;
            }
        }

        public bool CompareFloat(float IntValue)
        {
            switch (comparer)
            {
                case ComparerInt.Equal:
                    return IntValue == floatValue;
                case ComparerInt.Greater:
                    return IntValue > floatValue;
                case ComparerInt.Less:
                    return IntValue < floatValue;
                case ComparerInt.NotEqual:
                    return IntValue != floatValue;
                default:
                    return false;
            }
        }

        public override string GetDescription()
        {
            string description = $"Listener ID: {ListenerID.Value} \n";
            description += $"Check On: {checkOn} \n";

            //Need to add scritable objects
            switch (checkOn)
            {
                case Affect.Tag:
                    if (tag != null)
                    {
                        description += $"Tag: {tag.DisplayName} \n";
                    }
                    break;
            }
            return description;
        }
    }
}