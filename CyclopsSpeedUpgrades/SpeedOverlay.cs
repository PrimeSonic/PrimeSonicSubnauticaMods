namespace CyclopsSpeedUpgrades
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class SpeedOverlay : IconOverlay
    {
        private readonly SpeedHandler speedHandler;
        private int BoosterCount => speedHandler.Count;
        private bool MaxedBoosters => speedHandler.MaxLimitReached;

        public SpeedOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule, CyclopsSpeedModule speedBooster) : base(icon, upgradeModule)
        {
            speedHandler = MCUServices.Find.CyclopsUpgradeHandler<SpeedHandler>(base.Cyclops, speedBooster.TechType);
        }

        public override void UpdateText()
        {
            base.UpperText.TextString = $"Speed +{Mathf.CeilToInt((speedHandler.SpeedMultiplier - 1f) * 100f)}";

            base.MiddleText.TextString = $"Engine -{Mathf.FloorToInt((speedHandler.EfficiencyPenalty - 1f) * 100f)}\n" +
                                         $"Noise +{Mathf.FloorToInt((speedHandler.NoisePenalty - 1f) * 100f)}";
            base.MiddleText.FontSize = 16;

            base.LowerText.TextString = $"{(this.MaxedBoosters ? this.BoosterCount.ToString() : "Max")} Booster{(this.BoosterCount != 1 ? "s" : string.Empty)}";
            base.LowerText.FontSize = 14;
        }
    }
}
