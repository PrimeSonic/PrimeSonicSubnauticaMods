namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;

    internal class BatteryUpgradesManager
    {
        internal IList<BatteryDetails> SolarMk2Batteries => SolarMk2Upgrades.Batteries;
        internal IList<BatteryDetails> ThermalMk2Batteries => ThermalMk2Upgrades.Batteries;
        internal IList<BatteryDetails> NuclearModules => NuclearUpgrades.Batteries;

        internal readonly BatteryBackedCyclopsUpgrade SolarMk2Upgrades;
        internal readonly BatteryBackedCyclopsUpgrade ThermalMk2Upgrades;
        internal readonly BatteryBackedCyclopsUpgrade NuclearUpgrades;

        public BatteryUpgradesManager(
            BatteryBackedCyclopsUpgrade solarMk2Upgrades, 
            BatteryBackedCyclopsUpgrade thermalMk2Upgrades, 
            BatteryBackedCyclopsUpgrade nuclearUpgrades)
        {
            SolarMk2Upgrades = solarMk2Upgrades;
            ThermalMk2Upgrades = thermalMk2Upgrades;
            NuclearUpgrades = nuclearUpgrades;
        }
    }
}
