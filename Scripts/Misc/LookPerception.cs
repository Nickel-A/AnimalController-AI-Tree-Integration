//using MalbersAnimations;
//using MalbersAnimations.Conditions;
//using MalbersAnimations.Controller;
//using MalbersAnimations.Scriptables;
//using RenownedGames.AITree;
//using RenownedGames.AITree.PerceptionSystem;
//using RenownedGames.Apex;
//using System;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SocialPlatforms;
//using static Unity.VisualScripting.Member;
//using Random = UnityEngine.Random;




//namespace Malbers.Integration.AITree
//{

//    [SearchContent("Look", Image = "Images/Icons/Perception/EyeIcon.png")]
//    public class LookPerception : AIPerceptionConfig
//    {
//        public Color gizmoColor = Color.white;
//        public Vector3 Offset;

//        public override event Action<AIPerceptionSource> OnTargetUpdated;

//        /// <summary>Execute the Decide method every x Seconds</summary>
//        [Tooltip("Execute the Decide method every x Seconds to improve performance")]
//        public FloatReference interval = new(0.2f);

//        [Range(0, 1)]
//        /// <summary>Angle of Vision of the Animal</summary>
//        [Tooltip("Shorten the Look Ray to not found the ground by mistake")]
//        public float LookMultiplier;


//        /// <summary>Range for Looking forward and Finding something</summary>
//        [Tooltip("Range for Looking forward and Finding something")]
//        [KeyTypes(typeof(float))]
//        public FloatReference LookRange = new FloatReference(15);
//        [Range(0, 360)]
//        /// <summary>Angle of Vision of the Animal</summary>
//        [Tooltip("Angle of Vision of the Animal")]
//        public float LookAngle = 120;

//        /// <summary>What to look for?? </summary>
//        [Space, Tooltip("What to look for??")]
//        public LookFor lookFor = LookFor.MainAnimalPlayer;
//        [Tooltip("Layers that can block the Animal Eyes")]
//        public LayerReference ObstacleLayer = new LayerReference(1);


//        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
//        public bool AssignTarget = true;
//        [Tooltip("If the what we are looking for is found then also start moving")]
//        public bool MoveToTarget = true;
//        [Tooltip("Remove Target when loose sight:\nIf the Target No longer on the Field of View: Set the Target from the AIControl as NULL")]
//        public bool RemoveTarget = false;
//        [Tooltip("Select randomly one of the potential targets, not the first one found")]
//        public bool ChooseRandomly = false;

//        [Space]
//        [Tooltip("Look for this Unity Tag on an Object")]
//        public string UnityTag = string.Empty;
//        [Tooltip("Look for an Specific GameObject by its name")]
//        public string GameObjectName = string.Empty;


//        [RequiredField, Tooltip("Transform Reference value. This value should be set by a Transform Hook Component")]
//        public TransformVar transform;
//        [RequiredField, Tooltip("GameObject Reference value. This value should be set by a GameObject Hook Component")]
//        public GameObjectVar gameObject;

//        [RequiredField, Tooltip("GameObjectSet. Search for all  GameObjects Set in the Set")]
//        public RuntimeGameObjects gameObjectSet;

//        /// <summary>Animal Controller Tags you want to find</summary>
//        [Tooltip("Animal Controller Tags you want to find")]
//        public Tag[] tags;
//        /// <summary>Type of Zone we want to find</summary>
//        [Tooltip("Type of Zone we want to find")]
//        // [Utilities.Flag]
//        public ZoneType zoneType;

//        [Tooltip("Search for all zones")]
//        public bool AllZones = true;
//        /// <summary>ID value of the Zone we want to find</summary>
//        [Tooltip("ID value of the Zone we want to find")]
//        [Min(-1)] public int ZoneID = -1;

//        [Tooltip("Mode Zone Index")]
//        [Min(-1)] public int ZoneModeAbility = -1;

//        public Color debugColor = new Color(0, 0, 0.7f, 0.3f);

