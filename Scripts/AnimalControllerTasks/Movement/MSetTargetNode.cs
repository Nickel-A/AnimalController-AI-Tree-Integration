using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEditor;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Target", "Animal Controller/ACMovement/Set Target", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetTargetNode : TaskNode
    {
        public enum TargetToFollow { Transform, GameObject, RuntimeGameObjects, ClearTarget, Name, BBKey }

        [Space]
        public TargetToFollow targetType = TargetToFollow.Transform;

        [RequiredField] public TransformVar TargetT;
        [RequiredField] public GameObjectVar TargetG;
        [RequiredField] public RuntimeGameObjects TargetRG;
        [RequiredField] public TransformKey BBKey;

        public RuntimeSetTypeGameObject rtype = RuntimeSetTypeGameObject.Random;
        public IntReference RTIndex = new();
        public StringReference RTName = new();



        [Tooltip("When a new target is assinged it also sets that the Animal should move to that target")]
        public bool MoveToTarget = true;
        bool taskDone;
        AIBrain aiBrain;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();
        }
        protected override void OnEntry()
        {

            if (MoveToTarget)
            {
                aiBrain.AIControl.UpdateDestinationPosition = true;          //Check if the target has moved
            }
            else
            {
                if (aiBrain.AIControl.IsMoving) { aiBrain.AIControl.Stop(); } //Stop if the animal is already moving
            }

            switch (targetType)
            {
                case TargetToFollow.Transform:                    
                        aiBrain.AIControl.SetTarget(TargetT.Value, MoveToTarget);
                    break;
                case TargetToFollow.GameObject:
                    aiBrain.AIControl.SetTarget(TargetG.Value.transform, MoveToTarget);
                    break;
                case TargetToFollow.RuntimeGameObjects:
                    if (TargetRG != null && !TargetRG.IsEmpty)
                    {
                        var target = TargetRG.GetItem(rtype, RTIndex, RTName, aiBrain.Animal.gameObject);
                        if (target)
                        {
                            aiBrain.AIControl.SetTarget(target.transform, MoveToTarget);
                        }
                    }
                    break;
                case TargetToFollow.ClearTarget:
                    aiBrain.AIControl.ClearTarget();
                    break;
                case TargetToFollow.Name:
                    var GO = GameObject.Find(RTName);
                    if (GO != null)
                    {
                        aiBrain.AIControl.SetTarget(GO.transform, MoveToTarget);
                    }
                    break;
                case TargetToFollow.BBKey:
                    aiBrain.AIControl.SetTarget(BBKey.GetValue(), MoveToTarget);
                    break;
                default:
                    break;
            }

            taskDone = true;
        }

        protected override State OnUpdate()
        {
            if (taskDone)
            {
                if (MoveToTarget && !aiBrain.AIControl.HasArrived)
                {
                    return State.Running;
                }
                else
                {
                    return State.Success;
                }
            }
            else
            {
                return State.Running;
            }
        }
        protected override void OnExit()
        {
            base.OnExit();
            if (!MoveToTarget)
            aiBrain.AIControl.Stop();
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();
            if (!string.IsNullOrEmpty(description))
            {
                description += "\n";
            }
            description += $"Target Type: {targetType}\n";
            switch (targetType)
            {
                case TargetToFollow.Transform:
                    //    if (TargetT != null)
                    //    {
                    //        description += $"Set Target: {TargetT.Value.name}\n";
                    //    }
                    //    else
                    //    {
                    //        description += "Set Target: None\n";
                    //    }
                    description += "Set Target: Transform\n";
                    break;
                case TargetToFollow.GameObject:
                    if (TargetG != null)
                    {
                        description += $"Set Target: {TargetG.Value.name}\n";
                    }
                    else
                    {
                        description += "Set Target: None\n";
                    }
                    break;
                case TargetToFollow.RuntimeGameObjects:
                    if (TargetRG != null)
                    {
                        description += $"Set Target: {TargetRG.name}\n";
                    }
                    else
                    {
                        description += "Set Target: None\n";
                    }
                    break;
                case TargetToFollow.Name:
                    if (RTName != null)
                    {
                        description += $"Set Target: {RTName.Value}\n";
                    }
                    else
                    {
                        description += "Set Target: None\n";
                    }
                    break;
                default:
                    description += "No target set\n";
                    break;
            }

            if (MoveToTarget)
            {
                description += $"MoveToTarget: {MoveToTarget}";
            }
            else
            {
                description += $"MoveToTarget: {MoveToTarget}\n";
            }
            return description;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MSetTargetNode))]
    public class MSetTargetNodeEditor : Editor
    {
        SerializedProperty BBKey, nodeName, ignoreAbortSelf, targetType, TargetT, TargetG, TargetRG, rtype, RTIndex, RTName, MoveToTarget, TargetBB;

        private void OnEnable()
        {
            BBKey = serializedObject.FindProperty("BBKey");

            nodeName = serializedObject.FindProperty("nodeName");
            ignoreAbortSelf = serializedObject.FindProperty("ignoreAbortSelf");
            targetType = serializedObject.FindProperty("targetType");
            TargetT = serializedObject.FindProperty("TargetT");
            TargetG = serializedObject.FindProperty("TargetG");
            TargetRG = serializedObject.FindProperty("TargetRG");
            rtype = serializedObject.FindProperty("rtype");
            RTIndex = serializedObject.FindProperty("RTIndex");
            RTName = serializedObject.FindProperty("RTName");
            MoveToTarget = serializedObject.FindProperty("MoveToTarget");
            TargetBB = serializedObject.FindProperty("TargetBB");
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

            EditorGUILayout.PropertyField(targetType);

            var tt = (MSetTargetNode.TargetToFollow)targetType.intValue;

            switch (tt)
            {
                case MSetTargetNode.TargetToFollow.Transform:
                    EditorGUILayout.PropertyField(TargetT, new GUIContent("Transform Hook"));
                    break;
                case MSetTargetNode.TargetToFollow.GameObject:
                    EditorGUILayout.PropertyField(TargetG, new GUIContent("GameObject Hook"));
                    break;
                case MSetTargetNode.TargetToFollow.RuntimeGameObjects:
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
                case MSetTargetNode.TargetToFollow.ClearTarget:
                    break;
                case MSetTargetNode.TargetToFollow.Name:
                    EditorGUILayout.PropertyField(RTName, new GUIContent("GameObject name"));

                    break;
                case MSetTargetNode.TargetToFollow.BBKey:
                    EditorGUILayout.PropertyField(BBKey, new GUIContent("Blackboard Target"));
                    break;
                default:
                    break;
            }

            if (tt != MSetTargetNode.TargetToFollow.ClearTarget)
            {
                EditorGUILayout.PropertyField(MoveToTarget);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
