namespace CustomBatteries.Items
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#endif

    internal abstract class CbCore : ModPrefab
    {
        private const string BatteryCraftTab = "BatteryTab";
        private const string PowCellCraftTab = "PowCellTab";
        private const string ElecCraftTab = "Electronics";
        private const string ResCraftTab = "Resources";
        protected static readonly string[] BatteryCraftPath = new[] { ResCraftTab, ElecCraftTab, BatteryCraftTab };
        protected static readonly string[] PowCellCraftPath = new[] { ResCraftTab, ElecCraftTab, PowCellCraftTab };

        private static bool CraftingTabsPatched = false;

        public static string ExecutingFolder { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static List<TechType> BatteryTechTypes { get; } = new List<TechType>();
        public static TechType LastModdedBattery => BatteryTechTypes[BatteryTechTypes.Count - 1];

        public static List<TechType> PowerCellTechTypes { get; } = new List<TechType>();
        public static TechType LastModdedPowerCell => PowerCellTechTypes[PowerCellTechTypes.Count - 1];

        protected abstract TechType PrefabType { get; } // Should only ever be Battery or PowerCell
        protected abstract EquipmentType ChargerType { get; } // Should only ever be BatteryCharger or PowerCellCharger

        public TechType RequiredForUnlock { get; set; } = TechType.None;
        public bool UnlocksAtStart => this.RequiredForUnlock == TechType.None;

        public abstract RecipeData GetBlueprintRecipe();

        public float PowerCapacity { get; set; }

        public string FriendlyName { get; set; }

        public string Description { get; set; }

        public string IconFileName { get; set; }

        public string PluginPackName { get; set; }

        public string PluginFolder { get; set; }

        public Sprite Sprite { get; set; }

        public IList<TechType> Parts { get; set; }

        public bool IsPatched { get; private set; }

        protected CbCore(string classId)
            : base(classId, $"{classId}PreFab", TechType.None)
        {
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.PrefabType);
            var obj = GameObject.Instantiate(prefab);

            Battery battery = obj.GetComponent<Battery>();
            battery._capacity = this.PowerCapacity;
            battery.name = $"{this.ClassID}BatteryCell";

            // Add the component that will readjust position.
            if (ChargerType == EquipmentType.PowerCellCharger)
                obj.AddComponent<CustomPowerCellPlaceTool>();
            else
                obj.AddComponent<CustomBatteryPlaceTool>();
            // Make item placeable.
            AddPlaceTool(obj);

            return obj;
        }

        protected void CreateIngredients(IEnumerable<TechType> parts, List<Ingredient> partsList)
        {
            if (parts == null)
                return;

            foreach (TechType part in parts)
            {
                if (part == TechType.None)
                {
                    QuickLogger.Warning($"Parts list for '{this.ClassID}' contained an unidentified TechType");
                    continue;
                }

                Ingredient priorIngredient = partsList.Find(i => i.techType == part);

                if (priorIngredient != null)
#if SUBNAUTICA
                    priorIngredient.amount++;
#elif BELOWZERO
                    priorIngredient._amount++;
#endif
                else
                    partsList.Add(new Ingredient(part, 1));
            }
        }

        protected abstract void AddToList();

        protected abstract string[] StepsToFabricatorTab { get; }

        public void Patch()
        {
            if (this.IsPatched)
                return;

            this.TechType = TechTypeHandler.AddTechType(this.ClassID, this.FriendlyName, this.Description, this.UnlocksAtStart);

            if (!this.UnlocksAtStart)
                KnownTechHandler.SetAnalysisTechEntry(this.RequiredForUnlock, new TechType[] { this.TechType });

            if (this.Sprite == null)
                this.Sprite = ImageUtils.LoadSpriteFromFile(IOUtilities.Combine(ExecutingFolder, this.PluginFolder, this.IconFileName));

            SpriteHandler.RegisterSprite(this.TechType, this.Sprite);

            CraftDataHandler.SetTechData(this.TechType, GetBlueprintRecipe());

            CraftDataHandler.AddToGroup(TechGroup.Resources, TechCategory.Electronics, this.TechType);

            CraftDataHandler.SetEquipmentType(this.TechType, this.ChargerType);
            //CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.Hand);
            
            CraftDataHandler.SetQuickSlotType(this.TechType, QuickSlotType.Selectable); // We can select the item.

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, this.TechType, this.StepsToFabricatorTab);

            PrefabHandler.RegisterPrefab(this);

            AddToList();

            this.IsPatched = true;
        }

        private static void AddPlaceTool(GameObject customBattery)
        {
            PlaceTool placeTool = customBattery.AddComponent<PlaceTool>();
            placeTool.allowedInBase = true;
            placeTool.allowedOnBase = true;
            placeTool.allowedOnConstructable = true;
            placeTool.allowedOnGround = true;
            placeTool.allowedOnRigidBody = true;
            placeTool.allowedOutside = true;
#if BELOWZERO
            placeTool.allowedUnderwater = true;
#endif
            placeTool.allowedOnCeiling = false;
            placeTool.allowedOnWalls = false;
            placeTool.reloadMode = PlayerTool.ReloadMode.None;
            placeTool.socket = PlayerTool.Socket.RightHand;
            placeTool.rotationEnabled = true;
            placeTool.drawTime = 0.5f;
            placeTool.dropTime = 1f;
            placeTool.holsterTime = 0.35f;
            // Associate collider
            Collider mainCollider = customBattery.GetComponent<Collider>() ?? customBattery.GetComponentInChildren<Collider>();
            if (mainCollider != null)
                placeTool.mainCollider = mainCollider;
            // Associate pickupable
            placeTool.pickupable = customBattery.GetComponent<Pickupable>();
        }

        internal static void PatchCraftingTabs()
        {
            if (CraftingTabsPatched)
                return; // Just a safety

            QuickLogger.Info("Separating batteries and power cells into their own fabricator crafting tabs");

            // Remove original crafting nodes
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResCraftTab, ElecCraftTab, TechType.Battery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResCraftTab, ElecCraftTab, TechType.PrecursorIonBattery.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResCraftTab, ElecCraftTab, TechType.PowerCell.ToString());
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, ResCraftTab, ElecCraftTab, TechType.PrecursorIonPowerCell.ToString());

            // Add a new set of tab nodes for batteries and power cells
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, BatteryCraftTab, "Batteries", SpriteManager.Get(TechType.Battery), ResCraftTab, ElecCraftTab);
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, PowCellCraftTab, "Power Cells", SpriteManager.Get(TechType.PowerCell), ResCraftTab, ElecCraftTab);

            // Move the original batteries and power cells into these new tabs
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.Battery, BatteryCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonBattery, BatteryCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PowerCell, PowCellCraftPath);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.PrecursorIonPowerCell, PowCellCraftPath);

            CraftingTabsPatched = true;
        }
    }
}
