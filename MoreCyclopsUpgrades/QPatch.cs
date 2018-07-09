namespace MoreCyclopsUpgrades
{
    using System;
    using System.Reflection;
    using Harmony;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        internal static readonly SortedCyclopsModules ModulesToPatch = new SortedCyclopsModules(7)
        {
            new SolarCharger(),
            new SolarChargerMk2(),
            new ThermalChargerMk2(),
            new PowerUpgradeMk2(),
            new PowerUpgradeMk3(),
            new NuclearCharger(),
            new DepletedNuclearModule(),
        };

        public static void Patch()
        {
#if DEBUG
            try
            {
#endif
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
