namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;

    internal class ExosuitMk2 : UpgradedVehicle<Exosuit>
    {
        internal ExosuitMk2(VehiclePowerCore vehiclePowerCore)
            : base(
                  nameID: "ExosuitMk2",
                  friendlyName: "Prawn Suit MK2",
                  description: "An upgraded Prawn Suit now even tougher to take on anything.",
                  template: TechType.Exosuit,
                  healthModifier: 1.5f,
                  requiredAnalysis: TechType.ExoHullModule2,
                  powerCore: vehiclePowerCore)
        {
        }

        protected override void PostPatch()
        {
            MTechType.ExosuitMk2 = this.TechType;
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
                                 new Ingredient(PowerCore.TechType, 1),  // +2 to armor + speed without engine efficiency penalty
                             })
            };
        }
    }
}
