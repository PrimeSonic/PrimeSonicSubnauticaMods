namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;    

    internal class UpgradeConsoleCache
    {
        private static List<AuxUpgradeConsole> auxConsoleCache = new List<AuxUpgradeConsole>();

        internal static IList<AuxUpgradeConsole> AuxUpgradeConsoles => auxConsoleCache;

        internal static void SyncUpgradeConsoles(SubRoot cyclops, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            // This is a dirty workaround to get a reference to the Cyclops into the AuxUpgradeConsole
            // This is also an even dirtier workaround because of the double-references objects being returned.

            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
            {
                if (auxConsoleCache.Contains(auxConsole))
                    return;

                auxConsoleCache.Add(auxConsole);

                string log = $"AuxUpgradeConsole: {auxConsole.gameObject.name}: {auxConsole.gameObject.transform.position} - HasParentCyclops:{auxConsole.ParentCyclops != null}";

                ErrorMessage.AddMessage(log);
                System.Console.WriteLine(log);

                if (auxConsole.ParentCyclops == null)
                {
                    auxConsole.ParentCyclops = cyclops;
                    ErrorMessage.AddMessage("Auxiliary Upgrade Console has been connected");
                }
            }
        }
    }
}
