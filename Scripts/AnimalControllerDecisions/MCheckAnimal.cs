using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Animal", "Animal Controller/Check Animal", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckAnimal : ConditionDecorator
    {
        public enum Affect { Target, BlackBoardAnimal };
        public Affect affect;
        public enum LookFor { HasArrived, IsSprinting };
        public LookFor lookFor;
        public TransformKey BlackBoardAnimal;
        private AIBrain aiBrain;
        private MAnimal mAnimal;
        private MAnimalAIControl AIControl;
        private bool result;
        float remainingdistance;

        protected override void OnInitialize()
        {


        }

        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            if (aiBrain == null)
            {
                aiBrain = GetOwner().GetComponent<AIBrain>();
            }

            if (mAnimal == null)
            {
                if (affect == Affect.Target)
                {
                    mAnimal = aiBrain.AIControl.Target.gameObject.FindComponent<MAnimal>();
                    AIControl = aiBrain.AIControl.Target.gameObject.GetComponentInChildren<MAnimalAIControl>();
                }
                if (affect == Affect.BlackBoardAnimal)
                {
                    mAnimal = BlackBoardAnimal.GetValue().gameObject.FindComponent<MAnimal>();
                    if (BlackBoardAnimal.name != "Self")
                    {
                        AIControl = BlackBoardAnimal.GetValue().gameObject.GetComponentInChildren<MAnimalAIControl>();
                    }
                    else
                    {
                        AIControl = (MAnimalAIControl)aiBrain.AIControl;
                    }
                }
            }


            switch (lookFor)
            {
                case LookFor.IsSprinting:
                    result = mAnimal.Sprint;
                    break;
                case LookFor.HasArrived:
                    result = AIControl.HasArrived;
                    break;
            }
            /*
            remainingdistance = aiBrain.AIControl.RemainingDistance;

            if (string.IsNullOrEmpty(TargetName))
            {
                Result =
                    aiBrain.AIControl.HasArrived;
            }
            else
            {
                Result = aiBrain.AIControl.HasArrived && 
                   (aiBrain.Target.name == TargetName || aiBrain.Target.root.name == TargetName); //If we are looking for an specific Target
            }
            */
            return result;
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();
            if (!string.IsNullOrEmpty(description))
            {
                description += "\n";
            }

            //description += $"Remaining Distance: {remainingdistance}\n";
            description += $"Result: {result}\n";

            return description;
        }
    }
}