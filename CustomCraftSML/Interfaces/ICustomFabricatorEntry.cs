namespace CustomCraft2SML.Interfaces
{
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Entries;
    using SMLHelper.V2.Crafting;

    internal interface ICustomFabricatorEntry : ICustomCraft
    {
        CustomFabricator ParentFabricator { get; }
        CraftTree.Type TreeTypeID { get; }
        ModCraftTreeRoot RootNode { get; }
        bool IsAtRoot { get; }
        CraftingPath CraftingNodePath { get; }
    }
}
