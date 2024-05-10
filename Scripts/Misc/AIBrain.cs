using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Utilities;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace Malbers.Integration.AITree
{
    [RequireComponent(typeof(BehaviourRunner))]
    [AddComponentMenu("Malbers/Animal Controller/AI/AIBrain")]

    public class AIBrain : MonoBehaviour, IAnimatorListener
    {
        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Removes all AI Components when the Animal Dies. (Brain, AiControl, Agent)")]
        [FormerlySerializedAs("RemoveAIOnDeath")]
        public bool DisableAIOnDeath = true;

        [TabGroup("Tab Group 1", "General")]
        [RequiredField, NotNull]
        [Tooltip("Transform used to raycast Rays to interact with the world")]
        public Transform Eyes;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Reference of the Aim component some Behavior Tree nodes need it.")]
        public Aim aim;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Reference for the MPickUp component some Behavior Tree nodes need it.")]
        public MPickUp pickUpDrop;

        [TabGroup("Tab Group 1", "General")]
        [Tooltip("Reference for the Stats component some Behavior Tree nodes need it.")]
        public Stats stats;

        [Message("Leave the fields blank if you don't need them.", FontStyle = FontStyle.Italic)]
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

        [TabGroup("Tab Group 1", "Debug")]
        public bool debug = false;

        [ReadOnly,TabGroup("Tab Group 1", "Debug")]
        [Tooltip("List of the stats on the AI. It will be filled on start")]
        public List<Stat> statList;

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

        #region Unity Callbacks
        void Awake()
        {
            if (Animal == null)
            {
                Animal = gameObject.FindComponent<MAnimal>();
            }
            AIControl ??= gameObject.FindInterface<IAIControl>();
            AIControl.UpdateDestinationPosition = false; //Otherwise the AI will go to the current target

            var AnimalStatscomponent = Animal.FindComponent<Stats>();
            if (AnimalStatscomponent)
            {

                AnimalStats = AnimalStatscomponent.Stats_Dictionary();
            }
            Animal.isPlayer.Value = false; //If is using a AIBrain... disable that he is the main player
                                           // ResetVarsOnNewState();
        }

        private void Start()
        {
            behaviourRunner = GetComponent<BehaviourRunner>();
            //aim = Animal.GetComponentInChildren<Aim>();
            //weaponManager = Animal.GetComponent<MAIBrain.weaponManager>();
            //comboManager = Animal.GetComponentInChildren<ComboManager>();
            //stats = Animal.GetComponent<Stats>();
            //damageable = Animal.GetComponent<MDamageable>();
            if (stats != null)
            {
                statList = stats.stats;
            }
            AIControl.UpdateDestinationPosition = false; //Otherwise the AI will go to the current target
                                                         // Assuming 'parentObject' is the parent GameObject that contains the child objects
        }

        public void OnEnable()
        {
            AIControl.TargetSet.AddListener(OnTargetSet);
            Animal.OnStateChange.AddListener(OnAnimalStateChange);
        }

        public void OnDisable()
        {
            AIControl.TargetSet.RemoveListener(OnTargetSet);
            Animal.OnStateChange.RemoveListener(OnAnimalStateChange);
            AIControl.Stop();
            StopAllCoroutines();
        }
        #endregion

        void OnAnimalStateChange(int state)
        {
            if (state == StateEnum.Death) //meaning this animal has died
            {
                StopAllCoroutines();
                enabled = false;

                if (DisableAIOnDeath)
                {
                    behaviourRunner.enabled = false;
                    AIControl.SetActive(false);
                    Animal.enabled = false;
                    this.SetEnable(false);
                    if (weaponManager != null)
                    {
                        weaponManager.enabled = false;
                    }
                }
            }
        }

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

        /// <summary>
        /// Sets the position of the target.
        /// Can be called, for example, in the Animator if you need the current position of the target. 
        /// Useful if the AI has spells that need a position to instantiate an effect.
        /// </summary>
        public void SetTargetPosition()
        {
            if (Target != null && targetTransform != null)
            {
                targetTransform.position = Target.position;
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
#endif
    }
}

