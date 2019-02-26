namespace CustomCraft2SML.Interfaces
{
    public interface IMovedRecipe : ITechTyped
    {
        string OldPath { get; }
        string NewPath { get; }
        bool Hidden { get; }
        bool Copied { get; }
    }
}
