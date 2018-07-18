namespace MoreCyclopsUpgrades
{
    using System;
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal enum CyclopsModules : int
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

    internal class SortedCyclopsModules : SortedList<CyclopsModules, CyclopsModule>
    {
        public SortedCyclopsModules()
        {
        }

        public SortedCyclopsModules(int capacity) : base(capacity)
        {
        }

        internal void Add(CyclopsModule module) => Add(module.ModuleID, module);
    }

    internal abstract class CyclopsModule
    {
        private static readonly SortedCyclopsModules CyclopsModulesList = new SortedCyclopsModules(8);
        private static readonly Dictionary<TechType, CyclopsModules> TechTypeToModuleID = new Dictionary<TechType, CyclopsModules>(8);
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

        public abstract CyclopsModules ModuleID { get; }

        private readonly bool AddToCraftTree;

        protected CyclopsModule(string nameID, string friendlyName, string description, CraftTree.Type fabricator, string[] fabricatorTab, TechType requiredAnalysisItem = TechType.None, TechType preFabTemplate = TechType.CyclopsThermalReactorModule)
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

            PrefabHandler.RegisterPrefab(GetPrefab());

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

        protected abstract ModPrefab GetPrefab();

        internal static void PatchAllModules(bool vehicleUpgradesInCyclopsFabricator, bool modulesEnabled)
        {
            ModulesEnabled = modulesEnabled;

            CyclopsModulesList.Add(new SolarCharger(vehicleUpgradesInCyclopsFabricator));
            CyclopsModulesList.Add(new SolarChargerMk2());
            CyclopsModulesList.Add(new ThermalChargerMk2());
            CyclopsModulesList.Add(new PowerUpgradeMk2());
            CyclopsModulesList.Add(new PowerUpgradeMk3());
            CyclopsModulesList.Add(new CyclopsSpeedBooster(vehicleUpgradesInCyclopsFabricator));
            CyclopsModulesList.Add(new NuclearCharger());
            CyclopsModulesList.Add(new DepletedNuclearModule());

            foreach (KeyValuePair<CyclopsModules, CyclopsModule> module in CyclopsModulesList)
            {
                Console.WriteLine($"[MoreCyclopsUpgrades] Patching {module.Value.NameID} ");
                module.Value.Patch();
                TechTypeToModuleID.Add(module.Value.TechTypeID, module.Key);
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
                if (!TechTypeToModuleID.ContainsKey(techTypeID))
                    return null; // error condition

                // Get the CyclopsModule child class instance associated to this TechType
                CyclopsModules moduleID = TechTypeToModuleID[techTypeID];
                CyclopsModule cyclopsModule = CyclopsModulesList[moduleID];

                // Instantiate a new prefab of the appripriate template TechType
                gameObject = GameObject.Instantiate(CraftData.GetPrefabForTechType(cyclopsModule.PreFabTemplate));
                var ider = gameObject.GetComponent<PrefabIdentifier>();

                // Set the TechType value on the TechTag
                var tag = gameObject.GetComponent<TechTag>();
                if (tag != null)
                    tag.type = techTypeID;
                else // Add if needed since this is how these are identified throughout the mod
                    gameObject.AddComponent<TechTag>().type = techTypeID;

                // Set the class ID
                ider.ClassId = cyclopsModule.NameID;

                // If we're dealing with a module that has a battery component, add it.
                if (techTypeID == SolarChargerMk2ID)
                {
                    var pCell = gameObject.AddComponent<Battery>();
                    pCell.name = "SolarBackupBattery";
                    pCell._capacity = SolarChargerMk2.BatteryCapacity;
                }
                else if (techTypeID == ThermalChargerMk2ID)
                {
                    var pCell = gameObject.AddComponent<Battery>();
                    pCell.name = "ThermalBackupBattery";
                    pCell._capacity = ThermalChargerMk2.BatteryCapacity;
                }
                else if (techTypeID == NuclearChargerID)
                {
                    var pCell = gameObject.AddComponent<Battery>();
                    pCell.name = "NuclearBattery";
                    pCell._capacity = NuclearCharger.BatteryCapacity;
                }
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
