namespace CustomCraft2SML.Interfaces
{
    interface IMovedRecipe : ITechTyped
    {
        string OldPath { get; }
        string NewPath { get; }
        bool IsComplete { get; }
    }
}
