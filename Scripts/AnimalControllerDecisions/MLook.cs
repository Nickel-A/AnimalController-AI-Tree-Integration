using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
#endif

namespace Malbers.Integration.AITree
{
    public enum LookFor { MainAnimalPlayer, MalbersTag, UnityTag, Zones, GameObject, ClosestWayPoint, CurrentTarget, TransformVar, GameObjectVar, RuntimeGameobjectSet }

    [NodeContent("Look", "Animal Controller/Look", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLook : ObserverDecorator
    {

        [Range(0, 1)]
        /// <summary>Angle of Vision of the Animal</summary>
        [Tooltip("Shorten the Look Ray to not found the ground by mistake")]
        public float LookMultiplier = 0.9f;

        /// <summary>Range for Looking forward and Finding something</summary>
        [Tooltip("Range for Looking forward and Finding something")]
        public FloatReference LookRange = new FloatReference(15);
        [Range(0, 360)]
        /// <summary>Angle of Vision of the Animal</summary>
        [Tooltip("Angle of Vision of the Animal")]
        public float LookAngle = 120;

        /// <summary>What to look for?? </summary>
        [Space, Tooltip("What to look for??")]
        public LookFor lookFor = LookFor.MainAnimalPlayer;
        [Tooltip("Layers that can block the Animal Eyes")]
        public LayerReference ObstacleLayer = new LayerReference(1);


        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
        public bool AssignTarget = true;
        [Tooltip("If the what we are looking for is found then also start moving")]
        public bool MoveToTarget = true;
        [Tooltip("Remove Target when loose sight:\nIf the Target No longer on the Field of View: Set the Target from the AIControl as NULL")]
        public bool RemoveTarget = false;
        [Tooltip("Select randomly one of the potential targets, not the first one found")]
        public bool ChooseRandomly = false;

        [Space]
        [Tooltip("Look for this Unity Tag on an Object")]
        public string UnityTag = string.Empty;
        [Tooltip("Look for an Specific GameObject by its name")]
        public string GameObjectName = string.Empty;


        [RequiredField, Tooltip("Transform Reference value. This value should be set by a Transform Hook Component")]
        public TransformVar transform;
        [RequiredField, Tooltip("GameObject Reference value. This value should be set by a GameObject Hook Component")]
        public GameObjectVar gameObject;

        [RequiredField, Tooltip("GameObjectSet. Search for all  GameObjects Set in the Set")]
        public RuntimeGameObjects gameObjectSet;

        /// <summary>Custom Tags you want to find</summary>
        [Tooltip("Custom Tags you want to find")]
        public Tag[] tags;
        /// <summary>Type of Zone we want to find</summary>
        [Tooltip("Type of Zone we want to find")]
        // [Utilities.Flag]
        public ZoneType zoneType;

        [Tooltip("Search for all zones")]
        public bool AllZones = true;
        /// <summary>ID value of the Zone we want to find</summary>
        [Tooltip("ID value of the Zone we want to find")]
        [Min(-1)] public int ZoneID = -1;

        [Tooltip("Mode Zone Index")]
        [Min(-1)] public int ZoneModeAbility = -1;

        public Color debugColor = new Color(0, 0, 0.7f, 0.3f); AIBrain brain;
        private float interval;
        bool result;
        Decision[] DecisionsVars;
        int index = 0;

        public override event Action OnValueChange;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            brain = GetOwner().GetComponent<AIBrain>();

        }
        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }

        public override bool CalculateResult()
        {
            result = Look_For(brain, false, index);
            return result;
        }

        protected override void OnExit()
        {
            base.OnExit();
            if(result == true)
            Look_For(brain, AssignTarget, index); //This will assign the Target in case its true
        }



        protected override void OnEntry()
        {
            // Ensure the index is within the bounds of the DecisionsVars array
            if (DecisionsVars != null && index >= 0 && index < DecisionsVars.Length)
            {
                // Use the index for accessing the DecisionsVars array
                switch (lookFor)
                {
                    case LookFor.MalbersTag:
                        // Use index for the method call
                        if (Tags.TagsHolders == null || tags == null || tags.Length == 0)
                        {
                            return;
                        }

                        List<GameObject> gtags = new();

                        foreach (var t in Tags.TagsHolders)
                        {
                            if (t.gameObject.HasMalbersTag(tags))
                            {
                                gtags.Add(t.gameObject);
                            }
                        }

                        if (gtags.Count > 0)
                        {
                            DecisionsVars[index].gameobjects = gtags.ToArray();
                        }

                        break;
                    case LookFor.UnityTag:
                        if (string.IsNullOrEmpty(UnityTag))
                        {
                            return;
                        }

                        DecisionsVars[index].gameobjects = GameObject.FindGameObjectsWithTag(UnityTag);
                        break;
                    case LookFor.RuntimeGameobjectSet:
                        if (gameObjectSet == null || gameObjectSet.Count == 0)
                        {
                            return;
                        }

                        DecisionsVars[index].gameobjects = gameObjectSet.Items.ToArray();
                        break;
                    default:
                        break;
                }

                StoreColliders(brain, index); // Pass the index to the method
            }
        }


        /// <summary> Store all renderers found on the GameObjects  </summary>
        private void StoreColliders(AIBrain brain, int index)
        {
            if (DecisionsVars[index].gameobjects != null && DecisionsVars[index].gameobjects.Length > 0)
            {
                var colliders = new List<Collider>();

                for (int i = 0; i < DecisionsVars[index].gameobjects.Length; i++)
                {
                    var AllColliders = DecisionsVars[index].gameobjects[i].GetComponentsInChildren<Collider>();

                    foreach (var c in AllColliders)
                    {
                        if (!c.isTrigger && !MTools.Layer_in_LayerMask(c.gameObject.layer, ObstacleLayer.Value))
                        {
                            colliders.Add(c); //Save only good Colliders
                        }
                    }
                }
                DecisionsVars[index].AddComponents(colliders.ToArray());
            }
        }



        /// <summary>  Looks for a gameobject acording to the Look For type.</summary>
        private bool Look_For(AIBrain brain, bool assign, int index)
        {
            if (brain == null)
                return false;
            return lookFor switch
            {
                LookFor.MainAnimalPlayer => LookForAnimalPlayer(brain, assign),
                LookFor.MalbersTag => LookForMalbersTags(brain, assign, index),
                LookFor.UnityTag => LookForUnityTags(brain, assign, index),
                LookFor.Zones => LookForZones(brain, assign),
                LookFor.GameObject => LookForGameObjectByName(brain, assign),
                LookFor.ClosestWayPoint => LookForClosestWaypoint(brain, assign),
                LookFor.CurrentTarget => LookForTarget(brain, assign),
                LookFor.TransformVar => LookForTransformVar(brain, assign),
                LookFor.GameObjectVar => LookForGoVar(brain, assign),
                LookFor.RuntimeGameobjectSet => LookForGoSet(brain, assign, index),
                _ => false,
            };
        }

        public bool LookForTarget(AIBrain brain, bool assign)
        {

            if (brain.Target == null)
            {
                return false;
            }

            AssignMoveTarget(brain, brain.Target, assign);
            var Center = brain.TargetAnimal ? brain.TargetAnimal.Center : brain.Target.position;
            return IsInFieldOfView(brain, Center, out _);
        }

        public bool LookForTransformVar(AIBrain brain, bool assign)
        { 
            if (transform == null || transform.Value == null)
            {
                return false;
            }

            AssignMoveTarget(brain, transform.Value, assign);

            var Center =
                transform.Value == brain.Target && brain.AIControl.IsAITarget != null ?
                brain.AIControl.IsAITarget.GetCenterY() :
                transform.Value.position;

            return IsInFieldOfView(brain, Center, out _);
        }

        public bool LookForGoVar(AIBrain brain, bool assign)
        {
            if (gameObject == null && gameObject.Value && !gameObject.Value.IsPrefab())
            {
                return false;
            }

            AssignMoveTarget(brain, gameObject.Value.transform, assign);

            var Center =
                gameObject.Value.transform == brain.Target && brain.AIControl.IsAITarget != null ?
                brain.AIControl.IsAITarget.GetCenterY() :
                gameObject.Value.transform.position;

            return IsInFieldOfView(brain, Center, out _);
        }

        private bool IsInFieldOfView(AIBrain brain, Vector3 Center, out float Distance)
        {
            var Direction_to_Target = Center - brain.Eyes.position; //Put the Sight a bit higher

            //Important, otherwise it will find the ground for Objects to close to it. Also Apply the Scale
            Distance = Vector3.Distance(Center, brain.Eyes.position) * LookMultiplier;

            if (LookAngle == 0 || LookRange <= 0)
            {
                return true; //Means the Field of view can be ignored
            }

            if (Distance < LookRange.Value * brain.Animal.ScaleFactor) //Check if whe are inside the Look Radius
            {
                Vector3 EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, brain.Animal.UpVector);

                var angle = Vector3.Angle(Direction_to_Target, EyesForward);

                if (angle < (LookAngle / 2))
                {
                    //Need a RayCast to see if there's no obstacle in front of the Animal OBSTACLE LAYER
                    if (Physics.Raycast(brain.Eyes.position, Direction_to_Target, out RaycastHit hit, Distance, ObstacleLayer, QueryTriggerInteraction.Ignore))
                    {
                        if (brain.debug)
                        {
                            Debug.DrawRay(brain.Eyes.position, Direction_to_Target * LookMultiplier, Color.green, interval);
                            Debug.DrawLine(hit.point, Center, Color.red, interval);
                            MDebug.DrawWireSphere(Center, Color.red, interval);
                            MDebug.DrawCircle(hit.point, hit.normal, 0.1f, Color.red, true, interval);
                        }

                        return false; //Meaning there's something between the Eyes of the Animal and the Target
                    }
                    else
                    {
                        if (brain.debug)
                        {
                            Debug.DrawRay(brain.Eyes.position, Direction_to_Target, Color.green, interval);
                            MDebug.DrawWireSphere(Center, Color.green, interval);
                        }
                        return true;
                    }
                }

                return false;
            }
            //  Debug.Log($"False (NOT IN Distanc{Distance} > RANGE) {LookRange.Value}" );
            return false;
        }

