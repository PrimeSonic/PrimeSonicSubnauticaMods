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

        public BoosterOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            chargeHandler = MCUServices.Find.CyclopsCharger<BioChargeHandler>(base.Cyclops);
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<BioBoosterUpgradeHandler>(base.Cyclops, base.TechType);
            cyclopsManager = MCUServices.Find.AuxCyclopsManager<BioAuxCyclopsManager>(base.Cyclops);

            reactorCount = cyclopsManager.TrackedBuildablesCount;
            maxedReactors = reactorCount == BioChargeHandler.MaxBioReactors;
        }

        public override void UpdateText()
        {
            base.UpperText.TextString = $"{(maxedReactors ? "Max" : reactorCount.ToString())} Bioreactor{(reactorCount != 1 ? "s" : string.Empty)}";
            base.UpperText.FontSize = 14;

            int boosters = upgradeHandler.Count;
            base.MiddleText.TextString = $"{(upgradeHandler.MaxLimitReached ? "Max" : boosters.ToString())} Booster{(boosters != 1 ? "s" : string.Empty)}";
            base.MiddleText.FontSize = 14;

            if (reactorCount > 0)
            {
                base.UpperText.TextColor = Color.white;
                base.LowerText.TextString = chargeHandler.StatusText();
                base.LowerText.TextColor = chargeHandler.StatusTextColor();
            }
            else
            {
                base.UpperText.TextColor = Color.red;
                base.LowerText.TextString = string.Empty;
            }
        }
    }
}
