using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using Unity.VisualScripting;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Strafe", "Animal Controller/Animal/Set Strafe", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSetStrafeNode : TaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;

        public BoolReference strafe = new BoolReference(true);

        //  public enum StrafeActions { }


        [Hide(nameof(showSelf))]
        [Tooltip("The Strafe Target of this AI Character, will be this Current AI Target")]
        public bool TargetIsStrafeTarget;
        [Hide(nameof(showTarget))]
        [Tooltip("The Strafe Target of the current AI Target, will be this AI Character")]
        public bool SelfIsStrafeTarget = true;

        [Tooltip("Add a completely new Strafe Target to the Animal")]
      [Hide(nameof(showTransform))]
        public TransformVar NewStrafeTarget;

        AIBrain aiBrain;

        protected override void OnEntry()
        {
            aiBrain =  GetOwner().GetComponent<AIBrain>();
            var StrafeTarget = this.NewStrafeTarget != null ? this.NewStrafeTarget.Value : null;


            if (affect == Affected.Self)
            {
                aiBrain.Animal.Strafe = strafe.Value;

                if (StrafeTarget == null)
                {
                    StrafeTarget = aiBrain.AIControl.Target;
                }

                if (TargetIsStrafeTarget)
                {
                    aiBrain.Animal.Aimer.SetTarget(StrafeTarget);
                }
            }
            else
            {
                if (aiBrain.TargetAnimal)
                {
                    aiBrain.TargetAnimal.Strafe = strafe.Value;
                    if (StrafeTarget == null)
                    {
                        StrafeTarget = aiBrain.Animal.transform;
                    }

                    if (SelfIsStrafeTarget)
                    {
                        aiBrain.TargetAnimal.Aimer.SetTarget(StrafeTarget);
                    }
                }
            }

            aiBrain.TasksDone = true; //Set Done to this task

        }

        protected override State OnUpdate()
        {
            if (aiBrain.TasksDone)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }

        [HideInInspector, SerializeField] bool showTransform, showSelf, showTarget;
        private void OnValidate()
        {
            if (NewStrafeTarget != null)
            {
                TargetIsStrafeTarget = false;
                SelfIsStrafeTarget = false;
            }

            showTransform = (affect == Affected.Self && !TargetIsStrafeTarget) || (affect == Affected.Target && !SelfIsStrafeTarget);
            showSelf = affect == Affected.Self && NewStrafeTarget == null;
            showTarget = affect == Affected.Target && NewStrafeTarget == null;
        }


        public override string GetDescription()
        {
            string description = base.GetDescription();

            description += "Affect: ";
            if (affect == Affected.Self)
            {
                description += "Self\n";
                description += $"Strafe on Self: {SelfIsStrafeTarget}\n";
            }
            else
            {
                description += "Target\n";
                description += $"Strafe on Target: {TargetIsStrafeTarget}\n";
            }
            description += $"Strafe: {strafe.Value}\n";
            return description;
        }
    }
}
