using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{

    [NodeContent("Clear Target", "Animal Controller/ACMovement/Clear Target", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MClearTarget : MTaskNode
    {
        // public GameObject target;
        protected override void OnEntry()
        {
            AIBrain = GetOwner().gameObject.GetComponent<AIBrain>();
            AIBrain.AIControl.SetTarget(null, false);
        }

        protected override State OnUpdate()
        {

            return State.Success;


        }
    }
}

