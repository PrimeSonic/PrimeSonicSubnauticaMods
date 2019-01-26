namespace CustomCraft2SML.Interfaces
{
    public interface IAliasRecipe : IAddedRecipe
    {
        string DisplayName { get; }
        string Tooltip { get; }
    }
}