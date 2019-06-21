namespace MoreCyclopsUpgrades
{
    using System;
    using System.IO;
    using System.Reflection;
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.Config;
    using MoreCyclopsUpgrades.Items.AuxConsole;
    using MoreCyclopsUpgrades.Items.ThermalModule;
    using MoreCyclopsUpgrades.Managers;

    /// <summary>
    /// Entry point class for patching. For use by QModManager only.
    /// </summary>
    public class QPatch
    {
        /// <summary>
        /// Main patching method. For use by QModManager only.
        /// </summary>
        public static void Patch()
        {
            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                ModConfig.Main.Initialize();

                RemoveOldConfigs();

                RegisterCoreServices();

                RegisterOriginalUpgrades();

                PatchAuxUpgradeConsole();

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished Patching");

            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        private static void RemoveOldConfigs()
        {
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string oldConfig1 = Path.Combine(executingLocation, $"CyclopsNuclearChargerConfig.txt");
            string oldConfig2 = Path.Combine(executingLocation, $"MoreCyclopsUpgradesConfig.txt");

            if (File.Exists(oldConfig1))
                File.Delete(oldConfig1);

            if (File.Exists(oldConfig2))
                File.Delete(oldConfig2);
        }

        private static void RegisterOriginalUpgrades()
        {
            var originalUpgrades = new OriginalUpgrades.OriginalUpgrades();
            originalUpgrades.RegisterOriginalUpgrades();
        }

        private static void PatchUpgradeModules()
        {
            var thermalMk2 = new CyclopsThermalChargerMk2();
            thermalMk2.Patch();

            MCUServices.Register.CyclopsUpgradeHandler(thermalMk2);
            MCUServices.Register.CyclopsCharger(thermalMk2);
        }

        private static void PatchAuxUpgradeConsole()
        {
            if (ModConfig.Main.AuxConsoleEnabled)
                QuickLogger.Info("Patching Auxiliary Upgrade Console");
            else
                QuickLogger.Info("Auxiliary Upgrade Console disabled by config settings");

            var console = new CyUpgradeConsole();
            console.Patch(ModConfig.Main.AuxConsoleEnabled);
        }

        internal static void RegisterCoreServices()
        {
            MCUServices.Register.AuxCyclopsManager((SubRoot cyclops) =>
            {
                QuickLogger.Debug("Core UpgradeManager registered");
                return new UpgradeManager(cyclops);
            });

            MCUServices.Register.AuxCyclopsManager((SubRoot cyclops) =>
            {
                QuickLogger.Debug("Core ChargeManager registered");
                return new ChargeManager(cyclops);
            });

            MCUServices.Register.AuxCyclopsManager((SubRoot cyclops) =>
            {
                QuickLogger.Debug("Core CyclopsHUDManager registered");
                return new CyclopsHUDManager(cyclops);
            });
        }
    }
}
