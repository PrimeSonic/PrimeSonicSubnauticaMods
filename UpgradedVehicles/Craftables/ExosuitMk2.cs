namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal class ExosuitMk2 : ModPrefab
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "ExosuitMk2";
        public const string FriendlyName = "Prawn Suit Mk2";
        public const string Description = "An upgraded Prawn Suit now even tougher to take on anything.";

        internal readonly TechType PowerCoreID;

        internal ExosuitMk2(TechType vehiclePowerCore) : base(NameID, $"{NameID}Prefab")
        {
            PowerCoreID = vehiclePowerCore;
        }

        public void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(NameID, 
                                                     FriendlyName, 
                                                     Description,
                                                     ImageUtils.LoadSpriteFromFile(@"./QMods/UpgradedVehicles/Assets/ExosuitMk2.png"),
                                                     false);
            TechTypeID = this.TechType;

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Constructor, this.TechType, "Vehicles");            
            CraftDataHandler.SetTechData(this.TechType, GetRecipe());

            PrefabHandler.RegisterPrefab(this);


            KnownTechHandler.SetAnalysisTechEntry(TechType.ExoHullModule2, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");
            CraftDataHandler.AddToGroup(TechGroup.Constructor, TechCategory.Constructor, this.TechType);
        }

        private TechData GetRecipe()
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

        public override GameObject GetGameObject()
        {
            GameObject seamothPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit");
            GameObject obj = GameObject.Instantiate(seamothPrefab);

            var exosuit = obj.GetComponent<Exosuit>();

            var life = exosuit.GetComponent<LiveMixin>();

            LiveMixinData lifeData = ScriptableObject.CreateInstance<LiveMixinData>();

            life.data.CloneFieldsInto(lifeData);
            lifeData.maxHealth = life.maxHealth * 1.5f; // 50% more HP

            life.data = lifeData;
            life.health = life.data.maxHealth;
            lifeData.weldable = true;

            // Always on upgrades handled in OnUpgradeModuleChange patch

            var crush = obj.GetComponent<CrushDamage>();
            crush.vehicle = exosuit;
            crush.liveMixin = life;

            return obj;
        }
    }
}
