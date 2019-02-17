namespace CustomCraft2SML.Interfaces
{
    using CustomCraft2SML.Serialization.Entries;

    internal interface ICustomFabricatorEntry
    {
        CustomFabricator ParentFabricator { get; set; }
    }
}
