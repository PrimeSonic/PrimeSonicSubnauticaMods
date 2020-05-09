namespace CyclopsNuclearUpgrades.Management
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class NuclearIconOverlay : IconOverlay
    {
        private readonly NuclearChargeHandler chargeHandler;
        private readonly Battery battery;

        public NuclearIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            chargeHandler = MCUServices.Find.CyclopsCharger<NuclearChargeHandler>(base.Cyclops);
            battery = base.Item.item.GetComponent<Battery>();
        }

        public override void UpdateText()
        {
            float displayTemperature = Mathf.Max(chargeHandler.HeatLevel, 24f);

            if (chargeHandler.IsOverheated)
            {
                base.UpperText.TextString = CyclopsNuclearModule.OverheatMsg;
                base.UpperText.FontSize = 14;
            }
            else
            {
                base.UpperText.TextString = NumberFormatter.FormatValue(displayTemperature) + "°C";
                base.UpperText.FontSize = 20;
            }

            base.UpperText.TextColor = chargeHandler.StatusTextColor();

            if (battery != null)
            {
                base.LowerText.TextString = NumberFormatter.FormatValue(battery._charge);
                base.MiddleText.TextColor = NumberFormatter.GetNumberColor(battery._charge, CyclopsNuclearModule.NuclearEnergyPotential, 0f);
            }
        }
    }
}
