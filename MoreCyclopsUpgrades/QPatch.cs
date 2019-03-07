namespace MoreCyclopsUpgrades
{
    using System;
    using System.IO;
    using System.Reflection;
    using Buildables;
    using Caching;
    using Common;
    using Harmony;
    using Modules;
    using SaveData;

    public class QPatch
    {
        public static void Patch()
        {
#if RELEASE
                QuickLogger.DebugLogsEnabled = false;
#endif

#if DEBUG
                QuickLogger.DebugLogsEnabled = true;
#endif

            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                OtherMods.VehicleUpgradesInCyclops = Directory.Exists(@"./QMods/VehicleUpgradesInCyclops");

                if (OtherMods.VehicleUpgradesInCyclops)
                    QuickLogger.Debug("VehicleUpgradesInCyclops detected. Correcting placement of craft nodes in Cyclops Fabricator.");

                var modConfig = new EmModPatchConfig();
                modConfig.Initialize();

                PatchUpgradeModules(modConfig);

                PatchAuxUpgradeConsole(modConfig);

                PatchBioEnergy(modConfig);

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished Patching");

            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        private static void PatchUpgradeModules(EmModPatchConfig modConfig)
        {
            if (modConfig.EnableNewUpgradeModules)
                QuickLogger.Info("Patching new upgrade modules");
            else
                QuickLogger.Info("New upgrade modules disabled by config settings");

            CyclopsModule.PatchAllModules(modConfig.EnableNewUpgradeModules);
        }

        private static void PatchAuxUpgradeConsole(EmModPatchConfig modConfig)
        {
            if (modConfig.EnableAuxiliaryUpgradeConsoles)
                QuickLogger.Info("Patching Auxiliary Upgrade Console");
            else
                QuickLogger.Info("Auxiliary Upgrade Console disabled by config settings");

            var auxConsole = new CyUpgradeConsole();

            auxConsole.Patch(modConfig.EnableAuxiliaryUpgradeConsoles);
        }

        private static void PatchBioEnergy(EmModPatchConfig modConfig)
        {
            if (modConfig.EnableBioEnergy)
                QuickLogger.Info("Patching Cyclops Bioreactor");
            else
                QuickLogger.Info("Cyclops Bioreactor disabled by config settings");

            var cyBioEnergy = new CyBioReactor();

            cyBioEnergy.Patch(modConfig.EnableBioEnergy);                
        }
    }
}
