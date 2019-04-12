namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    internal class ChargingUpgradeHandler : UpgradeHandler
    {
        internal ChargingUpgradeHandler SiblingUpgrade = null;

        public ChargingUpgradeHandler(TechType techType) : base(techType)
        {
            IsAllowedToAdd = (SubRoot cyclops, Pickupable item, bool verbose) =>
            {
                if (SiblingUpgrade == null)
                    return this.Count < this.MaxCount;

                return (SiblingUpgrade.Count + this.Count) < this.MaxCount;
            };
        }
    }
}
