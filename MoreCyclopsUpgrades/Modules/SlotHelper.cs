namespace MoreCyclopsUpgrades
{
    internal static class SlotHelper
    {
        internal const int SlotCount = 6;

        // This is a copy of the private dictionary in SubRoot used to access the module slots.
        internal static readonly string[] SlotNames = new string[]
        {
                "Module1",
                "Module2",
                "Module3",
                "Module4",
                "Module5",
                "Module6"
        };
    }
}
