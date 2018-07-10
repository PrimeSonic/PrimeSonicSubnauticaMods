namespace MoreCyclopsUpgrades
{
    using System;
    using System.IO;
    using System.Reflection;
    using Harmony;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        private static SortedCyclopsModules ModulesToPatch;

        public static void Patch()
        {
#if DEBUG
            try
            {

#endif
                bool hasVehicleUpgradesInCyclops = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops");

                ModulesToPatch = new SortedCyclopsModules(7)
                {
                    new SolarCharger(hasVehicleUpgradesInCyclops),
                    new SolarChargerMk2(),
                    new ThermalChargerMk2(),
                    new PowerUpgradeMk2(),
                    new PowerUpgradeMk3(),
                    new NuclearCharger(),
                    new DepletedNuclearModule(),
                };

                foreach (CyclopsModule module in ModulesToPatch.Values)
                {
                    module.Patch();
                }

                HarmonyInstance harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
#if DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine("[MoreCyclopsUpgrades] ERROR: " + ex.ToString());
            }
#endif
        }
    }
}
