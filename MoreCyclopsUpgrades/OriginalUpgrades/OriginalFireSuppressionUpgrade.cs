namespace MoreCyclopsUpgrades.OriginalUpgrades
{
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class OriginalFireSuppressionUpgrade : UpgradeHandler
    {
        private readonly CyclopsHolographicHUD cyclopsHoloHUD;

        public OriginalFireSuppressionUpgrade(SubRoot cyclops) : base(TechType.CyclopsFireSuppressionModule, cyclops)
        {
            cyclopsHoloHUD = cyclops.GetComponentInChildren<CyclopsHolographicHUD>();

            OnClearUpgrades = () =>
            {
                cyclopsHoloHUD?.fireSuppressionSystem.SetActive(false);
            };
            OnUpgradeCounted = () =>
            {
                cyclopsHoloHUD?.fireSuppressionSystem.SetActive(true);
            };
        }
    }
}
