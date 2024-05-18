using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEditor;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Check Stat", "Animal Controller/MObserverDecorator/Check Stat", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckStat : ObserverDecorator
    {
        public enum checkStatOption { Compare, CompareNormalized, IsInmune, Regenerating, Degenerating, IsEmpty, IsFull, IsActive, ValueChanged, ValueReduced, ValueIncreased }
        [Header("Node")]

        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected checkOn = Affected.Self;

        [Tooltip("Stat you want to find")]
        public StatID Stat;
        [Tooltip("What do you want to do with the Stat?")]
        public checkStatOption Option = checkStatOption.Compare;
        [Tooltip("(Option Compare Only) Type of the comparison")]
        public ComparerInt StatIs = ComparerInt.Less;
        public float Value;
        [Tooltip("(Option Compare Only) Value to Compare the Stat")]

        [ContextMenuItem("Recover Value", "RecoverValue")]
        public FloatReference m_Value = new FloatReference();

        [Space, Tooltip("Uses TryGet Value in case you don't know if your target or your animal has the Stat you are looking for. Disabling this Improves performance")]
        public bool TryGetValue = true;

        [HideInInspector] public bool hideVars = false;
        bool result;

        AIBrain AIBrain;

        public override event Action OnValueChange;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            AIBrain = GetOwner().GetComponent<AIBrain>();
        }

        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }

        protected override void OnEntry()
        {
            // Store the Value the Stat has starting this Decision
            base.OnEntry();

            switch (checkOn)
            {
                case Affected.Self:
                    if (TryGetValue)
                    {
                        if (AIBrain.AnimalStats.TryGetValue(Stat.ID, out Stat statS))
                        {
                            AIBrain.statParameterInfo.Find(p => p.ID == Stat.ID)?.UpdatePreviousValue(statS.Value);
                        }
                    }
                    else
                    {
                        AIBrain.statParameterInfo.Find(p => p.ID == Stat.ID)?.UpdatePreviousValue(AIBrain.AnimalStats[Stat.ID].Value);
                    }
                    break;

                case Affected.Target:
                    if (AIBrain.TargetHasStats)
                    {
                        if (TryGetValue)
                        {
                            if (AIBrain.TargetStats.TryGetValue(Stat.ID, out Stat statS))
                            {
                                AIBrain.statParameterInfo.Find(p => p.ID == Stat.ID)?.UpdatePreviousValue(statS.Value);
                            }
                        }
                        else
                        {
                            AIBrain.statParameterInfo.Find(p => p.ID == Stat.ID)?.UpdatePreviousValue(AIBrain.TargetStats[Stat.ID].Value);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override bool CalculateResult()
        {
            switch (checkOn)
            {
                case Affected.Self:
                    if (TryGetValue && AIBrain != null)
                    {
                        if (AIBrain.AnimalStats.TryGetValue(Stat.ID, out Stat statS))
                        {
                            result = CheckStat(statS, AIBrain);
                        }
                    }
                    else if (AIBrain != null)
                    {
                        var SelfStatValue = AIBrain.AnimalStats[Stat.ID];
                        result = CheckStat(SelfStatValue, AIBrain);
                    }
                    break;
                case Affected.Target:
                    if (AIBrain != null && AIBrain.TargetHasStats)
                    {
                        if (TryGetValue && AIBrain.TargetStats.TryGetValue(Stat.ID, out Stat statT))
                        {
                            result = CheckStat(statT, AIBrain);
                        }
                        else if (AIBrain != null)
                        {
                            var TargetStatValue = AIBrain.TargetStats[Stat.ID];
                            result = CheckStat(TargetStatValue, AIBrain);
                        }
                    }
                    break;
            }

            return result;

        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void RecoverValue()
        {
            m_Value.Value = Value;
        }

        private bool CheckStat(Stat stat, AIBrain AIBrain)
        {
            switch (Option)
            {
                case checkStatOption.Compare:
                    return CompareWithValue(stat.value);
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
                    // Get the previous value of the stat from the ParameterInfo stored in AIBrain
                    float previousValue = AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.PreviousValue ?? 0f;
                    // Compare the current value of the stat with its previous value
                    bool valueChanged = stat.Value != previousValue;
                    // Set the current value as the previous value if changed
                    if (valueChanged)
                    {
                        AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.UpdatePreviousValue(stat.Value);
                    }
                    return valueChanged;
                case checkStatOption.ValueReduced:
                    // Get the previous value of the stat from the ParameterInfo stored in AIBrain
                    previousValue = AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.PreviousValue ?? 0f;
                    // Compare the current value of the stat with its previous value
                    bool valueReduced = stat.Value < previousValue;
                    // Set the current value as the previous value if reduced
                    if (valueReduced)
                    {
                        AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.UpdatePreviousValue(stat.Value);
                    }
                    return valueReduced;
                case checkStatOption.ValueIncreased:
                    // Get the previous value of the stat from the ParameterInfo stored in AIBrain
                    previousValue = AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.PreviousValue ?? 0f;
                    // Compare the current value of the stat with its previous value
                    bool valueIncreased = stat.Value > previousValue;
                    // Set the current value as the previous value if increased
                    if (valueIncreased)
                    {
                        AIBrain.statParameterInfo.Find(p => p.ID == stat.ID)?.UpdatePreviousValue(stat.Value);
                    }
                    return valueIncreased;
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
            description += $"Stat Is: {StatIs} \n";
            if (AIBrain)
            {
                description += $"Value: {result} \n";
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
            nodeName, checkOn, Stat, Option, StatIs, Value, TryGetValue, notifyObserver, observerAbort;

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
            EditorGUILayout.LabelField("Condition", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(observerAbort);
            EditorGUILayout.PropertyField(notifyObserver);
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
            {
                EditorGUILayout.PropertyField(StatIs, GUIContent.none, GUILayout.Width(90));
            }

            EditorGUILayout.EndHorizontal();

            if (compare)
            {
                EditorGUILayout.PropertyField(Value);
                if (o == MCheckStat.checkStatOption.CompareNormalized)
                {
                    EditorGUILayout.HelpBox("Compare Normalized Value must be between 0 and 1", MessageType.Info);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
