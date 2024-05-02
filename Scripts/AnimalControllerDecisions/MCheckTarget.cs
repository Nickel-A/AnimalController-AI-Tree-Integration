using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEngine;


namespace Malbers.Integration.AITree
{
    public enum CompareTarget { IsNull, isTransformVar, IsInRuntimeSet, HasName, IsActiveInHierarchy, BlackBoard, MalbersTags }

    [NodeContent("Check Target", "Animal Controller/Check Target", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckTarget : ConditionDecorator
    {
        [Header("Node")]
        public CompareTarget compare = CompareTarget.IsNull;

        [Hide("compare", 2)]
        public RuntimeGameObjects set;
        [Hide("compare", 1)]
        public TransformVar transform;
        [Hide("compare", 3)]
        public string m_name;
        [Hide("compare", 5)]
        public TransformKey blackBoard;
        [Hide("compare", 6)]
        public Tag[] tags;

        bool targetMatched;
        AIBrain aiBrain;


        protected override void OnInitialize()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();
        }

        protected override bool CalculateResult()
        {
            targetMatched = false; // Reset targetMatched

            switch (compare)
            {
                case CompareTarget.IsNull:
                    targetMatched = aiBrain.Target == null;
                    break;
                case CompareTarget.isTransformVar:
                    targetMatched = transform.Value != null && aiBrain.Target == transform.Value;
                    break;
                case CompareTarget.IsInRuntimeSet:
                    targetMatched = set != null && set.Items.Contains(aiBrain.Target.gameObject);
                    break;
                case CompareTarget.HasName:
                    targetMatched = !string.IsNullOrEmpty(m_name) && aiBrain.Target && aiBrain.Target.name.Contains(m_name);
                    break;
                case CompareTarget.IsActiveInHierarchy:
                    targetMatched = aiBrain.Target && aiBrain.Target.gameObject.activeInHierarchy;
                    break;
                case CompareTarget.BlackBoard:
                    targetMatched = aiBrain.Target && blackBoard.GetValue();
                    break;
                case CompareTarget.MalbersTags:
                    if (aiBrain.Target.GetComponent<Tags>() != null)
                    {
                        var filtredTags = Tags.GambeObjectbyTag(tags);
                        for (int i = 0; i < filtredTags.Count; i++)
                        {
                            if (aiBrain.Target.GetComponent<Tags>())//.HasTag(filtredTags[i])
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
            if (aiBrain == null)
            {
               targetName = "null";
            }
            else
            {
                if (aiBrain.Target != null)
                {
                    targetName = aiBrain.Target.ToString();
                }
            }
            description += "Target is: ";
            description += $"{targetName}";

            return description;
        }





    }
}