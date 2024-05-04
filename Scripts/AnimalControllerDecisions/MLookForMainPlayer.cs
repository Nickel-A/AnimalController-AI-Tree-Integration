using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using System;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Look For Main Player", "Animal Controller/Look/Look For Main Player", IconPath = "Icons/AIDecision_Icon.png")]
    public class MLookForMainPlayer : ObserverDecorator
    {
        [Header("Node")]

        public Color debugColor = new(0, 0, 0.7f, 0.3f);
        [Range(0, 1)]
        /// <summary>Angle of Vision of the Animal</summary>
        [Tooltip("Shorten the Look Ray to not found the ground by mistake")]
        public float lookMultiplier = 0.9f;

        /// <summary>Range for Looking forward and Finding something</summary>
        [Tooltip("Range for Looking forward and Finding something")]
        public float lookRange = 15;
        [Range(0, 360)]
        /// <summary>Angle of Vision of the Animal</summary>
        [Tooltip("Angle of Vision of the Animal")]
        public float lookAngle = 120;

        [Tooltip("Layers that can block the Animal Eyes")]
        public LayerReference obstacleLayer = new(1);

        [Space(20), Tooltip("If the what we are looking for is found then Assign it as a new Target")]
        public bool assignTarget = false;
        [Tooltip("If the what we are looking for is found then also start moving")]
        public bool moveToTarget = false;
        AIBrain brain;


        public override event Action OnValueChange;
        protected override void OnInitialize()
        {
            base.OnInitialize();
            brain = GetOwner().GetComponent<AIBrain>();
        }

        protected override void OnFlowUpdate()
        {
            base.OnFlowUpdate();
            OnValueChange?.Invoke();
        }

        /// <summary>
        /// Calculates the result of the condition.
        /// </summary>
        public override bool CalculateResult()
        {
            bool result = LookForAnimalPlayer(brain);
            if (result && assignTarget)
            {
                brain.AIControl.SetTarget(MAnimal.MainAnimal.transform, moveToTarget);
            }
            return result;
        }


        private bool LookForAnimalPlayer(AIBrain brain)
        {
            if (MAnimal.MainAnimal == null || MAnimal.MainAnimal.ActiveStateID == StateEnum.Death)
            {
                return false; //Means the animal is death or Disable
            }

            if (MAnimal.MainAnimal == brain.Animal) { Debug.LogError("AI Animal is set as MainAnimal. Fix it!", brain.Animal); return false; }

            return AIUtility.IsInFieldOfView(brain, MAnimal.MainAnimal.Center, lookAngle, lookRange, lookMultiplier, obstacleLayer, out _);
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            AIUtility.DrawFieldOfViewGizmos(brain, debugColor, lookAngle, lookRange);
        }
#endif
    }
}
