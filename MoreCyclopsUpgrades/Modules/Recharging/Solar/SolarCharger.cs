namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers; // by ahk1221 https://github.com/ahk1221/SMLHelper/    
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class SolarCharger
    {
        public static TechType CySolarChargerTechType { get; private set; }

        public const string NameID = "CyclopsSolarCharger";
        public const string FriendlyName = "Cyclops Solar Charger";
        public const string Description = "Recharge your Cyclops with the power of the sun itself.";

        public static void Patch(AssetBundle assetBundle)
        {
            // Get the custom icon from the Unity assets bundle
            Sprite cySolarSprite = assetBundle.LoadAsset<Sprite>("CySolarIcon");

            // Create a new TechType
            CySolarChargerTechType = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, cySolarSprite);

            // Create the in-game item that will behave like any other Cyclops upgrade module
            PrefabHandler.RegisterPrefab(new CySolarPreFab());

            // Pair the new item with its crafting recipe
            CraftDataHandler.SetTechData(CySolarChargerTechType, GetRecipe());

            // Add the new item to the Modification Station crafting tree
            var craftTree = CraftTreeHandler.GetExistingTree(CraftTree.Type.Workbench);
            craftTree.GetTabNode("CyclopsMenu").AddCraftingNode(CySolarChargerTechType);

            // Ensure that the new in-game item is classified as a Cyclops upgrade module. Otherwise you can't equip it.
            CraftDataHandler.SetEquipmentType(CySolarChargerTechType, EquipmentType.CyclopsModule);
        }

        private static TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[4]
                             {
                                 new Ingredient(TechType.SeamothSolarCharge, 1), // This is to make sure the player has access to vehicle solar charging
                                 new Ingredient(TechType.Quartz, 3),
                                 new Ingredient(TechType.Titanium, 3),
                                 new Ingredient(TechType.CopperWire, 1),
                             })
            };
        }
    }

    public class CySolarPreFab : ModPrefab
    {
        public CySolarPreFab() :
            base(SolarCharger.NameID, SolarCharger.NameID + "Pf", SolarCharger.CySolarChargerTechType)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/CyclopsThermalReactorModule");
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = SolarCharger.NameID;
            obj.GetComponent<TechTag>().type = SolarCharger.CySolarChargerTechType;

            return obj;
        }
    }
}