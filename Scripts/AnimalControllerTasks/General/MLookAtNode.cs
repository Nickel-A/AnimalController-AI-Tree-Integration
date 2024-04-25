using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look At", "Animal Controller/Look At", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MLookAtNode : TaskNode
    {

        public enum LookAtOption1 { CurrentTarget, TransformVar }
        public enum LookAtOption2 { AIAnimal, TransformVar }

        [Header("Node")]
        [Tooltip("Check the Look At Component on the Target or on Self")]
        [UnityEngine.Serialization.FormerlySerializedAs("SetLookAtOn")]
        public Affected SetAimOn = Affected.Self;

        [Hide("SetAimOn", (int)Affected.Self)]
        public LookAtOption1 LookAtTargetS = LookAtOption1.CurrentTarget;
        [Hide("SetAimOn", (int)Affected.Target)]
        public LookAtOption2 LookAtTargetT = LookAtOption2.AIAnimal;
        [Hide("showTransformVar")]
        public TransformVar TargetVar;

        [Tooltip("If true .. it will Look for a gameObject on the Target with the Tag[tag].... else it will look for the gameObject name")]
        public bool UseTag = false;

        [Hide("UseTag", true), Tooltip("Search for the Target Child gameObject name")]
        public string BoneName = "Head";
        [Hide("UseTag"), Tooltip("Look for a child gameObject on the Target with the Tag[tag]")]
        public Tag tag;
        [Tooltip("When the Task ends it will Remove the Target on the Aim Component")]
        public bool DisableOnExit = true;

        bool TaskDone;
        AIBrain aiBrain;
        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();

            Transform child = null;
            if (aiBrain != null)
            {


                if (SetAimOn == Affected.Self)
                {
                    switch (LookAtTargetS)
                    {
                        case LookAtOption1.CurrentTarget:
                            child = UseTag ? GetGameObjectByTag(aiBrain.Target) : GetChildByName(aiBrain.Target);
                            break;
                        case LookAtOption1.TransformVar:
                            child = UseTag ? GetGameObjectByTag(TargetVar.Value) : GetChildByName(TargetVar.Value);
                            break;
                        default:
                            break;
                    }

                    aiBrain.Animal.FindInterface<IAim>()?.SetTarget(child);
                }
                else
                {
                    if (LookAtTargetT == LookAtOption2.AIAnimal)
                    {
                        child = UseTag ? GetGameObjectByTag(aiBrain.Animal.transform) : GetChildByName(aiBrain.Animal.transform);
                    }
                    else
                    {
                        child = UseTag ? GetGameObjectByTag(TargetVar.Value) : GetChildByName(TargetVar.Value);
                    }

                    if (aiBrain.Target)
                    {
                        aiBrain.Target.FindInterface<IAim>()?.SetTarget(child);
                    }
                }

                TaskDone = true;
            }
        }
        protected override State OnUpdate()
        {
            if (TaskDone)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }

        protected override void OnExit()
        {
            if (DisableOnExit)
            {
                aiBrain.Animal.FindInterface<IAim>()?.SetTarget(null);
                if (aiBrain.Target)
                {
                    aiBrain.Target.FindInterface<IAim>()?.SetTarget(null);
                }
            }
        }
        private Transform GetChildByName(Transform Target)
        {
            if (Target && !string.IsNullOrEmpty(BoneName))
            {
                var child = Target.FindGrandChild(BoneName);
                if (child != null)
                {
                    return child;
                }
            }
            return Target;
        }

        private Transform GetGameObjectByTag(Transform Target)
        {
            if (Target)
            {
                var allTags = Target.root.GetComponentsInChildren<Tags>();

                if (allTags == null)
                {
                    return null;
                }

                foreach (var item in allTags)
                {
                    if (item.HasTag(tag))
                    {
                        return item.transform;
                    }
                }
            }
            return null;
        }

        [HideInInspector] public bool showTransformVar = false;
        private void OnValidate()
        {
            showTransformVar =
                (LookAtTargetS == LookAtOption1.TransformVar && SetAimOn == Affected.Self) ||
                (LookAtTargetT == LookAtOption2.TransformVar && SetAimOn == Affected.Target);
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();

            description += "Set aim on: ";
            if (SetAimOn == Affected.Self)
            {
                description += "Self\n";

                if (LookAtTargetS == LookAtOption1.TransformVar)
                {
                    //if (TargetVar != null)
                    //{
                    //    description += $"TargetVar: {TargetVar.Value.name}\n";
                    //}
                    //else
                    {
                        description += "Transform Variable\n";
                    }

                    if (UseTag)
                    {
                        if (tag != null)
                        {
                            description += $"Use Tag: {tag.DisplayName}\n";
                        }
                    }
                    else
                    {
                        description += $"Bone Name: {BoneName}\n";
                    }
                }
                else
                {
                    description += "Current Target\n";

                    if (UseTag)
                    {
                        if (tag != null)
                        {
                            description += $"Use Tag: {tag.DisplayName}\n";
                        }
                    }
                    else
                    {
                        description += $"Bone Name: {BoneName}\n";
                    }
                }
            }
            else
            {
                description += "Target\n";
                if (LookAtTargetT == LookAtOption2.TransformVar)
                {
                    //if (TargetVar != null)
                    //{
                    //    description += $"TargetVar: {TargetVar.Value.name}\n";
                    //}
                    //else
                    {
                        description += "Transform Variable\n";
                    }

                    if (UseTag)
                    {
                        if (tag != null)
                        {
                            description += $"Use Tag: {tag.DisplayName}\n";
                        }
                    }
                    else
                    {
                        description += $"Bone Name: {BoneName}\n";
                    }
                }
                else
                {
                    description += "AI Animal\n";
                    if (UseTag)
                    {
                        if (tag != null)
                        {
                            description += $"Use Tag: {tag.DisplayName}\n";
                        }
                    }
                    else
                    {
                        description += $"Bone Name: {BoneName}\n";
                    }
                }
            }
            description += $"Disable on Exit: {DisableOnExit}\n";
            return description;

        }

    }
}


