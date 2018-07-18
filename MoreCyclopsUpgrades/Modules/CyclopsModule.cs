namespace MoreCyclopsUpgrades
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;

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
    }
}
