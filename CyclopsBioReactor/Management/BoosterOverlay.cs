namespace CyclopsBioReactor.Management
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class BoosterOverlay : IconOverlay
    {
        private readonly BioBoosterUpgradeHandler upgradeHandler;
        private readonly BioChargeHandler chargeHandler;
        private readonly BioAuxCyclopsManager cyclopsManager;

        public BoosterOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            chargeHandler = MCUServices.Find.CyclopsCharger<BioChargeHandler>(base.Cyclops, BioChargeHandler.ChargerName);
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<BioBoosterUpgradeHandler>(base.Cyclops, base.TechType);
            cyclopsManager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(base.Cyclops, BioAuxCyclopsManager.ManagerName);
        }

        public override void UpdateText()
        {
            UpperText.TextString = $"Bioreactors[{cyclopsManager.CyBioReactors.Count}/{BioAuxCyclopsManager.MaxBioReactors}]";

            if (cyclopsManager.CyBioReactors.Count > 0)
            {                
                MiddleText.TextString = chargeHandler.GetIndicatorText();
            }
            else
            {
                UpperText.TextColor = Color.red;
            }

            LowerText.TextString = $"Boosters[{upgradeHandler.Count}/{upgradeHandler.MaxCount}]";
        }
    }
}
