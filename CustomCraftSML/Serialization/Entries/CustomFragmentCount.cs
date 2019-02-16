namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Components;

    internal class CustomFragmentCount : EmTechTyped, ICustomFragmentCount
    {
        private readonly EmProperty<int> emFragmentCount;

        protected static List<EmProperty> FragmentProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<int>("FragmentsToScan", 1),
        };

        public CustomFragmentCount() : this("CustomFragments", FragmentProperties)
        {
        }

        protected CustomFragmentCount(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emFragmentCount = (EmProperty<int>)Properties["FragmentsToScan"];
        }

        internal CustomFragmentCount(string itemID, int fragmentsToScan) : this()
        {
            this.ItemID = itemID;
            this.FragmentsToScan = fragmentsToScan;
        }

        public int FragmentsToScan
        {
            get => emFragmentCount.Value;
            set => emFragmentCount.Value = value;
        }

        internal override EmProperty Copy() => new CustomFragmentCount(this.Key, this.CopyDefinitions);
    }
}
