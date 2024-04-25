using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEditor;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Check Stat", "Animal Controller/Check Stat", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckStat : ObserverDecorator
    {
        public enum checkStatOption { Compare, CompareNormalized, IsInmune, Regenerating, Degenerating, IsEmpty, IsFull, IsActive, ValueChanged, ValueReduced, ValueIncreased }
        [Header("Node")]

        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected checkOn = Affected.Self;
        /// <summary>Range for Looking forward and Finding something</summary>

        [Tooltip("Stat you want to find")]
        public StatID Stat;
        [Tooltip("What do you want to do with the Stat?")]
        public checkStatOption Option = checkStatOption.Compare;
        [Tooltip("(Option Compare Only) Type of the comparation")]
        public ComparerInt StatIs = ComparerInt.Less;
        public float Value;
        [Tooltip("(Option Compare Only) Value to Compare the Stat")]

        [ContextMenuItem("Recover Value", "RecoverValue")]
        public FloatReference m_Value = new FloatReference();

        [Space, Tooltip("Uses TryGet Value in case you don't know if your target or your animal has the Stat you are looking for. Disabling this Improves performance")]
        public bool TryGetValue = true;

        AIBrain aiBrain;

        [HideInInspector] public bool hideVars = false;
        private bool checkResult;
        public override event Action OnValueChange;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();
        }

        protected override void OnEntry()
        {
            base.OnEntry();
            switch (checkOn)
            {
                case Affected.Self:
                    if (TryGetValue && aiBrain != null)
                    {
                        if (aiBrain.AnimalStats.TryGetValue(Stat.ID, out Stat statS))
                        {
                            aiBrain.DecisionsVars.floatValue = statS.Value;
                        }
                    }
                    else
                    {
                        aiBrain.DecisionsVars.floatValue = aiBrain.AnimalStats[Stat.ID].Value;
                    }
                    break;

                case Affected.Target:

                    if (aiBrain.TargetHasStats)
                    {
                        if (TryGetValue)
                        {
                            if (aiBrain.TargetStats.TryGetValue(Stat.ID, out Stat statS))
                            {
                                aiBrain.DecisionsVars.floatValue = statS.Value;
                            }
                        }
                        else
                        {
                            aiBrain.DecisionsVars.floatValue = aiBrain.TargetStats[Stat.ID].Value;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        // Override the Evaluate method or else your environment will throw an error
        /// <summary>
        /// Called every tick regardless of the node execution.
        /// </summary>
        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }
        public override bool CalculateResult()
        {
            bool result = false;

            switch (checkOn)
            {
                case Affected.Self:
                    if (TryGetValue)
                    {
                        if (aiBrain != null && aiBrain.AnimalStats.TryGetValue(Stat.ID, out Stat statS))
                        {
                            checkResult = CheckStat(statS, aiBrain);
                            result = checkResult;
                        }
                    }
                    else
                    {
                        if (aiBrain != null)
                        {

                        var SelfStatValue = aiBrain.AnimalStats[Stat.ID];
                        checkResult = CheckStat(SelfStatValue, aiBrain);
                        result = checkResult;
                        }
                    }
                    break;
                case Affected.Target:
                    if (aiBrain != null && aiBrain.TargetHasStats)
                    {
                        if (TryGetValue && aiBrain.TargetStats.TryGetValue(Stat.ID, out Stat statT))
                        {
                            checkResult = CheckStat(statT, aiBrain);
                            result = checkResult;
                        }
                        else if (aiBrain != null && !TryGetValue)
                        {
                            var TargetStatValue = aiBrain.TargetStats[Stat.ID];
                            checkResult = CheckStat(TargetStatValue, aiBrain);
                            result = checkResult;
                        }
                    }
                    break;
            }
            //Debug.Log(result);
            return result;

        }
        private void RecoverValue()
        {
            m_Value.Value = Value;
        }

        private bool CheckStat(Stat stat, AIBrain aiBrain)
        {


            switch (Option)
            {
                case checkStatOption.Compare:
                    return CompareWithValue(stat.Value);
                case checkStatOption.CompareNormalized:
                    return CompareWithValue(stat.NormalizedValue);
                case checkStatOption.IsInmune:
                    return stat.IsInmune;
                case checkStatOption.Regenerating:
                    return stat.IsRegenerating;
                case checkStatOption.Degenerating:
                    return stat.IsDegenerating;
                case checkStatOption.IsEmpty:
                    return stat.Value == stat.MinValue;
                case checkStatOption.IsFull:
                    return stat.Value == stat.MaxValue;
                case checkStatOption.IsActive:
                    return stat.Active;
                case checkStatOption.ValueChanged:
                    return stat.value != aiBrain.DecisionsVars.floatValue; ;
                case checkStatOption.ValueReduced:
                    return stat.value < aiBrain.DecisionsVars.floatValue; ;
                case checkStatOption.ValueIncreased:
                    return stat.value > aiBrain.DecisionsVars.floatValue; ;
                default:
                    return false;
            }
        }
        private bool CompareWithValue(float stat)
        {
            switch (StatIs)
            {
                case ComparerInt.Equal:
                    return stat == m_Value;
                case ComparerInt.Greater:
                    return stat > m_Value;
                case ComparerInt.Less:
                    return stat < m_Value;
                case ComparerInt.NotEqual:
                    return stat != m_Value;
                default:
                    return false;
            }
        }

        public override string GetDescription()
        {
            string description = "";
            if (Stat != null)
            {
                description += $"Stat ID: {Stat.DisplayName} \n";
            }
            description += $"Option: {Option} \n";

            switch (Option)
            {
                case checkStatOption.Compare:
                    description += $"Stat Is: {StatIs} \n";
                    description += $"Value: {Value} \n";
                    break;

                case checkStatOption.CompareNormalized:
                    description += $"Stat Is: {StatIs} \n";
                    description += $"Value: {Value} \n";
                    break;

            }

            return description;
        }

        private void OnValidate()
        {
            hideVars = Option != checkStatOption.Compare && Option != checkStatOption.CompareNormalized;
        }
    }
#if UNITY_EDITOR

    [CustomEditor(typeof(MCheckStat))]
    [CanEditMultipleObjects]
    public class MCheckStatEditor : Editor
    {
        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));

        SerializedProperty
            nodeName, notifyObserver, observerAbort, checkOn, Stat, Option, StatIs, Value, TryGetValue;

        MonoScript script;
        private void OnEnable()
        {
            script = MonoScript.FromScriptableObject((ScriptableObject)target);

            checkOn = serializedObject.FindProperty("checkOn");
            nodeName = serializedObject.FindProperty("nodeName");
            notifyObserver = serializedObject.FindProperty("notifyObserver");
            observerAbort = serializedObject.FindProperty("observerAbort");
            Stat = serializedObject.FindProperty("Stat");
            Option = serializedObject.FindProperty("Option");
            StatIs = serializedObject.FindProperty("StatIs");
            Value = serializedObject.FindProperty("m_Value");
            TryGetValue = serializedObject.FindProperty("TryGetValue");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();



            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(nodeName);
            EditorGUILayout.LabelField("Flow Control", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(notifyObserver);
            EditorGUILayout.PropertyField(observerAbort);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(checkOn);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(Stat);

            var m_stlye = new GUIStyle(EditorStyles.miniButton);
            m_stlye.fontStyle = TryGetValue.boolValue ? FontStyle.Bold : FontStyle.Normal;

            TryGetValue.boolValue = GUILayout.Toggle(TryGetValue.boolValue,
                new GUIContent("Try*",
                "Uses TryGet Value in case you don't know if your target or your animal has the Stat you are looking for. Disabling this Improves performance"),
                m_stlye, GUILayout.Width(50));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(Option);

            var o = (MCheckStat.checkStatOption)Option.intValue;

            var compare = o == MCheckStat.checkStatOption.Compare || o == MCheckStat.checkStatOption.CompareNormalized;

            if (compare)
                EditorGUILayout.PropertyField(StatIs, GUIContent.none, GUILayout.Width(90));

            EditorGUILayout.EndHorizontal();

            if (compare)
            {
                EditorGUILayout.PropertyField(Value);
                if (o == MCheckStat.checkStatOption.CompareNormalized)
                    EditorGUILayout.HelpBox("Compare Normalized Value must be between 0 and 1", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}