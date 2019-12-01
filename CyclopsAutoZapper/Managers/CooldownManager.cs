namespace CyclopsAutoZapper.Managers
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal abstract class CooldownManager : IAuxCyclopsManager
    {
        protected abstract float TimeBetweenUses { get; }

        public readonly SubRoot Cyclops;
        public readonly TechType UpgradeTechType;

        private UpgradeHandler upgradeHandler;
        private UpgradeHandler UpgradeHandler => upgradeHandler ?? (upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler(Cyclops, UpgradeTechType));

        private float timeOfLastUse = Time.time;

        protected CooldownManager(TechType upgradeTechType, SubRoot cyclops)
        {
            Cyclops = cyclops;
            UpgradeTechType = upgradeTechType;
        }

        public bool IsOnCooldown => Time.time < timeOfLastUse + this.TimeBetweenUses;
        public bool HasUpgrade => this.UpgradeHandler?.HasUpgrade ?? false;

        public bool Initialize(SubRoot cyclops)
        {
            return Cyclops == cyclops;
        }

        protected void UpdateCooldown()
        {
            timeOfLastUse = Time.time;
        }
    }
}
