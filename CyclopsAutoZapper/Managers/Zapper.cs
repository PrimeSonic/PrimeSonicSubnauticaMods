namespace CyclopsAutoZapper.Managers
{
    using UnityEngine;

    internal abstract class Zapper : CooldownManager
    {
        private GameObject seamothElectricalDefensePrefab = null;
        protected GameObject ElectricalDefensePrefab => seamothElectricalDefensePrefab ??
            (seamothElectricalDefensePrefab = CraftData.GetPrefabForTechType(TechType.Seamoth).GetComponent<SeaMoth>().seamothElectricalDefensePrefab);

        private class CreatureTarget
        {
            public bool IsValidTarget => liveMixin != null && liveMixin.IsAlive();
            public readonly LiveMixin liveMixin;

            public CreatureTarget(CyclopsSonarCreatureDetector.EntityData entityData)
            {
                if (entityData?.gameObject != null)
                {
                    liveMixin = entityData.gameObject.GetComponent<LiveMixin>();
                }
                else
                {
                    liveMixin = null;
                }
            }
        }

        private const float EnergyCostPerRadiusZap = 36f;
        private const float DirectZapMultiplier = 0.5f;
        private const float EnergyCostPerDirectZap = EnergyCostPerRadiusZap * DirectZapMultiplier;
        private const float ZapPower = 6f;
        private const float DamageMultiplier = 30f;
        private const float BaseCharge = 2f;
        private const float BaseRadius = 1f;

        private const float DirectZapDamage = (BaseRadius + ZapPower * BaseCharge) * DamageMultiplier * DirectZapMultiplier;
        // Calculations and initial values based off ElectricalDefense component

        internal const float EnergyRequiredToZap = EnergyCostPerRadiusZap + EnergyCostPerDirectZap;

        private CreatureTarget target;

        protected Zapper(TechType upgradeTechType, SubRoot cyclops)
            : base(upgradeTechType, cyclops)
        {
        }

        protected virtual bool AbleToZap()
        {
            if (!this.HasUpgrade)
                return false;

            if (this.IsOnCooldown)
                return false;

            if (GameModeUtils.RequiresPower() && Cyclops.powerRelay.GetPower() < EnergyRequiredToZap)
                return false;

            return true;
        }

        public void Zap(CyclopsSonarCreatureDetector.EntityData creatureToZap = null)
        {
            if (!AbleToZap())
                return;

            ZapRadius();

            UpdateCooldown();

            if (creatureToZap != null)
            {
                var newTarget = new CreatureTarget(creatureToZap);

                if (newTarget.IsValidTarget)
                    target = newTarget;
            }

            if (target != null && !target.IsValidTarget)
                target = null;

            if (target == null)
                return;

            ZapCreature();
        }

        private void ZapRadius()
        {
            GameObject gameObject = Utils.SpawnZeroedAt(this.ElectricalDefensePrefab, Cyclops.transform, false);
            ElectricalDefense defenseComponent = gameObject.GetComponent<ElectricalDefense>();
            defenseComponent.charge = ZapPower;
            defenseComponent.chargeScalar = ZapPower;
            defenseComponent.radius *= ZapPower;
            defenseComponent.chargeRadius *= ZapPower;

            if (GameModeUtils.RequiresPower())
                Cyclops.powerRelay.ConsumeEnergy(EnergyCostPerRadiusZap, out float amountConsumed);
        }

        private void ZapCreature()
        {
            target.liveMixin.TakeDamage(DirectZapDamage, default, DamageType.Electrical, Cyclops.gameObject);

            if (GameModeUtils.RequiresPower())
                Cyclops.powerRelay.ConsumeEnergy(EnergyCostPerDirectZap, out float amountConsumed);
        }
    }
}
