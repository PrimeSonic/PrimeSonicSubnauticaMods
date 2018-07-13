namespace MoreCyclopsUpgrades
{
    using Common;

    /// <summary>
    /// Power Index 0: No efficiency modules equipped.
    /// Power Index 1: Standard PowerUpgradeModule equipped.
    /// Power Index 2: PowerUpgradeModuleMk2 equipped.
    /// Power Index 3: PowerUpgradeModuleMk3 equipped.
    /// </summary>    
    internal static class PowerIndexManager
    {
        private const string currentPowerRatingField = "currPowerRating";

        private static readonly float[] EnginePowerRatings = new[]
        {
            1f, // Power Index 0: Base Value
            3f, // Power Index 1: 300% increase
            5f, // Power Index 2: 500% increase
            6f  // Power Index 3: 600% increase
        };

        private static readonly float[] SilentRunningPowerCosts = new[]
        {
            5f, // Power Index 0: Base Value
            5f, // Power Index 1: Base Value
            4f, // Power Index 2: 20% cost reduction
            3f  // Power Index 3: 40% cost reduction
        };

        private static readonly float[] SonarPowerCosts = new[]
        {
            10f, // Power Index 0: Base Value
            10f, // Power Index 1: Base Value
            8f,  // Power Index 2: 20% cost reduction
            7f   // Power Index 3: 30% cost reduction
        };

        private static readonly float[] ShieldPowerCosts = new[]
        {
            50f, // Power Index 0: Base Value
            50f, // Power Index 1: Base Value
            42f, // Power Index 2: 16% cost reduction
            34f  // Power Index 3: 32% cost reduction
        };

        internal static void UpdatePowerIndex(ref SubRoot cyclops)
        {
            float originalRating = (float)cyclops.GetPrivateField(currentPowerRatingField);

            Equipment modules = cyclops.upgradeConsole.modules;

            int powerIndex = GetPowerIndex(modules);

            cyclops.silentRunningPowerCost = SilentRunningPowerCosts[powerIndex];
            cyclops.sonarPowerCost = SonarPowerCosts[powerIndex];
            cyclops.shieldPowerCost = ShieldPowerCosts[powerIndex];

            float nextPowerRating = EnginePowerRatings[powerIndex];
            
            if (originalRating != nextPowerRating)
            {
                cyclops.SetPrivateField(currentPowerRatingField, nextPowerRating);
                // Inform the new power rating just like the original method would.
                string format = Language.main.GetFormat("PowerRatingNowFormat", nextPowerRating);
                ErrorMessage.AddMessage(format);
            }
        }

        private static int GetPowerIndex(Equipment modules)
        {
            int powerIndex = 0;

            if (modules.GetCount(TechType.PowerUpgradeModule) > 0)
            {
                powerIndex = 1;
            }

            if (modules.GetCount(CyclopsModule.PowerUpgradeMk2ID) > 0)
            {
                powerIndex = 2;
            }

            if (modules.GetCount(CyclopsModule.PowerUpgradeMk3ID) > 0)
            {
                powerIndex = 3;
            }

            return powerIndex;
        }
    }
}
