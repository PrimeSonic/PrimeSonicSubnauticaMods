namespace CustomCraftSML.Serialization
{
    public interface IAddedRecipe : IModifiedRecipe
    {
        string Path { get; }
    }
}