//        AIBrain aiBrain;
//        bool inSight;


//        public override void Initialize(AIPerception owner)
//        {
//            aiBrain = GetOwner().GetComponent<AIBrain>();

//            //aiBrain.DecisionsVars[index].gameobjects = null;
//            //aiBrain.DecisionsVars[index].Components = null;

//            switch (lookFor)
//            {
//                case LookFor.MalbersTag:

//                    if (Tags.TagsHolders == null || tags == null || tags.Length == 0)
//                    {
//                        return;
//                    }

//                    List<GameObject> gtags = new();

//                    foreach (var t in Tags.TagsHolders)
//                    {
//                        if (t.gameObject.HasMalbersTag(tags))
//                        {
//                            gtags.Add(t.gameObject);
//                        }
//                    }

//                    if (gtags.Count > 0)
//                    {
//                        aiBrain.DecisionsVars.gameobjects = gtags.ToArray();
//                    }

//                    break;
//                case LookFor.UnityTag:
//                    if (string.IsNullOrEmpty(UnityTag))
//                    {
//                        return;
//                    }

//                    aiBrain.DecisionsVars.gameobjects = GameObject.FindGameObjectsWithTag(UnityTag);
//                    break;
//                case LookFor.RuntimeGameobjectSet:
//                    if (gameObjectSet == null || gameObjectSet.Count == 0)
//                    {
//                        return;
//                    }

//                    aiBrain.DecisionsVars.gameobjects = gameObjectSet.Items.ToArray();
//                    break;
//                default:
//                    break;
//            }

//            StoreColliders(aiBrain);
//        }
//        protected override void OnFixedUpdate()
//        {
//            base.OnFixedUpdate();  

//            AIPerceptionSource source = Look_For(aiBrain, AssignTarget);
//            OnTargetUpdated?.Invoke(source);
//        }

//        /// <summary> Store all renderers found on the GameObjects  </summary>
//        private void StoreColliders(AIBrain aiBrain)
//        {
//            if (aiBrain.DecisionsVars.gameobjects != null && aiBrain.DecisionsVars.gameobjects.Length > 0)
//            {
//                var colliders = new List<Collider>();

//                for (int i = 0; i < aiBrain.DecisionsVars.gameobjects.Length; i++)
//                {
//                    var AllColliders = aiBrain.DecisionsVars.gameobjects[i].GetComponentsInChildren<Collider>();

//                    foreach (var c in AllColliders)
//                    {
//                        if (!c.isTrigger && !MTools.Layer_in_LayerMask(c.gameObject.layer, ObstacleLayer.Value))
//                        {
//                            colliders.Add(c); //Save only good Colliders
//                        }
//                    }
//                }
//                aiBrain.DecisionsVars.AddComponents(colliders.ToArray());
//            }
//        }


//        /// <summary>  Looks for a gameobject acording to the Look For type.</summary>
//        private AIPerceptionSource Look_For(AIBrain aiBrain, bool assign)
//        {
//            return lookFor switch
//            {
//                LookFor.MainAnimalPlayer => LookForAnimalPlayer(aiBrain, assign),
//                //LookFor.MalbersTag => LookForMalbersTags(aiBrain, assign),
//                //LookFor.UnityTag => LookForUnityTags(aiBrain, assign),
//                //LookFor.Zones => LookForZones(aiBrain, assign),
//                //LookFor.GameObject => LookForGameObjectByName(aiBrain, assign),
//                //LookFor.ClosestWayPoint => LookForClosestWaypoint(aiBrain, assign),
//                //LookFor.CurrentTarget => LookForTarget(aiBrain, assign),
//                //LookFor.TransformVar => LookForTransformVar(aiBrain, assign),
//                //LookFor.GameObjectVar => LookForGoVar(aiBrain, assign),
//                //LookFor.RuntimeGameobjectSet => LookForGoSet(aiBrain, assign),
//            };
//        }

