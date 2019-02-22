namespace CustomCraft2SML.Interfaces
{
    public interface IAddedRecipe : IModifiedRecipe
    {
        string Path { get; }
        TechGroup PdaGroup { get; }
        TechCategory PdaCategory { get; }
    }
}