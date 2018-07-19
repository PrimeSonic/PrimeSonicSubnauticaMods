namespace MoreCyclopsUpgrades
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal enum CyclopsModules : int
    {
        Solar = 1,
        SolarMk2 = 2,
        ThermalMk2 = 3,
        PowerMk2 = 4,
        PowerMk3 = 5,
        Nuclear = 6,
        DepletedNuclear = 7
    }

    internal abstract class CyclopsModule
    {
        public static TechType SolarChargerID { get; protected set; }
        public static TechType SolarChargerMk2ID { get; protected set; }
        public static TechType ThermalChargerMk2ID { get; protected set; }
        public static TechType PowerUpgradeMk2ID { get; protected set; }
        public static TechType PowerUpgradeMk3ID { get; protected set; }
        public static TechType NuclearChargerID { get; protected set; }
        public static TechType DepletedNuclearModuleID { get; protected set; }
        public static TechType RefillNuclearModuleID { get; protected set; }

        public TechType TechTypeID { get; protected set; }

        public readonly string NameID;
        public readonly string FriendlyName;
        public readonly string Description;
        public readonly TechType RequiredForUnlock;
        public readonly CraftTree.Type Fabricator;
        public readonly string[] FabricatorTabs;

        public abstract CyclopsModules ModuleID { get; }

        private readonly bool AddToCraftTree;

        protected CyclopsModule(string nameID, string friendlyName, string description, CraftTree.Type fabricator, string[] fabricatorTab, TechType requiredAnalysisItem = TechType.None)
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;
            RequiredForUnlock = requiredAnalysisItem;
            Fabricator = fabricator;
            FabricatorTabs = fabricatorTab;

            AddToCraftTree = FabricatorTabs != null;
        }

        protected CyclopsModule(string nameID, string friendlyName, string description, TechType requiredAnalysisItem = TechType.None)
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;
            RequiredForUnlock = requiredAnalysisItem;

            AddToCraftTree = false;
        }

        public virtual void Patch()
        {
            TechTypeID = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, RequiredForUnlock == TechType.None);

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

        public static InventoryItem SpawnCyclopsModule(TechType techTypeID)
        {
            GameObject gameObject;

            if (techTypeID < TechType.Databox)
            {
                GameObject prefab = CraftData.GetPrefabForTechType(techTypeID);
                gameObject = GameObject.Instantiate(prefab);
            }
            else if (techTypeID == DepletedNuclearModuleID)
            {
                GameObject prefab = CraftData.GetPrefabForTechType(TechType.DepletedReactorRod);
                gameObject = GameObject.Instantiate(prefab);

                gameObject.GetComponent<PrefabIdentifier>().ClassId = DepletedNuclearModule.DepletedNameID;
                gameObject.AddComponent<TechTag>().type = CyclopsModule.DepletedNuclearModuleID;
            }
            else
            {
                GameObject prefab = CraftData.GetPrefabForTechType(TechType.CyclopsThermalReactorModule);
                gameObject = GameObject.Instantiate(prefab);

                var tag = gameObject.GetComponent<TechTag>();
                if (tag != null)
                    tag.type = techTypeID;
                else
                    gameObject.AddComponent<TechTag>().type = techTypeID;

                var ider = gameObject.GetComponent<PrefabIdentifier>();

                if (techTypeID == SolarChargerID)
                {
                    ider.ClassId = QPatch.CyclopsModules[CyclopsModules.Solar].NameID;
                }
                else if (techTypeID == SolarChargerMk2ID)
                {
                    ider.ClassId = QPatch.CyclopsModules[CyclopsModules.SolarMk2].NameID;

                    var pCell = gameObject.AddComponent<Battery>();
                    pCell.name = "SolarBackupBattery";
                    pCell._capacity = PowerCharging.MaxMk2Charge;
                }
                else if (techTypeID == ThermalChargerMk2ID)
                {
                    ider.ClassId = QPatch.CyclopsModules[CyclopsModules.ThermalMk2].NameID;

                    var pCell = gameObject.AddComponent<Battery>();
                    pCell.name = "ThermalBackupBattery";
                    pCell._capacity = PowerCharging.MaxMk2Charge;
                }
                else if (techTypeID == PowerUpgradeMk2ID)
                {
                    ider.ClassId = QPatch.CyclopsModules[CyclopsModules.PowerMk2].NameID;
                }
                else if (techTypeID == PowerUpgradeMk3ID)
                {
                    ider.ClassId = QPatch.CyclopsModules[CyclopsModules.PowerMk3].NameID;
                }
                else if (techTypeID == NuclearChargerID)
                {
                    ider.ClassId = QPatch.CyclopsModules[CyclopsModules.Nuclear].NameID;

                    var pCell = gameObject.AddComponent<Battery>();
                    pCell.name = "NuclearBattery";
                    pCell._capacity = PowerCharging.MaxNuclearCharge;
                }
                else
                {
                    return null; // error condition
                }
            }

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