//        public bool LookForTarget(AIBrain aiBrain, bool assign)
//        {
//            if (aiBrain.Target == null)
//            {
//                return false;
//            }

//            AssignMoveTarget(aiBrain, aiBrain.Target, assign);
//            var Center = aiBrain.TargetAnimal ? aiBrain.TargetAnimal.Center : aiBrain.Target.position;
//            return FindSource();
//        }

//        public bool LookForTransformVar(AIBrain aiBrain, bool assign)
//        {
//            if (transform == null || transform.Value == null)
//            {
//                return false;
//            }

//            AssignMoveTarget(aiBrain, transform.Value, assign);

//            var Center =
//                transform.Value == aiBrain.Target && aiBrain.AIControl.IsAITarget != null ?
//                aiBrain.AIControl.IsAITarget.GetCenterY() :
//                transform.Value.position;

//            return FindSource();
//        }

//        public bool LookForGoVar(AIBrain aiBrain, bool assign)
//        {
//            if (gameObject == null && gameObject.Value && !gameObject.Value.IsPrefab())
//            {
//                return false;
//            }

//            AssignMoveTarget(aiBrain, gameObject.Value.transform, assign);

//            var Center =
//                gameObject.Value.transform == aiBrain.Target && aiBrain.AIControl.IsAITarget != null ?
//                aiBrain.AIControl.IsAITarget.GetCenterY() :
//                gameObject.Value.transform.position;

//            return FindSource();
//        }
//        private AIPerceptionSource FindSource()
//        {
//            Vector3 eyePos = transform.position + Vector3.up * heightOffset;

//            float minDistance = Mathf.Min(range.x, range.y);
//            float maxDistance = Mathf.Max(range.x, range.y);

//            int count = Physics.OverlapSphereNonAlloc(eyePos, maxDistance, colliders, cullingLayer);
//            for (int i = 0; i < count; i++)
//            {
//                Collider collider = colliders[i];
//                AIPerceptionSource source = collider.GetAIPerceptionSource();
//                if (source != null && source.IsObservable() && source.transform != transform)
//                {
//                    Vector3 point = collider.bounds.center;
//                    if (Physics.Linecast(eyePos, point, out RaycastHit hitInfo, cullingLayer | obstacleLayer))
//                    {
//                        if (hitInfo.transform == source.transform)
//                        {
//                            Vector3 direction = point - eyePos;
//                            direction.y = 0f;

//                            float angle = Vector3.Angle(transform.forward, direction.normalized);
//                            if (angle <= fov / 2 || Vector3.Distance(eyePos, point) <= minDistance)
//                            {
//                                return source;
//                            }
//                        }
//                    }
//                }
//            }
//            return null;
//        }


//        //private AIPerceptionSource IsInFieldOfView(AIBrain aiBrain, Vector3 Center, out float Distance)
//        //{
//        //    var Direction_to_Target = Center - aiBrain.Eyes.position; //Put the Sight a bit higher

//        //    //Important, otherwise it will find the ground for Objects to close to it. Also Apply the Scale
//        //    Distance = Vector3.Distance(Center, aiBrain.Eyes.position) * LookMultiplier;
//        //    if (LookAngle == 0 || LookRange <= 0)
//        //    {
//        //        inSight = true;
//        //        return true; //Means the Field of view can be ignored
//        //    }

//        //    if (Distance < LookRange.Value * aiBrain.Animal.ScaleFactor) //Check if whe are inside the Look Radius
//        //    {
//        //        Vector3 EyesForward = Vector3.ProjectOnPlane(aiBrain.Eyes.forward, aiBrain.Animal.UpVector);

//        //        var angle = Vector3.Angle(Direction_to_Target, EyesForward);

