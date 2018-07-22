namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class SpeedBooster : ModPrefab
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "SpeedModule";
        public const string FriendlyName = "Speed Boost Module";
        public const string Description = "Allows small vehicle engines to go into overdrive, adding greater speeds but at the cost of higher energy consumption rates.";

        internal SpeedBooster() : base(NameID, $"{NameID}Prefab")
        {
        }

        public void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(NameID,
                                                     FriendlyName,
                                                     Description,
                                                     ImageUtils.LoadSpriteFromFile(@"./QMods/UpgradedVehicles/Assets/SpeedBoost.png"),
                                                     false);
            TechTypeID = this.TechType;

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.SeamothUpgrades, this.TechType, "CommonModules");
            CraftDataHandler.SetTechData(this.TechType, GetRecipe());

            PrefabHandler.RegisterPrefab(this);
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.VehicleModule);

            KnownTechHandler.SetAnalysisTechEntry(TechType.BaseUpgradeConsole, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");
            CraftDataHandler.AddToGroup(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades, this.TechType);
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

        public override GameObject GetGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/PowerUpgradeModule");
            GameObject obj = GameObject.Instantiate(prefab);

            return obj;
        }
    }
}
