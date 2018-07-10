namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class VehiclePowerCore
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "VehiclePowerCore";
        public const string FriendlyName = "Vehicle Power Core";
        public const string Description = "A modified power core for upgraded vehicles. Enables permanent enhancements without use of external upgrade modules.";

        public static void Patch()
        {
            TechTypeID = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, false);

            SpriteHandler.RegisterSprite(TechTypeID, @"./QMods/UpgradedVehicles/Assets/VehiclePowerCore.png");

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.SeamothUpgrades, TechTypeID, "CommonModules");
            CraftDataHandler.SetTechData(TechTypeID, GetRecipe());

            PrefabHandler.RegisterPrefab(new VehiclePowerCorePreFab(TechTypeID, NameID));
            CraftDataHandler.SetEquipmentType(TechTypeID, EquipmentType.None);

            KnownTechHandler.SetAnalysisTechEntry(TechType.VehiclePowerUpgradeModule, new TechType[1] { TechTypeID }, $"{FriendlyName} blueprint discovered!");
        }

        private static TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[6]
                             {
                                 new Ingredient(TechType.Benzene, 1),
                                 new Ingredient(TechType.Lead, 2),
                                 new Ingredient(TechType.PowerCell, 1),

                                 new Ingredient(TechType.VehiclePowerUpgradeModule, 2), // Engine eficiency
                                 new Ingredient(TechType.VehicleArmorPlating, 2), // Armor                                 
                                 new Ingredient(SpeedBooster.TechTypeID, 2), // Speed boost
                             })
            };
        }

        internal class VehiclePowerCorePreFab : ModPrefab
        {
            internal VehiclePowerCorePreFab(TechType techType, string classId) : base(classId, $"{classId}Prefab", techType)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/PrecursorIonPowerCell");
                GameObject obj = GameObject.Instantiate(prefab);
                GameObject.DestroyImmediate(obj.GetComponent<Battery>());

                obj.GetComponent<TechTag>().type = TechTypeID;

                return obj;
            }
        }
    }
}
