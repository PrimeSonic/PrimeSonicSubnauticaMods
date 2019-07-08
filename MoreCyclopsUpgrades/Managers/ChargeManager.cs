namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.Config;
    using UnityEngine;

    internal class ChargeManager
    {
        private class CyclopsCharger
        {
            public readonly CreateCyclopsCharger Creator;
            public readonly bool IsRenewable;
            public readonly string ChargerName;

            public CyclopsCharger(CreateCyclopsCharger creator, string chargerName, bool isRenewable)
            {
                Creator = creator;
                IsRenewable = isRenewable;
                ChargerName = chargerName;
            }
        }

        #region Static

        internal static bool Initialized { get; private set; }
        internal const float BatteryDrainRate = 0.01f;
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        internal const float Mk2ChargeRateModifier = 1.10f; // The MK2 charging modules get a 15% bonus to their charge rate.

        private static readonly List<CyclopsCharger> CyclopsChargers = new List<CyclopsCharger>();

        internal static void RegisterChargerCreator(CreateCyclopsCharger createEvent, string name, bool isRenewable)
        {
            if (CyclopsChargers.Find(c => c.Creator == createEvent) != null)
            {
                QuickLogger.Warning($"Duplicate CyclopsChargerCreator '{name}' was blocked");
                return;
            }

            QuickLogger.Info($"Received CyclopsChargerCreator '{name}'");
            CyclopsChargers.Add(new CyclopsCharger(createEvent, name, isRenewable));
        }

        #endregion

        private readonly IDictionary<string, ICyclopsCharger> KnownChargers = new Dictionary<string, ICyclopsCharger>();
        private readonly ICollection<ICyclopsCharger> RenewablePowerChargers = new List<ICyclopsCharger>();
        private readonly ICollection<ICyclopsCharger> NonRenewablePowerChargers = new List<ICyclopsCharger>();

        private readonly SubRoot Cyclops;
        private float rechargePenalty = ModConfig.Main.RechargePenalty;
        private bool requiresVanillaCharging = false;

        private CyclopsHUDManager cyclopsHUDManager;
        private CyclopsHUDManager HUDManager => cyclopsHUDManager ?? (cyclopsHUDManager = CyclopsManager.GetManager(Cyclops)?.HUD);

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

        public ChargeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        internal T GetCharger<T>(string chargeHandlerName) where T : class, ICyclopsCharger
        {
            if (KnownChargers.TryGetValue(chargeHandlerName, out ICyclopsCharger cyclopsCharger))
            {
                return (T)cyclopsCharger;
            }

            return null;
        }

        public void InitializeChargers()
        {
            QuickLogger.Debug("ChargeManager InitializeChargingHandlers");

            // First, register chargers from other mods.
            foreach (CyclopsCharger chargerTemplate in CyclopsChargers)
            {
                QuickLogger.Debug($"ChargeManager creating charger '{chargerTemplate.ChargerName}'");
                ICyclopsCharger charger = chargerTemplate.Creator.Invoke(Cyclops);

                ICollection<ICyclopsCharger> powerChargers = chargerTemplate.IsRenewable ? RenewablePowerChargers : NonRenewablePowerChargers;

                if (charger == null)
                {
                    QuickLogger.Warning($"CyclopsCharger '{chargerTemplate.ChargerName}' was null");
                }
                else if (!KnownChargers.ContainsKey(chargerTemplate.ChargerName))
                {
                    powerChargers.Add(charger);
                    KnownChargers.Add(chargerTemplate.ChargerName, charger);
                    QuickLogger.Debug($"Created CyclopsCharger '{chargerTemplate.ChargerName}'");
                }
                else
                {
                    QuickLogger.Warning($"Duplicate CyclopsCharger '{chargerTemplate.ChargerName}' was blocked");
                }
            }

            // Next, check if an external mod has a different upgrade handler for the original CyclopsThermalReactorModule.
            // If not, then the original thermal charging code will be allowed to run.
            // This is to allow players to choose whether or not they want the newer form of charging.
            requiresVanillaCharging = VanillaUpgrades.Main.IsUsingVanillaUpgrade(TechType.CyclopsThermalReactorModule);

            Initialized = true;
        }

        internal void UpdateRechargePenalty(float penalty)
        {
            rechargePenalty = penalty;
        }

        /// <summary>
        /// Gets the total available reserve power across all equipment upgrade modules.
        /// </summary>
        /// <returns>The <see cref="int"/> value of the total available reserve power.</returns>
        internal int GetTotalReservePower()
        {
            float availableReservePower = 0f;

            foreach (ICyclopsCharger charger in KnownChargers.Values)
                availableReservePower += charger.TotalReservePower();

            return Mathf.FloorToInt(availableReservePower);
        }

        /// <summary>
        /// Recharges the cyclops' power cells using all charging modules across all upgrade consoles.
        /// </summary>
        /// <returns><c>True</c> if the original code for the vanilla Cyclops Thermal Reactor Module is required; Otherwise <c>false</c>.</returns>
        internal bool RechargeCyclops()
        {
            if (Time.timeScale == 0f) // Is the game paused?
                return false;

            // When in Creative mode or using the NoPower cheat, inform the chargers that there is no power deficit.
            // This is so that each charger can decide what to do individually rather than skip the entire charging cycle all together.
            float powerDeficit = GameModeUtils.RequiresPower()
                                 ? Cyclops.powerRelay.GetMaxPower() - Cyclops.powerRelay.GetPower()
                                 : 0f;

            this.HUDManager.UpdateTextVisibility();

            float producedPower = 0f;

            // Produce power from renewable energy first
            foreach (ICyclopsCharger charger in RenewablePowerChargers)
                producedPower += charger.ProducePower(powerDeficit);

            float alternatePowerDeficit = 0f;
            if (producedPower < MinimalPowerValue && // Did the renewable energy sources not produce any power?
                powerDeficit < ModConfig.Main.MinimumEnergyDeficit) // Is the power deficit over the threshhold to start consuming non-renewable energy?
            {
                // Start producing power from non-renewable energy
                alternatePowerDeficit = powerDeficit;
            }

            foreach (ICyclopsCharger charger in NonRenewablePowerChargers)
                producedPower += charger.ProducePower(powerDeficit);

            ChargeCyclops(producedPower, ref powerDeficit);

            return requiresVanillaCharging;
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
    }
}
