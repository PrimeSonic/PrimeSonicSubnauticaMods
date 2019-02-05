namespace CustomCraft2SML
{
    using System.Collections.Generic;

    internal static class ScannerMappings
    {
        internal static Dictionary<TechType, PDAScanner.EntryData> BlueprintToFragment = new Dictionary<TechType, PDAScanner.EntryData>();

        internal static void Load()
        {
            Dictionary<TechType, PDAScanner.EntryData>.Enumerator entries = PDAScanner.GetAllEntriesData();

            do
            {
                KeyValuePair<TechType, PDAScanner.EntryData> entry = entries.Current;

                BlueprintToFragment.Add(entry.Value.blueprint, entry.Value);

            } while (entries.MoveNext());
        }
    }
}
