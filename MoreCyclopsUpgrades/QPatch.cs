namespace MoreCyclopsUpgrades
{
    using System;
    using System.Reflection;
    using Buildables;
    using Common;
    using Harmony;
    using Modules;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
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
#if RELEASE
            QuickLogger.DebugLogsEnabled = false;
#endif

#if DEBUG
            QuickLogger.DebugLogsEnabled = true;
#endif

            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                ModConfigSavaData.Initialize();

                QuickLogger.Info($"Difficult set to {ModConfigSavaData.Settings.PowerLevel}");

                RegisterOriginalUpgrades();

                PatchUpgradeModules(ModConfigSavaData.Settings.EnableNewUpgradeModules);

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

        private static void PatchUpgradeModules(bool enableNewUpgradeModules)
        {
            if (enableNewUpgradeModules)
                QuickLogger.Info("Patching new upgrade modules");
            else
                QuickLogger.Info("New upgrade modules disabled by config settings");

            CyclopsModule.PatchAllModules(enableNewUpgradeModules);
        }

        private static void PatchAuxUpgradeConsole(bool enableAuxiliaryUpgradeConsoles)
        {
            if (enableAuxiliaryUpgradeConsoles)
                QuickLogger.Info("Patching Auxiliary Upgrade Console");
            else
                QuickLogger.Info("Auxiliary Upgrade Console disabled by config settings");

            CyUpgradeConsole.PatchAuxUpgradeConsole(enableAuxiliaryUpgradeConsoles);
        }

        private static void RegisterOriginalUpgrades()
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

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: Depth Upgrades Collection");
                return new CrushDepthUpgradesHandler(cyclops);
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsShieldModule");
                return new UpgradeHandler(TechType.CyclopsShieldModule, cyclops)
                {
                    OnClearUpgrades = () => { cyclops.shieldUpgrade = false; },
                    OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.shieldUpgrade = true; },
                };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsSonarModule");
                return new UpgradeHandler(TechType.CyclopsSonarModule, cyclops)
                {
                    OnClearUpgrades = () => { cyclops.sonarUpgrade = false; },
                    OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.sonarUpgrade = true; },
                };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsSeamothRepairModule");
                return new UpgradeHandler(TechType.CyclopsSeamothRepairModule, cyclops)
                {
                    OnClearUpgrades = () => { cyclops.vehicleRepairUpgrade = false; },
                    OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.vehicleRepairUpgrade = true; },
                };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsDecoyModule");
                return new UpgradeHandler(TechType.CyclopsDecoyModule, cyclops)
                {
                    OnClearUpgrades = () => { cyclops.decoyTubeSizeIncreaseUpgrade = false; },
                    OnUpgradeCounted = (Equipment modules, string slot) => { cyclops.decoyTubeSizeIncreaseUpgrade = true; },
                };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: CyclopsFireSuppressionModule");
                return new UpgradeHandler(TechType.CyclopsFireSuppressionModule, cyclops)
                {
                    OnClearUpgrades = () =>
                    {
                        CyclopsHolographicHUD fss = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();
                        if (fss != null)
                            fss.fireSuppressionSystem.SetActive(false);
                    },
                    OnUpgradeCounted = (Equipment modules, string slot) =>
                    {
                        CyclopsHolographicHUD fss = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();
                        if (fss != null)
                            fss.fireSuppressionSystem.SetActive(true);
                    },
                };
            });
        }
    }
}
