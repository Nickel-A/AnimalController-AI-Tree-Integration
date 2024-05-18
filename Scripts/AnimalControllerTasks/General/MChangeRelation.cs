using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Set Faction", "Animal Controller/General/Set Faction", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MChangeRelation : MTaskNode
    {
        public RelationshipType relationshipTypeToSet;
        public FactionID targetFactionID; // FactionID to set the relationship with

        /// <summary>
        /// Called when behaviour tree enter in node.
        /// </summary>
        protected override void OnEntry()
        {
            base.OnEntry();
        }

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            ChangeFaction();
            return State.Success;
        }

        private void ChangeFaction()
        {
            if (targetFactionID != null)
            {
                // Check if the MFaction component exists on the AIBrain
                MFaction mFaction = AIBrain.GetComponent<MFaction>();

                // Check if the MFaction component exists
                if (mFaction != null)
                {
                    // Iterate over each faction in the MFaction component
                    foreach (var factionID in mFaction.Factions)
                    {
                        // Set the relationship between the factionID and the target FactionID
                        factionID.SetRelationship(targetFactionID, relationshipTypeToSet);
                    }
                }
                else
                {
                    Debug.LogError("MFaction component not found on AIBrain GameObject.");
                }
            }
            else
            {
                Debug.LogError("Target FactionID is not assigned.");
            }
        }

        /// <summary>
        /// Called when behaviour tree exit from node.
        /// </summary>
        protected override void OnExit()
        {
            base.OnExit();
        }
    }
}
