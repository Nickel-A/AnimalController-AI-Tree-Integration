using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Circle Around", "Animal Controller/ACMovement/Circle Around", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MCircleAround : TaskNode
    {
        public enum CircleDirection { Left, Right };

        [Header("Node")]
        /// <summary> Distance for the Flee, Circle Around and keep Distance Task</summary>
        public FloatReference distance = new(10f);
        /// <summary> Distance Threshold for the Keep Distance Task</summary>
        public FloatReference stoppingDistance = new(0.5f);
        /// <summary> Animal Controller slowing Distance to Override the AI Movement Stopping Distance</summary>
        public FloatReference slowingDistance = new(0);

        /// <summary> Amount of Target Position around the Target</summary>
        public int arcsCount = 12;
        /// <summary> Animal Controller Stopping Distance to Override the AI Movement Stopping Distance</summary>
        public CircleDirection direction = CircleDirection.Left;
        public bool circleAroundForever;
        bool arrived;


        AIBrain aiBrain;

        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            aiBrain = GetOwner().GetComponent<AIBrain>();

        }

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            base.OnEntry();
            arrived = false;

            aiBrain.AIControl.UpdateDestinationPosition = false;
            aiBrain.AIControl.CurrentSlowingDistance = slowingDistance;          //Set the Animal to look Forward to the Target
            CalculateClosestCirclePoint(aiBrain);

        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            CircleAround(aiBrain);
            if (arrived)
            {
                return State.Success;
            }
            return State.Running;
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
            arrived = false;
        }

        private void CalculateClosestCirclePoint(AIBrain aiBrain)
        {
            if (aiBrain.Target == null)
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
                var CurrentPoint = aiBrain.Target.position + (currentDirection.normalized * distance);

                float DistCurrentPoint = Vector3.Distance(CurrentPoint, aiBrain.transform.position);

                if (minDist > DistCurrentPoint)
                {
                    minDist = DistCurrentPoint;
                    MinIndex = i;
                    MinPoint = CurrentPoint;
                }

                currentDirection = rotation * currentDirection;
            }

            aiBrain.AIControl.UpdateDestinationPosition = false;
            aiBrain.AIControl.StoppingDistance = stoppingDistance;

            aiBrain.TasksVars.intValue = MinIndex;   //Store the Point index on the vars of this Task
            aiBrain.TasksVars.boolValue = true;      //Store true on the Variables, so we can seek for the next point
                                                     // brain.TaskAddBool(index, circleAround, true);          //Store true on the Variables, so we can seek for the next point

            aiBrain.AIControl.UpdateDestinationPosition = false; //Means the Animal Wont Update the Destination Position with the Target position.
            aiBrain.AIControl.SetDestination(MinPoint, true);
            aiBrain.AIControl.HasArrived = false;
        }

        private void CircleAround(AIBrain aiBrain)
        {

            //  Debug.Log("circle aound = ");
            if (aiBrain.AIControl.HasArrived) //Means that we have arrived to the point so set the next point
            {
                aiBrain.TasksVars.intValue++;
                aiBrain.TasksVars.intValue = aiBrain.TasksVars.intValue % arcsCount;
                aiBrain.TasksVars.boolValue = true;      //Set this so we can seek for the next point
                if (!circleAroundForever)
                {
                    arrived = true;                                  //brain.TaskSetBool(index, circleAround, true);   //Set this so we can seek for the next point
                }
            }

            if (aiBrain.TasksVars.boolValue || aiBrain.AIControl.TargetIsMoving)
            // if (brain.TaskGetBool(index, circleAround) || brain.AIControl.TargetIsMoving)
            {
                int pointIndex = aiBrain.TasksVars.intValue;

                float arcDegree = 360.0f / arcsCount;
                int Dir = direction == CircleDirection.Right ? 1 : -1;
                Quaternion rotation = Quaternion.Euler(0, Dir * arcDegree * pointIndex, 0);


                // var distance = this.distance * brain.Animal.ScaleFactor; //Remember to use the scale

                Vector3 currentDirection = Vector3.forward;
                currentDirection = rotation * currentDirection;

                // Debug.Log(brain.Target);

                Vector3 CurrentPoint = aiBrain.Target.position + (currentDirection.normalized * distance);



                aiBrain.AIControl.UpdateDestinationPosition = false; //Means the Animal Wont Update the Destination Position with the Target position.
                aiBrain.AIControl.SetDestination(CurrentPoint, true);

                aiBrain.TasksVars.boolValue = false;           //Set this so we can seek for the next point
                                                               //brain.TaskSetBool(index, circleAround, false);      //Set this so we can seek for the next point
            }
        }
    }
}