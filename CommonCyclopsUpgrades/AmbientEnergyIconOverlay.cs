﻿namespace CommonCyclopsUpgrades
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class AmbientEnergyIconOverlay<T> : IconOverlay
        where T : AmbientEnergyUpgradeHandler
    {
        private readonly T upgradeHandler;
        private readonly AmbientEnergyCharger<T> charger;

        public AmbientEnergyIconOverlay(string chargerName, uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<T>(base.Cyclops, base.TechType);
            charger = MCUServices.Find.CyclopsCharger<AmbientEnergyCharger<T>>(base.Cyclops, chargerName);
        }

        public override void UpdateText()
        {
            if (upgradeHandler.TotalCount > 1)
                base.UpperText.TextString = $"+{Mathf.RoundToInt((upgradeHandler.ChargeMultiplier - 1f) * 100f)}%";
            else
                base.UpperText.TextString = string.Empty;

            if (upgradeHandler.TierCount(upgradeHandler.Tier2ID) > 0)
            {
                base.MiddleText.TextString = $"{charger.EnergyStatusText()}\n{charger.ReservePowerText()}";
            }
            else if (upgradeHandler.TierCount(upgradeHandler.Tier1ID) > 0)
            {
                base.MiddleText.TextString = $"{charger.EnergyStatusText()}";
            }
            
            base.MiddleText.TextColor = charger.GetIndicatorTextColor();

            base.LowerText.TextString = $"{upgradeHandler.TotalCount}/{upgradeHandler.MaxCount}";
        }
    }
}