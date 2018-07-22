namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class SeaMothMk3 : UpgradedVehicle<SeaMoth>
    {
        public static TechType TechTypeID { get; private set; } = TechType.Unobtanium; // Default for when not set but still used in comparisons
        public static TechType SeamothHullModule4 { get; private set; } = TechType.Unobtanium;
        public static TechType SeamothHullModule5 { get; private set; } = TechType.Unobtanium;

        internal readonly TechType PowerCoreID;

        internal SeaMothMk3(TechType vehiclePowerCore, TechType seamothHullModule4, TechType seamothHullModule5)
            : base(nameID: "SeaMothMk3",
                      friendlyName: "Seamoth ++",
                      description: "The highest end SeaMoth. Ready for adventures in the most hostile of environments.",
                      template: TechType.Seamoth,
                      healthModifier: 2.15f, // 2.15x Max HP. 115% more.
                      requiredAnalysis: TechType.VehicleHullModule3)
        {
            PowerCoreID = vehiclePowerCore;
            SeamothHullModule4 = seamothHullModule4;
            SeamothHullModule5 = seamothHullModule5;
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[5]
                             {
                                 new Ingredient(TechType.PlasteelIngot, 1), // Stronger than titanium ingot                                 
                                 new Ingredient(TechType.EnameledGlass, 2), // Stronger than glass
                                 new Ingredient(TechType.Lead, 1),

                                 new Ingredient(SeamothHullModule5, 1), // Minimum crush depth of 1700 without upgrades
                                 new Ingredient(PowerCoreID, 1), // armor and speed without engine efficiency penalty
                             })
            };
        }
    }
}
