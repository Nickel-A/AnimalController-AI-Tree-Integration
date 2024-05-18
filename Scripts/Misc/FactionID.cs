using MalbersAnimations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    public enum RelationshipType { None, Ally, Neutral, Hostile }
    [CreateAssetMenu(menuName = "Malbers Animations/ID/FactionID", fileName = "New Faction ID", order = -1000)]
    public class FactionID : IDs
    {
        #region CalculateID
#if UNITY_EDITOR
        private void Reset() => GetID();

        [ContextMenu("Get ID")]
        private void GetID() => FindID<FactionID>();
#endif
        #endregion

        public Sprite icon;
        public string description = "Describe your faction here.";

        [SerializeField]
        private List<FactionRelationship> relationships = new List<FactionRelationship>();

        public List<FactionRelationship> Relationships => relationships;

        // Ensure that the relationships list is marked as dirty when modified
        public void MarkAsDirty()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        // Add a new relationship
        public void AddRelationship(FactionID otherFaction, RelationshipType relationship)
        {
            var newRelationship = new FactionRelationship { otherFaction = otherFaction, relationship = relationship };
            relationships.Add(newRelationship);
            MarkAsDirty(); // Mark the asset as dirty when the relationship is added
        }

        // Remove a relationship
        public void RemoveRelationship(FactionID otherFaction)
        {
            var existingRelationship = relationships.Find(r => r.otherFaction == otherFaction);
            if (existingRelationship != null)
            {
                relationships.Remove(existingRelationship);
                MarkAsDirty(); // Mark the asset as dirty when the relationship is removed
            }
        }

        // Update an existing relationship
        public void UpdateRelationship(FactionID otherFaction, RelationshipType newRelationship)
        {
            var existingRelationship = relationships.Find(r => r.otherFaction == otherFaction);
            if (existingRelationship != null)
            {
                existingRelationship.relationship = newRelationship;
                MarkAsDirty(); // Mark the asset as dirty when the relationship is updated
            }
            else
            {
                AddRelationship(otherFaction, newRelationship);
            }
        }
        public void SetRelationship(FactionID otherFaction, RelationshipType relationshipType)
        {
            // Set the relationship in the current faction
            var existingRelationship = relationships.Find(r => r.otherFaction == otherFaction);
            if (existingRelationship != null)
            {
                existingRelationship.relationship = relationshipType;
            }
            else
            {
                var newRelationship = new FactionRelationship { otherFaction = otherFaction, relationship = relationshipType };
                relationships.Add(newRelationship);
            }

            MarkAsDirty(); // Mark the asset as dirty when the relationship is added or updated

            // Ensure the reciprocal relationship is set in the other faction
            var otherExistingRelationship = otherFaction.relationships.Find(r => r.otherFaction == this);
            if (otherExistingRelationship != null)
            {
                otherExistingRelationship.relationship = relationshipType;
            }
            else
            {
                var otherNewRelationship = new FactionRelationship { otherFaction = this, relationship = relationshipType };
                otherFaction.relationships.Add(otherNewRelationship);
            }

            otherFaction.MarkAsDirty(); // Mark the other faction's asset as dirty when the reciprocal relationship is added or updated
        }

    }

    [Serializable]
    public class FactionRelationship
    {
        public FactionID otherFaction;
        public RelationshipType relationship;
    }
}
