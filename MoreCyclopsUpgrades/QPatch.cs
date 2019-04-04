namespace MoreCyclopsUpgrades
{
    using Buildables;
    using Caching;
    using Common;
    using Harmony;
    using Modules;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using MoreCyclopsUpgrades.Managers;
    using SaveData;
    using System;
    using System.IO;
    using System.Reflection;

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

                EmModPatchConfig.Initialize();

                // TODO - Configure cyclops power levels

                PatchUpgradeModules(EmModPatchConfig.Settings.EnableNewUpgradeModules);

                PatchAuxUpgradeConsole(EmModPatchConfig.Settings.EnableAuxiliaryUpgradeConsoles);

                PatchBioEnergy(EmModPatchConfig.Settings.EnableBioReactors);

                RegisterExternalUpgrades();

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

        private static void PatchBioEnergy(bool enableBioreactors)
        {
            if (enableBioreactors)
                QuickLogger.Info("Patching Cyclops Bioreactor");
            else
                QuickLogger.Info("Cyclops Bioreactor disabled by config settings");
            
            CyBioReactor.PatchCyBioReactor(enableBioreactors);
        }

        private static void RegisterExternalUpgrades()
        {
            UpgradeManager.RegisterReusableHandlerCreator(() => { return new CrushDepthUpgradesHandler(); });

            UpgradeManager.RegisterReusableHandlerCreator(() =>
            {
                return new UpgradeHandler(TechType.CyclopsShieldModule)
                {
                    OnClearUpgrades = (SubRoot cyclops) => { cyclops.shieldUpgrade = false; },
                    OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.shieldUpgrade = true; },
                };
            });

            UpgradeManager.RegisterReusableHandlerCreator(() =>
            {
                return new UpgradeHandler(TechType.CyclopsSonarModule)
                {
                    OnClearUpgrades = (SubRoot cyclops) => { cyclops.sonarUpgrade = false; },
                    OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.sonarUpgrade = true; },
                };
            });

            UpgradeManager.RegisterReusableHandlerCreator(() =>
            {
                return new UpgradeHandler(TechType.CyclopsSeamothRepairModule)
                {
                    OnClearUpgrades = (SubRoot cyclops) => { cyclops.vehicleRepairUpgrade = false; },
                    OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.vehicleRepairUpgrade = true; },
                };
            });

            UpgradeManager.RegisterReusableHandlerCreator(() =>
            {
                return new UpgradeHandler(TechType.CyclopsDecoyModule)
                {
                    OnClearUpgrades = (SubRoot cyclops) => { cyclops.decoyTubeSizeIncreaseUpgrade = false; },
                    OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) => { cyclops.decoyTubeSizeIncreaseUpgrade = true; },
                };
            });

            UpgradeManager.RegisterReusableHandlerCreator(() =>
            {
                return new UpgradeHandler(TechType.CyclopsFireSuppressionModule)
                {
                    OnClearUpgrades = (SubRoot cyclops) =>
                    {
                        CyclopsHolographicHUD fss = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();
                        if (fss != null)
                            fss.fireSuppressionSystem.SetActive(false);
                    },
                    OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) =>
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
