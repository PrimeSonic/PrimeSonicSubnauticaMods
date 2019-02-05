namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class CustomFragmentCount : EmPropertyCollection, ICustomFragmentCount
    {
        private readonly EmProperty<string> emTechType;
        private readonly EmProperty<int> emFragmentCount;

        protected static List<EmProperty> FragmentProperties => new List<EmProperty>(3)
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<int>("FragmentsToScan", 1),
        };

        public CustomFragmentCount() : base("CustomFragments", FragmentProperties)
        {
            emTechType = (EmProperty<string>)Properties["ItemID"];
            emFragmentCount = (EmProperty<int>)Properties["FragmentsToScan"];
        }

        internal CustomFragmentCount(string itemID, int fragmentsToScan) : this()
        {
            this.ItemID = itemID;
            this.FragmentsToScan = fragmentsToScan;
        }

        public string ItemID { get; }
        public int FragmentsToScan { get; }

        internal override EmProperty Copy() => new CustomFragmentCount();
    }
}
