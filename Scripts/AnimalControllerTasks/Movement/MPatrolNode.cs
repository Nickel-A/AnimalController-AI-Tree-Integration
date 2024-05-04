using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEditor;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{


    [NodeContent("Patrol", "Animal Controller/ACMovement/Patrol", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MPatrolNode : TaskNode
    {
        [Tooltip("The Animal will Rotate/Look at the Target when he arrives to it")]
        public bool LookAtOnArrival = false;

        [Tooltip("Ignores the Wait time of all waypoints")]
        public bool IgnoreWaitTime = false;

        public PatrolType patrolType = PatrolType.LastWaypoint;

        [Tooltip("Use a Runtime GameObjects Set to find the Next waypoint")]
        public RuntimeGameObjects RuntimeSet;
        public RuntimeSetTypeGameObject rtype = RuntimeSetTypeGameObject.Random;
        public IntReference RTIndex = new();
        public StringReference RTName = new();
        private AIBrain aiBrain;
        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();

            aiBrain.AIControl.AutoNextTarget = true; //When Patrolling make sure AutoTarget is set to true... 

            switch (patrolType)
            {
                case PatrolType.LastWaypoint:
                    if (aiBrain.LastWayPoint != null)                                         //If we had a last Waypoint then move to it
                    {
                        aiBrain.TargetAnimal = null;                                          //Clean the Animal Target in case it was one
                        aiBrain.AIControl.SetTarget(aiBrain.LastWayPoint.WPTransform, true);    //Move to the last waypoint the animal  used
                    }
                    break;
                case PatrolType.UseRuntimeSet:
                    if (RuntimeSet != null)                                             //If we had a last Waypoint then move to it
                    {
                        aiBrain.TargetAnimal = null;                                      //Clean the Animal Target in case it was one
                        GameObject go = RuntimeSet.GetItem(rtype, RTIndex, RTName, aiBrain.Animal.gameObject);
                        if (go)
                        {
                            aiBrain.AIControl.SetTarget(go.transform, true);
                        }

                        break;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override State OnUpdate()
        {
            if (aiBrain.AIControl.HasArrived)
            {
                OnTargetArrived();
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }

        protected override void OnExit()
        {
            aiBrain.AIControl.StopWait(); //Remove in case it was waiting , when the State is interrupted.
        }

        void OnTargetArrived()
        {
            aiBrain.AIControl.AutoNextTarget = true; //When Patrolling make sure AutoTarget is set to true... 

            switch (patrolType)
            {
                case PatrolType.LastWaypoint:
                    if (IgnoreWaitTime)
                    {
                        aiBrain.AIControl.StopWait(); //Ingore wait time
                        aiBrain.AIControl.SetTarget(aiBrain.AIControl.NextTarget, true);
                    }
                    break;
                case PatrolType.UseRuntimeSet:

                    GameObject NextTarget = RuntimeSet.GetItem(rtype, RTIndex, RTName, aiBrain.Animal.gameObject);
                    if (NextTarget && aiBrain.AIControl.NextTarget == null)
                    {
                        if (IgnoreWaitTime)
                        {
                            aiBrain.AIControl.StopWait(); //Ingore wait time
                            aiBrain.AIControl.SetTarget(NextTarget.transform, true);
                        }
                        else
                        {
                            aiBrain.AIControl.SetNextTarget(NextTarget);
                            aiBrain.AIControl.MovetoNextTarget();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public override string GetDescription()
        {
            string description = base.GetDescription();


            switch (patrolType)
            {
                case PatrolType.LastWaypoint:
                    description += "Last Waypoint\n";
                    break;
                case PatrolType.UseRuntimeSet:
                    //description += "Patrol Type: Use Runtime Set\n";
                    if (RuntimeSet != null)
                    {
                        description += $"Runtime Set: {RuntimeSet.name}\n";

                    }
                    else description += "Runtime Set: null\n";

                    switch (rtype)
                    {
                        case RuntimeSetTypeGameObject.First:
                            description += "Get First\n";
                            break;
                        case RuntimeSetTypeGameObject.Random:
                            description += "Get Random\n";
                            break;
                        case RuntimeSetTypeGameObject.Index:
                            description += $"Get Index: {RTIndex.Value}\n";
                            break;
                        case RuntimeSetTypeGameObject.ByName:
                            description += $"Get By Name: {RTName.Value}\n";
                            break;
                        case RuntimeSetTypeGameObject.Closest:
                            description += "Get Closest\n";
                            break;
                    }
                    break;
            }
            description += $"Look at on arrival: {LookAtOnArrival}\n";
            description += $"Ignore wait time: {IgnoreWaitTime}\n";
            return description;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(MPatrolNode))]
    public class MPatrolNodeEditor : Editor
    {
        SerializedProperty Description, MessageID, patrolType, RuntimeSet, rtype, RTIndex, RTName,
            WaitForPreviousTask, LookAtOnArrival, IgnoreWaitTime, nodeName, ignoreAbortSelf;

        private void OnEnable()
        {

            nodeName = serializedObject.FindProperty("nodeName");

            ignoreAbortSelf = serializedObject.FindProperty("ignoreAbortSelf");

            WaitForPreviousTask = serializedObject.FindProperty("WaitForPreviousTask");
            Description = serializedObject.FindProperty("Description");
            MessageID = serializedObject.FindProperty("MessageID");
            patrolType = serializedObject.FindProperty("patrolType");
            rtype = serializedObject.FindProperty("rtype");
            RTIndex = serializedObject.FindProperty("RTIndex");
            RTName = serializedObject.FindProperty("RTName");
            RuntimeSet = serializedObject.FindProperty("RuntimeSet");
            LookAtOnArrival = serializedObject.FindProperty("LookAtOnArrival");
            IgnoreWaitTime = serializedObject.FindProperty("IgnoreWaitTime");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(nodeName);
            EditorGUILayout.PropertyField(ignoreAbortSelf);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(patrolType);

            var tt = (PatrolType)patrolType.intValue;

            switch (tt)
            {
                case PatrolType.LastWaypoint:

                    break;
                case PatrolType.UseRuntimeSet:

                    EditorGUILayout.PropertyField(RuntimeSet);
                    EditorGUILayout.PropertyField(rtype, new GUIContent("Get"));
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

                default:
                    break;
            }

            EditorGUILayout.PropertyField(LookAtOnArrival);
            EditorGUILayout.PropertyField(IgnoreWaitTime);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
