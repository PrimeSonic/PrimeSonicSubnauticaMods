namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class SpeedBooster
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "SpeedModule";
        public const string FriendlyName = "Speed Boost Module";
        public static readonly string Description = $"Allows small vehicle engines to go into overdrive, adding a {VehicleUpgrader.BonusSpeedText}% speed boost per module. Warning: expect higher energy consumption rates.";

        public static void Patch()
        {
            TechTypeID = TechTypeHandler.AddTechType(NameID, FriendlyName, Description);

            SpriteHandler.RegisterSprite(TechTypeID, @"./QMods/UpgradedVehicles/Assets/SpeedBoost.png");

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.SeamothUpgrades, TechTypeID, "CommonModules");
            CraftDataHandler.AddTechData(TechTypeID, GetRecipe());

            PrefabHandler.RegisterPrefab(new SpeedBoosterPreFab(TechTypeID, NameID));
            CraftDataHandler.EditEquipmentType(TechTypeID, EquipmentType.VehicleModule);

            KnownTechHandler.EditAnalysisTechEntry(TechType.VehiclePowerUpgradeModule, new List<TechType>(1) { TechTypeID }, $"{FriendlyName} blueprint discovered!");
        }

        private static TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                             {
                                 new Ingredient(TechType.Aerogel, 1),
                                 new Ingredient(TechType.Magnetite, 1),
                                 new Ingredient(TechType.ComputerChip, 1),
                             })
            };
        }

        internal class SpeedBoosterPreFab : ModPrefab
        {
            internal SpeedBoosterPreFab(TechType techType, string classId) : base(classId, $"{classId}Prefab", techType)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/PowerUpgradeModule");
                GameObject obj = GameObject.Instantiate(prefab);

                return obj;
            }
        }
    }
}
