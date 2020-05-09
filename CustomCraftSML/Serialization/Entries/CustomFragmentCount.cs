namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Lists;
    using EasyMarkup;
    using SMLHelper.V2.Handlers;

    internal class CustomFragmentCount : EmTechTyped, ICustomFragmentCount, ICustomCraft
    {
        private const string FragmentsToScanKey = "FragmentsToScan";
        private const string TypeName = "CustomFragments";

        public string[] TutorialText => CustomFragmentCountTutorial;

        internal static readonly string[] CustomFragmentCountTutorial = new[]
        {
            $"{CustomFragmentCountList.ListKey}: Change how many fragments must be scanned to unlock recipes/blueprints",
            $"    In addition to the usual {ItemIdKey}, you only need one more property for this one:",
            $"        {FragmentsToScanKey}: Simply set this to the total number of fragments that must be scanned to unlock the item in question.",
        };

        private readonly EmProperty<int> emFragmentCount;

        protected static List<EmProperty> FragmentProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<int>(FragmentsToScanKey, 1),
        };

        public CustomFragmentCount() : this(TypeName, FragmentProperties)
        {
        }

        protected CustomFragmentCount(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emFragmentCount = (EmProperty<int>)Properties[FragmentsToScanKey];
        }

        public OriginFile Origin { get; set; }

        public bool PassedSecondValidation => true;

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

        internal override EmProperty Copy()
        {
            return new CustomFragmentCount(this.Key, this.CopyDefinitions);
        }

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
                QuickLogger.Debug($"'{this.ItemID}' from {this.Origin} now requires {fragCount} fragments scanned to unlock.");
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
