namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class ParsingPackage<CustomCraftEntry, EmCollectionListT> : IParsingPackage
            where CustomCraftEntry : EmPropertyCollection, ICustomCraft
            where EmCollectionListT : EmPropertyCollectionList<CustomCraftEntry>, new()
    {
        public string ListKey { get; }

        internal IList<CustomCraftEntry> ParsedEntries { get; } = new List<CustomCraftEntry>();
        internal IDictionary<string, CustomCraftEntry> UniqueEntries { get; } = new Dictionary<string, CustomCraftEntry>();

        internal string TypeName { get; } = typeof(CustomCraftEntry).Name;

        public ParsingPackage(string listKey)
        {
            this.ListKey = listKey;
        }

        public int ParseEntries(string serializedData)
        {
            var list = new EmCollectionListT();

            bool successfullyParsed = list.Deserialize(serializedData);

            if (!successfullyParsed)
                return -1; // Error case

            if (list.Count == 0)
                return 0; // No entries

            int count = 0;
            foreach (CustomCraftEntry item in list)
            {
                this.ParsedEntries.Add(item);
                count++;
            }

            return count; // Return the number of unique entries added in this list
        }

        public void PrePassValidation()
        {
            //  Use the ToSet function as a copy constructor - this way we can iterate across the
            //      temp structure, but change the permanent one in the case of duplicates
            foreach (CustomCraftEntry item in this.ParsedEntries)
            {
                if (!item.PassesPreValidation())
                    continue;

                if (this.UniqueEntries.ContainsKey(item.ID))
                {
                    QuickLogger.Warning($"Duplicate entry for {this.TypeName} '{item.ID}' was already added by another working file. Kept first one. Discarded duplicate.");
                    continue;
                }

                // All checks passed
                this.UniqueEntries.Add(item.ID, item);
            }

            if (this.ParsedEntries.Count > 0)
                QuickLogger.Message($"{this.UniqueEntries.Count} of {this.ParsedEntries.Count} {this.TypeName} entries staged for patching");
        }

        public void SendToSMLHelper()
        {
            int successCount = 0;
            foreach (CustomCraftEntry item in this.UniqueEntries.Values)
            {
                if (item.SendToSMLHelper())
                    successCount++;
            }

            if (this.UniqueEntries.Count > 0)
                QuickLogger.Message($"{successCount} of {this.UniqueEntries.Count} {this.TypeName} entries were patched");
        }
    }
}
