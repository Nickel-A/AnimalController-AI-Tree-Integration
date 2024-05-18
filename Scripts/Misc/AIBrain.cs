using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Utilities;
using NUnit.Framework;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


namespace Malbers.Integration.AITree
{
    public enum FactionRelation
    {
        Friendly,
        Aggressive,
        Neutral
    }

    [RequireComponent(typeof(BehaviourRunner))]
    [AddComponentMenu("Malbers/Animal Controller/AI/AIBrain")]

    public class AIBrain : MonoBehaviour, IAnimatorListener
    {
        public AIStateID CurrentAIState;
        public AIStateID deadAIState;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Removes all AI Components when the Animal Dies. (Brain, AiControl, Agent)")]
        [FormerlySerializedAs("RemoveAIOnDeath")]
        public bool DisableAIOnDeath = true;

        [TabGroup("Tab Group 1", "General")]
        [RequiredField, NotNull]
        [Tooltip("Transform used to raycast Rays to interact with the world")]
        public Transform Eyes;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Reference of the Key wich represents the Target of the AI.")]
        public TransformKey targetBlackboardKey;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Reference of the Aim component some Behavior Tree nodes need it.")]
        public Aim aim;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Reference for the MPickUp component some Behavior Tree nodes need it.")]
        public MPickUp pickUpDrop;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Reference for the Stats component some Behavior Tree nodes need it.")]
        public Stats stats;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Reference for the MDamageable component some Behavior Tree nodes need it.")]
        public MDamageable damageable;

        [TabGroup("Tab Group 1", "Agressive")]
        [Tooltip("This transform is used with SetTargetTransform to align it with the position of the target.")]
        public Transform targetTransform;

        [TabGroup("Tab Group 1", "Agressive")]
        [Tooltip("Reference for the ComboManager component some Behavior Tree nodes need it.")]
        public ComboManager comboManager;

        [TabGroup("Tab Group 1", "Agressive")]
        [Tooltip("Reference for the MWeaponManager component some Behavior Tree nodes need it.")]
        public MWeaponManager weaponManager;

        [Message("Leave the fields blank if you don't need them.", FontStyle = FontStyle.Italic)]
        [TabGroup("Tab Group 1", "General")]
        public bool debug = false;


        /// <summary>Reference for the Ai Control Movement</summary>
        public IAIControl AIControl;

        [HideInInspector]
        public BehaviourRunner behaviourRunner;

        /// <summary>Reference for the Animal</summary>
        public MAnimal Animal { get; private set; }

        /// <summary>Reference for the AnimalStats</summary>
        public Dictionary<int, Stat> AnimalStats { get; set; }

        /// <summary>Gets the position of the AI </summary>
        public Vector3 Position => AIControl.Transform.position;

        /// <summary> Gets the height of the AI </summary>
        public float AIHeight => Animal.transform.lossyScale.y * AIControl.StoppingDistance;

        /// <summary>Reference for the Last WayPoint the Animal used</summary>
        public IWayPoint LastWayPoint { get; set; }

        /// <summary>Reference for the Current Target the Animal is using</summary>
        public Transform Target { get; set; }

        /// <summary>Reference for the Target the Animal Component</summary>
        public MAnimal TargetAnimal { get; set; }

        /// <summary>True if the Current Target has Stats</summary>
        public bool TargetHasStats { get; private set; }

        /// <summary>Reference for the Target the Stats Component</summary>
        public Dictionary<int, Stat> TargetStats { get; set; }

        Transform keyValue;

        public List<StatParameterInfo> statParameterInfo = new List<StatParameterInfo>();

        /// <summary>Stores the last position of the target</summary>
        private Vector3 lastTargetPosition;


    public AIStateID GetCurrentState()
        {
            return CurrentAIState;
        }

        public void SetAIState(AIStateID newState)
        {
            CurrentAIState = newState;
        }


        #region Unity Callbacks
        void Awake()
        {
            if (Animal == null)
            {
                Animal = gameObject.FindComponent<MAnimal>();
            }
            AIControl ??= gameObject.FindInterface<IAIControl>();
            AIControl.UpdateDestinationPosition = false; //Otherwise the AI will go to the current target
            behaviourRunner = GetComponent<BehaviourRunner>();
            var AnimalStatscomponent = Animal.FindComponent<Stats>();
            if (AnimalStatscomponent)
            {
                AnimalStats = AnimalStatscomponent.Stats_Dictionary();
            }
            Animal.isPlayer.Value = false; //If is using a AIBrain... disable that he is the main player
                                           // ResetVarsOnNewState();
            lastTargetPosition = transform.position;
        }

        private void Start()
        {
            //aim = Animal.GetComponentInChildren<Aim>();
            //weaponManager = Animal.GetComponent<MAIBrain.weaponManager>();
            //comboManager = Animal.GetComponentInChildren<ComboManager>();
            //stats = Animal.GetComponent<Stats>();
            //damageable = Animal.GetComponent<MDamageable>();

            AIControl.UpdateDestinationPosition = false; //Otherwise the AI will go to the current target
                                                         // Assuming 'parentObject' is the parent GameObject that contains the child objects
            SaveAllAnimalStats();
            SaveTargetPosition();
        }

