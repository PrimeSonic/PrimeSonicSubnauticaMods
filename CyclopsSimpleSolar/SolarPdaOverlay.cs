namespace CyclopsSimpleSolar
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class SolarPdaOverlay : IconOverlay
    {
        private readonly CySolarChargeManager cySolarChargeManager;

        private readonly string crossModText;

        public SolarPdaOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            cySolarChargeManager = MCUServices.Find.CyclopsCharger<CySolarChargeManager>(base.Cyclops);
            crossModText = Language.main.Get(MainPatcher.CrossModKey);
        }

        public override void UpdateText()
        {
            if (cySolarChargeManager.SolarEnergyAvailable)
            {
                base.MiddleText.FontSize = 16;
                base.MiddleText.TextColor = cySolarChargeManager.StatusTextColor();
                base.MiddleText.TextString = cySolarChargeManager.StatusText();
            }
            else if (cySolarChargeManager.OtherCySolarModsPresent &&
                     cySolarChargeManager.OtherSolarChargerModsEquipped)
            {
                base.MiddleText.FontSize = 12;
                base.MiddleText.TextColor = Color.red;
                base.MiddleText.TextString = crossModText;
            }
            else
            {
                base.MiddleText.TextString = string.Empty;
            }
        }
    }
}
