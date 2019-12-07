namespace CyclopsAutoZapper.Managers
{
    using MoreCyclopsUpgrades.API;


    internal class ShieldPulser : CooldownManager
    {
        protected override float TimeBetweenUses => 4.0f;

        private const float ShieldCostModifier = 0.1f;

        private CyclopsShieldButton shieldButton;
        private CyclopsShieldButton ShieldButton => shieldButton ?? (shieldButton = Cyclops.GetComponentInChildren<CyclopsShieldButton>());

        public bool HasShieldModule => MCUServices.CrossMod.HasUpgradeInstalled(Cyclops, TechType.CyclopsShieldModule) && this.ShieldButton != null;

        public ShieldPulser(TechType antiParasite, SubRoot cyclops)
            : base(antiParasite, cyclops)
        {
        }

        public void PulseShield()
        {
            if (!this.HasUpgrade)
                return;

            if (!this.HasShieldModule)
                return;

            if (this.IsOnCooldown)
                return;

            UpdateCooldown();

            float originalCost = Cyclops.shieldPowerCost;
            Cyclops.shieldPowerCost = originalCost * ShieldCostModifier;

            this.ShieldButton?.StartShield();
            Cyclops.powerRelay.ConsumeEnergy(Cyclops.shieldPowerCost, out float amountConsumed);
            this.ShieldButton?.StopShield();

            Cyclops.shieldPowerCost = originalCost;
        }
    }
}