        public void SaveAllAnimalStats()
        {
            if (AnimalStats != null)
            {
                statParameterInfo.Clear(); // Clear the list to avoid duplicates

                foreach (var pair in AnimalStats)
                {
                    // Create a new ParameterInfo instance for each entry in the dictionary
                    StatParameterInfo parameterInfo = new StatParameterInfo(pair.Value.Name, pair.Key, pair.Value.Value);

                    // Add the parameterInfo to the list
                    statParameterInfo.Add(parameterInfo);
                }
            }
            else
            {
                Debug.LogWarning("AnimalStats dictionary is null.");
            }
        }

        // Method to compare the saved parameters with the current ones
        //public void CompareParameters()
        //{
        //    // Iterate over the saved parameter infos
        //    foreach (var savedInfo in parameterInfos)
        //    {
        //        // Get the current value from the AnimalStats dictionary
        //        if (AnimalStats.TryGetValue(savedInfo.ID, out Stat stat))
        //        {
        //            float currentValue = stat.Value;

        //            // Log both previous and current values
        //            Debug.Log($"{savedInfo.Name}: Previous Value: {savedInfo.PreviousValue}, Current Value: {currentValue}");

        //            // Compare the values
        //            if (currentValue > savedInfo.PreviousValue)
        //            {
        //                Debug.Log($"{savedInfo.Name} increased.");
        //            }
        //            else if (currentValue < savedInfo.PreviousValue)
        //            {
        //                Debug.Log($"{savedInfo.Name} decreased.");
        //            }
        //            else
        //            {
        //                Debug.Log($"{savedInfo.Name} remained the same.");
        //            }

        //            // Set the current value as the previous value for the next comparison
        //            savedInfo.PreviousValue = currentValue;
        //        }
        //        else
        //        {
        //            Debug.LogWarning($"Parameter with ID {savedInfo.ID} not found.");
        //        }
        //    }
        //}


        //private void Update()
        //{
        //    if(Input.GetKeyDown(KeyCode.J))
        //    {
        //        CompareParameters();
        //    }
        //}

        public void OnEnable()
        {
            AIControl.TargetSet.AddListener(OnTargetSet);
            Animal.OnStateChange.AddListener(OnAnimalStateChange);
            if (behaviourRunner.GetBlackboard().TryGetKey(targetBlackboardKey.ToString(), out targetBlackboardKey))
            {
                targetBlackboardKey.ValueChanged += OnTargetValueChanged;
            }
        }

        public void OnDisable()
        {
            AIControl.TargetSet.RemoveListener(OnTargetSet);
            Animal.OnStateChange.RemoveListener(OnAnimalStateChange);
            AIControl.Stop();
            StopAllCoroutines();

            // Unsubscribe from the ValueChanged event to avoid memory leaks
            if (behaviourRunner.GetBlackboard().TryGetKey(targetBlackboardKey.ToString(), out targetBlackboardKey))
            {
                targetBlackboardKey.ValueChanged -= OnTargetValueChanged;
            }
        }
        #endregion

        void OnAnimalStateChange(int state)
        {
            if (state == StateEnum.Death) //meaning this animal has died
            {
                CurrentAIState = deadAIState;
                StopAllCoroutines();
                enabled = false;

                if (DisableAIOnDeath)
                {
                    behaviourRunner.enabled = false;
                    AIControl.SetActive(false);
                    //Animal.enabled = false;
                    this.SetEnable(false);
                    if (weaponManager != null)
                    {
                        weaponManager.enabled = false;
                    }
                }
            }
        }

        private void OnTargetValueChanged()
        {
            behaviourRunner.GetBlackboard().TryGetKey(targetBlackboardKey.ToString(), out TransformKey value);
            {
                AIControl.Target = value.GetValue();
                keyValue = value.GetValue();
            }
            SaveTargetPosition();
        }

        /// <summary>Stores if the Current Target is an Animal and if it has the Stats component </summary>
        private void OnTargetSet(Transform target)
        {
            
            if (keyValue != null)
            {
                OnTargetValueChanged();
                target = keyValue;
            }

            Target = target;
            SaveTargetPosition();
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

        /// <summary>
        /// Saves the position of the current target.
        /// </summary>
        private void SaveTargetPosition()
        {
            if (Target != null)
            {
                lastTargetPosition = Target.position;
            }
        }



        public Vector3 GetLastTargetPosition()
        {
            return lastTargetPosition;
        }

        public void SetLastWayPoint(Transform target)
        {
            var newLastWay = target.gameObject.FindInterface<IWayPoint>();
            if (newLastWay != null)
            {
                LastWayPoint = target?.gameObject.FindInterface<IWayPoint>(); //If not is a waypoint save the last one
            }
        }

        /// <summary>
        /// Invoked when receiving messages from animator
        /// </summary>
        public virtual bool OnAnimatorBehaviourMessage(string message, object value) => this.InvokeWithParams(message, value);

        protected virtual void Debuging(string Log, UnityEngine.Object val)
        {
            if (debug)
            {
                Debug.Log($"<B><color=green>[{Animal.name}]</color> - </B> " + Log, val);
            }
        }



#if UNITY_EDITOR
        void Reset()
        {
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

        private void OnDrawGizmos()
        {
            if (debug)
            {

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastTargetPosition, 0.5f);
            }
        }
#endif
    }

    public class StatParameterInfo
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public float PreviousValue { get; set; } // New field for previous value
        public float CurrentValue { get; set; }

        public StatParameterInfo(string name, int id, float value)
        {
            Name = name;
            ID = id;
            PreviousValue = value; // Initialize previous value with the current value
            CurrentValue = value;
        }

        // Method to set the current value as the previous value
        public void UpdatePreviousValue(float newValue)
        {
            PreviousValue = newValue;
        }
    }

}