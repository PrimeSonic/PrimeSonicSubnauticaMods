namespace CommonCyclopsUpgrades
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class AmbientEnergyIconOverlay<HandlerType, ChargerType> : IconOverlay
        where HandlerType : AmbientEnergyUpgradeHandler
        where ChargerType : AmbientEnergyCharger<HandlerType>, ICyclopsCharger
    {
        private readonly HandlerType upgradeHandler;
        private readonly ChargerType charger;
        private readonly Battery battery;

        public AmbientEnergyIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<HandlerType>(base.Cyclops, base.TechType);
            charger = MCUServices.Find.CyclopsCharger<ChargerType>(base.Cyclops);
            battery = base.Item.item.GetComponent<Battery>();
        }

        public override void UpdateText()
        {
            int count = upgradeHandler.Count;
            UpperText.TextString = $"{(upgradeHandler.MaxLimitReached ? "Max" : count.ToString())} Charger{(count != 1 ? "s" : string.Empty)}";
            UpperText.FontSize = 16;

            if (upgradeHandler.ChargeMultiplier > 1f)
                base.MiddleText.TextString = $"{charger.EnergyStatusText()}\n+{Mathf.CeilToInt((upgradeHandler.ChargeMultiplier - 1f) * 100f)}%";
            else
                base.MiddleText.TextString = $"{charger.EnergyStatusText()}";

            base.MiddleText.FontSize = 16;

            if (battery != null)
            {
                base.LowerText.TextString = NumberFormatter.FormatValue(battery._charge);
                base.LowerText.TextColor = NumberFormatter.GetNumberColor(battery._charge, battery._capacity, 0f);
            }
        }
    }
}
