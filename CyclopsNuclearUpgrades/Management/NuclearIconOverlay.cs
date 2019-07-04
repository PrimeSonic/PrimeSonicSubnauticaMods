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
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<NuclearUpgradeHandler>(base.Cyclops, base.TechType);
            chargeHandler = MCUServices.Find.CyclopsCharger<NuclearChargeHandler>(base.Cyclops);
            battery = base.Item.item.GetComponent<Battery>();
        }

        public override void UpdateText()
        {
            float cyclopsHeat = WaterTemperatureSimulation.main.GetTemperature(base.Cyclops.transform.position);
            float displayTemperature = Mathf.Max(chargeHandler.HeatLevel, cyclopsHeat);

            if (chargeHandler.IsOverheated)
            {
                base.UpperText.TextString = "SHUTDOWN";
            }
            else
            {
                base.UpperText.TextString = NumberFormatter.FormatValue(displayTemperature) + "°C";
            }

            base.UpperText.TextColor = NumberFormatter.GetNumberColor(displayTemperature, NuclearChargeHandler.MaxHeat, cyclopsHeat - 1f);

            if (battery != null)
            {
                base.LowerText.TextString = NumberFormatter.FormatValue(battery._charge);
                base.MiddleText.TextColor = NumberFormatter.GetNumberColor(battery._charge, CyclopsNuclearModule.NuclearEnergyPotential, 0f);
            }
        }
    }
}
