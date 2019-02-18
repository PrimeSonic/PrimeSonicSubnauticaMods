namespace CustomCraft2SML.Interfaces
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal interface ICustomFabricator<Tab, Alias, Added, Moved> : IAliasRecipe
        where Tab : CustomCraftingTab, new()
        where Alias : AliasRecipe, new()
        where Added : AddedRecipe, new()
        where Moved : MovedRecipe, new()
    {
        ModelTypes Model { get; }
        int HueOffset { get; }
        bool AllowedInBase { get; }
        bool AllowedInCyclops { get; }

        EmPropertyCollectionList<Tab> CustomCraftingTabs { get; }
        EmPropertyCollectionList<Alias> AliasRecipes { get; }
        EmPropertyCollectionList<Added> AddedRecipes { get; }
        EmPropertyCollectionList<Moved> MovedRecipes { get; }
    }
}
