namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using MoreCyclopsUpgrades.CyclopsUpgrades.CyclopsCharging;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.SaveData;
    using UnityEngine;

    internal class ChargeManager : IAuxCyclopsManager
    {
        internal const string ManagerName = "McuChargeMgr";

        internal const float BatteryDrainRate = 0.01f;
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        internal const float Mk2ChargeRateModifier = 1.15f; // The MK2 charging modules get a 15% bonus to their charge rate.
        
        private static readonly ICollection<ChargerCreator> CyclopsChargers = new List<ChargerCreator>();

        /// <summary>
        /// Registers a <see cref="ChargerCreator"/> method that creates returns a new <see cref="ICyclopsCharger"/> on demand and is only used once.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="ChargerCreator"/>.</param>
        internal static void RegisterChargerCreator(ChargerCreator createEvent, string assemblyName)
        {
            if (CyclopsChargers.Contains(createEvent))
            {
                QuickLogger.Warning($"Duplicate ChargerCreator blocked from {assemblyName}");
                return;
            }

            QuickLogger.Info($"Received ChargerCreator from {assemblyName}");
            CyclopsChargers.Add(createEvent);
        }

        internal readonly SubRoot Cyclops;

        internal ThermalChargeHandler ThermalCharging;

        internal ChargingUpgradeHandler ThermalCharger;
        internal BatteryUpgradeHandler ThermalChargerMk2;

        private int rechargeSkip = 10;

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

        public string Name { get; } = ManagerName;

        internal readonly IDictionary<string, ICyclopsCharger> KnownChargers = new Dictionary<string, ICyclopsCharger>();

        private readonly ICollection<ICyclopsCharger> RenewablePowerChargers = new List<ICyclopsCharger>();
        private readonly ICollection<ICyclopsCharger> NonRenewablePowerChargers = new List<ICyclopsCharger>();

        public ChargeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        public bool Initialize(SubRoot cyclops)
        {
            QuickLogger.Debug("PowerManager InitializeChargingHandlers");

            foreach (ChargerCreator method in CyclopsChargers)
            {
                ICyclopsCharger charger = method.Invoke(Cyclops);

                ICollection<ICyclopsCharger> powerChargers = charger.IsRenewable ? RenewablePowerChargers : NonRenewablePowerChargers;

                if (!powerChargers.Contains(charger))
                {
                    powerChargers.Add(charger);
                    KnownChargers.Add(charger.Name, charger);
                }
                else
                {
                    QuickLogger.Warning($"Duplicate Reusable ICyclopsCharger '{charger.GetType()?.Name}' was blocked");
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the total available reserve power across all equipment upgrade modules.
        /// </summary>
        /// <returns>The <see cref="int"/> value of the total available reserve power.</returns>
        internal int GetTotalReservePower()
        {
            float availableReservePower = 0f;

            foreach (ICyclopsCharger charger in RenewablePowerChargers)
                availableReservePower += charger.TotalReservePower();

            foreach (ICyclopsCharger charger in NonRenewablePowerChargers)
                availableReservePower += charger.TotalReservePower();

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

            // TODO
            //HUDManager.UpdateTextVisibility();

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

            MCUServices.Client.RegisterHandlerCreator((SubRoot cyclops) =>
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

            MCUServices.Client.RegisterHandlerCreator((SubRoot cyclops) =>
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


        }
    }
}
