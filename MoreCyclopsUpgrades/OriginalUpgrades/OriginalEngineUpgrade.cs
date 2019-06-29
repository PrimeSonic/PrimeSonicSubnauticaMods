namespace MoreCyclopsUpgrades.OriginalUpgrades
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class OriginalEngineUpgrade : UpgradeHandler
    {
        private float lastKnownRating = -1f;

        public OriginalEngineUpgrade(SubRoot cyclops) : base(TechType.PowerUpgradeModule, cyclops)
        {
            OnClearUpgrades = () =>
            {
                lastKnownRating = cyclops.currPowerRating;
                MCUServices.CrossMod.ApplyPowerRatingModifier(cyclops, TechType.PowerUpgradeModule, 1f);
            };

            OnUpgradeCountedDetailed = (Equipment modules, string slot, InventoryItem inventoryItem) =>
            {
                MCUServices.CrossMod.ApplyPowerRatingModifier(cyclops, TechType.PowerUpgradeModule, 3f);
            };

            OnFinishedUpgrades = () => Announcement();
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
