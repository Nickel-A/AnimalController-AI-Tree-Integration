using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Target From BB", "Animal Controller/ACMovement/Set Target From BB", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetTargetFromBB : MTaskNode
    {

        public TransformKey target;
        public bool MoveToTarget = true;

        protected override void OnEntry()
        {
            if (MoveToTarget)
            {
                AIBrain.AIControl.UpdateDestinationPosition = true;          //Check if the target has moved
            }
            else
            {
                if (AIBrain.AIControl.IsMoving) { AIBrain.AIControl.Stop(); } //Stop if the animal is already moving
            }
            AIBrain.AIControl.SetTarget(target.GetValue(), MoveToTarget);

        }

        protected override State OnUpdate()
        {
            return State.Success;
        }


    }
}
