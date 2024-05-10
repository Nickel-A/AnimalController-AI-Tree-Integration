using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEditor;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Move Stop", "Animal Controller/ACMovement/Move Stop", IconPath = "Icons/AnimalAI_Icon.png")]

    public class MMoveStopNode : MTaskNode
    {
        public enum MoveType
        {
            MoveToCurrentTarget,
            MoveToNextTarget,
            LockAnimalMovement,
            Stop,
            RotateInPlace,
            Flee,
            CircleAround,
            KeepDistance,
            MoveToLastKnownDestination
        };
        public enum CircleDirection { Left, Right };

        [Tooltip("Type of the Movement task")]
        public MoveType task = MoveType.MoveToCurrentTarget;
        /// <summary> Distance for the Flee, Circle Around and keep Distance Task</summary>
        public FloatReference distance = new(10f);
        /// <summary> Distance Threshold for the Keep Distance Task</summary>
        public FloatReference distanceThreshold = new(1f);
        /// <summary> Animal Controller Stopping Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference stoppingDistance = new(0.5f);
        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);

        /// <summary> Animal Controller Stopping Distance to Override the AI Movement Stopping Distance</summary>
        public CircleDirection direction = CircleDirection.Left;

        /// <summary> Amount of Target Position around the Target</summary>
        public int arcsCount = 12;

        public bool LookAtTarget = false;

        [Tooltip("It will flee from the Target forever. If this value is false it will flee once it has reached a safe distance and the Task will end.")]
        public bool FleeForever = true;

        [Tooltip("The AI will stop if it arrives to the current target")]
        public bool StopOnArrive = true;
        float defaultStopdistance;
        Transform currentTarget;
        bool arrived;
        bool failed;
        TaskVariables taskVars = new TaskVariables();

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {
            defaultStopdistance = AIBrain.AIControl.StoppingDistance;
            AIBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;      //IMPORTANT or the animal will try to Move if the Target moves

            switch (task)
            {
                case MoveType.MoveToCurrentTarget:
                    if (AIBrain.AIControl.Target)
                    {
                        AIBrain.AIControl.SetTarget(AIBrain.AIControl.Target, true); //Reset the Target
                        AIBrain.AIControl.UpdateDestinationPosition = true;          //Check if the target has moved
                    }
                    else
                    {
                        Debug.LogWarning("The Animal does not have a current Target", this);
                        failed = true;
                    }
                    break;
                case MoveType.MoveToNextTarget:
                    if (AIBrain.AIControl.NextTarget)
                        AIBrain.AIControl.MovetoNextTarget();
                    else
                    {
                        Debug.LogWarning("The Animal does not have a next Target", this);
                        failed = true;
                    }
                    break;
                case MoveType.Stop:
                    AIBrain.AIControl.Stop();
                    AIBrain.AIControl.UpdateDestinationPosition = false;         //IMPORTANT or the animal will try to Move if the Target moves
                    arrived = true;
                    break;
                case MoveType.LockAnimalMovement:
                    AIBrain.Animal.LockMovement = true;
                    arrived = true;
                    break;
                case MoveType.RotateInPlace:
                    AIBrain.AIControl.RemainingDistance = 0;
                    AIBrain.AIControl.DestinationPosition = AIBrain.AIControl.Transform.position;//Set yourself as the Destination Pos
                    AIBrain.AIControl.LookAtTargetOnArrival = true;          //Set the Animal to look Forward to the Target
                    AIBrain.AIControl.UpdateDestinationPosition = false;          //Set the Animal to look Forward to the Target
                    AIBrain.AIControl.HasArrived = true;      //Set the Stopping Distance to almost nothing that way the animal keeps trying to go towards the target
                    AIBrain.AIControl.Stop();
                    arrived = true;
                    break;
                case MoveType.Flee:
                    AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
                    Flee(AIBrain);
                    break;
                case MoveType.KeepDistance:
                    AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
                    KeepDistance(AIBrain);
                    break;
                case MoveType.CircleAround:
                    AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
                    CalculateClosestCirclePoint(AIBrain);
                    break;
                case MoveType.MoveToLastKnownDestination:
                    var LastDestination = AIBrain.AIControl.DestinationPosition; //Store the Last Destination
                    Debug.DrawRay(AIBrain.Position, Vector3.up, Color.white, 1);
                    AIBrain.AIControl.DestinationPosition = Vector3.zero;
                    AIBrain.AIControl.SetDestination(LastDestination, true); //Go to the last Destination position
                    AIBrain.AIControl.UpdateDestinationPosition = false;          //Set the Animal to look Forward to the Target
                    AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
                    break;
                default:
                    break;
            }
        }



        protected override State OnUpdate()
        {
            switch (task)
            {
                case MoveType.MoveToCurrentTarget: StopOnArrived(); break;
                case MoveType.MoveToNextTarget:
                    {
                        if (currentTarget != AIBrain.Target)
                        {
                            StopOnArrived();
                        }
                        break;
                    }
                case MoveType.Flee: Flee(AIBrain); break;
                case MoveType.KeepDistance: KeepDistance(AIBrain); break;
                case MoveType.CircleAround: CircleAround(AIBrain); break;
                case MoveType.MoveToLastKnownDestination:
                    if (AIBrain.AIControl.HasArrived)
                    {
                        OnTargetArrived(AIBrain);
                        AIBrain.AIControl.Stop();
                        arrived = true;
                    }
                    break;
                default: break;
            }
            if (failed)
            {
                return State.Failure;
            }

            if (arrived)
            {
                return State.Success;
            }
            return State.Running;
        }

        protected override void OnExit()
        {
            switch (task)
            {
                case MoveType.LockAnimalMovement: AIBrain.Animal.LockMovement = false; break;
                default: break;
            }
            failed = false;
            arrived = false;
            AIBrain.AIControl.StoppingDistance = defaultStopdistance;
        }

        private void StopOnArrived()
        {
            if (AIBrain.AIControl.HasArrived)
            {
                AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target

                if (StopOnArrive)
                {
                    AIBrain.AIControl.AutoNextTarget = false;
                    AIBrain.AIControl.Stop();
                }

                AIBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;
                arrived = true;
            }
        }


        void OnTargetArrived(AIBrain AIBrain)
        {
            switch (task)
            {
                case MoveType.MoveToCurrentTarget: StopOnArrived(); break;
                case MoveType.MoveToNextTarget: StopOnArrived(); break;
                case MoveType.LockAnimalMovement: arrived = true; break;
                case MoveType.Stop: arrived = true; break;
                case MoveType.RotateInPlace: arrived = true; break;

                //Do nothing!!
                case MoveType.Flee: break;
                case MoveType.CircleAround: break;
                case MoveType.KeepDistance: break;

                case MoveType.MoveToLastKnownDestination: AIBrain.AIControl.Stop(); arrived = true; break;
                default:
                    break;
            }
        }

        private void CalculateClosestCirclePoint(AIBrain AIBrain)
        {
            if (AIBrain.Target == null)
            {
                return;
            }

            float arcDegree = 360.0f / arcsCount;
            int Dir = direction == CircleDirection.Right ? 1 : -1;
            Quaternion rotation = Quaternion.Euler(0, Dir * arcDegree, 0);

            Vector3 currentDirection = Vector3.forward;
            Vector3 MinPoint = Vector3.zero;
            float minDist = float.MaxValue;

            int MinIndex = 0;

            for (int i = 0; i < arcsCount; ++i)
            {
                var CurrentPoint = AIBrain.Target.position + (currentDirection.normalized * distance);

                float DistCurrentPoint = Vector3.Distance(CurrentPoint, AIBrain.transform.position);

                if (minDist > DistCurrentPoint)
                {
                    minDist = DistCurrentPoint;
                    MinIndex = i;
                    MinPoint = CurrentPoint;
                }

                currentDirection = rotation * currentDirection;
            }

            AIBrain.AIControl.UpdateDestinationPosition = false;
            AIBrain.AIControl.StoppingDistance = stoppingDistance;

            taskVars.IntValue = MinIndex;
            taskVars.BoolValue = true;

            AIBrain.AIControl.UpdateDestinationPosition = false;
            AIBrain.AIControl.SetDestination(MinPoint, true);
            AIBrain.AIControl.HasArrived = false;
        }

        private void CircleAround(AIBrain AIBrain)
        {
            if (AIBrain.AIControl.HasArrived)
            {
                taskVars.IntValue++;
                taskVars.IntValue = taskVars.IntValue % arcsCount;
                taskVars.BoolValue = true;
            }

            if (taskVars.BoolValue || AIBrain.AIControl.TargetIsMoving)
            {
                int pointIndex = taskVars.IntValue;
                float arcDegree = 360.0f / arcsCount;
                int Dir = direction == CircleDirection.Right ? 1 : -1;
                Quaternion rotation = Quaternion.Euler(0, Dir * arcDegree * pointIndex, 0);

                Vector3 currentDirection = Vector3.forward;
                currentDirection = rotation * currentDirection;

                Vector3 CurrentPoint = AIBrain.Target.position + (currentDirection.normalized * distance);

                AIBrain.AIControl.UpdateDestinationPosition = false;
                AIBrain.AIControl.SetDestination(CurrentPoint, true);

                taskVars.BoolValue = false;
            }
        }

        private void KeepDistance(AIBrain AIBrain)
        {
            
            if (AIBrain.Target)
            {
                AIBrain.AIControl.UpdateDestinationPosition = true;

                AIBrain.AIControl.StoppingDistance = stoppingDistance;
                //AIBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;

                Vector3 KeepDistPoint = AIBrain.Animal.transform.position;

                var DirFromTarget = KeepDistPoint - AIBrain.Target.position;

                float halThreshold = distanceThreshold * 0.5f;
                float TargetDist = DirFromTarget.magnitude;

                var distance = this.distance * AIBrain.Animal.ScaleFactor; //Remember to use the scale

                if (TargetDist < distance - distanceThreshold) //Flee 
                {
                    float DistanceDiff = distance - TargetDist;
                    KeepDistPoint = CalculateDistance(AIBrain, DirFromTarget, DistanceDiff, halThreshold);
                    // AIBrain.TaskDone(index,false);


                }
                else if (TargetDist > distance + distanceThreshold) //Go to Target
                {
                    float DistanceDiff = TargetDist - distance;
                    KeepDistPoint = CalculateDistance(AIBrain, -DirFromTarget, DistanceDiff, -halThreshold);
                    //AIBrain.TaskDone(index, false);
                }
                else
                {
                    if (!AIBrain.AIControl.HasArrived)
                    {
                        AIBrain.AIControl.Stop(); //It need to stop
                    }
                    else
                    {
                        AIBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;

                    }


                    AIBrain.AIControl.HasArrived = true;
                    AIBrain.AIControl.StoppingDistance = distance + distanceThreshold; //Force to have a greater Stopping Distance so the animal can rotate around the target
                    AIBrain.AIControl.RemainingDistance = 0; //Force the remaining distance to be 0
                    //  AIBrain.TaskDone(index,false);
                    arrived = true;
                }
            }
        }

        private Vector3 CalculateDistance(AIBrain AIBrain, Vector3 DirFromTarget, float DistanceDiff, float halThreshold)
        {
            Vector3 KeepDistPoint = AIBrain.transform.position + (DirFromTarget.normalized * (DistanceDiff + halThreshold));
            AIBrain.AIControl.UpdateDestinationPosition = false; //Means the Animal Wont Update the Destination Position with the Target position.
            AIBrain.AIControl.StoppingDistance = stoppingDistance;
            AIBrain.AIControl.SetDestination(KeepDistPoint, true);
            return KeepDistPoint;
        }


        private void Flee(AIBrain AIBrain)
        {
            if (AIBrain.Target)
            {
                //Animal wont Update the Destination Position with the Target position. We need to go to the opposite side
                AIBrain.AIControl.UpdateDestinationPosition = false;

                //We can look at the target on arrival, we are fleeing from the target!
                // AIBrain.AIControl.LookAtTargetOnArrival = false;

                var CurrentPos = AIBrain.Animal.transform.position;

                var AgentDistance = Vector3.Distance(AIBrain.Animal.transform.position, AIBrain.Position);
                var TargetDirection = CurrentPos - AIBrain.Target.position;

                float TargetDistance = TargetDirection.magnitude;

                var distance = this.distance * AIBrain.Animal.ScaleFactor; //Remember to use the scale

                if (TargetDistance < distance)
                {
                    //player is too close from us, pick a point diametrically oppossite at twice that distance and try to move there.
                    Vector3 fleePoint = AIBrain.Target.position + (TargetDirection.normalized * (distance + (AgentDistance * 2f)));

                    AIBrain.AIControl.StoppingDistance = stoppingDistance;

                    Debug.DrawRay(fleePoint, Vector3.up * 3, Color.blue, 2f);

                    //If the New flee Point is not in the Stopping distance radius then set a new Flee Point
                    if (Vector3.Distance(CurrentPos, fleePoint) > stoppingDistance)
                    {
                        AIBrain.AIControl.UpdateDestinationPosition = false; //Means the Animal wont Update the Destination Position with the Target position.
                        AIBrain.AIControl.SetDestination(fleePoint, true);

                        if (AIBrain.debug)
                        {
                            Debug.DrawRay(fleePoint, AIBrain.transform.up, Color.blue, 2f);
                        }
                    }
                }
                else
                {
                    AIBrain.AIControl.Stop();
                    AIBrain.AIControl.LookAtTargetOnArrival = LookAtTarget;

                    //if (!FleeForever) taskDone=true; //If flee forever is false then flee is finished
                }
            }
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();


            switch (task)
            {
                case MoveType.MoveToCurrentTarget:
                    description += "Move to current target\n";
                    description += $"Look at Target: {LookAtTarget}\n";
                    description += $"Stop on Arrive: {StopOnArrive}\n";
                    break;
                case MoveType.MoveToNextTarget:
                    description += "Move to next target\n";
                    description += $"Look at Target: {LookAtTarget}\n";
                    description += $"Stop on Arrive: {StopOnArrive}\n";
                    break;
                case MoveType.LockAnimalMovement:
                    description += "Lock animal movement\n";
                    description += $"Look at Target: {LookAtTarget}\n";
                    break;
                case MoveType.Stop:
                    description += "Stop\n";
                    description += $"Look at Target: {LookAtTarget}\n";
                    break;
                case MoveType.RotateInPlace:
                    description += "Rotate in place\n";
                    break;
                case MoveType.Flee:
                    description += "Flee\n";                    
                    description += $"Flee forever: {FleeForever}\n";
                    description += $"Distance: {distance.Value}\n";
                    description += $"Stopping Distance: {stoppingDistance.Value}\n";
                    description += $"Slowing Distance: {slowingDistance.Value}\n";
                    description += $"Look at Target: {LookAtTarget}\n";
                    break;
                case MoveType.CircleAround:
                    description += "Circle around\n";
                    description += $"Distance: {distance.Value}\n";
                    description += $"Stopping Distance: {stoppingDistance.Value}\n";
                    description += $"Slowing Distance: {slowingDistance.Value}\n";
                    description += $"Circle Direction: {direction}\n";
                    description += $"Arcs Count: {arcsCount}\n";
                    break;
                case MoveType.KeepDistance:
                    description += "Keep distance\n";
                    description += $"Distance: {distance.Value}\n";
                    description += $"Distance Threshold: {distanceThreshold.Value}\n";
                    description += $"Stopping Distance: {stoppingDistance.Value}\n";
                    description += $"Slowing Distance: {slowingDistance.Value}\n";
                    description += $"Look at Target: {LookAtTarget}\n";
                    break;
                case MoveType.MoveToLastKnownDestination:
                    description += "Move to last known destination\n";
                    break;
            }
            return description;
        }



    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MMoveStopNode)), CanEditMultipleObjects]
    public class MMoveStopNodeEditor : Editor
    {
        SerializedProperty
            Description, distance, debugColor, distanceThreshold, active, WaitForPreviousTask,
            stoppingDistance, task, Direction, UpdateInterval, slowingDistance, FleeForever, StopOnArrive,
            ignoreAbortSelf, arcsCount, LookAtTarget, nodeName
            ;

        private void OnEnable()
        {
            //  script = MonoScript.FromScriptableObject((ScriptableObject)target);
            nodeName = serializedObject.FindProperty("nodeName");

            ignoreAbortSelf = serializedObject.FindProperty("ignoreAbortSelf");
            active = serializedObject.FindProperty("active");
            FleeForever = serializedObject.FindProperty("FleeForever");
            Description = serializedObject.FindProperty("Description");
            WaitForPreviousTask = serializedObject.FindProperty("WaitForPreviousTask");
            task = serializedObject.FindProperty("task");
            arcsCount = serializedObject.FindProperty("arcsCount");
            StopOnArrive = serializedObject.FindProperty("StopOnArrive");
            distance = serializedObject.FindProperty("distance");
            Direction = serializedObject.FindProperty("direction");
            distanceThreshold = serializedObject.FindProperty("distanceThreshold");
            UpdateInterval = serializedObject.FindProperty("UpdateInterval");
            debugColor = serializedObject.FindProperty("debugColor");
            stoppingDistance = serializedObject.FindProperty("stoppingDistance");
            LookAtTarget = serializedObject.FindProperty("LookAtTarget");
            slowingDistance = serializedObject.FindProperty("slowingDistance");
            //  Interact = serializedObject.FindProperty("Interact");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(nodeName);
            EditorGUILayout.PropertyField(ignoreAbortSelf);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);
                MoveStopTask.MoveType taskk = (MoveStopTask.MoveType)task.intValue;
                string Help = GetTaskType(taskk);
                EditorGUILayout.PropertyField(task, new GUIContent("Task", Help));

                switch (taskk)
                {
                    case MoveStopTask.MoveType.LockAnimalMovement: LookAt_T(); break;
                    case MoveStopTask.MoveType.Stop: LookAt_T(); break;
                    case MoveStopTask.MoveType.RotateInPlace: break;
                    case MoveStopTask.MoveType.MoveToCurrentTarget:
                        LookAt_T();
                        Interact_();
                        EditorGUILayout.PropertyField(StopOnArrive); break;

                    case MoveStopTask.MoveType.MoveToNextTarget:
                        LookAt_T();
                        Interact_();
                        EditorGUILayout.PropertyField(StopOnArrive); break;

                    case MoveStopTask.MoveType.Flee:
                        EditorGUILayout.PropertyField(FleeForever);
                        EditorGUILayout.PropertyField(distance, new GUIContent("Distance", "Flee Safe Distance away from the Target"));
                        EditorGUILayout.PropertyField(stoppingDistance, new GUIContent("Stop Distance", "Stopping Distance of the Flee point"));
                        EditorGUILayout.PropertyField(slowingDistance);
                        LookAt_T();
                        break;
                    case MoveStopTask.MoveType.CircleAround:
                        EditorGUILayout.PropertyField(distance, new GUIContent("Distance", "Flee Safe Distance away from the Target"));
                        EditorGUILayout.PropertyField(stoppingDistance, new GUIContent("Stop Distance", "Stopping Distance of the Circle Around Points"));
                        EditorGUILayout.PropertyField(slowingDistance);
                        EditorGUILayout.PropertyField(Direction, new GUIContent("Direction", "Direction to Circle around the Target... left or right"));
                        EditorGUILayout.PropertyField(arcsCount, new GUIContent("Arc Count", "Amount of Point to Form a Circle around the Target"));

                        break;
                    case MoveStopTask.MoveType.KeepDistance:
                        EditorGUILayout.PropertyField(distance);
                        EditorGUILayout.PropertyField(distanceThreshold);
                        EditorGUILayout.PropertyField(stoppingDistance);
                        EditorGUILayout.PropertyField(slowingDistance);
                        LookAt_T();
                        break;
                    default:
                        break;
                }


                EditorGUILayout.Space();
                MalbersEditor.DrawDescription(taskk.ToString() + ":\n" + Help);

            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Movement Task Inspector");
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void Interact_()
        {
            //  EditorGUILayout.PropertyField(Interact, new GUIContent("Interact", "If we Arrived to the Target and is Interactable, Interact!"));
        }

        private void LookAt_T()
        {
            EditorGUILayout.PropertyField(LookAtTarget, new GUIContent("Look at Target", "If we Arrived to the Target then Keep Looking At it"));
        }

        private string GetTaskType(MoveStopTask.MoveType taskk)
        {
            switch (taskk)
            {
                case MoveStopTask.MoveType.MoveToCurrentTarget:
                    return "The Animal will move towards current assigned Target.";
                case MoveStopTask.MoveType.MoveToNextTarget:
                    return "The Animal will move towards the Next Target, if it has a Current Target that is a Waypoint and has a Next Target.";
                case MoveStopTask.MoveType.LockAnimalMovement:
                    return "The Animal will Stop moving. [Animal.LockMovement] will be |True| at Start; and it will be |False| at the end of the Task.";
                case MoveStopTask.MoveType.Stop:
                    return "The Animal will Stop the Agent from moving. Calling AIAnimalControl.Stop(). \nIt will keep the Current Target Assigned.";
                case MoveStopTask.MoveType.RotateInPlace:
                    return "The Animal will not move but it will rotate on the Spot towards the current Target Direction.";
                case MoveStopTask.MoveType.Flee:
                    return "The Animal will move away from the current target until it reaches a the safe distance.";
                case MoveStopTask.MoveType.CircleAround:
                    return "The Animal will Circle around the current Target from a safe distance.";
                case MoveStopTask.MoveType.KeepDistance:
                    return "The Animal will Keep a safe distance from the target\nIf the distance is too close it will flee\nIf the distance is too far it will come near the Target.";
                case MoveStopTask.MoveType.MoveToLastKnownDestination:
                    return "The Animal will Move to the Last Know destination of the Previous Target";
                default:
                    return string.Empty;
            }
        }
    }
#endif
}
