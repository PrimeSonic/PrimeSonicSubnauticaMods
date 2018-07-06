namespace CustomCraft2SML.Serialization
{
    public interface IAddedRecipe : IModifiedRecipe
    {
        string Path { get; }
    }
}