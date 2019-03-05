namespace CustomCraft2SML.Interfaces
{
    using System.Collections.Generic;
    using CustomCraft2SML.Serialization.Components;

    public interface ICustomFood : ITechTyped
    {
        short AmountCrafted { get; }
        bool ForceUnlockAtStart { get; }

        IList<EmIngredient> Ingredients { get; }
        IList<string> LinkedItemIDs { get; }
        IList<string> Unlocks { get; }
        IList<string> UnlockedBy { get; }

        string DisplayName { get; }
        string Tooltip { get; }

        string Path { get; }
        TechCategory PdaCategory { get; }

        //FOOD SPECIFIC VALUES

        short FoodValue { get; }
        short WaterValue { get; }
        float DecayRate { get; }
        bool Overfill { get; }
    }
}
