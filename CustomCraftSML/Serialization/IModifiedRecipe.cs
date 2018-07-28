namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;

    public interface IModifiedRecipe
    {
        TechType ItemID { get; }
        short? AmountCrafted { get; }
        bool ForceUnlockAtStart { get; }

        int? IngredientsCount { get; }
        IEnumerable<EmIngredient> Ingredients { get; }
        EmIngredient GetIngredient(int index);

        int? LinkedItemsCount { get; }
        IEnumerable<TechType> LinkedItems { get; }
        TechType GetLinkedItem(int index);

        int? UnlocksCount { get; }
        IEnumerable<TechType> Unlocks { get; }
        TechType GetUnlock(int index);
    }
}