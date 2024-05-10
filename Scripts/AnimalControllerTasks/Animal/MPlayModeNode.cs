using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEditor;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    public enum PlayWhen { PlayOnce, PlayForever, Interrupt }

    [NodeContent("Play Mode", "Animal Controller/Animal/Play Mode", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MPlayModeNode : MTaskNode
    {

        [Tooltip("Mode you want to activate when the AIBrain is using this task")]
        public ModeID modeID;
        [Tooltip("Ability ID for the Mode... if is set to -99 it will play a random Ability")]
        public IntReference AbilityID = new(-99);
        public FloatReference ModePower = new();
        [Tooltip("Play the mode only when the animal has arrived to the target")]
        public bool near = false;

        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        [Tooltip("Play Once: it will play only at the start of the Task. Play Forever: will play forever using the Cooldown property")]
        public PlayWhen Play = PlayWhen.PlayForever;
        [Tooltip("Time elapsed to Play the Mode again and Again")]
        public FloatReference CoolDown = new(0f);
        [Tooltip("Play the Mode if the Animal is Looking at the Target. Avoid playing modes while the target is behind the animal when this value is set to 180")]
        [Range(0f, 360f)]
        public float ModeAngle = 360f;

        [Tooltip("Align with a Look At towards the Target when Playing a mode")]
        public bool lookAtAlign = false;

        [Tooltip("When the mode is said to Play Forever, it will ignore the first cooldown")]
        public bool IgnoreFirstCoolDown = true;

        [Tooltip("If the task was playing a mode when the AI State Exits, stop the playing Mode")]
        public bool StopModeOnExit = true;

        [Tooltip("Align time to rotate towards the Target")]
        public float alignTime = 0.3f;
        bool defaultIgnoreFirstCoolDown;
        bool modePlayed;
        float startTime;

        protected override void OnEntry()
        {
            modePlayed = false;
            startTime = Time.time;
            defaultIgnoreFirstCoolDown = IgnoreFirstCoolDown;
            AIBrain.AIControl.AutoNextTarget = false;
            if (CoolDown <= 0)
            {
                if (Play == PlayWhen.PlayOnce)
                {
                    if (near && !AIBrain.AIControl.HasArrived)
                    {
                        return; //Dont play if Play on target is true but we are not near the target.
                    }


                    PlayMode(AIBrain);

                }
                else if (Play == PlayWhen.Interrupt)
                {
                    switch (affect)
                    {
                        case Affected.Self:
                            AIBrain.Animal.Mode_Interrupt();
                            break;
                        case Affected.Target:
                            if (AIBrain.TargetAnimal != null)
                            {
                                AIBrain.TargetAnimal.Mode_Interrupt();
                            }
                            break;
                        default:
                            break;
                    }
                    modePlayed = true;
                }

            }
        }

        protected override State OnUpdate()
        {
            if (near && !AIBrain.AIControl.HasArrived)
            {
                Debug.Log("Not Arrived");
                return State.Running; //Dont play if Play on target is true but we are not near the target.
            }

            switch (Play)
            {
                case PlayWhen.PlayOnce:
                    {
                        if (!modePlayed) //Means the Mode has not played
                        {
                            if (MTools.ElapsedTime(startTime, CoolDown))
                            {
                                PlayMode(AIBrain);
                            }
                        }
                        break;
                    }
                case PlayWhen.PlayForever:

                    if (IgnoreFirstCoolDown) //Means the Mode has not played
                    {
                        IgnoreFirstCoolDown = false;
                        PlayMode(AIBrain);
                        startTime = Time.time;  // Reset the cooldown timer
                    }

                    if (!modePlayed && CoolDown <= 0)
                    {
                        PlayModeForever(AIBrain);
                    }

                    if (MTools.ElapsedTime(startTime, CoolDown) && CoolDown > 0) //If the animal is in range of the Target
                    {
                        PlayMode(AIBrain);
                        startTime = Time.time; // Reset the cooldown timer
                    }
                    break;
                case PlayWhen.Interrupt:
                    if (!modePlayed) //Means the Mode has not played
                    {
                        if (MTools.ElapsedTime(startTime, CoolDown))
                        {
                            switch (affect)
                            {
                                case Affected.Self:
                                    AIBrain.Animal.Mode_Interrupt();
                                    break;
                                case Affected.Target:
                                    if (AIBrain.TargetAnimal != null)
                                    {
                                        AIBrain.TargetAnimal.Mode_Interrupt();
                                    }
                                    break;
                                default:
                                    break;
                            }
                            modePlayed = true; //Set that te mode was Played Once!!
                            return State.Success;
                        }

                    }
                    break;
                default:
                    break;
            }
            if (modePlayed)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }

        }

        protected override void OnExit()
        {
            base.OnExit();
            var animal = affect == Affected.Self ? AIBrain.Animal : AIBrain.TargetAnimal;

            if (animal != null && animal.IsPlayingMode && StopModeOnExit)
            {
                animal.Mode_Stop();
            }
            modePlayed = false;
            IgnoreFirstCoolDown = defaultIgnoreFirstCoolDown;
        }

        private bool PlayMode(AIBrain AIBrain)
        {
            switch (affect)
            {
                case Affected.Self:
                    var Direction_to_Target = AIBrain.Target != null ? (AIBrain.Target.position - AIBrain.Eyes.position) : AIBrain.Animal.Forward;

                    var EyesForward = Vector3.ProjectOnPlane(AIBrain.Eyes.forward, AIBrain.Animal.UpVector);
                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (AIBrain.Animal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && AIBrain.Target)
                            {
                                AIBrain.StartCoroutine(MTools.AlignLookAtTransform(AIBrain.Animal.transform, AIBrain.AIControl.GetTargetPosition(), alignTime));
                            }
                            AIBrain.Animal.Mode_SetPower(ModePower);
                            if (Play == PlayWhen.PlayForever)
                            {
                                return true;
                            }
                            modePlayed = true;
                            return true;
                        }
                    }
                    break;
                case Affected.Target:
                    Direction_to_Target = AIBrain.Eyes.position - AIBrain.Target.position; //Reverse the Direction
                    EyesForward = Vector3.ProjectOnPlane(AIBrain.Target.forward, AIBrain.Animal.UpVector);

                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (AIBrain.TargetAnimal && AIBrain.TargetAnimal.Mode_ForceActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && AIBrain.Target)
                            {
                                AIBrain.StartCoroutine(MTools.AlignLookAtTransform(AIBrain.TargetAnimal.transform, AIBrain.transform, alignTime));
                            }
                            AIBrain.TargetAnimal.Mode_SetPower(ModePower);
                            if (Play == PlayWhen.PlayForever)
                            {
                                return true;
                            }
                            modePlayed = true;
                            return true;
                        }
                    }
                    break;
                default:
                    break;
            }
            modePlayed = false;
            return false;
        }

        private bool PlayModeForever(AIBrain AIBrain)
        {
            switch (affect)
            {
                case Affected.Self:
                    var Direction_to_Target = AIBrain.Target != null ? (AIBrain.Target.position - AIBrain.Eyes.position) : AIBrain.Animal.Forward;

                    var EyesForward = Vector3.ProjectOnPlane(AIBrain.Eyes.forward, AIBrain.Animal.UpVector);
                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (AIBrain.Animal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && AIBrain.Target)
                            {
                                AIBrain.StartCoroutine(MTools.AlignLookAtTransform(AIBrain.Animal.transform, AIBrain.AIControl.GetTargetPosition(), alignTime));
                            }

                            AIBrain.Animal.Mode_SetPower(ModePower);
                            return true;
                        }
                    }
                    break;
                case Affected.Target:
                    Direction_to_Target = AIBrain.Eyes.position - AIBrain.Target.position; //Reverse the Direction
                    EyesForward = Vector3.ProjectOnPlane(AIBrain.Target.forward, AIBrain.Animal.UpVector);

                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (AIBrain.TargetAnimal && AIBrain.TargetAnimal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && AIBrain.Target)
                            {
                                AIBrain.StartCoroutine(MTools.AlignLookAtTransform(AIBrain.TargetAnimal.transform, AIBrain.transform, alignTime));
                            }

                            AIBrain.TargetAnimal.Mode_SetPower(ModePower);
                            return true;
                        }
                    }
                    break;
                default:
                    break;

            }
            return false;
        }


        protected override float? GetProgress()
        {
            if (CoolDown > 0)
            {
                return (Time.time - startTime) / CoolDown.Value;

            }
            else
            {
                return null;
            }
        }
        public override string GetDescription()
        {
            string description = base.GetDescription();

            if (affect == Affected.Self)
            {
                description += "Self ";
                if (Play == PlayWhen.PlayOnce)
                {
                    description += "Play Once\n";
                    description += $"Play near: {near}\n";
                    description += $"Mode ID: {(modeID != null ? modeID.DisplayName : "null")}\n";
                    description += $"Ability ID: {AbilityID.Value}\n";
                    description += $"Cooldown: {CoolDown.Value}\n";
                    description += $"look at align: {lookAtAlign}\n";
                }
                else if (Play == PlayWhen.PlayForever)
                {
                    description += "Play Forever\n";
                    description += $"Play near: {near}\n";
                    description += $"Mode ID: {(modeID != null ? modeID.DisplayName : "null")}\n";
                    description += $"Ability ID: {AbilityID.Value}\n";
                    description += $"Cooldown: {CoolDown.Value}\n";
                    description += $"look at align: {lookAtAlign}\n";
                }
                else
                {
                    description += "\nInterupt";
                }
            }
            else
            {
                description += "Target ";
                if (Play == PlayWhen.PlayOnce)
                {
                    description += "Play Once\n";
                    description += $"Play near: {near}\n";
                    description += $"Mode ID: {(modeID != null ? modeID.DisplayName : "null")}\n";
                    description += $"Ability ID: {AbilityID.Value}\n";
                    description += $"Cooldown: {CoolDown.Value}\n";
                    description += $"look at align: {lookAtAlign}\n";
                }
                else if (Play == PlayWhen.PlayForever)
                {
                    description += "Play Forever\n";
                    description += $"Play near: {near}\n";
                    description += $"Mode ID: {(modeID != null ? modeID.DisplayName : "null")}\n";
                    description += $"Ability ID: {AbilityID.Value}\n";
                    description += $"Cooldown: {CoolDown.Value}\n";
                    description += $"look at align: {lookAtAlign}\n";
                }
                else
                {
                    description += "\nInterupt";
                }
            }
            return description;
        }
    }

    #region Inspector

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MPlayModeNode))]
    public class MPlayModeNodeEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty ignoreAbortSelf, nodeName, CoolDown, modeID, AbilityID, near, affect, ModePower, UpdateInterval, StopModeOnExit,
            Play, ModeAngle, lookAtAlign, alignTime, IgnoreFirstCoolDown;

        private void OnEnable()
        {
            ignoreAbortSelf = serializedObject.FindProperty("ignoreAbortSelf");
            nodeName = serializedObject.FindProperty("nodeName");
            ModePower = serializedObject.FindProperty("ModePower");
            modeID = serializedObject.FindProperty("modeID");
            AbilityID = serializedObject.FindProperty("AbilityID");
            near = serializedObject.FindProperty("near");
            affect = serializedObject.FindProperty("affect");
            Play = serializedObject.FindProperty("Play");
            CoolDown = serializedObject.FindProperty("CoolDown");
            ModeAngle = serializedObject.FindProperty("ModeAngle");
            lookAtAlign = serializedObject.FindProperty("lookAtAlign");
            alignTime = serializedObject.FindProperty("alignTime");
            IgnoreFirstCoolDown = serializedObject.FindProperty("IgnoreFirstCoolDown");
            UpdateInterval = serializedObject.FindProperty("UpdateInterval");
            StopModeOnExit = serializedObject.FindProperty("StopModeOnExit");
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(nodeName);
            EditorGUILayout.PropertyField(ignoreAbortSelf);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(affect);
            EditorGUILayout.PropertyField(Play, new GUIContent("TaskNode"));
            var playtype = (PlayModeTask.PlayWhen)Play.intValue;
            if (playtype != PlayModeTask.PlayWhen.Interrupt)
            {
                EditorGUILayout.PropertyField(near);

                var oldColor = GUI.color;
                GUI.color = modeID.objectReferenceValue == null ? new Color(1, 0.4f, 0.4f, 1) : oldColor;
                EditorGUILayout.PropertyField(modeID);
                GUI.color = oldColor;


                EditorGUILayout.PropertyField(AbilityID);
                EditorGUILayout.PropertyField(ModePower);
                EditorGUILayout.PropertyField(StopModeOnExit);


                EditorGUILayout.PropertyField(CoolDown);
                if (Play.intValue == 1)
                {
                    EditorGUILayout.PropertyField(IgnoreFirstCoolDown);
                }

                EditorGUILayout.PropertyField(ModeAngle);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(lookAtAlign, new GUIContent("Quick Align"));

                EditorGUILayout.Space();

                if (lookAtAlign.boolValue)
                {
                    EditorGUIUtility.labelWidth = 33;
                    EditorGUILayout.PropertyField(alignTime, new GUIContent("Time"));
                    EditorGUIUtility.labelWidth = 0;
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.PropertyField(CoolDown);

            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    #endregion
}
