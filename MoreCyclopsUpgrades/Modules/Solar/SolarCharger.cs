namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common;
    using Harmony;
    using SMLHelper; // by ahk1221 https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;
    using UnityEngine;
    using Object = UnityEngine.Object;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class SolarCharger
    {
        internal const string ModFolder = @"./QMods/CyclopsSolarPower";

        public static TechType CySolarChargerTechType { get; private set; }

        public const string NameID = "CyclopsSolarCharger";

        internal static CySolarConfig ChargeRateConfig { get; private set; }

        public static void Patch()
        {
            CreateCyclopsSolarCharger();

            LoadConfig();

            if (ChargeRateConfig.SolarChargeRate > 0)
            {
                SubRootPatcher.UserChargeRate = ChargeRateConfig.SolarChargeRate;
            }

            HarmonyInstance harmony = HarmonyInstance.Create("com.CyclopsSolarPower.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private static void CreateCyclopsSolarCharger()
        {
            // Create a new TechType
            CySolarChargerTechType = TechTypePatcher.AddTechType(NameID, "Cyclops Solar Charger", "Recharges the Cyclops' power cells while in sunlight. Stack multiple for even faster charging!", true);

            // Create a new Recipie
            var cySolarChargerRecipe = new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[1]
                             { new IngredientHelper(TechType.SeamothSolarCharge, 2) }),
                _techType = CySolarChargerTechType
            };

            // Make sure it's available from the start, it's hard enough to get as it is.
            KnownTechPatcher.unlockedAtStart.Add(CySolarChargerTechType);

            // Create the in-game item that will behave like any other Cyclops upgrade module
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameID, $"WorldEntities/Tools/{NameID}", CySolarChargerTechType, GetSolarChargerObject));

            // Get the custom icon from the Unity assets bundle
            CustomSpriteHandler.customSprites.Add(new CustomSprite(CySolarChargerTechType, AssetBundle.LoadFromFile($"{ModFolder}/Assets/cysolar.assets").LoadAsset<Sprite>("CySolarIcon")));

            // Add the new recipe to the Modification Station crafting tree
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(CySolarChargerTechType, CraftTree.Type.Workbench, $"CyclopsMenu/{NameID}"));

            // Pair the new recipie with the new TechType
            CraftDataPatcher.customTechData[CySolarChargerTechType] = cySolarChargerRecipe;

            // Ensure that the new in-game item is classified as a Cyclops upgrade module. Otherwise you can't equip it.
            CraftDataPatcher.customEquipmentTypes[CySolarChargerTechType] = EquipmentType.CyclopsModule;
        }

        private static void LoadConfig()
        {
            var cfgMgr = new ConfigManager<CySolarConfig>("CyclopsSolarPower", $"{ModFolder}/config.json");

            bool fileLoaded = cfgMgr.GetConfig(out CySolarConfig config);

            if (!fileLoaded)
            {
                // No file found or file corrupted. Save the default config.
                bool savedDefault = cfgMgr.SaveConfig(config);
            }

            ChargeRateConfig = config;
        }

        public static GameObject GetSolarChargerObject()
        {
            return GetCyclopsSolareObject(CySolarChargerTechType, NameID, "WorldEntities/Tools/CyclopsThermalReactorModule");
        }

        private static GameObject GetCyclopsSolareObject(TechType techType, string id, string resourcePath)
        {
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = id;
            obj.GetComponent<TechTag>().type = techType;

            return obj;
        }
    }
}