        private void AssignMoveTarget(AIBrain brain, Transform target, bool assign)
        {
            if (assign && AssignTarget)
            {
                    brain.AIControl.SetTarget(target, MoveToTarget);

            }
        }

        public bool LookForZones(AIBrain brain, bool assign)
        {
            var zones = Zone.Zones;
            if (zones == null || zones.Count == 0)
            {
                return false;  //There's no zone around here
            }

            float minDistance = float.PositiveInfinity;

            Zone FoundZone = null;

            foreach (var zone in zones)
            {
                if (AllZones ||
                    (zone && zone.zoneType == zoneType &&                      //Check the same Zone Types
                    ZoneID == -1) ||                                             //Check First if its Any Zone
                    zone.ZoneID == ZoneID ||                                    //Check Zone has the same ID
                    zone.zoneType != ZoneType.Mode ||                           //Check if its not a Zone Mode
                    (zone.zoneType == ZoneType.Mode && ZoneModeAbility == -1) ||  //Check if it's a Zone Mode but the Ability its any
                    zone.ModeAbilityIndex == ZoneModeAbility)                   //Check if it's a Zone Mode AND Ability Match
                {
                    if (IsInFieldOfView(brain, zone.ZoneCollider.bounds.center, out float Distance) && Distance < minDistance)
                    {
                        minDistance = Distance;
                        FoundZone = zone;
                    }
                }
            }

            if (FoundZone)
            {
                AssignMoveTarget(brain, FoundZone.transform, assign);
                return true;
            }
            return false;
        }

