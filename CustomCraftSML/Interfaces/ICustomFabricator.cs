namespace CustomCraft2SML.Interfaces
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal interface ICustomFabricator<Tab, Moved, Added, Alias> : IAliasRecipe
        where Tab : CustomCraftingTab, new()
        where Moved : MovedRecipe, new()        
        where Added : AddedRecipe, new()
        where Alias : AliasRecipe, new()
    {
        ModelTypes Model { get; }
        int HueOffset { get; }
        bool AllowedInBase { get; }
        bool AllowedInCyclops { get; }

        EmPropertyCollectionList<Tab> CustomCraftingTabs { get; }
        EmPropertyCollectionList<Moved> MovedRecipes { get; }
        EmPropertyCollectionList<Added> AddedRecipes { get; }
        EmPropertyCollectionList<Alias> AliasRecipes { get; }
    }
}
