namespace MoreCyclopsUpgrades.OriginalUpgrades
{
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class OriginalEngineUpgrade : UpgradeHandler
    {
        private float lastKnownRating = -1f;

        public OriginalEngineUpgrade(SubRoot cyclops) : base(TechType.PowerUpgradeModule, cyclops)
        {
            OnClearUpgrades = () =>
            {
                lastKnownRating = cyclops.currPowerRating;
                cyclops.currPowerRating = 1f;
            };

            OnUpgradeCountedDetailed = (Equipment modules, string slot, InventoryItem inventoryItem) =>
            {
                cyclops.currPowerRating = 3f;
            };

            OnFinishedWithUpgrades = () => Announcement();
            OnFinishedWithoutUpgrades = () => Announcement();
        }

        private void Announcement()
        {
            if (lastKnownRating != cyclops.currPowerRating)
            {
                // Inform the new power rating just like the original method would.
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", cyclops.currPowerRating));
            }
        }
    }
}
