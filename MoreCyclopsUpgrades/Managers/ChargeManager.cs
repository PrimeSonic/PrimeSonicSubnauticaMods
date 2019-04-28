namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.Monobehaviors;
    using MoreCyclopsUpgrades.SaveData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ChargeManager
    {
        internal readonly SubRoot Cyclops;
        internal CyclopsManager Manager;

        internal SolarChargeHandler SolarCharging;
        internal ThermalChargeHandler ThermalCharging;
        internal BioChargeHandler BioCharging;
        internal NuclearChargeHandler NuclearCharging;

        internal ChargingUpgradeHandler SolarCharger;
        internal ChargingUpgradeHandler ThermalCharger;
        internal BatteryUpgradeHandler SolarChargerMk2;
        internal BatteryUpgradeHandler ThermalChargerMk2;
        internal BatteryUpgradeHandler NuclearCharger;
        internal BioBoosterUpgradeHandler BioBoosters;

        internal int MaxBioReactors => BioCharging.MaxBioReactors;

        internal readonly List<CyBioReactorMono> CyBioReactors = new List<CyBioReactorMono>();
        private readonly List<CyBioReactorMono> TempCache = new List<CyBioReactorMono>();

        public ChargeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
            UpgradeManager.UpgradeManagerInitializing += SetupChargingUpgrades;
            PowerManager.CyclopsChargersInitializing += RegisterPowerChargers;
        }

        internal bool Initialize(CyclopsManager manager)
        {
            Manager = manager;
            return true;
        }

        /// <summary>
        /// Gets the total available reserve power across all equipment upgrade modules.
        /// </summary>
        /// <returns>The <see cref="int"/> value of the total available reserve power.</returns>
        internal int GetTotalReservePower()
        {
            float availableReservePower = 0f;
            availableReservePower += SolarChargerMk2.TotalBatteryCharge;
            availableReservePower += ThermalChargerMk2.TotalBatteryCharge;
            availableReservePower += NuclearCharger.TotalBatteryCharge;

            foreach (CyBioReactorMono reactor in CyBioReactors)
                availableReservePower += reactor.Battery._charge;

            return Mathf.FloorToInt(availableReservePower);
        }

        private void RegisterPowerChargers()
        {
            PowerManager.RegisterOneTimeUseChargerCreator((SubRoot cyclopsRef) =>
            {
                QuickLogger.Debug("CyclopsCharger Registered: Solar charging ready");
                var solarChargeHandler = new SolarChargeHandler(this);
                SolarCharging = solarChargeHandler;
                return solarChargeHandler;
            });

            PowerManager.RegisterOneTimeUseChargerCreator((SubRoot cyclopsRef) =>
            {
                QuickLogger.Debug("CyclopsCharger Registered: Thermal charging ready");
                var thermalChargeHandler = new ThermalChargeHandler(this);
                ThermalCharging = thermalChargeHandler;
                return thermalChargeHandler;
            });

            PowerManager.RegisterOneTimeUseChargerCreator((SubRoot cyclopsRef) =>
            {
                QuickLogger.Debug("CyclopsCharger Registered: Bio charging ready");
                var bioChargeHandler = new BioChargeHandler(this);
                BioCharging = bioChargeHandler;
                return bioChargeHandler;
            });

            PowerManager.RegisterOneTimeUseChargerCreator((SubRoot cyclopsRef) =>
            {
                QuickLogger.Debug("CyclopsCharger Registered: Nuclear charging ready");
                var nuclearChargeHandler = new NuclearChargeHandler(this);
                NuclearCharging = nuclearChargeHandler;
                return nuclearChargeHandler;
            });

            PowerManager.CyclopsChargersInitializing -= RegisterPowerChargers;
        }

        private void SetupChargingUpgrades()
        {
            int maxChargingModules = ModConfig.Settings.MaxChargingModules();

            UpgradeManager.RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: SolarCharger Upgrade");
                SolarCharger = new ChargingUpgradeHandler(CyclopsModule.SolarChargerID)
                {
                    MaxCount = maxChargingModules
                };
                SolarCharger.OnFirstTimeMaxCountReached += () =>
                {
                    ErrorMessage.AddMessage(CyclopsModule.MaxSolarReached());
                };
                return SolarCharger;
            });

            UpgradeManager.RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: SolarChargerMk2 Upgrade");
                SolarChargerMk2 = new BatteryUpgradeHandler(CyclopsModule.SolarChargerMk2ID, canRecharge: true)
                {
                    MaxCount = maxChargingModules
                };
                SolarChargerMk2.OnFirstTimeMaxCountReached += () =>
                {
                    ErrorMessage.AddMessage(CyclopsModule.MaxSolarReached());
                };

                SolarCharger.SiblingUpgrade = SolarChargerMk2;
                SolarChargerMk2.SiblingUpgrade = SolarCharger;
                return SolarChargerMk2;
            });

            UpgradeManager.RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: ThermalCharger Upgrade");
                ThermalCharger = new ChargingUpgradeHandler(TechType.CyclopsThermalReactorModule)
                {
                    MaxCount = maxChargingModules
                };
                ThermalCharger.OnFirstTimeMaxCountReached += () =>
                {
                    ErrorMessage.AddMessage(CyclopsModule.MaxThermalReached());
                };
                return ThermalCharger;
            });

            UpgradeManager.RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: ThermalChargerMk2 Upgrade");
                ThermalChargerMk2 = new BatteryUpgradeHandler(CyclopsModule.ThermalChargerMk2ID, canRecharge: true)
                {
                    MaxCount = maxChargingModules
                };
                ThermalChargerMk2.OnFirstTimeMaxCountReached += () =>
                {
                    ErrorMessage.AddMessage(CyclopsModule.MaxThermalReached());
                };
                ThermalCharger.SiblingUpgrade = ThermalChargerMk2;
                ThermalChargerMk2.SiblingUpgrade = ThermalCharger;
                return ThermalChargerMk2;
            });

            UpgradeManager.RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: NuclearReactor Upgrade");
                NuclearCharger = new NuclearUpgradeHandler()
                {
                    MaxCount = Math.Min(maxChargingModules, 3) // No more than 3 no matter what the difficulty
                };
                return NuclearCharger;
            });

            UpgradeManager.RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: BioBooster Upgrade");
                BioBoosters = new BioBoosterUpgradeHandler();
                return BioBoosters;
            });

            UpgradeManager.UpgradeManagerInitializing -= SetupChargingUpgrades;
        }

        internal void SyncBioReactors()
        {
            if (Manager == null)
                return;

            TempCache.Clear();

            SubRoot cyclops = Cyclops;

            CyBioReactorMono[] cyBioReactors = cyclops.GetAllComponentsInChildren<CyBioReactorMono>();

            foreach (CyBioReactorMono cyBioReactor in cyBioReactors)
            {
                if (TempCache.Contains(cyBioReactor))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                TempCache.Add(cyBioReactor);

                if (cyBioReactor.ParentCyclops == null)
                {
                    QuickLogger.Debug("CyBioReactorMono synced externally");
                    // This is a workaround to get a reference to the Cyclops into the AuxUpgradeConsole
                    cyBioReactor.ConnectToCyclops(cyclops, this);
                }
            }

            if (TempCache.Count != CyBioReactors.Count)
            {
                CyBioReactors.Clear();
                CyBioReactors.AddRange(TempCache);
            }
        }
    }
}
