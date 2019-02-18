namespace CustomCraft2SML.Interfaces
{
    public interface ITechTyped
    {
        string ItemID { get; }
        string Key { get; }
        TechType TechType { get; set; }
    }
}
