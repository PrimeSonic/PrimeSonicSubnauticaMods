namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class ModifiedFoodList : EmPropertyCollectionList<ModifiedFood>
    {
        internal const string ListKey = "ModifiedFoods";
        public ModifiedFoodList() : base(ListKey)
        {
        }
    }
}
