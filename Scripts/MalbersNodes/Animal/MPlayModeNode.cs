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

    [NodeContent("Play Mode", "Animal Controller/Play Mode", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MPlayModeNode : TaskNode
    {

        [Tooltip("Mode you want to activate when the brain is using this task")]
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
        AIBrain aiBrain;
        bool modePlayed;
        float startTime;

        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
            modePlayed = false;
            startTime = Time.time;
            defaultIgnoreFirstCoolDown = IgnoreFirstCoolDown;
            aiBrain.AIControl.AutoNextTarget = false;
            if (CoolDown <= 0)
            {
                if (Play == PlayWhen.PlayOnce)
                {
                    if (near && !aiBrain.AIControl.HasArrived)
                    {
                        return; //Dont play if Play on target is true but we are not near the target.
                    }


                    PlayMode(aiBrain);

                }
                else if (Play == PlayWhen.Interrupt)
                {
                    switch (affect)
                    {
                        case Affected.Self:
                            aiBrain.Animal.Mode_Interrupt();
                            break;
                        case Affected.Target:
                            if (aiBrain.TargetAnimal != null)
                            {
                                aiBrain.TargetAnimal.Mode_Interrupt();
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
            if (near && !aiBrain.AIControl.HasArrived)
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
                                PlayMode(aiBrain);
                            }
                        }
                        break;
                    }
                case PlayWhen.PlayForever:

                    if (IgnoreFirstCoolDown) //Means the Mode has not played
                    {
                        IgnoreFirstCoolDown = false;
                        PlayMode(aiBrain);
                        startTime = Time.time;  // Reset the cooldown timer
                    }

                    if (!modePlayed && CoolDown <= 0)
                    {
                        PlayModeForever(aiBrain);
                    }

                    if (MTools.ElapsedTime(startTime, CoolDown) && CoolDown > 0) //If the animal is in range of the Target
                    {
                        PlayMode(aiBrain);
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
                                    aiBrain.Animal.Mode_Interrupt();
                                    break;
                                case Affected.Target:
                                    if (aiBrain.TargetAnimal != null)
                                    {
                                        aiBrain.TargetAnimal.Mode_Interrupt();
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
            var animal = affect == Affected.Self ? aiBrain.Animal : aiBrain.TargetAnimal;

            if (animal != null && animal.IsPlayingMode && StopModeOnExit)
            {
                animal.Mode_Stop();
            }
            modePlayed = false;
            IgnoreFirstCoolDown = defaultIgnoreFirstCoolDown;
        }

        private bool PlayMode(AIBrain aiBrain)
        {
            switch (affect)
            {
                case Affected.Self:
                    var Direction_to_Target = aiBrain.Target != null ? (aiBrain.Target.position - aiBrain.Eyes.position) : aiBrain.Animal.Forward;

                    var EyesForward = Vector3.ProjectOnPlane(aiBrain.Eyes.forward, aiBrain.Animal.UpVector);
                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (aiBrain.Animal.Mode_ForceActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && aiBrain.Target)
                            {
                                aiBrain.StartCoroutine(MTools.AlignLookAtTransform(aiBrain.Animal.transform, aiBrain.AIControl.GetTargetPosition(), alignTime));
                            }
                            aiBrain.Animal.Mode_SetPower(ModePower);
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
                    Direction_to_Target = aiBrain.Eyes.position - aiBrain.Target.position; //Reverse the Direction
                    EyesForward = Vector3.ProjectOnPlane(aiBrain.Target.forward, aiBrain.Animal.UpVector);

                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (aiBrain.TargetAnimal && aiBrain.TargetAnimal.Mode_ForceActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && aiBrain.Target)
                            {
                                aiBrain.StartCoroutine(MTools.AlignLookAtTransform(aiBrain.TargetAnimal.transform, aiBrain.transform, alignTime));
                            }
                            aiBrain.TargetAnimal.Mode_SetPower(ModePower);
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

        private bool PlayModeForever(AIBrain brain)
        {
            switch (affect)
            {
                case Affected.Self:
                    var Direction_to_Target = brain.Target != null ? (brain.Target.position - brain.Eyes.position) : brain.Animal.Forward;

                    var EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, brain.Animal.UpVector);
                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (brain.Animal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && brain.Target)
                            {
                                brain.StartCoroutine(MTools.AlignLookAtTransform(brain.Animal.transform, brain.AIControl.GetTargetPosition(), alignTime));
                            }

                            brain.Animal.Mode_SetPower(ModePower);
                            return true;
                        }
                    }
                    break;
                case Affected.Target:
                    Direction_to_Target = brain.Eyes.position - brain.Target.position; //Reverse the Direction
                    EyesForward = Vector3.ProjectOnPlane(brain.Target.forward, brain.Animal.UpVector);

                    if (ModeAngle == 360f || Vector3.Dot(Direction_to_Target.normalized, EyesForward) > Mathf.Cos(ModeAngle * 0.5f * Mathf.Deg2Rad)) //Mean is in Range:
                    {
                        if (brain.TargetAnimal && brain.TargetAnimal.Mode_TryActivate(modeID, AbilityID))
                        {
                            if (lookAtAlign && brain.Target)
                            {
                                brain.StartCoroutine(MTools.AlignLookAtTransform(brain.TargetAnimal.transform, brain.transform, alignTime));
                            }

                            brain.TargetAnimal.Mode_SetPower(ModePower);
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
