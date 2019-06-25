namespace CyclopsNuclearUpgrades.Management
{
    using CommonCyclopsUpgrades;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class NuclearIconOverlay : IconOverlay
    {
        private readonly NuclearUpgradeHandler upgradeHandler;
        private readonly NuclearChargeHandler chargeHandler;

        public NuclearIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<NuclearUpgradeHandler>(base.cyclops, base.techType);
            chargeHandler = MCUServices.Find.CyclopsCharger<NuclearChargeHandler>(base.cyclops, NuclearChargeHandler.ChargerName);
        }

        public override void UpdateText()
        {
            const float adjustedMaxTemp = NuclearChargeHandler.MaxHeat / 500f;
            float temperature = WaterTemperatureSimulation.main.GetTemperature(base.cyclops.transform.position);
            float displayTemperature = Mathf.Max(chargeHandler.HeatLevel / 500f, temperature);

            base.middleText.TextString = $"{displayTemperature}°C";
            base.middleText.TextColor = NumberFormatter.GetNumberColor(displayTemperature, adjustedMaxTemp, temperature);

            if (chargeHandler.IsOverheated)
            {
                base.lowerText.TextString = "SHUTDOWN";
                base.lowerText.TextColor = Color.red;
            }
            else
            {
                base.lowerText.TextString = string.Empty;
            }

        }
    }
}
