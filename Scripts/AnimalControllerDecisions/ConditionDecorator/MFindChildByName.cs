using RenownedGames.AITree;
using UnityEngine;

namespace Malbers.Integration.AITree
{
    [NodeContent("Find Child By Name", "Animal Controller/MConditionDecorator/Find Child By Name", IconPath = "Icons/AnimalAI_Icon.png")]
    public class MFindChildByName : MConditionDecorator
    {
        [Header("Node")]
        public string childNameToFind = "";

        /// <summary>
        /// Calculates the result of the condition.
        /// </summary>
        protected override bool CalculateResult()
        {
            return FindChildByName(AIBrain.Animal.transform, childNameToFind);
        }

        private bool FindChildByName(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return true; // Child found
                }
                else
                {
                    if (FindChildByName(child, name))
                    {
                        return true; // Child found in descendants
                    }
                }
            }
            return false; // Child not found in this subtree
        }
    }
}
