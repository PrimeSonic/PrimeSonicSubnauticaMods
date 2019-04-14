namespace CustomCraft2SML.Interfaces
{
    internal interface IAliasRecipe : IAddedRecipe
    {
        string DisplayName { get; }
        string Tooltip { get; }
        string FunctionalID { get; }
        TechType SpriteItemID { get; }
    }
}