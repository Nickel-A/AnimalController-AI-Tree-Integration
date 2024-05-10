using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using RenownedGames.Apex;
using System;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    public enum CompareTarget { IsNull, isTransformVar, IsInRuntimeSet, HasName, IsActiveInHierarchy, BlackBoard, MalbersTags }

    [NodeContent("Check Target", "Animal Controller/MConditionDecorator/Check Target", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckTarget : MConditionDecorator
    {
        [Header("Node")]
        public CompareTarget compare = CompareTarget.IsNull;

        [ShowIf("compare", CompareTarget.IsInRuntimeSet)]
        public RuntimeGameObjects set;
        [ShowIf("compare", CompareTarget.isTransformVar)]
        public TransformVar transform;
        [ShowIf("compare", CompareTarget.HasName)]
        public string m_name;
        [ShowIf("compare", CompareTarget.BlackBoard)]
        public TransformKey blackBoard;
        [ShowIf("compare", CompareTarget.MalbersTags)]
        public Tag[] tags;

        bool targetMatched;


        protected override bool CalculateResult()
        {
            targetMatched = false; // Reset targetMatched

            switch (compare)
            {
                case CompareTarget.IsNull:
                    targetMatched = AIBrain.Target == null;
                    break;
                case CompareTarget.isTransformVar:
                    targetMatched = transform.Value != null && AIBrain.Target == transform.Value;
                    break;
                case CompareTarget.IsInRuntimeSet:
                    targetMatched = set != null && set.Items.Contains(AIBrain.Target.gameObject);
                    break;
                case CompareTarget.HasName:
                    targetMatched = !string.IsNullOrEmpty(m_name) && AIBrain.Target && AIBrain.Target.name.Contains(m_name);
                    break;
                case CompareTarget.IsActiveInHierarchy:
                    targetMatched = AIBrain.Target && AIBrain.Target.gameObject.activeInHierarchy;
                    break;
                case CompareTarget.BlackBoard:
                    targetMatched = AIBrain.Target && blackBoard.GetValue();
                    break;
                case CompareTarget.MalbersTags:
                    if (AIBrain.Target.GetComponent<Tags>() != null)
                    {
                        var filtredTags = Tags.GambeObjectbyTag(tags);
                        for (int i = 0; i < filtredTags.Count; i++)
                        {
                            if (AIBrain.Target.GetComponent<Tags>())//.HasTag(filtredTags[i])
                            {
                                return true;
                            }
                        }
                    }else
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return targetMatched;
        }


        public override string GetDescription()
        {
            string description = base.GetDescription();
            if (!string.IsNullOrEmpty(description))
            {
                description += "\n";
            }

            description += "Target should be: ";
            switch (compare)
            {
                case CompareTarget.IsNull:
                    description += "Null";
                    break;
                case CompareTarget.isTransformVar:
                    description += "Transform Variable";
                    //if (transform != null)
                    //{
                    //    description += $"TargetVar: {transform.Value.name}\n";
                    //}
                    //else
                    {
                        description += "Transform Variable\n";
                    }
                    //description += $"{(transform != null ? transform.Value.ToString() : "None")}";
                    break;
                case CompareTarget.IsInRuntimeSet:
                    //description += "Runtime Set: ";
                    description += $"{(set != null ? set.name : "None")}";
                    break;
                case CompareTarget.HasName:
                    //description += "Name: ";
                    description += $"{m_name}";
                    break;
                case CompareTarget.IsActiveInHierarchy:
                    description += "Active in Hierarchy";
                    break;
                default:
                    break;
            }
            description += "\n";

            string targetName="";
            if (AIBrain == null)
            {
               targetName = "null";
            }
            else
            {
                if (AIBrain.Target != null)
                {
                    targetName = AIBrain.Target.ToString();
                }
            }
            description += "Target is: ";
            description += $"{targetName}";

            return description;
        }





    }
}