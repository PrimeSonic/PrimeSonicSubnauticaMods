namespace MoreCyclopsUpgrades
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal enum ModuleTypes : int
    {
        ThermalMk2 = 1,
        Solar = 2,
        SolarMk2 = 3,
        PowerMk2 = 4,
        PowerMk3 = 5,
        Speed = 6,
        Nuclear = 7,
        DepletedNuclear = 8
    }

    internal abstract class CyclopsModule : ModPrefab
    {
        private static readonly SortedList<ModuleTypes, CyclopsModule> CyclopsModulesByModuleType = new SortedList<ModuleTypes, CyclopsModule>(8);
        private static readonly Dictionary<TechType, CyclopsModule> CyclopsModulesByTechType = new Dictionary<TechType, CyclopsModule>(8);
        internal static bool ModulesEnabled { get; private set; } = true;

        public static TechType SolarChargerID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit
        public static TechType SolarChargerMk2ID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit
        public static TechType ThermalChargerMk2ID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit
        public static TechType PowerUpgradeMk2ID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit
        public static TechType PowerUpgradeMk3ID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit
        public static TechType SpeedBoosterModuleID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit
        public static TechType NuclearChargerID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit
        public static TechType DepletedNuclearModuleID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit
        public static TechType RefillNuclearModuleID { get; protected set; } = TechType.Unobtanium; // Default value that shouldn't get hit

        public TechType TechTypeID { get; protected set; }

        public readonly string NameID;
        public readonly string FriendlyName;
        public readonly string Description;
        public readonly TechType RequiredForUnlock;
        public readonly CraftTree.Type Fabricator;
        public readonly string[] FabricatorTabs;

        protected readonly TechType PreFabTemplate;

        public abstract ModuleTypes ModuleID { get; }

        private readonly bool AddToCraftTree;

        protected CyclopsModule(string nameID, string friendlyName, string description, CraftTree.Type fabricator, string[] fabricatorTab, TechType requiredAnalysisItem = TechType.None, TechType preFabTemplate = TechType.CyclopsThermalReactorModule)
            : base(nameID, $"{nameID}PreFab")
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;
            RequiredForUnlock = requiredAnalysisItem;
            Fabricator = fabricator;
            FabricatorTabs = fabricatorTab;

            AddToCraftTree = FabricatorTabs != null;
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

        public virtual void Patch()
        {
            TechTypeID = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, RequiredForUnlock == TechType.None);

            if (!ModulesEnabled) // Even if the options have this be disabled,
                return; // we still want to run through the AddTechType methods to prevent mismatched TechTypeIDs as these settings are switched

            if (RequiredForUnlock == TechType.None)
                KnownTechHandler.UnlockOnStart(TechTypeID);
            else
                KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { TechTypeID }, $"{FriendlyName} blueprint discovered!");

            PrefabHandler.RegisterPrefab(this);

            SpriteHandler.RegisterSprite(TechTypeID, $"./QMods/MoreCyclopsUpgrades/Assets/{NameID}.png");

            CraftDataHandler.SetTechData(TechTypeID, GetRecipe());

            if (AddToCraftTree)
                CraftTreeHandler.AddCraftingNode(Fabricator, TechTypeID, FabricatorTabs);

            CraftDataHandler.SetEquipmentType(TechTypeID, EquipmentType.CyclopsModule);
            CraftDataHandler.AddToGroup(TechGroup.Cyclops, TechCategory.CyclopsUpgrades, TechTypeID);

            SetStaticTechTypeID(TechTypeID);
        }

        protected abstract void SetStaticTechTypeID(TechType techTypeID);

        protected abstract TechData GetRecipe();

        internal static void PatchAllModules(bool vehicleUpgradesInCyclopsFabricator, bool modulesEnabled)
        {
            ModulesEnabled = modulesEnabled;

            CyclopsModulesByModuleType.Add(ModuleTypes.Solar, new SolarCharger(vehicleUpgradesInCyclopsFabricator));
            CyclopsModulesByModuleType.Add(ModuleTypes.SolarMk2, new SolarChargerMk2());
            CyclopsModulesByModuleType.Add(ModuleTypes.ThermalMk2, new ThermalChargerMk2());
            CyclopsModulesByModuleType.Add(ModuleTypes.PowerMk2, new PowerUpgradeMk2());
            CyclopsModulesByModuleType.Add(ModuleTypes.PowerMk3, new PowerUpgradeMk3());
            CyclopsModulesByModuleType.Add(ModuleTypes.Speed, new CyclopsSpeedBooster(vehicleUpgradesInCyclopsFabricator));
            CyclopsModulesByModuleType.Add(ModuleTypes.Nuclear, new NuclearCharger());
            CyclopsModulesByModuleType.Add(ModuleTypes.DepletedNuclear, new DepletedNuclearModule());

            foreach (KeyValuePair<ModuleTypes, CyclopsModule> module in CyclopsModulesByModuleType)
            {
                Console.WriteLine($"[MoreCyclopsUpgrades] Patching {module.Value.NameID} ");
                module.Value.Patch();
                CyclopsModulesByTechType.Add(module.Value.TechTypeID, module.Value);
            }
        }

        public static InventoryItem SpawnCyclopsModule(TechType techTypeID)
        {
            GameObject gameObject;

            if (techTypeID < TechType.Databox) // This is a standard upgrade module
            {
                gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(techTypeID));
            }
            else if (ModulesEnabled) // Safety check in case these are disabled in the config
            {
                if (!CyclopsModulesByTechType.ContainsKey(techTypeID))
                    return null; // error condition

                // Get the CyclopsModule child class instance associated to this TechType
                CyclopsModule cyclopsModule = CyclopsModulesByTechType[techTypeID];

                // Instantiate a new prefab of the appripriate template TechType
                gameObject = cyclopsModule.GetGameObject();
                var ider = gameObject.GetComponent<PrefabIdentifier>();

                // Set the TechType value on the TechTag
                var tag = gameObject.GetComponent<TechTag>();
                if (tag != null)
                    tag.type = techTypeID;
                else // Add if needed since this is how these are identified throughout the mod
                    gameObject.AddComponent<TechTag>().type = techTypeID;

                // Set the class ID
                ider.ClassId = cyclopsModule.NameID;
            }
            else
            {
                return null; // error condition
            }

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
