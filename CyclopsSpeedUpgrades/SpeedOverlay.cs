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
            base.UpperText.TextString = $"{(this.MaxedBoosters ? "Max" : this.BoosterCount.ToString())} Booster{(this.BoosterCount != 1 ? "s" : string.Empty)}";
            base.UpperText.FontSize = 14;

            base.MiddleText.TextString = $"Speed {Mathf.CeilToInt(speedHandler.SpeedMultiplier * 100f)}%";
            base.MiddleText.FontSize = 16;

            base.LowerText.TextString = $"Engine -{Mathf.FloorToInt((1f - speedHandler.EfficiencyPenalty) * 100f)}%\n" +
                                        $"Noise +{Mathf.FloorToInt((speedHandler.NoisePenalty - 1f) * 100f)}%";
            base.LowerText.FontSize = 14;
        }
    }
}
