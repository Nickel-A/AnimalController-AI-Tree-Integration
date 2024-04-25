using MalbersAnimations;
using MalbersAnimations.Utilities;
using RenownedGames.AITree;
using UnityEngine;
using State = RenownedGames.AITree.State;

namespace Malbers.Integration.AITree
{
    [NodeContent("Massage", "Animal Controller/Massage", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MMassageNode : TaskNode
    {
        [Header("Node")]
        [Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;
        [Tooltip("When you want to send the Message")]
        public ExecuteTask when = ExecuteTask.OnStart;
        public bool UseSendMessage = false;
        public bool SendToChildren = false;
        [Tooltip("Send the message only when the AI is near the target. AI has Arrived")]
        public bool NearTarget = true;
        [Tooltip("The message will be send to the Root of the Hierarchy")]
        public bool SendToRoot = true;
        [NonReorderable]
        public MesssageItem[] messages;
        bool messageDone;
        AIBrain aiBrain;


        protected override void OnEntry()
        {
            aiBrain = GetOwner().GetComponent<AIBrain>();

            if (when == ExecuteTask.OnStart)
            {
                if (!NearTarget || (NearTarget && aiBrain.AIControl.HasArrived))
                {
                    Execute_Task(aiBrain);
                    messageDone = true;
                }
            }
        }

        protected override State OnUpdate()
        {
            if (when == ExecuteTask.OnStart && messageDone)
            {
                return State.Success;
            }
            else if (when == ExecuteTask.OnExit)
            {
                return State.Success;
            }

            if (when == ExecuteTask.OnUpdate)
            {
                if (!NearTarget || (NearTarget && aiBrain.AIControl.HasArrived))
                {
                    Execute_Task(aiBrain);
                    return State.Success;
                }
            }
            return State.Running;

        }

        protected override void OnExit()
        {
            if (when == ExecuteTask.OnExit)
            {
                if (!NearTarget || (NearTarget && aiBrain.AIControl.HasArrived))
                {
                    Execute_Task(aiBrain);
                    messageDone = true;
                }
            }
            messageDone = false;
        }
        private void Execute_Task(AIBrain aiBrain)
        {
            if (affect == Affected.Self)
            {
                SendMessage(SendToRoot ? aiBrain.Animal.transform : aiBrain.transform);
            }
            else
            {
                if (aiBrain.Target != null)
                {
                    SendMessage(SendToRoot ? aiBrain.Target.FindObjectCore() : aiBrain.Target);
                }
            }
        }


        public virtual void SendMessage(Transform t)
        {
            IAnimatorListener[] listeners;

            if (SendToChildren)
            {
                listeners = t.GetComponentsInChildren<IAnimatorListener>();
            }
            else
            {
                listeners = t.GetComponents<IAnimatorListener>();
            }

            foreach (var msg in messages)
            {
                if (UseSendMessage)
                {
                    msg.DeliverMessage(t, SendToChildren);
                }
                else
                {
                    foreach (var animListener in listeners)
                    {
                        msg.DeliverAnimListener(animListener);
                    }
                }
            }
        }
        public override string GetDescription()
        {
            string description = base.GetDescription();
            
            if (affect == Affected.Self)
            {
                description += "Self ";
                if (when == ExecuteTask.OnStart)
                {
                    description += "OnStart\n";
                    description += $"Use send message: {UseSendMessage}\n";
                    description += $"Send to children: {SendToChildren}\n";
                    description += $"Near target: {NearTarget}\n";
                    description += $"Send to root: {SendToRoot}\n";
                    description += $"Messages: {messages.Length}\n";
                }
                else if (when == ExecuteTask.OnUpdate)
                {
                    description += "OnUpdate\n";
                    description += $"Use send message: {UseSendMessage}\n";
                    description += $"Send to children: {SendToChildren}\n";
                    description += $"Near target: {NearTarget}\n";
                    description += $"Send to root: {SendToRoot}\n";
                    description += $"Messages: {messages.Length}\n";
                }
                else
                {
                    description += "OnExit\n";
                    description += $"Use send message: {UseSendMessage}\n";
                    description += $"Send to children: {SendToChildren}\n";
                    description += $"Near target: {NearTarget}\n";
                    description += $"Send to root: {SendToRoot}\n";
                    description += $"Messages: {messages.Length}\n";
                }
            }
            else
            {
                description += "Target ";
                if (when == ExecuteTask.OnStart)
                {
                    description += "OnStart\n";
                    description += $"Use send message: {UseSendMessage}\n";
                    description += $"Send to children: {SendToChildren}\n";
                    description += $"Near target: {NearTarget}\n";
                    description += $"Send to root: {SendToRoot}\n";
                    description += $"Messages: {messages.Length}\n";


                }
                else if (when == ExecuteTask.OnUpdate)
                {
                    description += "OnUpdate\n";
                    description += $"Use send message: {UseSendMessage}\n";
                    description += $"Send to children: {SendToChildren}\n";
                    description += $"Near target: {NearTarget}\n";
                    description += $"Send to root: {SendToRoot}\n";
                    description += $"Messages: {messages.Length}\n";


                }
                else
                {
                    description += "OnExit\n";
                    description += $"Use send message: {UseSendMessage}\n";
                    description += $"Send to children: {SendToChildren}\n";
                    description += $"Near target: {NearTarget}\n";
                    description += $"Send to root: {SendToRoot}\n";
                    description += $"Messages: {messages.Length}\n";

                }
            }

            return description;
        }
    }


}


//public bool UseSendMessage = false;
//public bool SendToChildren = false;

//public bool NearTarget = true;

//public bool SendToRoot = true;

//public MesssageItem[] messages;
