using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{

    [NodeContent("Clear Target", "Animal Controller/ACMovement/Clear Target", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MClearTarget : MTaskNode
    {
        // public GameObject target;

        protected override void OnInitialize()
        {
            base.OnInitialize();

        }

        protected override void OnEntry()
        {
            //AIBrain.AIControl.ClearTarget();
        }

        protected override State OnUpdate()
        {
            AIBrain.AIControl.ClearTarget();
            return State.Success;


        }
    }
}

