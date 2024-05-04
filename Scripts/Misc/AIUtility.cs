using MalbersAnimations.Scriptables;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    public static class AIUtility
    {
        /// <summary>
        /// Determines if a target is within the field of view of an AI brain.
        /// </summary>
        /// <param name="brain">The AIBrain component.</param>
        /// <param name="center">The position of the target.</param>
        /// <param name="lookAngle">The angle of the AI's field of view.</param>
        /// <param name="lookRange">The range of the AI's field of view.</param>
        /// <param name="lookMultiplier">A multiplier for adjusting the look range.</param>
        /// <param name="obstacleLayer">The layer mask for obstacles.</param>
        /// <param name="distance">Output parameter for the distance to the target.</param>
        /// <returns>True if the target is within the field of view, false otherwise.</returns>
        public static bool IsInFieldOfView(AIBrain brain, Vector3 center, float lookAngle, float lookRange, float lookMultiplier, LayerReference obstacleLayer, out float distance)
        {
            if (brain != null)
            {
                var directionToTarget = center - brain.Eyes.position; // Put the Sight a bit higher

                // Important, otherwise it will find the ground for Objects to close to it. Also Apply the Scale
                distance = Vector3.Distance(center, brain.Eyes.position) * lookMultiplier;

                if (lookAngle == 0 || lookRange <= 0)
                {
                    return true; // Means the Field of view can be ignored
                }

                if (distance < lookRange * brain.Animal.ScaleFactor) // Check if we are inside the Look Radius
                {
                    Vector3 eyesForward = Vector3.ProjectOnPlane(-brain.Eyes.up, brain.Animal.UpVector);

                    var angle = Vector3.Angle(directionToTarget, eyesForward);

                    if (angle < (lookAngle / 2))
                    {
                        // Need a RayCast to see if there's no obstacle in front of the Animal OBSTACLE LAYER
                        if (Physics.Raycast(brain.Eyes.position, directionToTarget, out RaycastHit _, distance, obstacleLayer, QueryTriggerInteraction.Ignore))
                        {
                            return false; // Meaning there's something between the Eyes of the Animal and the Target
                        }
                        else
                        {
                            return true;
                        }
                    }
                    return false;
                }
                // Debug.Log($"False (NOT IN Distance {distance} > RANGE) {lookRange.Value}");
                return false;
            }
            distance = 0;
            return false;
        }

        /// <summary>
        /// Draws the field of view gizmos for debugging purposes.
        /// </summary>
        /// <param name="brain">The AIBrain component.</param>
        /// <param name="debugColor">The color to use for drawing.</param>
        /// <param name="lookAngle">The angle of the field of view.</param>
        /// <param name="lookRange">The range of the field of view.</param>
        public static void DrawFieldOfViewGizmos(AIBrain brain, Color debugColor, float lookAngle, FloatReference lookRange)
        {
#if UNITY_EDITOR
            var eyes = brain.Eyes;

            if (eyes != null)
            {
                var scale = brain.Animal ? brain.Animal.ScaleFactor : brain.transform.root.localScale.y;

                Color c = debugColor;
                c.a = 1f;

                Vector3 eyesForward = Vector3.ProjectOnPlane(-brain.Eyes.up, Vector3.up);

                Vector3 rotatedForward = Quaternion.Euler(0, -lookAngle * 0.5f, 0) * eyesForward;
                UnityEditor.Handles.color = c;
                UnityEditor.Handles.DrawWireArc(eyes.position, Vector3.up, rotatedForward, lookAngle, lookRange * scale);
                UnityEditor.Handles.color = debugColor;
                UnityEditor.Handles.DrawSolidArc(eyes.position, Vector3.up, rotatedForward, lookAngle, lookRange * scale);
            }
#endif
        }
    }

    [System.Serializable]
    public class GameObjectCollection
    {
        public GameObject[] gameobjects;
        public Component[] Components;

        /// <summary>
        /// Adds components to the GameObject collection.
        /// </summary>
        /// <param name="components">The components to add to the collection.</param>
        public void AddComponents(Component[] components)
        {
            Components = components;
            Debug.Log($"Added {components.Length} components to the collection.");
        }
    }

}