//        //        if (angle < (LookAngle / 2))
//        //        {
//        //            //Need a RayCast to see if there's no obstacle in front of the Animal OBSTACLE LAYER
//        //            if (Physics.Raycast(aiBrain.Eyes.position, Direction_to_Target, out RaycastHit hit, Distance, ObstacleLayer, QueryTriggerInteraction.Ignore))
//        //            {
//        //                AIPerceptionSource source = collider.GetAIPerceptionSource();
//        //                if (aiBrain.debug)
//        //                {
//        //                    Debug.DrawRay(aiBrain.Eyes.position, Direction_to_Target * LookMultiplier, Color.green, interval);
//        //                    Debug.DrawLine(hit.point, Center, Color.red, interval);
//        //                    MDebug.DrawWireSphere(Center, Color.red, interval);
//        //                    MDebug.DrawCircle(hit.point, hit.normal, 0.1f, Color.red, true, interval);
//        //                }
//        //                return source; //Meaning there's something between the Eyes of the Animal and the Target
//        //            }
//        //            else
//        //            {
//        //                if (aiBrain.debug)
//        //                {
//        //                    Debug.DrawRay(aiBrain.Eyes.position, Direction_to_Target, Color.green, interval);
//        //                    MDebug.DrawWireSphere(Center, Color.green, interval);
//        //                }
//        //                return source;
//        //            }
//        //        }
//        //        inSight = false;
//        //        return source;
//        //    }

//        //    return source;
//        //}

//        private void AssignMoveTarget(AIBrain aiBrain, Transform target, bool assign)
//        {
//            if (assign)
//            {

//                if (AssignTarget)
//                {
//                    aiBrain.AIControl.SetTarget(target, MoveToTarget);
//                }
//                else if (RemoveTarget)
//                {
//                    aiBrain.AIControl.ClearTarget();
//                }
//            }
//        }

//        public bool LookForZones(AIBrain aiBrain, bool assign)
//        {
//            var zones = Zone.Zones;
//            if (zones == null || zones.Count == 0)
//            {
//                return false;  //There's no zone around here
//            }

//            float minDistance = float.PositiveInfinity;

//            Zone FoundZone = null;

//            foreach (var zone in zones)
//            {
//                if (AllZones ||
//                    (zone && zone.zoneType == zoneType &&                      //Check the same Zone Types
//                    ZoneID == -1) ||                                             //Check First if its Any Zone
//                    zone.ZoneID == ZoneID ||                                    //Check Zone has the same ID
//                    zone.zoneType != ZoneType.Mode ||                           //Check if its not a Zone Mode
//                    (zone.zoneType == ZoneType.Mode && ZoneModeAbility == -1) ||  //Check if it's a Zone Mode but the Ability its any
//                    zone.ModeAbilityIndex == ZoneModeAbility)                   //Check if it's a Zone Mode AND Ability Match
//                {
//                    if (IsInFieldOfView(aiBrain, zone.ZoneCollider.bounds.center, out float Distance) && Distance < minDistance)
//                    {
//                        minDistance = Distance;
//                        FoundZone = zone;
//                    }
//                }
//            }
//            if (FoundZone)
//            {
//                AssignMoveTarget(aiBrain, FoundZone.transform, assign);
//                return true;
//            }
//            return false;
//        }

//        public bool LookForMalbersTags(AIBrain aiBrain, bool assign)
//        {
//            if (Tags.TagsHolders == null || tags == null || tags.Length == 0)
//            {
//                return false;
//            }

//            float minDistance = float.MaxValue;
//            Transform Closest = null;

//            var filtredTags = Tags.GambeObjectbyTag(tags);
//            if (filtredTags == null)
//            {
//                return false;
//            }

//            if (ChooseRandomly)
//            {
//                while (filtredTags.Count != 0)
//                {
//                    int newIndex = Random.Range(0, filtredTags.Count);
//                    var go = filtredTags[newIndex].transform;

//                    if (go != null)
//                    {
//                        if (IsInFieldOfView(aiBrain, go.position, out _))
//                        {
//                            AssignMoveTarget(aiBrain, go, assign);
//                            return true;
//                        }
//                    }
//                    filtredTags.RemoveAt(newIndex);
//                }
//            }
//            else
//            {
//                for (int i = 0; i < filtredTags.Count; i++)
//                {
//                    var go = filtredTags[i].transform;

