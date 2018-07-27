namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;    
    using SMLHelper.V2.Crafting;

    public interface IModifiedRecipe
    {
        TechType ItemID { get; }
        TechData SmlHelperRecipe();
        bool ForceUnlockAtStart { get; }
        IList<TechType> Unlocks { get; }
    }
}