﻿namespace CustomCraft2SML.Interfaces
{
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Entries;

    internal interface ICustomFabricatorEntry : ICustomCraft
    {
        CustomFabricator ParentFabricator { get; set; }
        CraftTree.Type TreeTypeID { get; }
        bool IsAtRoot { get; }
        CraftingPath CraftingNodePath { get; }
    }
}
