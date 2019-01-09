namespace CustomCraft2SML.Interfaces
{
    using System.Collections.Generic;
    using CustomCraft2SML.Serialization;

    public interface IModifiedRecipe : ITechTyped
    {
        short? AmountCrafted { get; }
        bool ForceUnlockAtStart { get; }

        int? IngredientsCount { get; }
        IEnumerable<EmIngredient> Ingredients { get; }
        EmIngredient GetIngredient(int index);

        int? LinkedItemsCount { get; }
        IEnumerable<string> LinkedItems { get; }
        string GetLinkedItem(int index);

        int? UnlocksCount { get; }
        IEnumerable<string> Unlocks { get; }
        string GetUnlock(int index);
    }
}