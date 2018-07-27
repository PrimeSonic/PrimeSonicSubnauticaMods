namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;    
    using SMLHelper.V2.Crafting;

    public interface IModifiedRecipe
    {
        TechType ItemID { get; }
        int IngredientCount { get; }
        int LinkedItemCount { get; }
        TechData SmlHelperRecipe();
        bool ForceUnlockAtStart { get; }
        IList<TechType> Unlocks { get; }
    }
}