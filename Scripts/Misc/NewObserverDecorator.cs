using RenownedGames.AITree;
using System;

[NodeContent("NewObserverDecorator", "Custom/NewObserverDecorator")]
public class NewObserverDecorator : ObserverDecorator
{
    public override event Action OnValueChange;

    /// <summary>
    /// Calculates the result of the condition.
    /// </summary>
    public override bool CalculateResult()
    {
        return true;
    }
}