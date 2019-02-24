namespace CustomCraft2SML.Interfaces
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    public interface ICustomFabricator<Tab, Moved, Added, Alias> : IAliasRecipe
        where Tab : EmPropertyCollection, ICraftingTab, new()
        where Moved : EmPropertyCollection, IMovedRecipe, new()        
        where Added : EmPropertyCollection, IAddedRecipe, new()
        where Alias : EmPropertyCollection, IAliasRecipe, new()
    {
        ModelTypes Model { get; }
        int HueOffset { get; } // For future use
        bool AllowedInBase { get; }
        bool AllowedInCyclops { get; }

        EmPropertyCollectionList<Tab> CustomCraftingTabs { get; }
        EmPropertyCollectionList<Moved> MovedRecipes { get; }
        EmPropertyCollectionList<Added> AddedRecipes { get; }
        EmPropertyCollectionList<Alias> AliasRecipes { get; }
    }
}
