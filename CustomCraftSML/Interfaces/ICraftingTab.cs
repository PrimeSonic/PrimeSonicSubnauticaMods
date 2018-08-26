namespace CustomCraft2SML.Interfaces
{
    interface ICraftingTab
    {
        string TabID { get; }
        string DisplayName { get; }
        CraftTree.Type FabricatorType { get; }
        TechType ItemForSprite { get; }
        string ParentTabID { get; }
    }
}
