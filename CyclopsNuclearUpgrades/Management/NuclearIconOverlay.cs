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
        private readonly Battery battery;

        public NuclearIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<NuclearUpgradeHandler>(base.cyclops, base.techType);
            chargeHandler = MCUServices.Find.CyclopsCharger<NuclearChargeHandler>(base.cyclops, NuclearChargeHandler.ChargerName);
            battery = base.module.item.GetComponent<Battery>();
        }

        public override void UpdateText()
        {
            if (chargeHandler.IsOverheated)
            {
                base.upperText.TextString = "SHUTDOWN";
                base.upperText.TextColor = Color.red;
            }

            const float adjustedMaxTemp = NuclearChargeHandler.MaxHeat / 500f;
            float temperature = WaterTemperatureSimulation.main.GetTemperature(base.cyclops.transform.position);
            float displayTemperature = Mathf.Max(chargeHandler.HeatLevel / 500f, temperature);

            base.middleText.TextString = NumberFormatter.FormatValue(displayTemperature) + "°C";
            base.middleText.TextColor = NumberFormatter.GetNumberColor(displayTemperature, adjustedMaxTemp, temperature - 1f);

            if (battery != null)
            {
                base.lowerText.TextString = NumberFormatter.FormatValue(battery._charge);
                base.middleText.TextColor = NumberFormatter.GetNumberColor(battery._charge, CyclopsNuclearModule.NuclearEnergyPotential, 0f);
            }            
        }
    }
}
