namespace CyclopsAutoZapper
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.General;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal class Zapper : IAuxCyclopsManager
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

        private const float TimeBetweenZaps = 7.0f;
        private const float EnergyCostPerRadiusZap = 36f;
        private const float DirectZapMultiplier = 0.5f;
        private const float EnergyCostPerDirectZap = EnergyCostPerRadiusZap * DirectZapMultiplier;
        private const float ZapPower = 6f;
        private const float DamageMultiplier = 30f;
        private const float BaseCharge = 2f;
        private const float BaseRadius = 1f;

        private const float DirectZapDamage = (BaseRadius + ZapPower * BaseCharge) * DamageMultiplier * DirectZapMultiplier;
        // Calculations and initial values base off ElectricalDefense component

        internal const float EnergyRequiredToZap = EnergyCostPerRadiusZap + EnergyCostPerDirectZap;

        private readonly SubRoot Cyclops;
        private readonly TechType UpgradeTechType;

        private VehicleDockingBay dockingBay;
        private VehicleDockingBay DockingBay => dockingBay ?? (dockingBay = Cyclops.GetComponentInChildren<VehicleDockingBay>());

        private UpgradeHandler upgradeHandler;
        private UpgradeHandler UpgradeHandler => upgradeHandler ?? (upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler(Cyclops, UpgradeTechType));

        private float timeOfLastZap = Time.time;
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

        public bool IsOnCooldown => Time.time < timeOfLastZap + TimeBetweenZaps;

        public Zapper(TechType zapperTechType, SubRoot cyclops)
        {
            Cyclops = cyclops;
            UpgradeTechType = zapperTechType;
        }

        public void Zap(CyclopsSonarCreatureDetector.EntityData creatureToZap = null)
        {
            if (!this.UpgradeHandler.HasUpgrade)
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

            timeOfLastZap = Time.time; // Update time of last zap for cooldown purposes

            if (creatureToZap != null)
            {
                var newTarget = new CreatureTarget(creatureToZap);

                if (newTarget.IsValidTarget)
                    target = newTarget;
            }

            if (!target.IsValidTarget)
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

        public bool Initialize(SubRoot cyclops)
        {
            return Cyclops == cyclops;
        }
    }
}
