namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class CustomFoodList : EmPropertyCollectionList<CustomFood>
    {
        internal const string ListKey = "CustomFoods";

        public CustomFoodList() : base(ListKey)
        {
        }
    }
}
