namespace MoreCyclopsUpgrades
{
    internal class UpgradeConsoleCache
    {
        private static int LastKnownAuxUpgradeConsoleCount = 0;

        internal static void SyncUpgradeConsoles(SubRoot cyclops, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            // This is a dirty workaround to get a reference to the Cyclops into the AuxUpgradeConsole
            if (auxUpgradeConsoles.Length == LastKnownAuxUpgradeConsoleCount)
                return;
            
            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
                auxConsole.ParentCyclops = cyclops;

            if (LastKnownAuxUpgradeConsoleCount < auxUpgradeConsoles.Length)
                ErrorMessage.AddMessage("Auxiliary Upgrade Console has been connected");

            LastKnownAuxUpgradeConsoleCount = auxUpgradeConsoles.Length;
        }
    }
}
