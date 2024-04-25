using MalbersAnimations;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using UnityEngine.Serialization;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    public enum AlignTo { TransformHook, GameObjectHook, CurrentTarget }


    [NodeContent("Quick Align", "Animal Controller/Quick Align", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MQuickAlignNode : TaskNode
    {
        [Header("Node")]

        public AlignTo alignTo = AlignTo.TransformHook;

        [FormerlySerializedAs("AlignTarget")]
        [Hide("alignTo", (int)AlignTo.TransformHook)]

        public TransformVar TransformHook;
        [Hide("alignTo", (int)AlignTo.GameObjectHook)]

        public GameObjectVar GameObjectHook;

        [Tooltip("Align time to rotate towards the Target")]
        public float alignTime = 0.3f;

        float updateTime = 1f;
        AIBrain aiBrain;

        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();


            switch (alignTo)
            {
                case AlignTo.TransformHook:
                    if (TransformHook != null || TransformHook.Value == null)
                    {
                        aiBrain.StartCoroutine(MTools.AlignLookAtTransform(aiBrain.Animal.transform, TransformHook.Value, alignTime));
                    }
                    else
                    {
                        Debug.LogWarning($"The Hook Target is empty or Null", this);
                    }

                    break;
                case AlignTo.GameObjectHook:
                    if (GameObjectHook != null || GameObjectHook.Value == null)
                    {
                        aiBrain.StartCoroutine(MTools.AlignLookAtTransform(aiBrain.Animal.transform, GameObjectHook.Value.transform, alignTime));
                    }
                    else
                    {
                        Debug.LogWarning($"The Hook is empty or Null", this);
                    }

                    break;
                case AlignTo.CurrentTarget:
                    if (aiBrain.Target)
                    {
                        aiBrain.StartCoroutine(MTools.AlignLookAtTransform(aiBrain.Animal.transform, aiBrain.Target, alignTime));
                    }
                    else
                    {
                        Debug.LogWarning($"The Hook is empty or Null", this);
                    }

                    break;
                default:
                    break;
            }


        }

        protected override State OnUpdate()
        {

            if (MTools.ElapsedTime(alignTime, updateTime))
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }


        public override string GetDescription()
        {
            string description = base.GetDescription();
            if (alignTo == AlignTo.TransformHook)
            {
                description += "TransformHook\n";

                //if (TransformHook.Value == null)
                //{
                //    description += TransformHook != null ? $"alignTo: {TransformHook.Value}\n" : "TransformHook is null\n";
                //}
                //else
                //{
                //    description += TransformHook.Value!= null ? $"alignTo: {TransformHook.Value.name}\n" : "TransformHook is null\n";
                //}
            }
            else if (alignTo == AlignTo.GameObjectHook)
            {
                description += GameObjectHook != null ? $"alignTo: {GameObjectHook.Value.name}\n" : "GameObject Hook is null\n";
            }
            else
            {
                description += "Current Target\n";
            }
            description += $"alignTime: {alignTime}\n";
            return description;
        }
    }
}
