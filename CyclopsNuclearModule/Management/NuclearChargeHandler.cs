namespace CyclopsNuclearUpgrades.Management
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using UnityEngine;

    internal class NuclearChargeHandler : ICyclopsCharger
    {
        internal const string ChargerName = "CyNukeChgr";
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float MaxNuclearChargeRate = 0.16f;
        private const float MinNuclearChargeRate = MinimalPowerValue * 2;
        private const float CooldownRate = MaxNuclearChargeRate * 6f;
        private const float MaxHeat = 1500f;

        private readonly Atlas.Sprite sprite;
        private readonly NuclearUpgradeHandler upgradeHandler;
        private float heat = 0f;
        private float chargeRate = MinNuclearChargeRate;
        private NuclearState nuclearState = NuclearState.None;

        public NuclearChargeHandler(SubRoot cyclops, TechType nuclearModule)
        {
            sprite = SpriteManager.Get(nuclearModule);
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<NuclearUpgradeHandler>(cyclops, nuclearModule);
        }

        public bool IsRenewable { get; } = false;
        public string Name { get; } = ChargerName;

        public Atlas.Sprite GetIndicatorSprite()
        {
            return sprite;
        }

        public string GetIndicatorText()
        {
            return FormatNumber(upgradeHandler.TotalBatteryCharge);
        }

        public Color GetIndicatorTextColor()
        {
            // Use color to inform heat levels
            return GetNumberColor(MaxHeat - heat, MaxHeat, 0f);
        }

        public bool HasPowerIndicatorInfo()
        {
            return nuclearState == NuclearState.NuclearPowerEngaged;
        }

        public float ProducePower(float requestedPower)
        {
            if (nuclearState != NuclearState.NuclearPowerEngaged && heat > 0f)
            {
                chargeRate = MinNuclearChargeRate;
                heat -= CooldownRate; // Cooldown
            }

            if (upgradeHandler.TotalBatteryCharge <= MinimalPowerValue)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                nuclearState = NuclearState.None;
                return 0f;
            }
            else if (heat >= MaxHeat)
            {
                chargeRate = Mathf.Max(MinNuclearChargeRate, chargeRate - MinNuclearChargeRate);
                nuclearState = NuclearState.Overheated;
                return 0f;
            }
            else if (nuclearState == NuclearState.Overheated)
            {
                if (heat <= 0) // Do not allow nuclear power to charge again until heat has returned to zero
                    nuclearState = NuclearState.None;

                return 0f;
            }
            else
            {
                nuclearState = NuclearState.NuclearPowerEngaged;

                chargeRate = Mathf.Min(MaxNuclearChargeRate, chargeRate + MinNuclearChargeRate);

                float generatedPower = upgradeHandler.GetBatteryPower(chargeRate, requestedPower);

                heat += generatedPower;
                return generatedPower;
            }
        }

        public float TotalReservePower()
        {
            return upgradeHandler.TotalBatteryCharge;
        }

        internal static Color GetNumberColor(float value, float max, float min)
        {
            if (value > max)
                return Color.white;

            if (value <= min)
                return Color.red;

            const float greenHue = 120f / 360f;
            float percentOfMax = (value - min) / (max - min);

            const float saturation = 1f;
            const float lightness = 0.8f;

            return Color.HSVToRGB(percentOfMax * greenHue, saturation, lightness);
        }

        private static readonly IDictionary<int, string> _formattedAmountCache = new Dictionary<int, string>();

        internal static string FormatNumber(float totalCharge)
        {
            int value = Mathf.CeilToInt(totalCharge);
            if (!_formattedAmountCache.TryGetValue(value, out string amountString))
            {
                amountString = $"{HandleLargeNumbers(value)}";
                _formattedAmountCache.Add(value, amountString);
            }
            return amountString;
        }

        private static string HandleLargeNumbers(int possiblyLargeValue)
        {
            if (possiblyLargeValue > 9999999)
            {
                return $"{possiblyLargeValue / 1000000f:F1}M";
            }

            if (possiblyLargeValue > 9999)
            {
                return $"{possiblyLargeValue / 1000f:F1}K";
            }

            return $"{Mathf.CeilToInt(possiblyLargeValue)}";
        }
    }
}
