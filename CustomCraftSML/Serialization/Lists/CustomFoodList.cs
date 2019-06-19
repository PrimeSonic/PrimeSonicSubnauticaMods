namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomFoodList : EmPropertyCollectionList<CustomFood>
    {
        internal const string ListKey = "CustomFoods";

        public CustomFoodList() : base(ListKey)
        {
        }
    }
}
