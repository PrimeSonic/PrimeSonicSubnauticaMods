namespace MoreCyclopsUpgrades.Modules
{
    using Common;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using System.Collections.Generic;
    using UnityEngine;

    // TODO - Delete this class and move all upgrades into their own (optional) mods
    internal abstract class CyclopsModule : ModPrefab
    {
        private static readonly List<CyclopsModule> ModulesToPatch = new List<CyclopsModule>();

        internal static bool ModulesEnabled { get; private set; } = true;

        private const string MaxThermalReachedKey = "MaxThermalMsg";
        internal static string MaxThermalReached()
        {
            return Language.main.Get(MaxThermalReachedKey);
        }
      
        public static TechType ThermalChargerMk2ID { get; protected set; } = TechType.UnusedOld;

        public readonly string NameID;
        public readonly string FriendlyName;
        public readonly string Description;
        public readonly TechType RequiredForUnlock;
        public readonly CraftTree.Type Fabricator;
        public readonly string[] FabricatorTabs;

        protected readonly TechType PreFabTemplate;

        private readonly bool AddToCraftTree;

        protected CyclopsModule(string nameID, string friendlyName, string description, CraftTree.Type fabricator, string[] fabricatorTab, TechType requiredAnalysisItem = TechType.None, TechType preFabTemplate = TechType.CyclopsThermalReactorModule)
            : base(nameID, $"{nameID}PreFab")
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;
            RequiredForUnlock = requiredAnalysisItem;
            Fabricator = fabricator;
            FabricatorTabs = fabricatorTab ?? new string[0];

            AddToCraftTree = true;
            PreFabTemplate = preFabTemplate;
        }

        protected CyclopsModule(string nameID, string friendlyName, string description, TechType requiredAnalysisItem = TechType.None, TechType preFabTemplate = TechType.CyclopsThermalReactorModule)
            : base(nameID, $"{nameID}PreFab")
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;
            RequiredForUnlock = requiredAnalysisItem;

            AddToCraftTree = false;
            PreFabTemplate = preFabTemplate;
        }

        protected virtual void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, RequiredForUnlock == TechType.None);

            if (!ModulesEnabled) // Even if the options have this be disabled,
                return; // we still want to run through the AddTechType methods to prevent mismatched TechTypeIDs as these settings are switched

            if (RequiredForUnlock == TechType.None)
                KnownTechHandler.UnlockOnStart(this.TechType);
            else
                KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");

            PrefabHandler.RegisterPrefab(this);

            SpriteHandler.RegisterSprite(this.TechType, $"./QMods/MoreCyclopsUpgrades/Assets/{NameID}.png");

            if (AddToCraftTree)
            {
                CraftDataHandler.SetTechData(this.TechType, GetRecipe());
                QuickLogger.Debug($"Setting crafting node for {this.ClassID}");
                CraftTreeHandler.AddCraftingNode(Fabricator, this.TechType, FabricatorTabs);
            }

            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.CyclopsModule);
            CraftDataHandler.AddToGroup(TechGroup.Cyclops, TechCategory.CyclopsUpgrades, this.TechType);

            SetStaticTechTypeID(this.TechType);
        }

        protected abstract void SetStaticTechTypeID(TechType techTypeID);

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(PreFabTemplate);
            var obj = GameObject.Instantiate(prefab);

            return obj;
        }

        protected abstract TechData GetRecipe();

        internal static void PatchAllModules(bool modulesEnabled)
        {
            ModulesEnabled = modulesEnabled;

            foreach (CyclopsModule module in ModulesToPatch)
            {
                QuickLogger.Debug($"Patching {module.NameID}");
                module.Patch();
            }

            
            LanguageHandler.SetLanguageLine(MaxThermalReachedKey, "Max number of thermal chargers reached.");
            
        }


    }
}