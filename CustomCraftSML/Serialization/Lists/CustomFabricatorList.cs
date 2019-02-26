namespace CustomCraft2SML.Serialization.Lists
{
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomFabricatorList : EmPropertyCollectionList<CustomFabricator>
    {
        internal const string ListKey = "CustomFabricators";

        public CustomFabricatorList() : base(ListKey)
        {
        }
    }
}
