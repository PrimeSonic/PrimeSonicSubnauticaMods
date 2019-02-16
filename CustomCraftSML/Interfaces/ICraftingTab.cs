namespace CustomCraft2SML.Interfaces
{
    interface ICraftingTab : ICustomCraft
    {
        string TabID { get; }
        string DisplayName { get; }
        TechType SpriteItemID { get; }
        string ParentTabPath { get; }
    }
}
