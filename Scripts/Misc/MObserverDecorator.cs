using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    /// <summary>
    /// Base class for Malbers Animal AI Task nodes
    /// Provides shared and abstract definitions for all Malbers Animal AI nodes designed for use
    /// with the AI Tree asset and Malbers Animal Controller.
    /// </summary>
    public abstract class MObserverDecorator : DecoratorNode
    {
        [Title("Flow Control")]
        [SerializeField]
        private NotifyObserver notifyObserver = NotifyObserver.OnResultChange;

        [SerializeField]
        [Enum(HideValues = nameof(GetHiddenObserverAborts))]
        [DisableInPlayMode]
        protected ObserverAbort observerAbort = ObserverAbort.None;

        // Stored required properties.
        private bool running;
        private bool lastResult;

        /// <summary>
        /// Exposes the AIBrain to all inheriting classes. This is a wrapper for Malbers components
        /// including AIControl and any other components we may need in the future.
        /// </summary>
        protected AIBrain AIBrain;

        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();

            OnValueChange += OnValueChangeAction;

            if (!AIBrain)
            {
                AIBrain = GetOwner().GetComponent<AIBrain>();

                if (!AIBrain)
                {
                    Debug.LogError("The AIBrain component must be present on the same GameObject as BehaviourRunner!");
                }
            }
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected sealed override State OnUpdate()
        {
            bool result = CalculateResult();

            if (!running)
            {
                lastResult = result;
            }

            if (!result && (observerAbort & ObserverAbort.Self) != 0)
            {
                return State.Failure;
            }

            if (result || running)
            {
                running = true;
                return UpdateChild();
            }

            return State.Failure;
        }

        /// <summary>
        /// Called every tick regardless of the node execution.
        /// </summary>
        protected override void OnFlowUpdate() { }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
            running = false;
        }

        /// <summary>
        /// Called when the value changes.
        /// </summary>
        private void OnValueChangeAction()
        {
            bool result = CalculateResult();

            switch (notifyObserver)
            {
                case NotifyObserver.OnValueChange:
                    OnNotifyObserver?.Invoke(result);
                    break;
                default:
                case NotifyObserver.OnResultChange:
                    if (lastResult != result)
                    {
                        OnNotifyObserver?.Invoke(result);
                    }
                    break;
            }

            lastResult = result;
        }

        /// <summary>
        /// Calculates the observed value of the result.
        /// </summary>
        public abstract bool CalculateResult();

        /// <summary>
        /// Detail description of entity.
        /// </summary>
        public override string GetDescription()
        {
            string description = base.GetDescription();

            NodeContentAttribute content = GetType().GetCustomAttribute<NodeContentAttribute>();
            if (content != null)
            {
                string name = "Undefined";
                if (!string.IsNullOrWhiteSpace(content.name))
                {
                    name = content.name;
                }
                else if (!string.IsNullOrWhiteSpace(content.path))
                {
                    name = System.IO.Path.GetFileName(content.path);
                }
                description += name;
            }

            switch (observerAbort)
            {
                case ObserverAbort.None:
                    break;
                case ObserverAbort.Self:
                    description += $"\n( aborts self )";
                    break;
                case ObserverAbort.LowPriority:
                    description += $"\n( aborts lower priority )";
                    break;
                case ObserverAbort.Both:
                    description += $"\n( aborts both )";
                    break;
            }

            return description;
        }

        /// <summary>
        /// Dynamically hides the values of Observer Abort.
        /// </summary>
        /// <returns>Values to hide.</returns>
        protected virtual IEnumerable<ObserverAbort> GetHiddenObserverAborts()
        {
            // Temporary solution, to be fixed during a major update.
            // Move basic composites to the core assembly and perform type-based comparison.
            Node parent = GetParent();
            if (parent != null && parent.GetType().Name == "SequencerNode")
            {
                yield return ObserverAbort.LowPriority;
                yield return ObserverAbort.Both;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called when the value in the inspector changes.
        /// </summary>
        protected override void OnInspectorChanged()
        {
            foreach (ObserverAbort hiddenObserverAbort in GetHiddenObserverAborts())
            {
                if ((observerAbort & hiddenObserverAbort) == hiddenObserverAbort)
                {
                    observerAbort &= ~hiddenObserverAbort;
                    break;
                }
            }

            base.OnInspectorChanged();

            if (Application.isPlaying)
            {
                OnValueChangeAction();
            }
        }
#endif

        #region [Events]
        /// <summary>
        /// An event that should be called when the observed value changes.
        /// </summary>
        public abstract event Action OnValueChange;

        /// <summary>
        /// An event that notifies the observer of a change in value.
        /// </summary>
        public event Action<bool> OnNotifyObserver;
        #endregion

        #region [Getter / Setter]
        /// <summary>
        /// Returns a notify observer.
        /// </summary>
        /// <returns>NotifyObserver</returns>
        public NotifyObserver GetNotifyObserver()
        {
            return notifyObserver;
        }

        /// <summary>
        /// Sets the notify observer.
        /// </summary>
        /// <param name="notifyObserver">NotifyObserver</param>
        public void SetNotifyObserver(NotifyObserver notifyObserver)
        {
            this.notifyObserver = notifyObserver;
        }

        /// <summary>
        /// Returns an observer abort.
        /// </summary>
        /// <returns>ObserverAbort</returns>
        public ObserverAbort GetObserverAbort()
        {
            return observerAbort;
        }

        /// <summary>
        /// Sets the observer abort.
        /// </summary>
        /// <param name="observerAbort">ObserverAbort</param>
        public void SetObserverAbort(ObserverAbort observerAbort)
        {
            this.observerAbort = observerAbort;
        }
        #endregion






        /// <summary>
        /// Determines if a target is within the field of view of an AI AIBrain.
        /// </summary>
        /// <param name="AIBrain">The AIBrain component.</param>
        /// <param name="center">The position of the target.</param>
        /// <param name="lookAngle">The angle of the AI's field of view.</param>
        /// <param name="lookRange">The range of the AI's field of view.</param>
        /// <param name="lookMultiplier">A multiplier for adjusting the look range.</param>
        /// <param name="obstacleLayer">The layer mask for obstacles.</param>
        /// <param name="distance">Output parameter for the distance to the target.</param>
        /// <returns>True if the target is within the field of view, false otherwise.</returns>
        public bool IsInFieldOfView(AIBrain AIBrain, Vector3 center, float lookAngle, float lookRange, float lookMultiplier, LayerReference obstacleLayer, out float distance)
        {
            if (AIBrain != null)
            {
                var directionToTarget = center - AIBrain.Eyes.position; // Put the Sight a bit higher

                // Important, otherwise it will find the ground for Objects to close to it. Also Apply the Scale
                distance = Vector3.Distance(center, AIBrain.Eyes.position) * lookMultiplier;

                if (lookAngle == 0 || lookRange <= 0)
                {
                    return true; // Means the Field of view can be ignored
                }

                if (distance < lookRange * AIBrain.Animal.ScaleFactor) // Check if we are inside the Look Radius
                {
                    Vector3 eyesForward = Vector3.ProjectOnPlane(AIBrain.Eyes.forward, AIBrain.Animal.UpVector);

                    var angle = Vector3.Angle(directionToTarget, eyesForward);

                    if (angle < (lookAngle / 2))
                    {
                        // Need a RayCast to see if there's no obstacle in front of the Animal OBSTACLE LAYER
                        if (Physics.Raycast(AIBrain.Eyes.position, directionToTarget, out RaycastHit _, distance, obstacleLayer, QueryTriggerInteraction.Ignore))
                        {
                            return false; // Meaning there's something between the Eyes of the Animal and the Target
                        }
                        else
                        {
                            return true;
                        }
                    }
                    return false;
                }
                // Debug.Log($"False (NOT IN Distance {distance} > RANGE) {lookRange.Value}");
                return false;
            }
            distance = 0;
            return false;
        }

        /// <summary>
        /// Draws the field of view gizmos for debugging purposes.
        /// </summary>
        /// <param name="AIBrain">The AIBrain component.</param>
        /// <param name="debugColor">The color to use for drawing.</param>
        /// <param name="lookAngle">The angle of the field of view.</param>
        /// <param name="lookRange">The range of the field of view.</param>
        public void DrawFieldOfViewGizmos(AIBrain AIBrain, Color debugColor, float lookAngle, FloatReference lookRange)
        {
#if UNITY_EDITOR
            var eyes = AIBrain.Eyes;

            if (eyes != null)
            {
                var scale = AIBrain.Animal ? AIBrain.Animal.ScaleFactor : AIBrain.transform.root.localScale.y;

                Color c = debugColor;
                c.a = 1f;

                Vector3 eyesForward = Vector3.ProjectOnPlane(AIBrain.Eyes.forward, Vector3.up);

                Vector3 rotatedForward = Quaternion.Euler(0, -lookAngle * 0.5f, 0) * eyesForward;
                UnityEditor.Handles.color = c;
                UnityEditor.Handles.DrawWireArc(eyes.position, Vector3.up, rotatedForward, lookAngle, lookRange * scale);
                UnityEditor.Handles.color = debugColor;
                UnityEditor.Handles.DrawSolidArc(eyes.position, Vector3.up, rotatedForward, lookAngle, lookRange * scale);
            }
#endif
        }
    }
}
