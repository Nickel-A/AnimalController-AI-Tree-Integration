using MalbersAnimations.HAP;
using RenownedGames.AITree;
using UnityEngine;
using RenownedGames.Apex;

namespace Malbers.Integration.AITree
{

    [NodeContent("Find Who Mounted", "Tasks/Mount/Find Who Mounted")]
    public class MFindWhoMounted : TaskNode
    {
        private GameObject mount;

        [Title("Blackboard")]
        [SerializeField]
        [HideSelfKey]
        [NonLocal]
        private TransformKey storeResult;

        protected override void OnEntry()
        {
            base.OnEntry();
            storeResult.SetValue(GetOwner().transform.root.GetComponentInChildren<Mount>().Rider.transform);

        }

        protected override State OnUpdate()
        {
                return State.Success;
        }
    }
}

