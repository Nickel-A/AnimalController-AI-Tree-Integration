using Malbers.Integration.AITree;
using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using MalbersAnimations.Utilities;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("SendACMessage", "Animal Controller/General/Message", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MSendMessageNode : MTaskNode
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
        /// <summary>
        /// Called on behaviour tree is awake.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            base.OnEntry();
            if (when == ExecuteTask.OnStart)
            {
                if (!NearTarget || (NearTarget && AIBrain.AIControl.HasArrived))
                {
                    Execute_Task(AIBrain);
                    messageDone = true;
                }
            }
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
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
                if (!NearTarget || (NearTarget && AIBrain.AIControl.HasArrived))
                {
                    Execute_Task(AIBrain);
                    return State.Success;
                }
            }
            return State.Running;
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
            if (when == ExecuteTask.OnExit)
            {
                if (!NearTarget || (NearTarget && AIBrain.AIControl.HasArrived))
                {
                    Execute_Task(AIBrain);
                    messageDone = true;
                }
            }
            messageDone = false;
        }
        private void Execute_Task(AIBrain AIBrain)
        {
            if (affect == Affected.Self)
            {
                SendMessage(SendToRoot ? AIBrain.Animal.transform : AIBrain.transform);
            }
            else
            {
                if (AIBrain.Target != null)
                {
                    SendMessage(SendToRoot ? AIBrain.Target.FindObjectCore() : AIBrain.Target);
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
