using System;
using UnityEditor;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [CustomEditor(typeof(FactionID))]
    public class FactionIDEditor : Editor
    {
        private FactionID factionID;
        private Color[] relationColors;

        private void OnEnable()
        {
            if (target is FactionID)
            {
                factionID = (FactionID)target;
            }


            // Initialize relationColors array
            relationColors = new Color[Enum.GetValues(typeof(RelationshipType)).Length];
            relationColors[(int)RelationshipType.None] = Color.white;
            relationColors[(int)RelationshipType.Ally] = Color.green;
            relationColors[(int)RelationshipType.Neutral] = Color.yellow;
            relationColors[(int)RelationshipType.Hostile] = Color.red;
        }

        public override void OnInspectorGUI()
        {
            // Editable name and ID fields
            factionID.icon = (Sprite)EditorGUILayout.ObjectField("Icon", factionID.icon, typeof(Sprite), false);
            factionID.DisplayName = EditorGUILayout.TextField("Name", factionID.DisplayName);
            factionID.ID = EditorGUILayout.IntField("ID", factionID.ID);
            factionID.description = EditorGUILayout.TextField("Description", factionID.description);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(factionID.DisplayName, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Display relationships with color
            EditorGUILayout.LabelField("Relationships", EditorStyles.boldLabel);
            if (factionID.Relationships.Count == 0)
            {
                EditorGUILayout.LabelField("No relationships defined.");
            }
            else
            {
                foreach (var relationship in factionID.Relationships)
                {
                    EditorGUILayout.BeginHorizontal();

                    // Display the display name with color
                    Color color = relationColors[(int)relationship.relationship];
                    GUIStyle style = new GUIStyle(EditorStyles.label);
                    style.normal.textColor = color;
                    EditorGUILayout.LabelField($"{relationship.otherFaction.DisplayName}: ", style, GUILayout.Width(80));
                    EditorGUILayout.LabelField("is", GUILayout.Width(20));
                    EditorGUILayout.LabelField(relationship.relationship.ToString(),style);

                    EditorGUILayout.EndHorizontal();
                }

            }

            EditorGUILayout.Space();

            // Add button to open FactionEditorWindow
            if (GUILayout.Button("Open Faction Editor"))
            {
                FactionEditorWindow.ShowWindow();
            }

            // Apply any changes made to the serializedObject
            if (GUI.changed)
            {
                EditorUtility.SetDirty(factionID);
                serializedObject.ApplyModifiedProperties();
            }
        }



    }
}
