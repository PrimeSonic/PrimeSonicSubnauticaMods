namespace IonCubeGenerator.Craftables
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using System;
    using UnityEngine;

    internal class AlienIngot : Craftable
    {
        internal static TechType TechTypeID { get; private set; }

        public AlienIngot() : base("AlienIngot", "Infused Plasteel", "An incredibly sturdy alloy capable of withstanding tremendous energy output.")
        {
            OnFinishedPatching = () => { TechTypeID = this.TechType; };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Fabricator;
        public override TechGroup GroupForPDA { get; } = TechGroup.Resources;
        public override TechCategory CategoryForPDA { get; } = TechCategory.AdvancedMaterials;
        public override string AssetsFolder { get; } = "IonCubeGenerator/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "Resources", "AdvancedMaterials" };
        public override TechType RequiredForUnlock { get; } = TechType.PrecursorPrisonIonGenerator;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.PlasteelIngot);

            throw new NotImplementedException();
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.PrecursorIonCrystal, 3),
                    new Ingredient(TechType.Nickel, 2),
                }
            };
        }
    }
}
