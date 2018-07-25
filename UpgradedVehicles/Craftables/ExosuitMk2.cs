namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class ExosuitMk2 : UpgradedVehicle<Exosuit>
    {
        public static TechType TechTypeID { get; private set; }

        internal readonly TechType PowerCoreID;

        internal ExosuitMk2(VehiclePowerCore vehiclePowerCore) 
            : base("ExosuitMk2",
                  "Prawn Suit MK2",
                  "An upgraded Prawn Suit now even tougher to take on anything.",
                  TechType.Exosuit,
                  1.5f,
                  TechType.ExoHullModule2)
        {
            PowerCoreID = vehiclePowerCore.TechType;
        }

        public override void Patch()
        {
            base.Patch();

            TechTypeID = this.TechType;
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[6]
                             {
                                 new Ingredient(TechType.PlasteelIngot, 2),
                                 new Ingredient(TechType.Kyanite, 4), // Better than Aerogel
                                 new Ingredient(TechType.EnameledGlass, 1),
                                 new Ingredient(TechType.Diamond, 2),

                                 new Ingredient(TechType.ExoHullModule2, 1), // Minimum crush depth of 1700 without upgrades
                                 new Ingredient(PowerCoreID, 1),  // +2 to armor + speed without engine efficiency penalty
                             })
            };
        }
    }
}
