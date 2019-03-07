namespace CustomCraft2SML.Interfaces
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;
    using UnityEngine;

    public interface ICustomFabricator<Tab, Moved, Added, Alias, Food> : IAliasRecipe
        where Tab : EmPropertyCollection, ICraftingTab, new()
        where Moved : EmPropertyCollection, IMovedRecipe, new()        
        where Added : EmPropertyCollection, IAddedRecipe, new()
        where Alias : EmPropertyCollection, IAliasRecipe, new()
        where Food : EmPropertyCollection, ICustomFood, new()
    {
        ModelTypes Model { get; }
        Color ColorTint { get; }
        bool AllowedInBase { get; }
        bool AllowedInCyclops { get; }

        EmPropertyCollectionList<Tab> CustomCraftingTabs { get; }
        EmPropertyCollectionList<Moved> MovedRecipes { get; }
        EmPropertyCollectionList<Added> AddedRecipes { get; }
        EmPropertyCollectionList<Alias> AliasRecipes { get; }
        EmPropertyCollectionList<Food> CustomFoods { get; }
    }
}
