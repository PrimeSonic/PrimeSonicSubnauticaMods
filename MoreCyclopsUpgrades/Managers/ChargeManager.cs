namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.Monobehaviors;
    using MoreCyclopsUpgrades.SaveData;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    internal class ChargeManager
    {
        internal const float BatteryDrainRate = 0.01f;
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        internal const float Mk2ChargeRateModifier = 1.15f; // The MK2 charging modules get a 15% bonus to their charge rate.

        private static readonly ICollection<ChargerCreator> CyclopsChargers = new List<ChargerCreator>();

        /// <summary>
        /// Registers a <see cref="ChargerCreator"/> method that creates returns a new <see cref="ICyclopsCharger"/> on demand and is only used once.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="ChargerCreator"/>.</param>
        internal static void RegisterChargerCreator(ChargerCreator createEvent)
        {
            if (CyclopsChargers.Contains(createEvent))
            {
                QuickLogger.Warning($"Duplicate ChargerCreator blocked from {Assembly.GetCallingAssembly().GetName().Name}");
                return;
            }

            QuickLogger.Info($"Received ChargerCreator from {Assembly.GetCallingAssembly().GetName().Name}");
            CyclopsChargers.Add(createEvent);
        }

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


        private int rechargeSkip = 10;

        internal readonly List<CyBioReactorMono> CyBioReactors = new List<CyBioReactorMono>();
        private readonly List<CyBioReactorMono> TempCache = new List<CyBioReactorMono>();

        private readonly int skips = ModConfig.Settings.RechargeSkipRate();
        private readonly float rechargePenalty = ModConfig.Settings.RechargePenalty();

        internal int PowerChargersCount => RenewablePowerChargers.Count + NonRenewablePowerChargers.Count;
        internal IEnumerable<ICyclopsCharger> PowerChargers
        {
            get
            {
                foreach (ICyclopsCharger charger in RenewablePowerChargers)
                    yield return charger;

                foreach (ICyclopsCharger charger in NonRenewablePowerChargers)
                    yield return charger;
            }
        }

        private readonly ICollection<ICyclopsCharger> RenewablePowerChargers = new HashSet<ICyclopsCharger>();
        private readonly ICollection<ICyclopsCharger> NonRenewablePowerChargers = new HashSet<ICyclopsCharger>();

        public ChargeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        internal bool Initialize(CyclopsManager manager)
        {
            Manager = manager;

            InitializeChargingHandlers();

            return true;
        }

        internal void InitializeChargingHandlers()
        {
            QuickLogger.Debug("PowerManager InitializeChargingHandlers");

            foreach (ChargerCreator method in CyclopsChargers)
            {
                ICyclopsCharger charger = method.Invoke(Cyclops);

                ICollection<ICyclopsCharger> powerChargers = charger.IsRenewable ? RenewablePowerChargers : NonRenewablePowerChargers;

                if (!powerChargers.Contains(charger))
                    powerChargers.Add(charger);
                else
                    QuickLogger.Warning($"Duplicate Reusable ICyclopsCharger '{charger.GetType()?.Name}' was blocked");
            }
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

        /// <summary>
        /// Recharges the cyclops' power cells using all charging modules across all upgrade consoles.
        /// </summary>
        internal void RechargeCyclops()
        {
            if (Time.timeScale == 0f) // Is the game paused?
                return;

            if (rechargeSkip < skips)
            {
                rechargeSkip++; // Slightly slows down recharging with more speed boosters and higher difficulty
                return;
            }

            rechargeSkip = 0;

            // When in Creative mode or using the NoPower cheat, inform the chargers that there is no power deficit.
            // This is so that each charger can decide what to do individually rather than skip the entire charging cycle all together.
            float powerDeficit = GameModeUtils.RequiresPower()
                                 ? Cyclops.powerRelay.GetMaxPower() - Cyclops.powerRelay.GetPower()
                                 : 0f;

            Manager.HUDManager.UpdateTextVisibility();

            float producedPower = 0f;
            foreach (ICyclopsCharger charger in RenewablePowerChargers)
                producedPower += charger.ProducePower(powerDeficit);

            // Charge with renewable energy first
            ChargeCyclops(producedPower, ref powerDeficit);

            if (producedPower <= MinimalPowerValue || (powerDeficit > NuclearModuleConfig.MinimumEnergyDeficit))
            {
                // If needed, produce and charge with non-renewable energy
                foreach (ICyclopsCharger charger in NonRenewablePowerChargers)
                    producedPower += charger.ProducePower(powerDeficit);

                ChargeCyclops(producedPower, ref powerDeficit);
            }
        }

        private void ChargeCyclops(float availablePower, ref float powerDeficit)
        {
            if (powerDeficit < MinimalPowerValue)
                return; // No need to charge

            if (availablePower < MinimalPowerValue)
                return; // No power available

            availablePower *= rechargePenalty;

            Cyclops.powerRelay.AddEnergy(availablePower, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - availablePower);
        }

        private void SetupChargingUpgrades(SubRoot cyclops1)
        {
            int maxChargingModules = ModConfig.Settings.MaxChargingModules();

            UpgradeManager.RegisterHandlerCreator((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: SolarCharger Upgrade");
                SolarCharger = new ChargingUpgradeHandler(CyclopsModule.SolarChargerID, cyclops)
                {
                    MaxCount = maxChargingModules
                };
                SolarCharger.OnFirstTimeMaxCountReached += () =>
                {
                    ErrorMessage.AddMessage(CyclopsModule.MaxSolarReached());
                };
                return SolarCharger;
            });

            UpgradeManager.RegisterHandlerCreator((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: SolarChargerMk2 Upgrade");
                SolarChargerMk2 = new BatteryUpgradeHandler(CyclopsModule.SolarChargerMk2ID, canRecharge: true, cyclops)
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

            UpgradeManager.RegisterHandlerCreator((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: ThermalCharger Upgrade");
                ThermalCharger = new ChargingUpgradeHandler(TechType.CyclopsThermalReactorModule, cyclops)
                {
                    MaxCount = maxChargingModules
                };
                ThermalCharger.OnFirstTimeMaxCountReached += () =>
                {
                    ErrorMessage.AddMessage(CyclopsModule.MaxThermalReached());
                };
                return ThermalCharger;
            });

            UpgradeManager.RegisterHandlerCreator((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: ThermalChargerMk2 Upgrade");
                ThermalChargerMk2 = new BatteryUpgradeHandler(CyclopsModule.ThermalChargerMk2ID, canRecharge: true, cyclops)
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

            UpgradeManager.RegisterHandlerCreator((SubRoot cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: NuclearReactor Upgrade");
                NuclearCharger = new NuclearUpgradeHandler(cyclops)
                {
                    MaxCount = Math.Min(maxChargingModules, 3) // No more than 3 no matter what the difficulty
                };
                return NuclearCharger;
            });

            UpgradeManager.RegisterHandlerCreator((cyclops) =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: BioBooster Upgrade");
                BioBoosters = new BioBoosterUpgradeHandler(cyclops);
                return BioBoosters;
            });
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
