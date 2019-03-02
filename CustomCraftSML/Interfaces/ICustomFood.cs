namespace CustomCraft2SML.Interfaces
{
    interface ICustomFood : IAliasRecipe
    {
        short FoodValue { get; }
        short WaterValue { get; }
        bool? Decomposes { get; }
        short? DecayRate { get; }
    }
}
