namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class CustomFragmentCount : EmPropertyCollection, ICustomFragmentCount
    {
        private readonly EmProperty<string> emTechType;
        private readonly EmProperty<short> emFragmentCount;

        protected static List<EmProperty> FragmentProperties => new List<EmProperty>(3)
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<short>("FragmentsToScan", 1),
        };

        public CustomFragmentCount() : base("CustomFragments", FragmentProperties)
        {
            emTechType = (EmProperty<string>)Properties["ItemID"];
            emFragmentCount = (EmProperty<short>)Properties["FragmentsToScan"];
        }

        public string ItemID { get; }
        public short FragmentsToScan { get; }

        internal override EmProperty Copy() => new CustomFragmentCount();
    }
}
