using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    public class TaskVariables
    {
        public int IntValue { get; set; }
        public bool BoolValue { get; set; }
    }

    [NodeContent("Circle Around", "Animal Controller/ACMovement/Circle Around", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MCircleAround : MTaskNode
    {
        public enum CircleDirection { Left, Right };

        [Header("Node")]
        public FloatReference distance = new(10f);
        public FloatReference stoppingDistance = new(0.5f);
        public FloatReference slowingDistance = new(0);
        public int arcsCount = 12;
        public CircleDirection direction = CircleDirection.Left;
        public bool circleAroundForever;
        bool arrived;

        TaskVariables taskVars = new TaskVariables();

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {
            base.OnEntry();
            arrived = false;
            AIBrain.AIControl.UpdateDestinationPosition = false;
            AIBrain.AIControl.CurrentSlowingDistance = slowingDistance;
            CalculateClosestCirclePoint(AIBrain);
        }

        protected override State OnUpdate()
        {
            CircleAround(AIBrain);
            if (arrived)
            {
                return State.Success;
            }
            return State.Running;
        }

        protected override void OnExit()
        {
            base.OnExit();
            arrived = false;
        }

        private void CalculateClosestCirclePoint(AIBrain AIBrain)
        {
            if (AIBrain.Target == null)
            {
                return;
            }

            float arcDegree = 360.0f / arcsCount;
            int Dir = direction == CircleDirection.Right ? 1 : -1;
            Quaternion rotation = Quaternion.Euler(0, Dir * arcDegree, 0);

            Vector3 currentDirection = Vector3.forward;
            Vector3 MinPoint = Vector3.zero;
            float minDist = float.MaxValue;

            int MinIndex = 0;

            for (int i = 0; i < arcsCount; ++i)
            {
                var CurrentPoint = AIBrain.Target.position + (currentDirection.normalized * distance);

                float DistCurrentPoint = Vector3.Distance(CurrentPoint, AIBrain.transform.position);

                if (minDist > DistCurrentPoint)
                {
                    minDist = DistCurrentPoint;
                    MinIndex = i;
                    MinPoint = CurrentPoint;
                }

                currentDirection = rotation * currentDirection;
            }

            AIBrain.AIControl.UpdateDestinationPosition = false;
            AIBrain.AIControl.StoppingDistance = stoppingDistance;

            taskVars.IntValue = MinIndex;
            taskVars.BoolValue = true;

            AIBrain.AIControl.UpdateDestinationPosition = false;
            AIBrain.AIControl.SetDestination(MinPoint, true);
            AIBrain.AIControl.HasArrived = false;
        }

        private void CircleAround(AIBrain AIBrain)
        {
            if (AIBrain.AIControl.HasArrived)
            {
                taskVars.IntValue++;
                taskVars.IntValue = taskVars.IntValue % arcsCount;
                taskVars.BoolValue = true;
                if (!circleAroundForever)
                {
                    arrived = true;
                }
            }

            if (taskVars.BoolValue || AIBrain.AIControl.TargetIsMoving)
            {
                int pointIndex = taskVars.IntValue;
                float arcDegree = 360.0f / arcsCount;
                int Dir = direction == CircleDirection.Right ? 1 : -1;
                Quaternion rotation = Quaternion.Euler(0, Dir * arcDegree * pointIndex, 0);

                Vector3 currentDirection = Vector3.forward;
                currentDirection = rotation * currentDirection;

                Vector3 CurrentPoint = AIBrain.Target.position + (currentDirection.normalized * distance);

                AIBrain.AIControl.UpdateDestinationPosition = false;
                AIBrain.AIControl.SetDestination(CurrentPoint, true);

                taskVars.BoolValue = false;
            }
        }
    }
}
