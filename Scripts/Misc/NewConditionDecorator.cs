using RenownedGames.AITree;

[NodeContent("NewConditionDecorator", "Custom/NewConditionDecorator")]
public class NewConditionDecorator : ConditionDecorator
{
    /// <summary>
    /// Calculates the result of the condition.
    /// </summary>
    protected override bool CalculateResult()
    {
        return true;
    }
}