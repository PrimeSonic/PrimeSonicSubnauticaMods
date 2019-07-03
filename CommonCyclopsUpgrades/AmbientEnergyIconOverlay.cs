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
        private readonly Battery battery;

        public AmbientEnergyIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<T>(base.Cyclops, base.TechType);
            charger = MCUServices.Find.CyclopsCharger<AmbientEnergyCharger<T>>(base.Cyclops);
            battery = base.Item.item.GetComponent<Battery>();
        }

        public override void UpdateText()
        {
            if (upgradeHandler.TotalCount > 1)
                base.UpperText.TextString = $"+{Mathf.RoundToInt((upgradeHandler.ChargeMultiplier - 1f) * 100f)}%";
            else
                base.UpperText.TextString = string.Empty;

            base.MiddleText.TextString = $"{charger.EnergyStatusText()}";
            base.MiddleText.TextColor = charger.GetIndicatorTextColor();

            if (battery != null)
            {
                base.LowerText.TextString = NumberFormatter.FormatValue(battery._charge);
                base.LowerText.TextColor = NumberFormatter.GetNumberColor(battery._charge, battery._capacity, 0f);
            }
        }
    }
}
