namespace MoreCyclopsUpgrades
{
    using System;
    using System.Reflection;
    using Buildables;
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using MoreCyclopsUpgrades.Craftables;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using MoreCyclopsUpgrades.Managers;
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

            CyUpgradeConsole.PatchAuxUpgradeConsole(enableAuxiliaryUpgradeConsoles);
        }

        internal static void RegisterCoreServices()
        {
            MCUServices.Register.AuxCyclopsManager((SubRoot cyclops) => 
            {
                return new UpgradeManager(cyclops);
            });

            MCUServices.Register.AuxCyclopsManager((SubRoot cyclops) =>
            {
                return new ChargeManager(cyclops);
            });

            MCUServices.Register.AuxCyclopsManager((SubRoot cyclops) =>
            {
                return new CyclopsHUDManager(cyclops);
            });
        }
    }
}
