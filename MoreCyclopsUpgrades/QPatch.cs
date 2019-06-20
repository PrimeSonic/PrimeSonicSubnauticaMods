namespace MoreCyclopsUpgrades
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.Items.AuxConsole;
    using MoreCyclopsUpgrades.Items.ThermalModule;
    using MoreCyclopsUpgrades.Managers;
    using MoreCyclopsUpgrades.StandardUpgrades;
    using SaveData;

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
            // TODO - Make user configurable
            QuickLogger.DebugLogsEnabled = true;

            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                RegisterCoreServices();

                ModConfigSavaData.Initialize();

                var originalUpgrades = new OriginalUpgrades();
                originalUpgrades.RegisterOriginalUpgrades();

                PatchAuxUpgradeConsole(ModConfigSavaData.Settings.EnableAuxiliaryUpgradeConsoles);

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished Patching");

            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        private static void PatchUpgradeModules()
        {
            var thermalMk2 = new CyclopsThermalChargerMk2();
            thermalMk2.Patch();

            MCUServices.Register.CyclopsUpgradeHandler(thermalMk2);
            MCUServices.Register.CyclopsCharger(thermalMk2);
        }

        private static void PatchAuxUpgradeConsole(bool enableAuxiliaryUpgradeConsoles)
        {
            if (enableAuxiliaryUpgradeConsoles)
                QuickLogger.Info("Patching Auxiliary Upgrade Console");
            else
                QuickLogger.Info("Auxiliary Upgrade Console disabled by config settings");

            var console = new CyUpgradeConsole();
            console.Patch(enableAuxiliaryUpgradeConsoles);
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
