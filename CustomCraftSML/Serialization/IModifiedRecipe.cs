namespace CustomCraftSML.Serialization
{
    using System.Collections.Generic;
    using SMLHelper.Patchers;

    public interface IModifiedRecipe
    {
        short AmountCrafted { get; }
        TechType ItemID { get; }
        List<TechType> LinkedItems { get; }
        TechDataHelper SmlHelperRecipe();
    }
}