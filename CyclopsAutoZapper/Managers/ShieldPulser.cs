namespace CyclopsAutoZapper.Managers
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using UnityEngine;

    internal class ShieldPulser : CooldownManager
    {
        protected override float TimeBetweenUses => 4.0f;

        private const float ShieldCostModifier = 0.1f;

        public bool HasShieldModule => MCUServices.CrossMod.HasUpgradeInstalled(Cyclops, TechType.CyclopsShieldModule);

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

            if (GameModeUtils.RequiresPower())
            {
                float originalCost = Cyclops.shieldPowerCost;
                Cyclops.shieldPowerCost = originalCost * ShieldCostModifier;

                Cyclops.powerRelay.ConsumeEnergy(Cyclops.shieldPowerCost, out float amountConsumed);

                Cyclops.shieldPowerCost = originalCost;
            }

            LavaLarva[] leaches = Cyclops.gameObject.GetComponentsInChildren<LavaLarva>();

            if (leaches == null)
            {
                QuickLogger.Warning("GetComponentsInChildren<LavaLarva>() returned null");
                return;
            }

            for (int i = 0; i < leaches.Length; i++)
            {
                leaches[i].GetComponent<LiveMixin>().TakeDamage(1f, default(Vector3), DamageType.Electrical, null);
            }
        }
    }
}
