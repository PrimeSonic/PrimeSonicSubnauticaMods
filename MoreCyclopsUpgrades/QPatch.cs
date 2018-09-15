namespace MoreCyclopsUpgrades
{
    using System;
    using System.IO;
    using System.Reflection;
    using Caching;
    using Common;
    using Harmony;
    using Buildables;
    using Modules;
    using SaveData;

    public class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Message("Started patching " + QuickLogger.GetAssemblyVersion());

                OtherMods.VehicleUpgradesInCyclops = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops");

                if (OtherMods.VehicleUpgradesInCyclops)
                    QuickLogger.Message("VehicleUpgradesInCyclops detected. Correcting placement of craft nodes in Cyclops Fabricator.");

                var modConfig = new EmModPatchConfig();
                modConfig.Initialize();

                QuickLogger.Message("Patching new upgrade modules");
                CyclopsModule.PatchAllModules(modConfig.EnableNewUpgradeModules);

                if (!modConfig.EnableNewUpgradeModules)
                    QuickLogger.Message("New upgrade modules disabled by config settings");

                QuickLogger.Message("Patching Auxiliary Upgrade Console");

                var auxConsole = new AuxCyUpgradeConsole();

                auxConsole.Patch(modConfig.EnableAuxiliaryUpgradeConsoles);

                if (!modConfig.EnableAuxiliaryUpgradeConsoles)
                    QuickLogger.Message("Auxiliary Upgrade Console disabled by config settings");

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Message("Finished Patching");

            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
