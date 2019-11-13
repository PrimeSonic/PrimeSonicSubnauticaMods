namespace CustomBatteries.Items
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class CustomPowerCell : CbCore
    {
        private readonly CustomBattery baseBattery;

        public CustomPowerCell(string classId, CustomBattery customBattery) : base(classId)
        {
            baseBattery = customBattery;
        }

        protected override TechType PrefabType => TechType.PowerCell;
        protected override EquipmentType ChargerType => EquipmentType.PowerCellCharger;

        public void CreateBlueprintData(params TechType[] parts)
        {
            var partsList = new List<Ingredient>()
            {
                new Ingredient(baseBattery.TechType, 2),
                new Ingredient(TechType.Silicone, 1),
            };

            CreateIngredients(parts, partsList);

            var batteryBlueprint = new TechData
            {
                craftAmount = 1,
                Ingredients = partsList
            };

            this.BlueprintRecipe = batteryBlueprint;
        }

        protected override void AddToList()
        {
            PowerCellTechTypes.Add(this.TechType);
            HasPowerCells = true;
        }
    }
}
