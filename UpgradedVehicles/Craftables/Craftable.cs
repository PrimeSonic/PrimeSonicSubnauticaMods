namespace UpgradedVehicles
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal abstract class Craftable : ModPrefab
    {
        public readonly string NameID;
        public readonly string FriendlyName;
        public readonly string Description;

        protected readonly TechType PrefabTemplate;

        protected readonly CraftTree.Type FabricatorType;
        protected readonly string FabricatorTab;

        protected readonly TechType RequiredForUnlock;
        protected readonly TechGroup GroupForPDA;
        protected readonly TechCategory CategoryForPDA;

        protected Craftable(
            string nameID, 
            string friendlyName, 
            string description, 
            TechType template, 
            CraftTree.Type fabricatorType,
            string fabricatorTab,
            TechType requiredAnalysis,
            TechGroup groupForPDA,
            TechCategory categoryForPDA)
            : base(nameID, $"{nameID}Prefab")
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;

            PrefabTemplate = template;
            FabricatorType = fabricatorType;
            FabricatorTab = fabricatorTab;

            RequiredForUnlock = requiredAnalysis;
            GroupForPDA = groupForPDA;
            CategoryForPDA = categoryForPDA;
        }

        public virtual void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(NameID,
                                                     FriendlyName,
                                                     Description,
                                                     ImageUtils.LoadSpriteFromFile($"./QMods/UpgradedVehicles/Assets/{NameID}.png"),
                                                     false);

            

            CraftTreeHandler.AddCraftingNode(FabricatorType, this.TechType, FabricatorTab);
            CraftDataHandler.SetTechData(this.TechType, GetRecipe());

            PrefabHandler.RegisterPrefab(this);

            KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");
            CraftDataHandler.AddToGroup(GroupForPDA, CategoryForPDA, this.TechType);
        }

        protected abstract TechData GetRecipe();

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(PrefabTemplate);
            GameObject obj = GameObject.Instantiate(prefab);

            return obj;
        }
    }
}
