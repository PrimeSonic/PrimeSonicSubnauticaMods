namespace CustomCraft2SML.Interfaces
{
    using CustomCraft2SML.Serialization.Entries;

    internal interface ICustomFood : IAliasRecipe
    {
        FoodModel FoodType { get; }

        short FoodValue { get; }
        short WaterValue { get; }
        float DecayRateMod { get; }
        bool AllowOverfill { get; }
    }
}
