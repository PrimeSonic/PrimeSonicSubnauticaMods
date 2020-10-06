namespace CustomBatteries.Items
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Common;
    using CustomBatteries.API;
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
        internal const string BatteryCraftTab = "BatteryTab";
        internal const string PowCellCraftTab = "PowCellTab";
        internal const string ElecCraftTab = "Electronics";
        internal const string ResCraftTab = "Resources";

        private static readonly WorldEntitiesCache worldEntities = new WorldEntitiesCache();

        internal static readonly string[] BatteryCraftPath = new[] { ResCraftTab, ElecCraftTab, BatteryCraftTab };
        internal static readonly string[] PowCellCraftPath = new[] { ResCraftTab, ElecCraftTab, PowCellCraftTab };

        public static string ExecutingFolder { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static List<CbCore> BatteryItems { get; } = new List<CbCore>();

        internal static Dictionary<TechType, Texture2D> BatteryModels { get; } = new Dictionary<TechType, Texture2D>();

        public static List<CbCore> PowerCellItems { get; } = new List<CbCore>();

        internal static Dictionary<TechType, Texture2D> PowerCellModels { get; } = new Dictionary<TechType, Texture2D>();

        protected abstract TechType PrefabType { get; } // Should only ever be Battery or PowerCell
        protected abstract EquipmentType ChargerType { get; } // Should only ever be BatteryCharger or PowerCellCharger

        public TechType RequiredForUnlock { get; set; } = TechType.None;
        public bool UnlocksAtStart => this.RequiredForUnlock == TechType.None;

        public virtual RecipeData GetBlueprintRecipe()
        {
            var partsList = new List<Ingredient>();

            CreateIngredients(this.Parts, partsList);

            if (partsList.Count == 0)
                partsList.Add(new Ingredient(TechType.Titanium, 1));

            var batteryBlueprint = new RecipeData
            {
                craftAmount = 1,
                Ingredients = partsList
            };

            return batteryBlueprint;
        }

        public float PowerCapacity { get; set; }

        public string FriendlyName { get; set; }

        public string Description { get; set; }

        public string IconFileName { get; set; }

        public string PluginPackName { get; set; }

        public string PluginFolder { get; set; }

        public Sprite Sprite { get; set; }

        public IList<TechType> Parts { get; set; }

        public bool IsPatched { get; private set; }

        public bool UsingIonCellSkins { get; }

        public Texture2D CustomSkin { get; set; }

        public bool ExcludeFromChargers { get; set; }

        protected CbCore(string classId, bool ionCellSkins)
            : base(classId, $"{classId}PreFab", TechType.None)
        {
            this.UsingIonCellSkins = ionCellSkins;
        }

        protected CbCore(CbItem packItem)
            : base(packItem.ID, $"{packItem.ID}PreFab", TechType.None)
        {
            this.UsingIonCellSkins = packItem.CustomSkin == null;

            if (packItem.CustomIcon != null)
                this.Sprite = packItem.CustomIcon;

            if (packItem.CustomSkin != null)
                this.CustomSkin = packItem.CustomSkin;

            this.ExcludeFromChargers = packItem.ExcludeFromChargers;
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.PrefabType);
            var obj = GameObject.Instantiate(prefab);

            Battery battery = obj.GetComponent<Battery>();
            battery._capacity = this.PowerCapacity;
            battery.name = $"{this.ClassID}BatteryCell";


            if (this.CustomSkin != null)
            {
                MeshRenderer meshRenderer = obj.GetComponentInChildren<MeshRenderer>();
                if (meshRenderer != null)
                    meshRenderer.material.mainTexture = this.CustomSkin;

                SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                    skinnedMeshRenderer.material.mainTexture = this.CustomSkin;
            }

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

            if (this.CustomSkin != null)
            {
                if (this.ChargerType == EquipmentType.BatteryCharger && !BatteryModels.ContainsKey(this.TechType))
                {
                    BatteryModels.Add(this.TechType, this.CustomSkin);
                }
                else if (this.ChargerType == EquipmentType.PowerCellCharger && !PowerCellModels.ContainsKey(this.TechType))
                {
                    PowerCellModels.Add(this.TechType, this.CustomSkin);
                }
            }
            else if (this.UsingIonCellSkins)
            {
                if (this.ChargerType == EquipmentType.BatteryCharger)
                {
                    GameObject battery = worldEntities.IonBattery();
                    Texture2D texture = battery?.GetComponentInChildren<MeshRenderer>()?.material?.GetTexture(ShaderPropertyID._MainTex) as Texture2D;
                    if (texture != null)
                    {
                        BatteryModels.Add(this.TechType, texture);
                    }
                }
                else if (this.ChargerType == EquipmentType.PowerCellCharger)
                {
                    GameObject battery = worldEntities.IonPowerCell();
                    Texture2D texture = battery?.GetComponentInChildren<MeshRenderer>()?.material?.GetTexture(ShaderPropertyID._MainTex) as Texture2D;
                    if (texture != null)
                    {
                        BatteryModels.Add(this.TechType, texture);
                    }
                }
            }
            else
            {
                if (this.ChargerType == EquipmentType.BatteryCharger)
                {
                    GameObject battery = worldEntities.Battery();
                    Texture2D texture = battery?.GetComponentInChildren<MeshRenderer>()?.material?.GetTexture(ShaderPropertyID._MainTex) as Texture2D;
                    if (texture != null)
                    {
                        BatteryModels.Add(this.TechType, texture);
                    }
                }
                else if (this.ChargerType == EquipmentType.PowerCellCharger)
                {
                    GameObject battery = worldEntities.PowerCell();
                    Texture2D texture = battery?.GetComponentInChildren<MeshRenderer>()?.material?.GetTexture(ShaderPropertyID._MainTex) as Texture2D;
                    if (texture != null)
                    {
                        BatteryModels.Add(this.TechType, texture);
                    }
                }
            }

            if (!this.UnlocksAtStart)
                KnownTechHandler.SetAnalysisTechEntry(this.RequiredForUnlock, new TechType[] { this.TechType });

            if (this.Sprite == null)
            {
                string imageFilePath = IOUtilities.Combine(ExecutingFolder, this.PluginFolder, this.IconFileName);

                if (File.Exists(imageFilePath))
                    this.Sprite = ImageUtils.LoadSpriteFromFile(imageFilePath);
                else
                {
                    QuickLogger.Warning($"Did not find a matching image file at {imageFilePath}.{Environment.NewLine}Using default sprite instead.");
                    this.Sprite = SpriteManager.Get(this.PrefabType);
                }
            }

            SpriteHandler.RegisterSprite(this.TechType, this.Sprite);

            CraftDataHandler.SetTechData(this.TechType, GetBlueprintRecipe());

            CraftDataHandler.AddToGroup(TechGroup.Resources, TechCategory.Electronics, this.TechType);

            CraftDataHandler.SetEquipmentType(this.TechType, this.ChargerType);

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, this.TechType, this.StepsToFabricatorTab);

            PrefabHandler.RegisterPrefab(this);

            AddToList();

            this.IsPatched = true;
        }
    }
}
