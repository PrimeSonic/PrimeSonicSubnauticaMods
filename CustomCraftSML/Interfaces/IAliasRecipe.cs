namespace CustomCraft2SML.Interfaces
{
    public interface IAliasRecipe : IAddedRecipe
    {
        string ItemName { get; }
        string DisplayName { get; }
        string Tooltip { get; }
    }
}