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
        private readonly int reactorCount;
        private readonly bool maxedReactors;
        private int BoosterCount => upgradeHandler.Count;
        private bool MaxedBoosters => upgradeHandler.MaxLimitReached;

        public BoosterOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            chargeHandler = MCUServices.Find.CyclopsCharger<BioChargeHandler>(base.Cyclops);
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<BioBoosterUpgradeHandler>(base.Cyclops, base.TechType);
            cyclopsManager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(base.Cyclops);

            reactorCount = cyclopsManager.CyBioReactors.Count;
            maxedReactors = reactorCount == BioAuxCyclopsManager.MaxBioReactors;
        }

        public override void UpdateText()
        {
            UpperText.TextString = $"{(maxedReactors ? reactorCount.ToString() : "Max")} Bioreactor{(reactorCount != 1 ? "s" : string.Empty)}";
            UpperText.FontSize = 14;

            if (reactorCount > 0)
            {
                UpperText.TextColor = Color.white;
                MiddleText.TextString = chargeHandler.GetIndicatorText();
                MiddleText.TextColor = chargeHandler.GetIndicatorTextColor();
            }
            else
            {
                UpperText.TextColor = Color.red;
                MiddleText.TextString = string.Empty;
            }

            LowerText.TextString = $"{(this.MaxedBoosters ? this.BoosterCount.ToString() : "Max")} Booster{(this.BoosterCount != 1 ? "s" : string.Empty)}";
            LowerText.FontSize = 14;
        }
    }
}
