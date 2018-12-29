﻿namespace MidGameBatteries.Craftables
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class DeepLithiumBattery : DeepLithiumBase
    {
        // This battery provides 2.5x the power of a normal battery
        internal const float BatteryCapacity = 250f;

        public DeepLithiumBattery()
            : base(classId: "DeepBattery",
                   friendlyName: "Deep Battery",
                   description: "A longer lasting battery created from rare materials and stronger chemicals.")
        {
            // This event will be invoked after all patching done by the Craftable class is complete
            OnFinishedPatching += SetStaticTechType;
        }

        protected override TechType BaseType { get; } = TechType.Battery;
        protected override float PowerCapacity { get; } = BatteryCapacity;
        protected override EquipmentType ChargerType { get; } = EquipmentType.BatteryCharger;

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

        private void SetStaticTechType() => BatteryID = this.TechType;
    }
}
