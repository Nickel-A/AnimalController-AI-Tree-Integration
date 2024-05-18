using System.Collections.Generic;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [DisallowMultipleComponent]
    public class MFaction : MonoBehaviour
    {
        [SerializeField] private List<FactionID> factions;

        public List<FactionID> Factions => factions;

        public void AddFaction(FactionID faction)
        {
            if (factions == null)
            {
                factions = new List<FactionID>();
            }
            factions.Add(faction);
        }

        public void RemoveFaction(FactionID faction)
        {
            factions.Remove(faction);
        }

        public bool HasFaction(FactionID faction)
        {
            return factions.Contains(faction);
        }

        public void SetFactions(List<FactionID> newFactions)
        {
            factions = newFactions;
        }
    }
}
