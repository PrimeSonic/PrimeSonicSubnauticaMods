namespace CustomCraft2SML.Interfaces
{
    public interface IAddedRecipe : IModifiedRecipe
    {
        string Path { get; }
    }
}