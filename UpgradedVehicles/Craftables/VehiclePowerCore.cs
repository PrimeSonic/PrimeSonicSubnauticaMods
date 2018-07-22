namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class VehiclePowerCore : ModPrefab
    {
        public static TechType TechTypeID { get; private set; }

        public const string NameID = "VehiclePowerCore";
        public const string FriendlyName = "Vehicle Power Core";
        public const string Description = "A modified power core for upgraded vehicles. Enables permanent enhancements without use of external upgrade modules.";

        internal readonly TechType SpeedBoosterID;

        internal VehiclePowerCore(TechType speedBoostModule)
            : base(NameID, $"{NameID}Prefab")
        {
            SpeedBoosterID = speedBoostModule;
        }

        public void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(NameID,
                                                     FriendlyName,
                                                     Description,
                                                     ImageUtils.LoadSpriteFromFile(@"./QMods/UpgradedVehicles/Assets/VehiclePowerCore.png"),
                                                     false);
            TechTypeID = this.TechType;

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.SeamothUpgrades, this.TechType, "CommonModules");
            CraftDataHandler.SetTechData(this.TechType, GetRecipe());

            PrefabHandler.RegisterPrefab(this);
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.None);

            KnownTechHandler.SetAnalysisTechEntry(TechType.BaseUpgradeConsole, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");
            CraftDataHandler.AddToGroup(TechGroup.Resources, TechCategory.Electronics, this.TechType);
        }

        private TechData GetRecipe()
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
                                 new Ingredient(SpeedBoosterID, 2), // Speed boost
                             })
            };
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/PrecursorIonPowerCell");
            GameObject obj = GameObject.Instantiate(prefab);
            GameObject.DestroyImmediate(obj.GetComponent<Battery>());

            return obj;
        }
    }
}
