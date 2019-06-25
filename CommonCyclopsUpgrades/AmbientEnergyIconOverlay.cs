namespace CommonCyclopsUpgrades
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
            upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<T>(base.cyclops, base.techType);
            charger = MCUServices.Find.CyclopsCharger<AmbientEnergyCharger<T>>(base.cyclops, chargerName);
        }

        public override void UpdateText()
        {
            if (upgradeHandler.TotalCount > 0)
                base.upperText.TextString = $"+{Mathf.RoundToInt((upgradeHandler.ChargeMultiplier - 1f) * 100f)}%";
            else
                base.upperText.TextString = string.Empty;

            base.middleText.TextString = $"{charger.EnergyStatusPercent()}";
            base.middleText.TextColor = charger.GetIndicatorTextColor();

            base.lowerText.TextString = $"{upgradeHandler.TotalCount}/{upgradeHandler.MaxCount}";
        }
    }
}
