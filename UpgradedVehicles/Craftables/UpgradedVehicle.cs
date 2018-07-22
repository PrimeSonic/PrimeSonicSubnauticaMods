namespace UpgradedVehicles
{
    using System;
    using Common;    
    using UnityEngine;

    internal abstract class UpgradedVehicle<T> : Craftable where T : Vehicle
    {
        private readonly Type VehicleComponentType = typeof(T);
        protected readonly float HealthModifier;

        protected UpgradedVehicle(
            string nameID,
            string friendlyName,
            string description,
            TechType template,
            float healthModifier,
            TechType requiredAnalysis)
            : base(nameID, friendlyName, description, template, CraftTree.Type.Constructor, "Vehicles", requiredAnalysis, TechGroup.Constructor, TechCategory.Constructor)
        {
            HealthModifier = healthModifier;
        }

        public override GameObject GetGameObject()
        {
            GameObject seamothPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit");
            GameObject obj = GameObject.Instantiate(seamothPrefab);

            var vehicle = obj.GetComponent(VehicleComponentType);

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
