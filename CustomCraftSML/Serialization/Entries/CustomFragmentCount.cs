namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization.Components;
    using SMLHelper.V2.Handlers;

    internal class CustomFragmentCount : EmTechTyped, ICustomFragmentCount, ICustomCraft
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

        public OriginFile Origin { get; set; }

        internal CustomFragmentCount(string itemID, int fragmentsToScan) : this()
        {
            this.ItemID = itemID;
            this.FragmentsToScan = fragmentsToScan;
        }

        public string ID => this.ItemID;

        public int FragmentsToScan
        {
            get => emFragmentCount.Value;
            set => emFragmentCount.Value = value;
        }

        internal override EmProperty Copy() => new CustomFragmentCount(this.Key, this.CopyDefinitions);

        public bool SendToSMLHelper()
        {
            try
            {
                int fragCount = this.FragmentsToScan;
                if (fragCount < PDAScanner.EntryData.minFragments ||
                    fragCount > PDAScanner.EntryData.maxFragments)
                {
                    QuickLogger.Warning($"Invalid number of FragmentsToScan for entry '{this.ItemID}'. Must be between {PDAScanner.EntryData.minFragments} and {PDAScanner.EntryData.maxFragments}.");
                    return false;
                }

                if (this.TechType > TechType.Databox)
                {
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} appears to be a modded item. {this.Key} can only be applied to existing game items.");
                    return false;
                }

                PDAHandler.EditFragmentsToScan(this.TechType, fragCount);
                QuickLogger.Message($"'{this.ItemID}' from {this.Origin} now requires {fragCount} fragments scanned to unlock.");
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Custom Fragment Count '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }
    }
}
