using RenownedGames.AITree;
using UnityEngine;
using UnityEditor;
using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.HAP;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Animal", "Animal Controller/MConditionDecorator/Check Animal", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckAnimal : MConditionDecorator
    {
        public enum LookFor { HasArrived, IsSprinting, IsMounted, CanMount, MyRiderHasTarget, AttackedBy, IsDead };
        public LookFor lookFor;
        public enum Affect { Target, BlackBoardAnimal };
        public Affect affect;

        public TransformKey BlackBoardAnimal;
        private MAnimal mAnimal;
        private MAnimalAIControl AIControl;
        private bool result;
        float remainingdistance;
        private Faction faction;
        private MRider mRider;
        private Mount mount;
        public TransformKey myRiderTarget;
        protected override void OnInitialize()
        {
            faction = GetOwner().gameObject.GetComponent<Faction>();

        }

        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            if (mAnimal == null && lookFor!=LookFor.MyRiderHasTarget)
            {
                if (affect == Affect.Target)
                {
                    mAnimal = AIBrain.AIControl.Target.gameObject.FindComponent<MAnimal>();
                    AIControl = AIBrain.AIControl.Target.gameObject.GetComponentInChildren<MAnimalAIControl>();
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
                        AIControl = (MAnimalAIControl)AIBrain.AIControl;
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
                    remainingdistance = AIBrain.AIControl.RemainingDistance;
                    break;
                case LookFor.CanMount:
                    result = mAnimal.GetComponent<MRider>().CanMount;
                    break;
                case LookFor.IsMounted:
                    result = mAnimal.GetComponent<MRider>().Mounted;
                    break;
                case LookFor.IsDead:
                    result = mAnimal.ActiveState.StateIDName == "Death";
                    break;
                case LookFor.MyRiderHasTarget:
                    if (mRider == null) mRider = faction.mount.Rider;
                    if (mRider.gameObject.GetComponentInChildren<MAnimalAIControl>().Target != null)
                    {
                        myRiderTarget.SetValue(mRider.gameObject.GetComponentInChildren<MAnimalAIControl>().Target);
                        return true;
                    }else
                    {
                        return false;
                    }
                    /*
                        Blackboard blackboard = mRider.gameObject.GetComponentInChildren<Faction>().behaviourRunner.GetBlackboard();
                        if (blackboard.TryFindKey<TransformKey>(BBRiderEnemyKey, out TransformKey transformKey))
                        {
                            if (transformKey.GetValue() != null)
                            {
                                MyRiderEnemy.SetValue(transformKey.GetValue());
                                result = true;
                            }
                        }
                    */
            }
             
            return result;
        }

        public override string GetDescription()
        {
            string description = "";
            switch (lookFor)
            {
                case LookFor.HasArrived:
                    description += $"Remaining Distance: {remainingdistance}\n";
                    break;
            }

            description += $"Result: {result}\n";

            return description;
        }
    }
}