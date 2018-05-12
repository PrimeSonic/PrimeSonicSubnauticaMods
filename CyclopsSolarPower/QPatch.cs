namespace CyclopsSolarPower
{
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;
    using SMLHelper; // by ahk1221 https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;
    using UnityEngine;
    using Object = UnityEngine.Object;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        public static TechType CySolarChargerTechType { get; private set; }

        public static void Patch()
        {
            CreateCyclopsSolarCharger();

            HarmonyInstance harmony = HarmonyInstance.Create("com.CyclopsSolarPower.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private static void CreateCyclopsSolarCharger()
        {
            CySolarChargerTechType = TechTypePatcher.AddTechType("CyclopsSolarCharger", "Cyclops Solar Charger", "Recharges the Cyclops' power cells while in sunlight. Stack multiple for even faster charging!", true);

            var cySolarChargerRecipe = new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[1]
                             { new IngredientHelper(TechType.SeamothSolarCharge, 2) }),
                _techType = CySolarChargerTechType
            };

            KnownTechPatcher.unlockedAtStart.Add(CySolarChargerTechType);

            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab("CyclopsSolarCharger", "WorldEntities/Tools/CyclopsSolarCharger", CySolarChargerTechType, GetSolarChargerObject));

            CustomSpriteHandler.customSprites.Add(GetCySolarIcon());

            CraftTreePatcher.customNodes.Add(new CustomCraftNode(CySolarChargerTechType, CraftScheme.Workbench, "CyclopsMenu/CyclopsSolarCharger"));
            CraftDataPatcher.customTechData[CySolarChargerTechType] = cySolarChargerRecipe;
            CraftDataPatcher.customEquipmentTypes[CySolarChargerTechType] = EquipmentType.CyclopsModule;
        }

        public static GameObject GetSolarChargerObject()
        {
            return GetUpgradeObject(CySolarChargerTechType, "CyclopsSolarCharger", "WorldEntities/Tools/CyclopsThermalReactorModule");
        }

        private static GameObject GetUpgradeObject(TechType techType, string id, string resourcePath)
        {
            GameObject prefab = Resources.Load<GameObject>(resourcePath);
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = id;
            obj.GetComponent<TechTag>().type = techType;

            return obj;
        }

        private static CustomSprite GetCySolarIcon()
        {
            var fallbackSprite = SpriteManager.Get(TechType.SolarPanel);

            return new CustomSprite(CySolarChargerTechType, fallbackSprite);
            // For some reason, the custom sprite code is causing access violation errors and crashing the game during starup
            // TODO - Find a fix for this and include the proper custom icon.

            //try
            //{
            //    var assetBundle = AssetBundle.LoadFromFile(@"./QMods/CyclopsSolarPower/Assets/cysolar.assets");
            //    var sprite = assetBundle.LoadAsset<Sprite>("CySolarIcon");

            //    return new CustomSprite(CySolarChargerTechType, sprite);
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error(ex.ToString());
            //    Logger.Error(ex.StackTrace);
            //    // In the event of any error, fallback to the SolarPanel blueprint sprite
            //    var fallbackSprite = SpriteManager.Get(TechType.SolarPanel);

            //    return new CustomSprite(CySolarChargerTechType, fallbackSprite);
            //}
        }
    }
}