        public bool LookForMalbersTags(AIBrain brain, bool assign, int index)
        {
            if (Tags.TagsHolders == null || tags == null || tags.Length == 0)
            {
                return false;
            }

            float minDistance = float.MaxValue;
            Transform Closest = null;

            var filtredTags = Tags.GambeObjectbyTag(tags);
            if (filtredTags == null)
            {
                return false;
            }

            if (ChooseRandomly)
            {
                while (filtredTags.Count != 0)
                {
                    int newIndex = Random.Range(0, filtredTags.Count);
                    var go = filtredTags[newIndex].transform;

                    if (go != null)
                    {
                        if (IsInFieldOfView(brain, go.position, out _))
                        {
                            AssignMoveTarget(brain, go, assign);
                            return true;
                        }
                    }
                    filtredTags.RemoveAt(newIndex);
                }
            }
            else
            {
                for (int i = 0; i < filtredTags.Count; i++)
                {
                    var go = filtredTags[i].transform;

                    if (go != null)
                    {
                        if (IsInFieldOfView(brain, go.position, out float Distance))
                        {
                            if (Distance < minDistance)
                            {
                                minDistance = Distance;
                                Closest = go;
                            }
                        }
                    }
                }
            }

            if (Closest)
            {
                AssignMoveTarget(brain, Closest.transform, assign);
                return true;
            }
            return false;
        }

