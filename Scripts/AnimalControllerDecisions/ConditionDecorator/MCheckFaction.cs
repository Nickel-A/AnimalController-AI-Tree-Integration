using MalbersAnimations;
using MalbersAnimations.Controller.AI;
using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Check Faction", "Animal Controller/MConditionDecorator/Check Faction", IconPath = "Icons/AIDecision_Icon.png")]
    public class MCheckFaction : MConditionDecorator
    {
        public RelationshipType relationshipTypeToCheck;

        protected override bool CalculateResult()
        {
            return CheckRelationship();
        }

        private bool CheckRelationship()
        {
            if (AIBrain != null && AIBrain.Target != null)
            {
                MFaction targetFaction = AIBrain.Target.GetComponent<MFaction>();
                MFaction aiBrainFaction = AIBrain.GetComponent<MFaction>();

                if (targetFaction != null && aiBrainFaction != null)
                {
                    FactionID targetFactionID = targetFaction.Factions[0];
                    FactionID aiBrainFactionID = aiBrainFaction.Factions[0];

                    // Log the display names of the factions
                    Debug.Log($"AI Brain Faction: {aiBrainFactionID.name}, Target Faction: {targetFactionID.name}");

                    RelationshipType relationship = RelationshipType.None;

                    // Check if the AI brain's faction has a relationship with the target faction
                    if (aiBrainFactionID.Relationships != null)
                    {
                        var existingRelationship = aiBrainFactionID.Relationships.Find(r => r.otherFaction == targetFactionID);
                        if (existingRelationship != null)
                        {
                            relationship = existingRelationship.relationship;
                        }
                    }

                    // Debug log to print the relationship for debugging purposes
                    Debug.Log($"Relationship between AI Brain Faction and Target Faction: {relationship}");

                    return relationship == relationshipTypeToCheck;
                }
            }

            return false;
        }

    }
}
