namespace MoreCyclopsUpgrades
{
    using System;
    using System.IO;
    using System.Reflection;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {
#if DEBUG
            try
            {

#endif          
                var modConfig = new EmModPatchConfig();

                modConfig.Initialize();

                bool hasVehicleUpgradesInCyclops = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops");

                Console.WriteLine("[MoreCyclopsUpgrades] Patching new upgrade modules");
                CyclopsModule.PatchAllModules(hasVehicleUpgradesInCyclops, modConfig.EnableNewUpgradeModules);

                if (!modConfig.EnableNewUpgradeModules)
                {
                    Console.WriteLine("[MoreCyclopsUpgrades] New upgrade modules disabled by config settings");
                }

                Console.WriteLine("[MoreCyclopsUpgrades] Patching Auxiliary Upgrade Console");
                AuxCyUpgradeConsole.Patch(modConfig.EnableAuxiliaryUpgradeConsoles);

                if (!modConfig.EnableAuxiliaryUpgradeConsoles)
                {
                    Console.WriteLine("[MoreCyclopsUpgrades] Auxiliary Upgrade Console disabled by config settings");
                }

                HarmonyInstance harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Console.WriteLine("[MoreCyclopsUpgrades] Finished Patching");
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
