using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


namespace Malbers.Integration.AITree
{
    public enum Affected { Self, Target }
    public enum ExecuteTask { OnStart, OnUpdate, OnExit }


    public class AIBrain : MonoBehaviour, IAnimatorListener
    {
        /// <summary>Reference for the Ai Control Movement</summary>
        public IAIControl AIControl;
        [Obsolete("Use AIControl Instead")]
        public IAIControl AIMovement => AIControl;

        //[Tooltip("Use a temporal Brain from another animal (MOUNTING)")]
        //public BehaviorBrain TemporalBrain;
        //public BehaviorBrain Brain => TemporalBrain != null ? TemporalBrain : this;

        /// <summary>Transform used to raycast Rays to interact with the world</summary>
        [RequiredField, Tooltip("Transform used to raycast Rays to interact with the world")]
        public Transform Eyes;
        /// <summary>Time needed to make a new transition. Necesary to avoid Changing to multiple States in the same frame</summary>
        [Tooltip("Time needed to make a new transition. Necessary to avoid Changing to multiple States in the same frame")]
        public FloatReference TransitionCoolDown = new FloatReference(0.2f);

        ///// <summary>Reference AI State for the animal</summary>
        //[CreateScriptableAsset] public MAIState currentState;

        [Tooltip("Removes all AI Components when the Animal Dies. (Brain, AiControl, Agent)")]
        [FormerlySerializedAs("RemoveAIOnDeath")]
        public bool DisableAIOnDeath = true;
        public bool debug = false;       

        /// <summary>Last Time the Animal  started a transition</summary>
        public float StateLastTime { get; set; }



        /// <summary>Tasks Local Vars (1 Int,1 Bool,1 Float)</summary>
        public BrainVars TasksVars;
        /// <summary>Saves on the a Task that it has finish is stuff</summary>
        internal bool TasksDone;

        ///// <summary>Current Decision Results</summary>
        //internal bool[] DecisionResult;
        ///// <summary>Store if a Task has Started</summary>
        //internal bool[] TasksStarted;

        /// <summary>Decision Local Vars to store values on Prepare Decision</summary>
        

        /// <summary>Reference for the Animal</summary>
        public MAnimal Animal { get; private set; }
        //public MAnimalAIControl mAnimalAIControl;
        /// <summary>Reference for the AnimalStats</summary>
        public Dictionary<int, Stat> AnimalStats { get; set; }

        #region Target References
        /// <summary>Reference for the Current Target the Animal is using</summary>
        public Transform Target { get; set; }
        //{ 
        //    get => target; 
        //    set 
        //    {
        //    target = value;
        //    }
        //}
        //private Transform target;

        /// <summary>Reference for the Target the Animal Component</summary>
        public MAnimal TargetAnimal { get; set; }

        public Vector3 Position => AIControl.Transform.position;

        public float AIHeight => Animal.transform.lossyScale.y * AIControl.StoppingDistance;

        /// <summary>True if the Current Target has Stats</summary>
        public bool TargetHasStats { get; private set; }

        /// <summary>Reference for the Target the Stats Component</summary>
        public Dictionary<int, Stat> TargetStats { get; set; }
        #endregion

        /// <summary>Reference for the Last WayPoint the Animal used</summary>
        public IWayPoint LastWayPoint { get; set; }

        ///// <summary>Time Elapsed for the Tasks on an AI State</summary>
        //public float[] TasksStartTime { get; set; }
        //public float[] TasksUpdateTime { get; set; }

        /// <summary>Time Elapsed for the State Decisions</summary>
        [HideInInspector] public float[] DecisionsTime;// { get; set; }


        #region Unity Callbakcs
        void Awake()
        {
            if (Animal == null)
            {
                Animal = gameObject.FindComponent<MAnimal>();
            }

            AIControl ??= gameObject.FindInterface<IAIControl>();
            //mAnimalAIControl ??= gameObject.FindInterface<MAnimalAIControl>();

            var AnimalStatscomponent = Animal.FindComponent<Stats>();
            if (AnimalStatscomponent)
            {
                AnimalStats = AnimalStatscomponent.Stats_Dictionary();
            }

            Animal.isPlayer.Value = false; //If is using a brain... disable that he is the main player
                                           // ResetVarsOnNewState();
        }

        public void OnEnable()
        {
            //AIMovement.OnTargetArrived.AddListener(OnTargetArrived);
            //AIMovement.OnTargetPositionArrived.AddListener(OnPositionArrived);
            AIControl.TargetSet.AddListener(OnTargetSet);
            //AIControl.OnArrived.AddListener(OnTargetArrived);
            //StartBrain();
            Animal.OnStateChange.AddListener(OnAnimalStateChange);
            //Animal.OnStanceChange.AddListener(OnAnimalStanceChange);
            //Animal.OnModeStart.AddListener(OnAnimalModeStart);
            //Animal.OnModeEnd.AddListener(OnAnimalModeEnd);

        }

