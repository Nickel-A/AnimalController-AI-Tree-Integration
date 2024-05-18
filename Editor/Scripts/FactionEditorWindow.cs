using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    public class FactionEditorWindow : EditorWindow
    {
        private List<FactionID> factions; // List to hold all FactionID assets
        private Vector2 scrollPosition; // Position for scrolling
        private Dictionary<FactionID, bool> foldouts = new Dictionary<FactionID, bool>(); // Dictionary to track foldout states for factions
        private RelationshipType[,] relationships; // 2D array to hold relationships between factions

        // Define colors for each relationship type
        private readonly Color[] relationColors = { Color.white, Color.green, Color.yellow, Color.red };

        // MenuItem attribute to create the window from the Unity Editor
        [MenuItem("Window/Faction Editor")]
        public static void ShowWindow()
        {
            GetWindow<FactionEditorWindow>("Faction Editor"); // Show the window with title "Faction Editor"
        }

        private void OnEnable()
        {
            LoadFactions();
        }

        private void LoadFactions()
        {
            // Load all FactionID assets from the project
            factions = new List<FactionID>(Resources.FindObjectsOfTypeAll<FactionID>());
            relationships = new RelationshipType[factions.Count, factions.Count]; // Initialize the relationship matrix

            // Initialize the relationship matrix based on existing relationships between factions
            for (int i = 0; i < factions.Count; i++)
            {
                for (int j = 0; j < factions.Count; j++)
                {
                    if (i != j)
                    {
                        var relationship = factions[i].Relationships.Find(r => r.otherFaction == factions[j]);
                        relationships[i, j] = relationship != null ? relationship.relationship : RelationshipType.None;
                    }
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(5);
            GUILayout.Label("Faction Editor", EditorStyles.boldLabel); // Display the title label
            EditorGUILayout.Space(5);

            // Button to refresh faction data
            if (GUILayout.Button("Refresh"))
            {
                LoadFactions();
            }

            // Button to log faction relationships
            if (GUILayout.Button("Log Faction Relationships"))
            {
                LogFactionRelationships();
            }

            // Begin scroll view
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Iterate over each faction
            for (int i = 0; i < factions.Count; i++)
            {
                var faction = factions[i];

                // Check if the foldout dictionary contains the faction
                if (!foldouts.ContainsKey(faction))
                {
                    foldouts.Add(faction, false); // Add the faction to the dictionary with default foldout state (closed)
                }

                // Display the foldout for the faction
                foldouts[faction] = EditorGUILayout.Foldout(foldouts[faction], faction.DisplayName, true);

                // If the foldout is open, display the faction's details
                if (foldouts[faction])
                {
                    EditorGUI.indentLevel++; // Increase the indent level to visually separate faction details
                    EditorGUILayout.Space(5);

                    // Display relationships with all other factions
                    for (int j = 0; j < factions.Count; j++)
                    {
                        if (i != j)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"{faction.DisplayName} is", GUILayout.Width(120));
                            RelationshipType currentRelation = relationships[i, j];

                            // Get the color based on the relationship type
                            Color color = GetRelationshipColor(currentRelation);
                            GUI.backgroundColor = color;

                            // Display the dropdown with the relationship type
                            RelationshipType newRelation = (RelationshipType)EditorGUILayout.EnumPopup(currentRelation, GUILayout.Width(100));

                            // Reset the background color
                            GUI.backgroundColor = Color.white;

                            if (newRelation != currentRelation)
                            {
                                // Update the relationship bidirectionally
                                UpdateRelationship(faction, factions[j], newRelation);
                            }

                            Color factionColor = GetRelationshipColor(currentRelation);
                            GUIStyle factionStyle = new GUIStyle(GUI.skin.label)
                            {
                                normal = { textColor = factionColor } // Set the text color for faction display name
                            };

                            GUILayout.Label(" with ", GUILayout.Width(32));
                            GUILayout.Label(factions[j].DisplayName, factionStyle, GUILayout.Width(150));

                            GUILayout.EndHorizontal();
                        }
                    }

                    EditorGUI.indentLevel--; // Decrease the indent level
                }

                EditorGUILayout.Space(5);
            }

            GUILayout.EndScrollView(); // End scroll view
        }

        // Method to get color based on the relationship type
        private Color GetRelationshipColor(RelationshipType relationshipType)
        {
            return relationColors[(int)relationshipType];
        }

        // Method to update the relationship between two factions
        private void UpdateRelationship(FactionID faction, FactionID otherFaction, RelationshipType newRelation)
        {
            // Update the relationship matrix for both factions
            int factionIndex = factions.IndexOf(faction);
            int otherFactionIndex = factions.IndexOf(otherFaction);

            // Ensure correct indexing for both factions
            if (factionIndex != -1 && otherFactionIndex != -1)
            {
                relationships[factionIndex, otherFactionIndex] = newRelation; // Update the relationship in the matrix
                relationships[otherFactionIndex, factionIndex] = newRelation; // Update the reverse relationship

                // Update the relationship in the FactionID assets
                faction.SetRelationship(otherFaction, newRelation);
                otherFaction.SetRelationship(faction, newRelation);

                // Mark both faction assets as dirty to save changes
                EditorUtility.SetDirty(faction);
                EditorUtility.SetDirty(otherFaction);
            }
            else
            {
                Debug.LogError("Error updating relationship: Faction index not found.");
            }
        }

        private void LogFactionRelationships()
        {
            // Iterate over each faction
            for (int i = 0; i < factions.Count; i++)
            {
                var faction = factions[i];

                Debug.Log($"Faction: {faction.DisplayName}");

                // Iterate over each other faction
                for (int j = 0; j < factions.Count; j++)
                {
                    if (i != j)
                    {
                        var otherFaction = factions[j];
                        var relationship = relationships[i, j];

                        // Get color based on the relationship type
                        Color color = GetRelationshipColor(relationship);

                        // Convert color to hexadecimal string
                        string hexColor = ColorUtility.ToHtmlStringRGBA(color);

                        // Create rich text string with colored relationship
                        string logMessage = $"<color=#{hexColor}>- {faction.DisplayName} has {relationship} relationship with {otherFaction.DisplayName}</color>";

                        Debug.Log(logMessage);
                    }
                }
            }
        }
    }
}
