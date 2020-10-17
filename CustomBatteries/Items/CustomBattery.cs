namespace CustomBatteries.Items
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

    internal class CustomBattery : CbCore
    {
        public CustomBattery(string classId, bool ionCellSkins) : base(classId, ionCellSkins)
        {
        }

        protected override TechType PrefabType => this.UsingIonCellSkins ? TechType.PrecursorIonBattery : TechType.Battery;
        protected override EquipmentType ChargerType => EquipmentType.BatteryCharger;
        protected override string[] StepsToFabricatorTab => CbDatabase.BatteryCraftPath;

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
            CbDatabase.BatteryItems.Add(this);
            CbDatabase.TrackItems.Add(this.TechType);
        }
    }
}
