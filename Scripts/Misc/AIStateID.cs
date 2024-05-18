namespace MalbersAnimations
{
    [System.Serializable]
    [UnityEngine.CreateAssetMenu(menuName = "Malbers Animations/ID/AIState", fileName = "New AIState ID", order = -1000)]
    public class AIStateID : IDs
    {
        #region CalculateID
#if UNITY_EDITOR
        private void Reset() => GetID();

        [UnityEngine.ContextMenu("Get ID")]
        private void GetID() => FindID<AIStateID>();
#endif
        #endregion
    }
}