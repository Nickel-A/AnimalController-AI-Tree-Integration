using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Scriptables;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{

    [NodeContent("Play State", "Animal Controller/Animal/Play State", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MPlayStateNode : MTaskNode
    {
        [Header("Node")]
        [Tooltip("State to play")]
        public StateID StateID;
        [Tooltip("Play the State only when the animal has arrived to the target")]
        public bool PlayNearTarget = false;

        [Space, Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;
        [Tooltip("What to do with the State")]
        public StateAction action = StateAction.Activate;

        public ExecuteTask Play = ExecuteTask.OnStart;
        [Tooltip("Time elapsed to Play the Mode again and Again")]
        public FloatReference CoolDown = new FloatReference(2f); 
        [Tooltip("Specify the exit status to be set.")]
        public IntReference exitStatus = new IntReference(0);

        bool taskDone;

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnEntry()
        {

            if (Play == ExecuteTask.OnStart)
            {
                StateActivate(AIBrain);
                taskDone = true;
            }
        }

        protected override State OnUpdate()
        {
            if (Play == ExecuteTask.OnUpdate)
            {
                StateActivate(AIBrain); //If the animal is in range of the Target
            }

            if (taskDone || Play == ExecuteTask.OnExit)
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
            if (Play == ExecuteTask.OnExit) //If the animal is in range of the Target
            {
                StateActivate(AIBrain);               
            }
            taskDone = false;
        }

        private void StateActivate(AIBrain AIBrain)
        {
            if (PlayNearTarget && !AIBrain.AIControl.HasArrived)
            {
                return; //Dont play if Play on target is true but we are not near the target.
            }

            switch (affect)
            {
                case Affected.Self:
                    PlayState(AIBrain.Animal);
                    taskDone = true;
                    break;
                case Affected.Target:
                    if (AIBrain.TargetAnimal)
                    {
                        PlayState(AIBrain.TargetAnimal);
                    }

                    taskDone = true;
                    break;
                default:
                    break;
            }
        }

        public void PlayState(MAnimal CurrentAnimal)
        {
            switch (action)
            {
                case StateAction.Activate:
                    CurrentAnimal.State_Activate(StateID);
                    break;
                case StateAction.AllowExit:
                    if (CurrentAnimal.ActiveStateID == StateID)
                    {
                        CurrentAnimal.ActiveState.AllowExit();
                    }
                    break;
                case StateAction.ForceActivate:
                    CurrentAnimal.State_Force(StateID);
                    break;
                case StateAction.Enable:
                    CurrentAnimal.State_Enable(StateID);
                    break;
                case StateAction.Disable:
                    CurrentAnimal.State_Disable(StateID);
                    break;
                case StateAction.SetExitStatus:
                    CurrentAnimal.State_SetExitStatus(exitStatus);
                    break;
                default:
                    break;
            }
        }

        public override string GetDescription()
        {
            string description = base.GetDescription();

            description += $"Play near: {PlayNearTarget}\n";
            description += $"State ID: {(StateID != null ? StateID.DisplayName : "null")}\n";
            if (affect == Affected.Self)
            {
                description += "Self ";
                if (Play == ExecuteTask.OnStart)
                {
                    description += "Play OnStart\n";
                }
                else if (Play == ExecuteTask.OnUpdate)
                {
                    description += "Play OnUpdate\n";
                }
                else
                {
                    description += "Play OnExit\n";
                }

                switch (action)
                {
                    case StateAction.Activate:
                        description += $"StateAction: Activate\n";
                        break;
                    case StateAction.AllowExit:
                        description += $"StateAction: AllowExit\n";
                        break;
                    case StateAction.ForceActivate:
                        description += $"StateAction: ForceActivate\n";
                        break;
                    case StateAction.Enable:
                        description += $"StateAction: Enable\n";
                        break;
                    case StateAction.Disable:
                        description += $"StateAction: Disable\n";
                        break;
                    case StateAction.SetExitStatus:
                        description += $"Set exit status: {exitStatus.Value}\n";
                        break;
                    case StateAction.None:
                        description += $"StateAction: None\n";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                description += "Target ";
                if (Play == ExecuteTask.OnStart)
                {
                    description += "Play OnStart\n";
                }
                else if (Play == ExecuteTask.OnUpdate)
                {
                    description += "Play OnUpdate\n";
                }
                else
                {
                    description += "Play OnExit\n";
                }

                switch (action)
                {
                    case StateAction.Activate:
                        description += $"State Action: Activate\n";
                        break;
                    case StateAction.AllowExit:
                        description += $"State Action: Allow Exit\n";
                        break;
                    case StateAction.ForceActivate:
                        description += $"State Action: Force Activate\n";
                        break;
                    case StateAction.Enable:
                        description += $"State Action: Enable\n";
                        break;
                    case StateAction.Disable:
                        description += $"State Action: Disable\n";
                        break;
                    case StateAction.SetExitStatus:
                        description += $"Set exit status: {exitStatus.Value}\n";
                        break;
                    case StateAction.None:
                        description += $"State Action: None\n";
                        break;
                    default:
                        break;
                }
            }
            //description += $"Cooldown: {CoolDown.Value}\n";
            return description;
        }
    }
}