//                    if (go != null)
//                    {
//                        if (IsInFieldOfView(aiBrain, go.position, out float Distance))
//                        {
//                            if (Distance < minDistance)
//                            {
//                                minDistance = Distance;
//                                Closest = go;
//                            }
//                        }
//                    }
//                }
//            }

//            if (Closest)
//            {
//                AssignMoveTarget(aiBrain, Closest.transform, assign);
//                return true;
//            }
//            return false;
//        }

//        public bool LookForUnityTags(AIBrain aiBrain, bool assign)
//        {
//            if (string.IsNullOrEmpty(UnityTag))
//            {
//                return false;
//            }

//            if (ChooseRandomly)
//            {
//                return ChooseRandomObject(aiBrain, assign);
//            }

//            return ClosestGameObject(aiBrain, assign);
//        }

//        public bool LookForGoSet(AIBrain aiBrain, bool assign)
//        {
//            if (gameObjectSet == null || gameObjectSet.Count == 0)
//            {
//                return false;
//            }

//            if (ChooseRandomly)
//            {
//                return ChooseRandomObject(aiBrain, assign);
//            }

//            return ClosestGameObject(aiBrain, assign);
//        }



//        private bool ClosestGameObject(AIBrain aiBrain, bool assign)
//        {
//            var All = aiBrain.DecisionsVars.gameobjects; //catch all the saved gameobjects

//            if (All == null || All.Length == 0)
//            {
//                return false;
//            }

//            float minDistance = float.MaxValue;

//            GameObject ClosestGameObject = null;

//            for (int i = 0; i < All.Length; i++)
//            {
//                var go = All[i];

//                if (go != null)
//                {
//                    var Center = go.transform.position;// + new Vector3(0, aiBrain.Animal.Height, 0); //In case there's no height use the animal Default

//                    if (aiBrain.DecisionsVars.Components != null && aiBrain.DecisionsVars.Components.Length > 0)
//                    {
//                        var bounds = Vector3.zero;
//                        int total = 0;
//                        foreach (var c in aiBrain.DecisionsVars.Components)
//                        {
//                            if (c != null && c is Collider && c.transform.SameHierarchy(go.transform))
//                            {
//                                bounds += (c as Collider).bounds.center;
//                                total++;
//                            }
//                        }
//                        bounds /= total;

//                        if (bounds != Vector3.zero)
//                        {
//                            Center = bounds;
//                        }
//                    }


//                    if (IsInFieldOfView(aiBrain, Center, out float Distance))
//                    {
//                        if (Distance < minDistance)
//                        {
//                            minDistance = Distance;
//                            ClosestGameObject = go;
//                        }
//                    }
//                }
//            }

//            if (ClosestGameObject)
//            {
//                AssignMoveTarget(aiBrain, ClosestGameObject.transform, assign);
//                return true;
//            }
//            return false;
//        }

//        public bool ChooseRandomObject(AIBrain aiBrain, bool assign)
//        {
//            var All = new List<GameObject>();
//            if (aiBrain.DecisionsVars.gameobjects != null)
//            {
//                All.AddRange(aiBrain.DecisionsVars.gameobjects); //catch all the saved gameobjects with a tag
//            }

//            if (All.Count == 0)
//            {
//                return false;
//            }

//            while (All.Count != 0)
//            {
//                int newIndex = Random.Range(0, All.Count);
//                if (All[newIndex] != null)
//                {
//                    var Center = All[newIndex].transform.position + new Vector3(0, aiBrain.Animal.Height, 0);

//                    var renderer = aiBrain.DecisionsVars.Components[newIndex];

//                    if (renderer != null && renderer is Renderer)
//                    {
//                        Center = (renderer as Renderer).bounds.center;
//                    }

