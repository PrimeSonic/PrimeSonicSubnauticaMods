namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class DeepLithiumPowerCell : DeepLithiumBase
    {
        private const int BatteriesRequired = 2;
        private readonly TechType deepLithiumBattery;

        public DeepLithiumPowerCell(DeepLithiumBattery lithiumBattery)
            : base(classId: "DeepLithiumPowerCell",
                   friendlyName: "Deep Lithium Power Cell",
                   description: "A stronger power cell created from rare materials.")
        {
            if (!lithiumBattery.IsPatched)
                lithiumBattery.Patch();

            deepLithiumBattery = lithiumBattery.TechType;
            OnFinishedPatching += EquipmentPatching;
        }

        public override GameObject GetGameObject() => this.CreateBattery(TechType.PowerCell, BatteryCapacity * BatteriesRequired);

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
