namespace UpgradedVehicles
{
    using System;
    using Common;
    using UnityEngine;

    internal abstract class UpgradedVehicle<T> : Craftable where T : Vehicle
    {
        private readonly Type VehicleComponentType = typeof(T);
        protected readonly float HealthModifier;

        protected readonly VehiclePowerCore PowerCore;

        protected UpgradedVehicle(
            string nameID,
            string friendlyName,
            string description,
            TechType template,
            float healthModifier,
            TechType requiredAnalysis,
            VehiclePowerCore powerCore)
            : base(
                  nameID: nameID,
                  friendlyName: friendlyName,
                  description: description,
                  template: template,
                  fabricatorType: CraftTree.Type.Constructor,
                  fabricatorTab: "Vehicles",
                  requiredAnalysis: requiredAnalysis,
                  groupForPDA: TechGroup.Constructor,
                  categoryForPDA: TechCategory.Constructor,
                  prerequisite: powerCore)
        {
            HealthModifier = healthModifier;
            PowerCore = powerCore;
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = base.GetGameObject();

            var vehicle = obj.GetComponent(VehicleComponentType);

            obj.GetComponent<TechTag>().type = this.TechType;

            var life = vehicle.GetComponent<LiveMixin>();

            LiveMixinData lifeData = ScriptableObject.CreateInstance<LiveMixinData>();

            life.data.CloneFieldsInto(lifeData);
            lifeData.maxHealth = life.maxHealth * HealthModifier;

            life.data = lifeData;
            life.health = life.data.maxHealth;
            lifeData.weldable = true;

            // Always on upgrades handled in OnUpgradeModuleChange patch

            var crush = obj.GetComponent<CrushDamage>();
            crush.vehicle = (Vehicle)vehicle;
            crush.liveMixin = life;

            return obj;
        }
    }
}
