namespace IonCubeGenerator.Craftables
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using System;
    using UnityEngine;

    internal class AlienEletronicsCase : Craftable
    {
        internal static TechType TechTypeID { get; private set; }

        public AlienEletronicsCase() : base("AlienCase", "Energy Condensor", "Used in highly advanced and high powered electrical applications.")
        {
            OnFinishedPatching = () => { TechTypeID = this.TechType; };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Fabricator;
        public override TechGroup GroupForPDA { get; } = TechGroup.Resources;
        public override TechCategory CategoryForPDA { get; } = TechCategory.Electronics;
        public override string AssetsFolder { get; } = "IonCubeGenerator/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "Resources", "Electronics" };
        public override TechType RequiredForUnlock { get; } = TechType.PrecursorPrisonIonGenerator;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.AdvancedWiringKit);

            throw new NotImplementedException();
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.PrecursorIonCrystal, 2),
                    new Ingredient(TechType.Titanium, 1),
                    new Ingredient(TechType.Magnetite, 1),
                    new Ingredient(TechType.Aerogel, 1),
                    new Ingredient(TechType.Diamond, 1),
                }
            };
        }
    }
}