        public void OnDisable()
        {
            //AIMovement.OnTargetArrived.RemoveListener(OnTargetArrived);
            //AIMovement.OnTargetPositionArrived.RemoveListener(OnPositionArrived);
            AIControl.TargetSet.RemoveListener(OnTargetSet);
            //AIControl.OnArrived.RemoveListener(OnTargetArrived);

            Animal.OnStateChange.RemoveListener(OnAnimalStateChange);
            //Animal.OnStanceChange.RemoveListener(OnAnimalStanceChange);
            //Animal.OnModeStart.RemoveListener(OnAnimalModeStart);
            //Animal.OnModeEnd.RemoveListener(OnAnimalModeEnd);


            AIControl.Stop();
            StopAllCoroutines();                      
        }

        #endregion

        protected virtual void Debuging(string Log, UnityEngine.Object val)
        {
            if (debug)
            {
                Debug.Log($"<B><color=green>[{Animal.name}]</color> - </B> " + Log, val);
            }
        }

        public virtual bool OnAnimatorBehaviourMessage(string message, object value) => this.InvokeWithParams(message, value);

        #region SelfAnimal Event Listeners
        void OnAnimalStateChange(int state)
        {
            if (state == StateEnum.Death) //meaning this animal has died
            {
                StopAllCoroutines();
               
                //schemaAgent.SetEnable(false);

                enabled = false;

                if (DisableAIOnDeath)
                {
                    AIControl.SetActive(false);
                    //this.SetEnable(false);
                }
            }
        }

        //void OnAnimalStanceChange(int stance) => currentState.OnAnimalStanceChange(this, Animal.Stance.ID);

        // void OnAnimalModeStart(int mode, int ability) => currentState.OnAnimalModeStart(this, Animal.ActiveMode);

        // void OnAnimalModeEnd(int mode, int ability) => currentState.OnAnimalModeEnd(this, Animal.ActiveMode);


        #endregion

        // private void OnTargetArrived(Transform target) => AIControl.Target.OnTargetArrived(this, target);

        /// <summary>Stores if the Current Target is an Animal and if it has the Stats component </summary>
        private void OnTargetSet(Transform target)
        {
            Target = target;
            if (target)
            {

                TargetAnimal = target.FindComponent<MAnimal>();// ?? target.GetComponentInChildren<MAnimal>();
                TargetStats = null;
                var TargetStatsC = target.FindComponent<Stats>();// ?? target.GetComponentInChildren<Stats>();

                TargetHasStats = TargetStatsC != null;
                if (TargetHasStats)
                {
                    TargetStats = TargetStatsC.Stats_Dictionary();
                }
            }
        }

        public void SetLastWayPoint(Transform target)
        {
            var newLastWay = target.gameObject.FindInterface<IWayPoint>();
            if (newLastWay != null)
            {
                LastWayPoint = target?.gameObject.FindInterface<IWayPoint>(); //If not is a waypoint save the last one
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            //   remainInState = MTools.GetInstance<MAIState>("Remain in State");
            AIControl = this.FindComponent<MAnimalAIControl>();

            if (AIControl != null)
            {
                AIControl.AutoNextTarget = false;
                AIControl.UpdateDestinationPosition = false;
                AIControl.LookAtTargetOnArrival = false;

                if (Animal)
                {
                    Animal.isPlayer.Value = false; //Make sure this animal is not the Main Player
                }
            }
            else
            {
                Debug.LogWarning("There's No AI Control in this GameObject, Please add one");
            }
        }
        public BrainVars DecisionsVars;

#endif
        [System.Serializable]
        public struct BrainVars
        {
            public int intValue;
            public float floatValue;
            public bool boolValue;
            public Vector3 vector3;
            public Component[] Components;
            public MonoBehaviour mono;
            public GameObject[] gameobjects;

            public Dictionary<int, int> ints;
            public Dictionary<int, float> floats;
            public Dictionary<int, bool> bools;
            // public Dictionary<int, Component> D_components;

            public void SetVar(int key, bool value) => bools[key] = value;
            public void SetVar(int key, int value) => ints[key] = value;
            public void SetVar(int key, float value) => floats[key] = value;
            public bool GetBool(int key) => bools[key];
            public int GetInt(int key) => ints[key];
            public float GetFloat(int key) => floats[key];


            public bool TryGetBool(int key, out bool value) => bools.TryGetValue(key, out value);
            public bool TryGetInt(int key, out int value) => ints.TryGetValue(key, out value);
            public bool TryGetFloat(int key, out float value) => floats.TryGetValue(key, out value);

            public void AddVar(int key, bool value)
            {
                if (bools == null)
                {
                    bools = new Dictionary<int, bool>();
                }

                bools.Add(key, value);
            }

            public void AddVar(int key, int value)
            {
                if (ints == null)
                {
                    ints = new Dictionary<int, int>();
                }

                ints.Add(key, value);
            }

            public void AddVar(int key, float value)
            {
                if (floats == null)
                {
                    floats = new Dictionary<int, float>();
                }

                floats.Add(key, value);
            }

            public void AddComponents(Component[] components)
            {
                if (Components == null || Components.Length == 0)
                {
                    Components = components;
                }
                else
                {
                    Components = Components.Concat(components).ToArray();
                }
            }


            public void AddComponent(Component comp)
            {
                if (Components == null || Components.Length == 0)
                {
                    Components = new Component[1] { comp };
                }
                else
                {
                    var ComponentsL = Components.ToList();
                    ComponentsL.Add(comp);
                    Components = ComponentsL.ToArray();
                }
            }
        }
    }
}

