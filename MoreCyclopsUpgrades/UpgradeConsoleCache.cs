namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;

    internal class UpgradeConsoleCache
    {
        private static List<AuxUpgradeConsole> TempCache = new List<AuxUpgradeConsole>();

        internal static List<AuxUpgradeConsole> AuxUpgradeConsoles { get; } = new List<AuxUpgradeConsole>();

        internal static void SyncUpgradeConsoles(SubRoot cyclops, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            // This is a dirty workaround to get a reference to the Cyclops into the AuxUpgradeConsole
            // This is also an even dirtier workaround because of the double-references objects being returned.
            TempCache.Clear();

            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
            {
                if (TempCache.Contains(auxConsole))
                    continue;

                TempCache.Add(auxConsole);

                if (auxConsole.ParentCyclops == null)
                {
                    auxConsole.ParentCyclops = cyclops;
                    ErrorMessage.AddMessage("Auxiliary Upgrade Console has been connected");
                }
            }

            if (TempCache.Count != AuxUpgradeConsoles.Count)
            {
                AuxUpgradeConsoles.Clear();
                AuxUpgradeConsoles.AddRange(TempCache);
            }
        }
    }
}
