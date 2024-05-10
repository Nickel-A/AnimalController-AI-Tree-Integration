using MalbersAnimations;
using RenownedGames.AITree;
using RenownedGames.Apex;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Target To Blackboard", "Animal Controller/ACMovement/Set Target To Blackboard", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetTargetToBlackboard : MTaskNode
    {

        [RequiredField, NotNull] public TransformKey BBKey;

        [Tooltip("When a new target is assinged it also sets that the Animal should move to that target")]
        public bool MoveToTarget = true;
        bool taskDone;

        protected override void OnEntry()
        {
            BBKey.SetValue(AIBrain.Target);
        }

        protected override State OnUpdate()
        {
            if (BBKey.GetValue() != null)
            {
                return State.Success;
            }
                return State.Running;
        }
        protected override void OnExit()
        {
            base.OnExit();
            if (!MoveToTarget)
            {
                AIBrain.AIControl.Stop();
            }
        }
    }
}