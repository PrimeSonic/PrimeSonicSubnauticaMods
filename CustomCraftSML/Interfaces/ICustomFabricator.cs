namespace CustomCraft2SML.Interfaces
{
    using CustomCraft2SML.Serialization.Entries;

    internal interface ICustomFabricator : IAliasRecipe
    {
        ModelTypes Model { get; }
        int HueOffset { get; }
    }
}
