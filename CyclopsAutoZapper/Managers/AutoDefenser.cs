namespace CyclopsAutoZapper.Managers
{
    using UnityEngine;

    internal class AutoDefenser : CooldownManager
    {
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

        protected override float TimeBetweenUses => 7.0f;

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

        private VehicleDockingBay dockingBay;
        private VehicleDockingBay DockingBay => dockingBay ?? (dockingBay = Cyclops.GetComponentInChildren<VehicleDockingBay>());

        private SeaMoth seaMoth;
        private CreatureTarget target;

        public bool SeamothInBay
        {
            get
            {
                seaMoth = this.DockingBay?.dockedVehicle as SeaMoth;
                return seaMoth != null;
            }
        }

        public bool HasElectricalDefense
        {
            get
            {
                Equipment modules = seaMoth?.modules;

                if (modules == null)
                    return false;

                return modules.GetCount(TechType.SeamothElectricalDefense) > 0;
            }
        }

        public AutoDefenser(TechType autoDefense, SubRoot cyclops)
            : base(autoDefense, cyclops)
        {
        }

        public void Zap(CyclopsSonarCreatureDetector.EntityData creatureToZap = null)
        {
            if (!this.HasUpgrade)
                return;

            if (!this.SeamothInBay)
                return;

            if (!this.HasElectricalDefense)
                return;

            if (this.IsOnCooldown)
                return;

            if (Cyclops.powerRelay.GetPower() < EnergyRequiredToZap)
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
            GameObject gameObject = Utils.SpawnZeroedAt(seaMoth.seamothElectricalDefensePrefab, Cyclops.transform, false);
            ElectricalDefense defenseComponent = gameObject.GetComponent<ElectricalDefense>();
            defenseComponent.charge = ZapPower;
            defenseComponent.chargeScalar = ZapPower;
            defenseComponent.radius *= ZapPower;
            defenseComponent.chargeRadius *= ZapPower;

            Cyclops.powerRelay.ConsumeEnergy(EnergyCostPerRadiusZap, out float amountConsumed);
        }

        private void ZapCreature()
        {
            target.liveMixin.TakeDamage(DirectZapDamage, default, DamageType.Electrical, Cyclops.gameObject);

            Cyclops.powerRelay.ConsumeEnergy(EnergyCostPerDirectZap, out float amountConsumed);
        }

    }
}
