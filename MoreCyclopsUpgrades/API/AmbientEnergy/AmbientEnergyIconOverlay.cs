namespace MoreCyclopsUpgrades.API.AmbientEnergy
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    /// <summary>
    /// A standarized <see cref="IconOverlay"/> implementation for <see cref="AmbientEnergyCharger{T}"/>s.
    /// </summary>
    /// <typeparam name="HandlerType">The type of the andler type.</typeparam>
    /// <typeparam name="ChargerType">The type of the harger type.</typeparam>
    /// <seealso cref="IconOverlay" />
    public class AmbientEnergyIconOverlay<HandlerType, ChargerType> : IconOverlay
        where HandlerType : AmbientEnergyUpgradeHandler
        where ChargerType : AmbientEnergyCharger<HandlerType>
    {
        private readonly HandlerType upgradeHandler;
        private readonly ChargerType charger;
        private readonly Battery battery;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientEnergyIconOverlay{HandlerType, ChargerType}"/> class.
        /// </summary>
        /// <param name="icon">The PDA icon.</param>
        /// <param name="upgradeModule">The upgrade module item.</param>
        public AmbientEnergyIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            upgradeHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<HandlerType>(base.Cyclops, base.TechType);
            charger = MCUServices.Find.CyclopsCharger<ChargerType>(base.Cyclops);
            battery = base.Item.item.GetComponent<Battery>();
        }

        /// <summary>
        /// Update the values in <see cref="IconOverlay.UpperText" />, <see cref="IconOverlay.MiddleText" />, and <see cref="IconOverlay.LowerText" /> in this method.
        /// </summary>
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
