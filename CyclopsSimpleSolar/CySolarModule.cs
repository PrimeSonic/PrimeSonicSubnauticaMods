namespace CyclopsSimpleSolar
{
    using System.IO;
    using System.Reflection;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class CySolarModule : Equipable
    {
        private static Atlas.Sprite customSprite;

        public static Atlas.Sprite CustomSprite => customSprite ?? SpriteManager.Get(TechType.SeamothSolarCharge);

        public CySolarModule()
            : base("CySimpSolarCharger",
                   "Cyclops Solar Charging Module",
                   "Recharges the Cyclops power cells while in sunlight.\n" +
                   "DOES NOT STACK with other solar chargers.")
        {
            OnStartedPatching += () =>
            {
                // Load the custom texture
                string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string folderPath = Path.Combine(executingLocation, "Assets");
                string spriteLocation = Path.Combine(folderPath, "CySimpSolarCharger.png");

                customSprite = ImageUtils.LoadSpriteFromFile(spriteLocation);
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.CyclopsModule;

        public override TechGroup GroupForPDA =>  TechGroup.Cyclops;

        public override TechCategory CategoryForPDA => TechCategory.CyclopsUpgrades;

        public override CraftTree.Type FabricatorType => CraftTree.Type.CyclopsFabricator;

        public override string[] StepsToFabricatorTab => MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;

        public override TechType RequiredForUnlock => TechType.SeamothSolarCharge;

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsShieldModule);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Glass, 1),
                    new Ingredient(TechType.EnameledGlass, 1),
                    new Ingredient(TechType.Titanium, 1),
                }
            };
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            return CustomSprite;
        }
    }
}
