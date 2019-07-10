namespace MoreCyclopsUpgrades.Managers
{
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.Config;
    using UnityEngine;

    internal class ChargeManager : IChargeManager
    {
        private class ChargerCreator
        {
            public readonly CreateCyclopsCharger Creator;
            public readonly string ChargerName;

            public ChargerCreator(CreateCyclopsCharger creator, string chargerName)
            {
                Creator = creator;
                ChargerName = chargerName;
            }
        }

        #region Static

        internal static bool Initialized { get; private set; }
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;

        private static readonly List<ChargerCreator> CyclopsChargerCreators = new List<ChargerCreator>();

        internal static void RegisterChargerCreator(CreateCyclopsCharger createEvent, string name)
        {
            if (CyclopsChargerCreators.Find(c => c.Creator == createEvent || c.ChargerName == name) != null)
            {
                QuickLogger.Warning($"Duplicate CyclopsChargerCreator '{name}' was blocked");
                return;
            }

            QuickLogger.Info($"Received CyclopsChargerCreator '{name}'");
            CyclopsChargerCreators.Add(new ChargerCreator(createEvent, name));
        }

        #endregion

        private readonly IDictionary<string, CyclopsCharger> KnownChargers = new Dictionary<string, CyclopsCharger>();

        private readonly SubRoot Cyclops;
        private float rechargePenalty = ModConfig.Main.RechargePenalty;
        private readonly IModConfig config = ModConfig.Main;
        private bool requiresVanillaCharging = false;
        private float producedPower = 0f;
        float powerDeficit = 0f;

        public ICollection<CyclopsCharger> Chargers => KnownChargers.Values;

        public ChargeManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        internal T GetCharger<T>(string chargeHandlerName) where T : CyclopsCharger
        {
            if (KnownChargers.TryGetValue(chargeHandlerName, out CyclopsCharger cyclopsCharger))
            {
                return (T)cyclopsCharger;
            }

            return null;
        }

        public void InitializeChargers()
        {
            QuickLogger.Debug("ChargeManager InitializeChargingHandlers");

            // First, register chargers from other mods.
            foreach (ChargerCreator chargerTemplate in CyclopsChargerCreators)
            {
                QuickLogger.Debug($"ChargeManager creating charger '{chargerTemplate.ChargerName}'");
                CyclopsCharger charger = chargerTemplate.Creator.Invoke(Cyclops);

                if (charger == null)
                {
                    QuickLogger.Warning($"CyclopsCharger '{chargerTemplate.ChargerName}' was null");
                }
                else if (!KnownChargers.ContainsKey(chargerTemplate.ChargerName))
                {
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
        public int GetTotalReservePower()
        {
            float availableReservePower = 0f;

            foreach (CyclopsCharger charger in KnownChargers.Values)
                availableReservePower += charger.TotalReserveEnergy;

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
            powerDeficit = GameModeUtils.RequiresPower()
                            ? Cyclops.powerRelay.GetMaxPower() - Cyclops.powerRelay.GetPower()
                            : 0f;

            producedPower = 0f;

            foreach (ICyclopsCharger charger in KnownChargers.Values)
            {
                // Get renewable energy first
                producedPower += charger.Generate(powerDeficit);
            }

            if (producedPower < MinimalPowerValue && // Did the renewable energy sources not produce any power?
                powerDeficit < config.MinimumEnergyDeficit) // Is the power deficit over the threshhold to start consuming non-renewable energy?
            {
                // Get non-renewable energy if no renewable energy was available
                foreach (ICyclopsCharger charger in KnownChargers.Values)
                {
                    producedPower += charger.Drain(powerDeficit);

                    if (producedPower > powerDeficit)
                        break;
                }
            }

            if (producedPower > MinimalPowerValue)
            {
                producedPower *= rechargePenalty;

                Cyclops.powerRelay.ModifyPower(producedPower, out float amountStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - producedPower);
            }

            return requiresVanillaCharging;
        }
    }
}
