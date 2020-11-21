namespace CustomBatteries.Items
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        protected abstract TechType PrefabType { get; } // Should only ever be Battery, IonBattery, PowerCell or IonPowerCell
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

        public CBModelData CustomModelData { get; set; }

        protected Action<GameObject> EnhanceGameObject { get; set; }

        public bool AddToFabricator { get; set; } = true;

        protected CbCore(string classId, bool ionCellSkins)
            : base(classId, $"{classId}PreFab", TechType.None)
        {
            this.UsingIonCellSkins = ionCellSkins;
        }

        protected CbCore(CbItem packItem)
            : base(packItem.ID, $"{packItem.ID}PreFab", TechType.None)
        {
            if (packItem.CBModelData != null)
            {
                this.CustomModelData = packItem.CBModelData;
            }

            this.UsingIonCellSkins = packItem.CBModelData?.UseIonModelsAsBase ?? false;

            this.Sprite = packItem.CustomIcon;

            this.EnhanceGameObject = packItem.EnhanceGameObject;

            this.AddToFabricator = packItem.AddToFabricator;
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(this.PrefabType);
            var obj = GameObject.Instantiate(prefab);

            Battery battery = obj.GetComponent<Battery>();
            battery._capacity = this.PowerCapacity;
            battery.name = $"{this.ClassID}BatteryCell";

            // If "Enable batteries/powercells placement" feature from Decorations mod is ON.
#if SUBNAUTICA
            if (CbDatabase.PlaceBatteriesFeatureEnabled && CraftData.GetEquipmentType(this.TechType) != EquipmentType.Hand)
#elif BELOWZERO
            if (CbDatabase.PlaceBatteriesFeatureEnabled && TechData.GetEquipmentType(this.TechType) != EquipmentType.Hand)
#endif
            {
                CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.Hand); // Set equipment type to Hand.
                CraftDataHandler.SetQuickSlotType(this.TechType, QuickSlotType.Selectable); // We can select the item.
            }

            SkyApplier skyApplier = obj.EnsureComponent<SkyApplier>();
            skyApplier.renderers = obj.GetComponentsInChildren<Renderer>(true);
            skyApplier.anchorSky = Skies.Auto;

            if (CustomModelData != null)
            {
                foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>(true))
                {
                    if (CustomModelData.CustomTexture != null)
                        renderer.material.SetTexture(ShaderPropertyID._MainTex, this.CustomModelData.CustomTexture);

                    if (CustomModelData.CustomNormalMap != null)
                        renderer.material.SetTexture(ShaderPropertyID._BumpMap, this.CustomModelData.CustomNormalMap);

                    if (CustomModelData.CustomSpecMap != null)
                        renderer.material.SetTexture(ShaderPropertyID._SpecTex, this.CustomModelData.CustomSpecMap);

                    if (CustomModelData.CustomIllumMap != null)
                    {
                        renderer.material.SetTexture(ShaderPropertyID._Illum, this.CustomModelData.CustomIllumMap);
                        renderer.material.SetFloat(ShaderPropertyID._GlowStrength, this.CustomModelData.CustomIllumStrength);
                        renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, this.CustomModelData.CustomIllumStrength);
                    }
                }
            }

            this.EnhanceGameObject?.Invoke(obj);

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

            ProcessBatterySkins();

            if (!this.UnlocksAtStart)
                KnownTechHandler.SetAnalysisTechEntry(this.RequiredForUnlock, new TechType[] { this.TechType });

            if (this.Sprite == null)
            {
                string imageFilePath = null;

                if (this.PluginFolder != null && this.IconFileName != null)
                    imageFilePath = IOUtilities.Combine(CbDatabase.ExecutingFolder, this.PluginFolder, this.IconFileName);

                if (imageFilePath != null && File.Exists(imageFilePath))
                    this.Sprite = ImageUtils.LoadSpriteFromFile(imageFilePath);
                else
                {
                    QuickLogger.Warning($"Did not find a matching image file at {imageFilePath} or in {nameof(CbBattery.CustomIcon)}.{Environment.NewLine}Using default sprite instead.");
                    this.Sprite = SpriteManager.Get(this.PrefabType);
                }
            }

            SpriteHandler.RegisterSprite(this.TechType, this.Sprite);

            CraftDataHandler.SetTechData(this.TechType, GetBlueprintRecipe());

            CraftDataHandler.AddToGroup(TechGroup.Resources, TechCategory.Electronics, this.TechType);

            CraftDataHandler.SetEquipmentType(this.TechType, this.ChargerType);

            if (this.AddToFabricator)
                CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, this.TechType, this.StepsToFabricatorTab);

            PrefabHandler.RegisterPrefab(this);

            AddToList();

            this.IsPatched = true;
        }

        private void ProcessBatterySkins()
        {
            if (this.CustomModelData != null)
            {
                if (this.ChargerType == EquipmentType.BatteryCharger && !CbDatabase.BatteryModels.ContainsKey(this.TechType))
                {
                    CbDatabase.BatteryModels.Add(this.TechType, this.CustomModelData);
                }
                else if (this.ChargerType == EquipmentType.PowerCellCharger && !CbDatabase.PowerCellModels.ContainsKey(this.TechType))
                {
                    CbDatabase.PowerCellModels.Add(this.TechType, this.CustomModelData);
                }
            }
            else
            {
                if (this.ChargerType == EquipmentType.BatteryCharger)
                {
                    GameObject battery = CbDatabase.Battery();
                    Material material = battery?.GetComponentInChildren<MeshRenderer>()?.material;

                    Texture2D texture = material?.GetTexture(ShaderPropertyID._MainTex) as Texture2D;
                    Texture2D bumpmap = material?.GetTexture(ShaderPropertyID._BumpMap) as Texture2D;
                    Texture2D spec = material?.GetTexture(ShaderPropertyID._SpecTex) as Texture2D;
                    Texture2D illum = material?.GetTexture(ShaderPropertyID._Illum) as Texture2D;
                    float illumStrength = material.GetFloat(ShaderPropertyID._GlowStrength);

                    CbDatabase.BatteryModels.Add(this.TechType, this.CustomModelData);
                }
                else if (this.ChargerType == EquipmentType.PowerCellCharger)
                {
                    GameObject battery = CbDatabase.PowerCell();
                    Material material = battery?.GetComponentInChildren<MeshRenderer>()?.material;

                    Texture2D texture = material?.GetTexture(ShaderPropertyID._MainTex) as Texture2D;
                    Texture2D bumpmap = material?.GetTexture(ShaderPropertyID._BumpMap) as Texture2D;
                    Texture2D spec = material?.GetTexture(ShaderPropertyID._SpecTex) as Texture2D;
                    Texture2D illum = material?.GetTexture(ShaderPropertyID._Illum) as Texture2D;
                    float illumStrength = material.GetFloat(ShaderPropertyID._GlowStrength);

                    CbDatabase.PowerCellModels.Add(this.TechType, this.CustomModelData);
                }
            }
        }
    }
}
