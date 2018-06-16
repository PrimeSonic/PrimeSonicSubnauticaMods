namespace CustomCraftSML.Serialization
{
    public class Ingredient
    {
        public Ingredient(TechType itemID, short required = 1)
        {
            ItemID = itemID;
            Required = required;
        }

        public TechType ItemID { get; }
        public short Required { get; }
    }
}
