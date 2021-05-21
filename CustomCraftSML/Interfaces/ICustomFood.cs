namespace CustomCraft2SML.Interfaces
{
    internal interface ICustomFood : IAliasRecipe
    {
        FoodModel FoodType { get; }

        short FoodValue { get; }
        short WaterValue { get; }
#if BELOWZERO
        short HeatValue { get; }
#endif
        float DecayRateMod { get; }
        bool UseDrinkSound { get; }
    }
}
