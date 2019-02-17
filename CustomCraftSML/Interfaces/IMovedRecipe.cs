namespace CustomCraft2SML.Interfaces
{
    interface IMovedRecipe : ITechTyped, ICustomCraft
    {
        string OldPath { get; }
        string NewPath { get; }
        bool Hidden { get; }
        bool CopyToNewPath { get; }
    }
}
