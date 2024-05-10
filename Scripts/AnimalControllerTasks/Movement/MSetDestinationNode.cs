using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEditor;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Destination", "Animal Controller/ACMovement/Set Destination", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetDestinationNode : MTaskNode
    {
        public enum DestinationType { Transform, GameObject, RuntimeGameObjects, Vector3, Name }

        [Tooltip("Slow multiplier to set on the Destination")]
        public float SlowMultiplier = 0;
        [Space]
        public DestinationType targetType = DestinationType.Transform;

        [RequiredField] public TransformVar TargetT;
        [RequiredField] public Vector3Var Destination;
        [RequiredField] public GameObjectVar TargetG;
        [RequiredField] public RuntimeGameObjects TargetRG;

        public RuntimeSetTypeGameObject rtype = RuntimeSetTypeGameObject.Random;

        public IntReference RTIndex = new();
        public StringReference RTName = new();

        [Tooltip("When a new target is assinged it also sets that the Animal should move to that target")]
        public bool MoveToTarget = true;

        bool taskDone;

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {
            AIBrain.AIControl.ClearTarget();

            AIBrain.AIControl.CurrentSlowingDistance = AIBrain.AIControl.StoppingDistance * SlowMultiplier;

            switch (targetType)
            {
                case DestinationType.Transform:

                    if (TargetT == null)
                    { Debug.LogError("Set Destination Task is missing the Transform Hook", this); return; }

                    AIBrain.AIControl.SetDestination(TargetT.Value.position, MoveToTarget);
                    break;
                case DestinationType.GameObject:

                    if (TargetG == null)
                    { Debug.LogError("Set Destination Task is missing the GameObject Hook", this); return; }

                    AIBrain.AIControl.SetDestination(TargetG.Value.transform.position, MoveToTarget);
                    break;
                case DestinationType.RuntimeGameObjects:

                    if (TargetRG == null)
                    { Debug.LogError("Set Destination Task is missing the RuntimeSet", this); return; }

                    var go = TargetRG.GetItem(rtype, RTIndex, RTName, AIBrain.Animal.gameObject);
                    if (go != null)
                    {
                        AIBrain.AIControl.SetDestination(go.transform.position, MoveToTarget);
                    }

                    break;
                case DestinationType.Vector3:
                    if (Destination == null)
                    { Debug.LogError("Set Destination Task is missing the Vector Scriptable Variable", this); return; }


                    AIBrain.AIControl.SetDestination(Destination.Value, MoveToTarget);
                    break;
                case DestinationType.Name:
                    var GO = GameObject.Find(RTName);
                    if (GO != null)
                    {
                        AIBrain.AIControl.SetDestination(GO.transform.position, MoveToTarget);
                    }
                    else
                    {
                        Debug.LogError("Using SetTarget.ByName() but there's no Gameobject with that name", this);
                    }
                    break;
                default:
                    break;
            }


            taskDone = true;

        }

        protected override State OnUpdate()
        {
            if (taskDone && AIBrain.AIControl.HasArrived && MoveToTarget)
            {
                return State.Success;

            }
            else if (taskDone && !MoveToTarget)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();
            if (!string.IsNullOrEmpty(description))
            {
                description += "\n";
            }

            description += "Target should be: ";
            switch (targetType)
            {
                case DestinationType.Vector3:
                    description += "Vector3";
                    break;
                case DestinationType.Transform:
                    description += "Transform Variable";

                    break;
                case DestinationType.GameObject:
                    description += "GameObject Variable";

                    break;
                case DestinationType.Name:
                    description += $"{RTName.Value}";

                    break;
                case DestinationType.RuntimeGameObjects:
                    description += "RuntimeGameObjects";
                    break;
                default:
                    break;
            }
            description += "\n";
            description += $" Move to Target: {MoveToTarget}";

            return description;
        }
    }
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MSetDestinationNode))]
    public class MSetDestinationNodeEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty nodeName, ignoreAbortSelf, Description, WaitForPreviousTask, SlowMultiplier, MessageID, targetType, TargetT, TargetG, TargetRG, rtype, RTIndex, RTName, MoveToTarget, Destination;

        private void OnEnable()
        {
            nodeName = serializedObject.FindProperty("nodeName");

            ignoreAbortSelf = serializedObject.FindProperty("ignoreAbortSelf");


            SlowMultiplier = serializedObject.FindProperty("SlowMultiplier");
            targetType = serializedObject.FindProperty("targetType");
            Destination = serializedObject.FindProperty("Destination");
            TargetT = serializedObject.FindProperty("TargetT");
            TargetG = serializedObject.FindProperty("TargetG");
            TargetRG = serializedObject.FindProperty("TargetRG");
            rtype = serializedObject.FindProperty("rtype");
            RTIndex = serializedObject.FindProperty("RTIndex");
            RTName = serializedObject.FindProperty("RTName");
            MoveToTarget = serializedObject.FindProperty("MoveToTarget");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(nodeName);
            EditorGUILayout.PropertyField(ignoreAbortSelf);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("All targets must be set at Runtime. Scriptable asset cannot have scenes References", UnityEditor.MessageType.Info);

            EditorGUILayout.PropertyField(SlowMultiplier);
            EditorGUILayout.PropertyField(targetType);

            var tt = (SetDestinationTask.DestinationType)targetType.intValue;

            switch (tt)
            {
                case SetDestinationTask.DestinationType.Transform:
                    EditorGUILayout.PropertyField(TargetT, new GUIContent("Transform Hook"));
                    break;
                case SetDestinationTask.DestinationType.GameObject:
                    EditorGUILayout.PropertyField(TargetG, new GUIContent("GameObject Hook"));
                    break;
                case SetDestinationTask.DestinationType.RuntimeGameObjects:
                    EditorGUILayout.PropertyField(TargetRG, new GUIContent("Runtime Set"));
                    EditorGUILayout.PropertyField(rtype, new GUIContent("Selection"));

                    var Sel = (RuntimeSetTypeGameObject)rtype.intValue;
                    switch (Sel)
                    {
                        case RuntimeSetTypeGameObject.Index:
                            EditorGUILayout.PropertyField(RTIndex, new GUIContent("Element Index"));
                            break;
                        case RuntimeSetTypeGameObject.ByName:
                            EditorGUILayout.PropertyField(RTName, new GUIContent("Element Name"));
                            break;
                        default:
                            break;
                    }
                    break;
                case SetDestinationTask.DestinationType.Vector3:
                    EditorGUILayout.PropertyField(Destination, new GUIContent("Global Vector3"));

                    break;
                case SetDestinationTask.DestinationType.Name:
                    EditorGUILayout.PropertyField(RTName, new GUIContent("GameObject name"));

                    break;
                default:
                    break;
            }
            EditorGUILayout.PropertyField(MoveToTarget);
            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}
