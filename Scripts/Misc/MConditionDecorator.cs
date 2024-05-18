using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    /// <summary>
    /// Base class for Malbers Animal AI ConditionDecorator nodes
    /// Provides shared and abstract definitions for all Malbers Animal AI nodes designed for use
    /// with the AI Tree asset and Malbers Animal Controller.
    /// </summary>
    public abstract class MConditionDecorator : DecoratorNode
    {
        [Title("Condition")]
        [SerializeField]
        private bool inverseCondition;

        [SerializeField]
        private bool waitUntilTrue;

        /// <summary>
        /// Exposes the AIBrain to all inheriting classes. This is a wrapper for Malbers components
        /// including AIControl and any other components we may need in the future.
        /// </summary>
        protected AIBrain AIBrain;

        // Stored required properties.
        private bool running;
        /// <summary>
        /// Find the AIBrain component. This must be on the same GameObject as the BehaviourRunner component
        /// </summary>
        protected override void OnInitialize()
        {
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

            if (inverseCondition)
            {
                result = !result;
            }

            if (result || running)
            {
                running = true;
                return UpdateChild();
            }

            if (waitUntilTrue)
            {
                return State.Running;
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
        /// Calculates the result of the condition.
        /// </summary>
        protected abstract bool CalculateResult();

        #region [Getter / Setter]
        public bool InverseCondition()
        {
            return inverseCondition;
        }

        public void InverseCondition(bool value)
        {
            inverseCondition = value;
        }
        #endregion
    }
}