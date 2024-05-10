using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Is Target In Set", "Animal Controller/MConditionDecorator/Is Target In Set", IconPath = "Icons/AIDecision_Icon.png")]
    public class MIsTargetInSet : MConditionDecorator
    {
        [Header("Node")]

        public RuntimeGameObjects Set;

        [Tooltip("Check if the Current Target is the closest ")]
        public bool IsClosest = false;

        [Tooltip("If the closest Object is not the Current Target. Assign it as a new target")]
        [Hide("IsClosest")]
        public bool ClosestIsNewTarget = false;

        [Tooltip("When the new target is assigned, set it to move to the target")]
        [Hide("ClosestIsNewTarget")]
        public bool Move = true;

        // Override the Evaluate method or else your environment will throw an error
        protected override bool CalculateResult()
        {
            if (AIBrain.AIControl.Target != null)
            {
                var IsInSet = Set.Items.Contains(AIBrain.Target.gameObject);

                if (IsInSet)
                {
                    if (IsClosest)
                    {
                        var ClosestObject = Set.Item_GetClosest(AIBrain.gameObject);

                        if (ClosestObject != AIBrain.Target.gameObject)
                        {
                            if (ClosestIsNewTarget)
                            {
                                AIBrain.AIControl.SetTarget(ClosestObject.transform, Move);
                            }

                            return false; //Return false if the current target IS not the closest one
                        }
                        return true; //Return true if the current target IS the closest one
                    }
                    return IsInSet;
                }
            }
            return false;
        }
    }
}