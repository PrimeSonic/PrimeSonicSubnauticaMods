namespace CustomCraft2SML.Serialization.Lists
{
    using CustomCraft2SML.Serialization.Entries;
    using EasyMarkup;

    internal class CustomFabricatorList : EmPropertyCollectionList<CustomFabricator>
    {
        internal const string ListKey = "CustomFabricators";

        public CustomFabricatorList() : base(ListKey)
        {
        }
    }
}
