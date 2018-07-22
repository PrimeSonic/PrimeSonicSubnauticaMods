namespace UpgradedVehicles.Craftables
{
    using System.Collections.Generic;
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

        protected readonly TechType TemplateTechType;

        protected Craftable(string nameID, string friendlyName, string description, TechType templateTechType)
            : base(nameID, $"{nameID}Prefab")
        {
            NameID = nameID;
            FriendlyName = friendlyName;
            Description = description;

            TemplateTechType = templateTechType;
        }

        public void Patch()
        {
            this.TechType = TechTypeHandler.AddTechType(NameID,
                                                     FriendlyName,
                                                     Description,
                                                     ImageUtils.LoadSpriteFromFile(@"./QMods/UpgradedVehicles/Assets/VehiclePowerCore.png"),
                                                     false);

            CraftTreeHandler.AddCraftingNode(CraftTree.Type.SeamothUpgrades, this.TechType, "CommonModules");
            CraftDataHandler.SetTechData(this.TechType, GetRecipe());

            PrefabHandler.RegisterPrefab(this);
            CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.None);

            KnownTechHandler.SetAnalysisTechEntry(TechType.BaseUpgradeConsole, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");
            CraftDataHandler.AddToGroup(TechGroup.Resources, TechCategory.Electronics, this.TechType);
        }

        protected abstract TechData GetRecipe();

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TemplateTechType);
            GameObject obj = GameObject.Instantiate(prefab);

            return obj;
        }
    }
}
