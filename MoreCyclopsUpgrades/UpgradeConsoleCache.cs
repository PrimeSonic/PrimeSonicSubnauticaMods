namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;

    internal class UpgradeConsoleCache
    {
        internal static List<AuxUpgradeConsole> AuxUpgradeConsoles { get; } = new List<AuxUpgradeConsole>();

        internal static void SyncUpgradeConsoles(SubRoot cyclops, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            // This is a dirty workaround to get a reference to the Cyclops into the AuxUpgradeConsole
            // This is also an even dirtier workaround because of the double-references objects being returned.

            var tempCache = new List<AuxUpgradeConsole>();

            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
            {
                if (tempCache.Contains(auxConsole))
                    continue;

                tempCache.Add(auxConsole);

                if (auxConsole.ParentCyclops == null)
                {
                    auxConsole.ParentCyclops = cyclops;
                    ErrorMessage.AddMessage("Auxiliary Upgrade Console has been connected");
                }
            }

            if (tempCache.Count != AuxUpgradeConsoles.Count)
            {
                AuxUpgradeConsoles.Clear();
                AuxUpgradeConsoles.AddRange(tempCache);
            }
        }
    }
}