        public bool LookForUnityTags(AIBrain brain, bool assign, int index)
        {
            if (string.IsNullOrEmpty(UnityTag))
            {
                return false;
            }

            if (ChooseRandomly)
            {
                return ChooseRandomObject(brain, assign, index);
            }

            return ClosestGameObject(brain, assign, index);
        }

        public bool LookForGoSet(AIBrain brain, bool assign, int index)
        {
            if (gameObjectSet == null || gameObjectSet.Count == 0)
            {
                return false;
            }

            if (ChooseRandomly)
            {
                return ChooseRandomObject(brain, assign, index);
            }

            return ClosestGameObject(brain, assign, index);
        }



        private bool ClosestGameObject(AIBrain brain, bool assign, int index)
        {
            // Überprüfen, ob DecisionsVars nicht null ist und ob das Array an der index-Position initialisiert wurde
            if (DecisionsVars != null && index >= 0 && index < DecisionsVars.Length && DecisionsVars[index] != null)
            {
                var All = DecisionsVars[index].gameobjects; //catch all the saved gameobjects

                if (All == null || All.Length == 0)
                {
                    return false;
                }

                float minDistance = float.MaxValue;
                GameObject closestGameObject = null;

                foreach (var go in All)
                {
                    if (go != null)
                    {
                        Vector3 center = go.transform.position;
                        Collider[] colliders = go.GetComponentsInChildren<Collider>();

                        if (colliders != null && colliders.Length > 0)
                        {
                            Vector3 boundsCenter = Vector3.zero;
                            foreach (var collider in colliders)
                            {
                                boundsCenter += collider.bounds.center;
                            }
                            center = boundsCenter / colliders.Length;
                        }

                        if (IsInFieldOfView(brain, center, out float distance))
                        {
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestGameObject = go;
                            }
                        }
                    }
                }

                if (closestGameObject != null)
                {
                    AssignMoveTarget(brain, closestGameObject.transform, assign);
                    return true;
                }
            }

