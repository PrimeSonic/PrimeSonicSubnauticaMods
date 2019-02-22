namespace CustomCraft2SML.Interfaces
{
    public interface ICraftingTab
    {
        string TabID { get; }
        string DisplayName { get; }
        TechType SpriteItemID { get; }
        string ParentTabPath { get; }
    }
}
