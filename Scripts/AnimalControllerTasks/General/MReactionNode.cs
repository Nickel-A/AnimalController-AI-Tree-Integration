using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Reactions;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Reaction", "Animal Controller/General/Reaction", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MReactionNode : MTaskNode
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

        bool taskDone;
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {
            React(AIBrain, reaction);
        }

        protected override State OnUpdate()
        {
            if (taskDone)
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
            React(AIBrain, reactionOnExit);
            taskDone = false;
        }

        private void React(AIBrain AIBrain, Reaction reaction)
        {
            if (affect == Affected.Self)
            {
                reaction?.React(AIBrain.Animal);
            }
            else
            {
                if (AIBrain.Target)
                {
                    reaction?.React(AIBrain.Target);
                }
            }
            taskDone = true;
        }
    }
}