            return false;
        }


        public bool ChooseRandomObject(AIBrain brain, bool assign, int index)
        {
            var All = new List<GameObject>();
            if (DecisionsVars[index] != null && DecisionsVars[index].gameobjects != null)
            {
                All.AddRange(DecisionsVars[index].gameobjects); //catch all the saved gameobjects with a tag
            }

            if (All.Count == 0)
            {
                return false;
            }

            while (All.Count != 0)
            {
                int newIndex = Random.Range(0, All.Count);
                if (All[newIndex] != null)
                {
                    var Center = All[newIndex].transform.position + new Vector3(0, brain.Animal.Height, 0);

                    if (DecisionsVars[index] != null && DecisionsVars[index].Components != null && newIndex < DecisionsVars[index].Components.Length)
                    {
                        var renderer = DecisionsVars[index].Components[newIndex];

                        if (renderer != null && renderer is Renderer)
                        {
                            Center = (renderer as Renderer).bounds.center;
                        }
                    }

                    if (IsInFieldOfView(brain, Center, out float Distance))
                    {
                        AssignMoveTarget(brain, All[newIndex].transform, assign);
                        return true;
                    }
                }
                All.RemoveAt(newIndex);
            }

            return false;
        }



        public bool LookForGameObjectByName(AIBrain brain, bool assign)
        {
            if (string.IsNullOrEmpty(GameObjectName))
            {
                return false;
            }

            var gameObject = GameObject.Find(GameObjectName);

            if (gameObject)
            {
                AssignMoveTarget(brain, gameObject.transform, assign);
                return IsInFieldOfView(brain, gameObject.transform.position, out _);   //Find if is inside the Field of view
            }
            return false;
        }

        public bool LookForClosestWaypoint(AIBrain brain, bool assign)
        {
            var allWaypoints = MWayPoint.WayPoints;
            if (allWaypoints == null || allWaypoints.Count == 0)
            {
                return false;  //There's no waypoints  around here
            }

            float minDistance = float.MaxValue;

            MWayPoint closestWayPoint = null;

            foreach (var way in allWaypoints)
            {
                var center = way.GetCenterY();
                if (IsInFieldOfView(brain, center, out float Distance))
                {
                    if (Distance < minDistance)
                    {
                        minDistance = Distance;
                        closestWayPoint = way;
                    }
                }
            }

            if (closestWayPoint)
            {
                AssignMoveTarget(brain, closestWayPoint.transform, assign);
                return true; //Find if is inside the Field of view
            }
            return false;
        }

        private bool LookForAnimalPlayer(AIBrain brain, bool assign)
        {
            if (MAnimal.MainAnimal == null || MAnimal.MainAnimal.ActiveStateID == StateEnum.Death)
            {
                return false; //Means the animal is death or Disable
            }

            if (MAnimal.MainAnimal == brain.Animal) { Debug.LogError("AI Animal is set as MainAnimal. Fix it!", brain.Animal); return false; }

            AssignMoveTarget(brain, MAnimal.MainAnimal.transform, assign);
            return IsInFieldOfView(brain, MAnimal.MainAnimal.Center, out _);
        }


        public override string GetDescription()
        {
            string description = base.GetDescription() + "\n";

            switch (lookFor)
            {
                case LookFor.MainAnimalPlayer:
                    description += "Look For Main Animal Player";
                    break;
                case LookFor.MalbersTag:
                    description += "Look For Malbers Tag: \n";
                    if (tags != null)
                    {
                        for (int i = 0; i < tags.Length; i++)
                        {
                            if (tags[i] != null)
                            {
                                description += $"{tags[i].DisplayName}";
                            }
                            if (i != tags.Length - 1) description += ", ";
                        }
                    }
                    if (ChooseRandomly)
                        description += $"\nChoose Randomly: {ChooseRandomly} \n";
                    break;
                case LookFor.UnityTag:
                    description += "Look For Unity Tag:\n";
                    if (!string.IsNullOrEmpty(UnityTag))
                    {
                        description += UnityTag;
                    }
                    if (ChooseRandomly)
                        description += $"\nChoose Randomly: {ChooseRandomly} \n";
                    break;
                case LookFor.Zones:
                    description += "Look for";
                    if (!AllZones)
                    {
                        switch (zoneType)
                        {
                            case ZoneType.Mode:
                                description += $" ZoneType: {ZoneType.Mode}\nID: {ZoneID}\nAbility: {ZoneModeAbility}";
                                break;
                            case ZoneType.State:
                                description += $" ZoneType: {ZoneType.State}\nID: {ZoneID}";
                                break;
                            case ZoneType.Stance:
                                description += $" ZoneType: {ZoneType.Stance}\nID: {ZoneID}";
                                break;
                            case ZoneType.Force:
                                description += $" ZoneType: {ZoneType.Force}\nID: {ZoneID}";
                                break;
                            case ZoneType.ReactionsOnly:
                                description += $" ZoneType: {ZoneType.ReactionsOnly}\nID: {ZoneID}";
                                break;
                        }
                    }
                    else
                    { description += " all Zones"; }
                    break;
                case LookFor.GameObject:
                    description += "Look For GameObject: \n"; 
                    if (!string.IsNullOrEmpty(GameObjectName))
                    {
                        description += GameObjectName;
                    }
                    break;
                case LookFor.ClosestWayPoint:
                    description += "Look For Closest WayPoint";
                    break;
                case LookFor.CurrentTarget:
                    description += "Look For Current Target";
                    break;
                case LookFor.TransformVar:
                    description += "Look For Transform Var"; 
                    if (transform != null)
                    {
                        description += $"\n{transform.name}";

                    }
                    else description += "\nnull";
                    break;
                case LookFor.GameObjectVar:
                    description += "Look For GameObject Var"; 
                    if (gameObject != null)
                    {
                        description += $"\n{gameObject.name}";

                    }
                    else description += "\nnull";

                    break;
                case LookFor.RuntimeGameobjectSet:
                    description += "Look For Runtime Gameobject Set"; 
                    if (gameObjectSet != null)
                    {
                        description += $"\n{gameObjectSet.name}";

                    }
                    else description += "\nnull";
                    if (ChooseRandomly)
                        description += $"\nChoose Randomly: {ChooseRandomly} \n";
                    break;
            }
            description += "\n";
            
            if (AssignTarget)
                description += $"Assign Target: {AssignTarget} \n";
            if (MoveToTarget)
                description += $"Move To Target: {MoveToTarget} \n";

            return description;            
        }


