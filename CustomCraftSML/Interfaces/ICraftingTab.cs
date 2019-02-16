namespace CustomCraft2SML.Interfaces
{
    interface ICraftingTab : ICustomCraft
    {
        string TabID { get; }
        string DisplayName { get; }
        CraftTree.Type FabricatorType { get; }
        TechType SpriteItemID { get; }
        string ParentTabPath { get; }
        string[] StepsToTab { get; }
    }
}
