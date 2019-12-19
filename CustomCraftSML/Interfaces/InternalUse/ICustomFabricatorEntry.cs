namespace CustomCraft2SML.Interfaces.InternalUse
{
    using CustomCraft2SML.Serialization;
    using CustomCraft2SML.Serialization.Entries;

    internal interface ICustomFabricatorEntry : ICustomCraft
    {
        CustomFabricator ParentFabricator { get; set; }
        CraftTree.Type TreeTypeID { get; }
        bool IsAtRoot { get; }

        CraftTreePath GetCraftTreePath();
    }
}
