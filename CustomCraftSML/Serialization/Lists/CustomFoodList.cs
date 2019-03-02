namespace CustomCraft2SML.Serialization.Lists
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Entries;

    internal class CustomFoodList : EmPropertyCollectionList<CustomFood>
    {
        internal const string ListKey = "CustomFood";

        public CustomFoodList() : base(ListKey)
        {
        }
    }
}
