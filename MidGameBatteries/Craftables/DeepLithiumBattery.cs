namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class DeepLithiumBattery : DeepLithiumBase
    {
        public DeepLithiumBattery()
            : base(classId: "DeepLithiumBattery",
                   friendlyName: "Deep Lithium Battery",
                   description: "A stronger battery created from rare materials.")
        {
            OnFinishedPatching += EquipmentPatching;
        }

        public override GameObject GetGameObject() => this.CreateBattery(TechType.Battery, BatteryCapacity);

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(3)
                {
                    new Ingredient(TechType.WhiteMushroom, 3),
                    new Ingredient(TechType.Lithium, 1),
                    new Ingredient(TechType.Magnetite, 1),
                }
            };
        }

        private void EquipmentPatching()
        {
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.BatteryCharger);
            BatteryID = this.TechType;
        }
    }
}
