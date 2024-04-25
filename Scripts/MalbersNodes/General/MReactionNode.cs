using MalbersAnimations;
using MalbersAnimations.Reactions;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Reaction", "Animal Controller/Reaction", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MReactionNode : TaskNode
    {
        [Header("Node")]

        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        [SerializeReference, SubclassSelector]
        [Tooltip("Reaction when the AI Task begin")]
        public Reaction reaction;

        [SerializeReference, SubclassSelector]
        [Tooltip("Reaction when the AI State ends")]
        public Reaction reactionOnExit;

        AIBrain aiBrain;
        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
            React(aiBrain, reaction);
        }

        protected override State OnUpdate()
        {
            if (aiBrain.TasksDone)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }

        protected override void OnExit()
        {
            React(aiBrain, reactionOnExit);
        }

        private void React(AIBrain aiBrain, Reaction reaction)
        {
            if (affect == Affected.Self)
            {
                reaction?.React(aiBrain.Animal);
            }
            else
            {
                if (aiBrain.Target)
                {
                    reaction?.React(aiBrain.Target);
                }
            }
            aiBrain.TasksDone = true;
        }
    }
}
