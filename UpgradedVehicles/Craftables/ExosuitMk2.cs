﻿namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class ExosuitMk2
    {
        public static TechType TechTypeID { get; private set; }
        public const string NameID = "ExosuitMk2";
        public const string FriendlyName = "Prawn Suit Mk2";
        public const string Description = "An upgraded Prawn Suit now even tougher to take on anything.";

        public static void Patch()
        {
            TechTypeID = TechTypeHandler.AddTechType(NameID, FriendlyName, Description);

            SpriteHandler.RegisterSprite(TechTypeID, @"./QMods/UpgradedVehicles/Assets/ExosuitMk2.png");

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Constructor, TechTypeID, "Vehicles");
            CraftDataHandler.AddCraftingTime(TechTypeID, 15f);
            CraftDataHandler.AddTechData(TechTypeID, GetRecipe());

            PrefabHandler.RegisterPrefab(new ExosuitMk2Prefab(TechTypeID, NameID));
            KnownTechHandler.EditAnalysisTechEntry(TechType.ExoHullModule2, new List<TechType>(1) { TechTypeID }, $"{FriendlyName} blueprint discovered!");
        }

        private static TechData GetRecipe()
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
                                 new Ingredient(VehiclePowerCore.TechTypeID, 1),  // +2 to armor + speed without engine efficiency penalty
                             })
            };
        }

        internal class ExosuitMk2Prefab : ModPrefab
        {
            internal ExosuitMk2Prefab(TechType techtype, string nameID) : base(nameID, $"{nameID}Prefab", techtype)
            {
            }

            public override GameObject GetGameObject()
            {
                GameObject seamothPrefab = Resources.Load<GameObject>("WorldEntities/Tools/Exosuit");
                GameObject obj = GameObject.Instantiate(seamothPrefab);

                var exosuit = obj.GetComponent<Exosuit>();

                var life = exosuit.GetComponent<LiveMixin>();

                LiveMixinData lifeData = (LiveMixinData)ScriptableObject.CreateInstance(typeof(LiveMixinData));

                life.data.CloneFieldsInto(lifeData);
                lifeData.maxHealth = life.maxHealth * 1.5f; // 50% more HP

                life.data = lifeData;
                life.health = life.data.maxHealth;

                // Always on upgrades handled in OnUpgradeModuleChange patch

                return obj;
            }
        }
    }
}