//                    if (IsInFieldOfView(aiBrain, Center, out float Distance))
//                    {
//                        AssignMoveTarget(aiBrain, All[newIndex].transform, assign);
//                        return true;
//                    }
//                }
//                All.RemoveAt(newIndex);
//            }

//            return false;
//        }


//        public bool LookForGameObjectByName(AIBrain aiBrain, bool assign)
//        {
//            if (string.IsNullOrEmpty(GameObjectName))
//            {
//                return false;
//            }

//            var gameObject = GameObject.Find(GameObjectName);

//            if (gameObject)
//            {
//                AssignMoveTarget(aiBrain, gameObject.transform, assign);
//                return IsInFieldOfView(aiBrain, gameObject.transform.position, out _);   //Find if is inside the Field of view
//            }
//            return false;
//        }

//        public bool LookForClosestWaypoint(AIBrain aiBrain, bool assign)
//        {
//            var allWaypoints = MWayPoint.WayPoints;
//            if (allWaypoints == null || allWaypoints.Count == 0)
//            {
//                return false;  //There's no waypoints  around here
//            }

//            float minDistance = float.MaxValue;

//            MWayPoint closestWayPoint = null;

//            foreach (var way in allWaypoints)
//            {
//                var center = way.GetCenterY();
//                if (IsInFieldOfView(aiBrain, center, out float Distance))
//                {
//                    if (Distance < minDistance)
//                    {
//                        minDistance = Distance;
//                        closestWayPoint = way;
//                    }
//                }
//            }

//            if (closestWayPoint)
//            {
//                AssignMoveTarget(aiBrain, closestWayPoint.transform, assign);
//                return true; //Find if is inside the Field of view
//            }
//            return false;
//        }

//        private bool LookForAnimalPlayer(AIBrain aiBrain, bool assign)
//        {
//            if (MAnimal.MainAnimal == null || MAnimal.MainAnimal.ActiveStateID == StateEnum.Death)
//            {
//                return false; //Means the animal is death or Disable
//            }

//            if (MAnimal.MainAnimal == aiBrain.Animal) { Debug.LogError("AI Animal is set as MainAnimal. Fix it!", aiBrain.Animal); return false; }

//            AssignMoveTarget(aiBrain, MAnimal.MainAnimal.transform, assign);
//            return IsInFieldOfView(aiBrain, MAnimal.MainAnimal.Center, out _);
//        }
//        protected override void OnDrawGizmosSelected()
//        {
//            Vector3 center = GetOwner().transform.position + Offset;
//            Vector3 forward = GetOwner().transform.forward;


//            Color c = gizmoColor;
//            c.a = 1f;

//            Vector3 EyesForward = Vector3.ProjectOnPlane(forward, Vector3.up);

//            Vector3 rotatedForward = Quaternion.Euler(0, -LookAngle * 0.5f, 0) * EyesForward;
//            UnityEditor.Handles.color = c;
//            UnityEditor.Handles.DrawWireArc(center, Vector3.up, rotatedForward, LookAngle, LookRange * 1);
//            UnityEditor.Handles.color = gizmoColor;
//            UnityEditor.Handles.DrawSolidArc(center, Vector3.up, rotatedForward, LookAngle, LookRange * 1);

//        }
//    }
//}

////protected override void OnDrawGizmosSelected()
////{
////    ///base.OnDrawGizmosSelect;
////    Vector3 position = GetOwner().transform.position;

////    // Draw the cone
////    Vector3 apex = position + Vector3.up * height;
////    Gizmos.DrawLine(apex, apex + Quaternion.Euler(0, -360, 0) * Vector3.forward * radius);

////    // Draw the circular base
////    for (int i = 0; i < 360; i += 10)
////    {
////        float angle = i * Mathf.Deg2Rad;
////        Vector3 start = apex + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
////        angle = (i + 10) * Mathf.Deg2Rad;
////        Vector3 end = apex + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
////    Gizmos.DrawLine(start, end);
////    }
////}