#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            var Eyes = brain.Eyes;

            var scale = brain.Animal ? brain.Animal.ScaleFactor : brain.transform.root.localScale.y;

            if (Eyes != null)
            {
                Color c = debugColor;
                c.a = 1f;

                Vector3 EyesForward = Vector3.ProjectOnPlane(brain.Eyes.forward, Vector3.up);

                Vector3 rotatedForward = Quaternion.Euler(0, -LookAngle * 0.5f, 0) * EyesForward;
                UnityEditor.Handles.color = c;
                UnityEditor.Handles.DrawWireArc(Eyes.position, Vector3.up, rotatedForward, LookAngle, LookRange * scale);
                UnityEditor.Handles.color = debugColor;
                UnityEditor.Handles.DrawSolidArc(Eyes.position, Vector3.up, rotatedForward, LookAngle, LookRange * scale);
            }
        }
#endif
    }
    [System.Serializable]
    public class Decision
    {
        public GameObject[] gameobjects;
        public Component[] Components;

        public void AddComponents(Component[] components)
        {
            Components = components;
        }
    }



    /// <summary>  Inspector!!!  </summary>

#if UNITY_EDITOR

    [CustomEditor(typeof(MLook))]
    [CanEditMultipleObjects]
    public class MLookEditor : Editor
    {
        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));

        SerializedProperty
            observerAbort,notifyObserver, nodeName, UnityTag, debugColor, zoneType, 
            ZoneID, tags, LookRange, LookAngle, lookFor, transform, gameobject, 
            gameObjectSet, AllZones, LookMultiplier, ObstacleLayer, MoveToTarget, 
            AssignTarget, GameObjectName, ZoneModeIndex, ChooseRandomly;

        //MonoScript script;
        private void OnEnable()
        {
            // script = MonoScript.FromScriptableObject((ScriptableObject)target);

            nodeName = serializedObject.FindProperty("nodeName");
            notifyObserver = serializedObject.FindProperty("notifyObserver");
            observerAbort = serializedObject.FindProperty("observerAbort");
            tags = serializedObject.FindProperty("tags");
            ChooseRandomly = serializedObject.FindProperty("ChooseRandomly");
            GameObjectName = serializedObject.FindProperty("GameObjectName");
            UnityTag = serializedObject.FindProperty("UnityTag");
            LookRange = serializedObject.FindProperty("LookRange");
            zoneType = serializedObject.FindProperty("zoneType");
            lookFor = serializedObject.FindProperty("lookFor");
            LookAngle = serializedObject.FindProperty("LookAngle");
            ObstacleLayer = serializedObject.FindProperty("ObstacleLayer");
            AssignTarget = serializedObject.FindProperty("AssignTarget");
            MoveToTarget = serializedObject.FindProperty("MoveToTarget");
            debugColor = serializedObject.FindProperty("debugColor");
            ZoneID = serializedObject.FindProperty("ZoneID");
            ZoneModeIndex = serializedObject.FindProperty("ZoneModeAbility");
            transform = serializedObject.FindProperty("transform");
            gameobject = serializedObject.FindProperty("gameObject");
            gameObjectSet = serializedObject.FindProperty("gameObjectSet");
            AllZones = serializedObject.FindProperty("AllZones");
            LookMultiplier = serializedObject.FindProperty("LookMultiplier");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(nodeName);
                EditorGUILayout.LabelField("Flow Control", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(notifyObserver);
                EditorGUILayout.PropertyField(observerAbort);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(debugColor);

                EditorGUILayout.PropertyField(LookRange);
                EditorGUILayout.PropertyField(LookAngle);
                EditorGUILayout.PropertyField(LookMultiplier);
                EditorGUILayout.PropertyField(ObstacleLayer);

                EditorGUILayout.PropertyField(lookFor);

                LookFor lookforval = (LookFor)lookFor.intValue;

                switch (lookforval)
                {
                    case LookFor.MainAnimalPlayer:
                        break;
                    case LookFor.MalbersTag:
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(tags, true);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.PropertyField(ChooseRandomly);
                        break;
                    case LookFor.UnityTag:
                        EditorGUILayout.PropertyField(UnityTag);
                        EditorGUILayout.PropertyField(ChooseRandomly);
                        break;
                    case LookFor.Zones:
                        var ZoneName = ((ZoneType)zoneType.intValue).ToString();

                        EditorGUILayout.PropertyField(AllZones);

                        if (!AllZones.boolValue)
                        {
                            EditorGUILayout.PropertyField(zoneType, new GUIContent("Zone Type", "Choose between a Mode or a State for the Zone"));
                            EditorGUILayout.PropertyField(ZoneID, new GUIContent(ZoneName + " ID", "ID of the Zone.\n" +
                                "For States is the StateID value\n" +
                                "For Stances is the StanceID value\n" +
                                "For Modes is the ModeID value\n"));

                            if (zoneType.intValue == 0)
                            {
                                EditorGUILayout.PropertyField(ZoneModeIndex, new GUIContent("Ability Index"));

                                if (ZoneModeIndex.intValue == -1)
                                {
                                    EditorGUILayout.HelpBox("Ability Index is (-1), it will search for any Ability on the Mode Zone", MessageType.None);
                                }
                            }

                            if (ZoneID.intValue < 0)
                            {
                                EditorGUILayout.HelpBox(ZoneName + " ID is (-1). It will search for any " + ((ZoneType)zoneType.intValue).ToString() + " zone.", MessageType.None);
                            }
                        }
                        break;
                    case LookFor.GameObject:
                        EditorGUILayout.PropertyField(GameObjectName, new GUIContent("GameObject"));
                        break;
                    case LookFor.ClosestWayPoint:
                        break;
                    case LookFor.CurrentTarget:
                        break;
                    case LookFor.TransformVar:
                        EditorGUILayout.PropertyField(transform);
                        break;
                    case LookFor.GameObjectVar:
                        EditorGUILayout.PropertyField(gameobject);
                        break;
                    case LookFor.RuntimeGameobjectSet:
                        EditorGUILayout.PropertyField(gameObjectSet);
                        EditorGUILayout.PropertyField(ChooseRandomly);
                        break;
                    default:
                        break;
                }

                EditorGUILayout.PropertyField(AssignTarget);
                EditorGUILayout.PropertyField(MoveToTarget);

            }
            // EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Look Decision Inspector");
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
