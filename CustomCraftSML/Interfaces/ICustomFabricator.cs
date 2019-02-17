namespace CustomCraft2SML.Interfaces
{
    using System.Collections.Generic;
    using CustomCraft2SML.Serialization.Entries;

    internal interface ICustomFabricator<Tab, Alias, Added, Moved> : IAliasRecipe
        where Tab : ICraftingTab
        where Alias : IAliasRecipe
        where Added : IAddedRecipe
        where Moved : IMovedRecipe
    {
        ModelTypes Model { get; }
        int HueOffset { get; }
        bool AllowedInBase { get; }
        bool AllowedInCyclops { get; }

        IList<Tab> CustomCraftingTabs { get; }
        IList<Alias> AliasRecipes { get; }
        IList<Added> AddedRecipes { get; }
        IList<Moved> MovedRecipes { get; }
    }
}
