using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    [NodeContent("Check Malbers Tag", "Animal Controller/MConditionDecorator/Check Malbers Tag", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckMalbersTag : MConditionDecorator
    {
        [Header("Node")]

        public Affected CheckOn = Affected.Self;

        public bool CheckInParent = true;
        public Tag[] tags;
        private bool result;


        protected override bool CalculateResult()
        {

            if (CheckOn == Affected.Self)
            {
                if (CheckInParent)
                {
                    result = AIBrain.gameObject.HasMalbersTagInParent(tags);
                    return result;
                }
                else
                {
                    result = AIBrain.gameObject.HasMalbersTag(tags);
                    return result;
                }
            }
            else
            {
                if (AIBrain.Target)
                {
                    if (CheckInParent)
                    {
                        result = AIBrain.Target.HasMalbersTagInParent(tags);
                        return result;
                    }
                    else
                    {
                        result = AIBrain.Target.HasMalbersTag(tags);
                        return result;
                    }
                        
                }
            }
            return false;
        }

        public override string GetDescription()
        {
            string description = $"Tag(s): ";
            if (tags !=null)
            {
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i] != null)
                    {
                        description += $"{tags[i].DisplayName}";
                    }
                        if (i != tags.Length - 1) description += ", ";
                }
                description += "\n";
            }
            else
            {
                description += "\n";
            }
            description += $"Result: {result} \n";

            return description;
        }



    }


}