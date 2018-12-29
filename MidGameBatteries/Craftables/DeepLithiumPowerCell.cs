namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class DeepLithiumPowerCell : DeepLithiumBattery
    {
        internal static TechType PowerCellID { get; private set; }

        private const string ClassIDString = "DeepLithiumPowerCell";
        private readonly TechType deepLithiumBattery;
        private readonly int BatteriesRequired = 2;

        public DeepLithiumPowerCell(DeepLithiumBattery lithiumBattery)
            : base(classId: ClassIDString,
                   friendlyName: "Deep Lithium Power Cell",
                   description: "A stronger power cell created from rare materials.")
        {
            deepLithiumBattery = lithiumBattery.TechType;
            OnFinishedPatching += EquipmentPatching;
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.PowerCell);
            var obj = GameObject.Instantiate(prefab);

            var battery = obj.GetComponent<Battery>();
            battery._capacity = BatteryCapacity * BatteriesRequired;
            battery.name = ClassIDString;

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(2)
                {
                    new Ingredient(deepLithiumBattery, BatteriesRequired),
                    new Ingredient(TechType.Silicone, 1),
                }
            };
        }

        private void EquipmentPatching()
        {
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.PowerCellCharger);
            PowerCellID = this.TechType;
        }
    }
}
