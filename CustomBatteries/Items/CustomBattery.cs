namespace CustomBatteries.Items
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

    internal class CustomBattery : CbCore
    {
        public CustomBattery(string classId) : base(classId)
        {
        }

        protected override TechType PrefabType => TechType.Battery;
        protected override EquipmentType ChargerType => EquipmentType.BatteryCharger;
        protected override string[] StepsToFabricatorTab => CbCore.BatteryCraftPath;

        public override RecipeData GetBlueprintRecipe()
        {
            var partsList = new List<Ingredient>();

            CreateIngredients(this.Parts, partsList);

            if (partsList.Count == 0)
                partsList.Add(new Ingredient(TechType.Titanium, 1));

            var batteryBlueprint = new RecipeData
            {
                craftAmount = 1,
                Ingredients = partsList
            };

            return batteryBlueprint;
        }

        protected override void AddToList()
        {
            BatteryTechTypes.Add(this.TechType);
        }
    }
}
