namespace CustomCraft2SML.Interfaces
{
    internal interface IAddedRecipe : IModifiedRecipe
    {
        string Path { get; }
        TechGroup PdaGroup { get; }
        TechCategory PdaCategory { get; }
    }
}