namespace CustomCraftSML.Serialization
{
    using SMLHelper.V2.Crafting;

    public interface IModifiedRecipe
    {
        TechType ItemID { get; }
        TechData SmlHelperRecipe();
    }
}