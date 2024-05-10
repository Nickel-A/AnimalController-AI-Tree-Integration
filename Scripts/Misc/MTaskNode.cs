using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    /// <summary>
    /// Base class for Malbers Animal AI Task nodes
    /// Provides shared and abstract definitions for all Malbers Animal AI nodes designed for use
    /// with the AI Tree asset and Malbers Animal Controller.
    /// </summary>
    public abstract class MTaskNode : TaskNode
    {
        /// <summary>
        /// Exposes the AIBrain to all inheriting classes. This is a wrapper for Malbers components
        /// including AIControl and any other components we may need in the future.
        /// </summary>
        protected AIBrain AIBrain;

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
    }
}