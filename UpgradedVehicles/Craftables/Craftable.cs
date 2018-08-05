namespace UpgradedVehicles
{
    using System.Collections.Generic;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;

    internal abstract class Craftable : ModPrefab
    {
        private static readonly List<Craftable> Items = new List<Craftable>(MTechType.Count);
        private static readonly EmUnlockConfig Config = new EmUnlockConfig();

        internal static T AddForPatching<T>(T item) where T : Craftable
        {
            Items.Add(item);
            return item;
        }

        internal static void PatchAll()
        {
            foreach (Craftable item in Items)
                item.Patch();
        }

        protected readonly string NameID;
        protected readonly string FriendlyName;
        protected readonly string Description;

        protected readonly TechType PrefabTemplate;

        protected readonly CraftTree.Type FabricatorType;
        protected readonly string FabricatorTab;

        protected readonly TechType RequiredForUnlock;
        protected readonly TechGroup GroupForPDA;
        protected readonly TechCategory CategoryForPDA;

        public bool IsPatched { get; protected set; } = false;
        protected bool PatchTechTypeOnly { get; set; } = false;

        protected readonly Craftable Prerequisite;

        protected Craftable(
            string nameID,
            string friendlyName,
            string description,
            TechType template,
            CraftTree.Type fabricatorType,
            string fabricatorTab,
            TechType requiredAnalysis,
            TechGroup groupForPDA,
            TechCategory categoryForPDA,
            Craftable prerequisite = null)
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

            Prerequisite = prerequisite;
        }

        protected virtual void PrePatch() { }

        private void Patch()
        {
            if (IsPatched)
                return; // Already patched. Skip all this.

            if (Prerequisite != null && !Prerequisite.IsPatched)
                Prerequisite.Patch(); // Go and patch the prerequisite craftable first

            PrePatch(); // Run any prepatch overrides

            if (PatchTechTypeOnly) // Register just the TechType to preserve the ID.
            {
                this.TechType = TechTypeHandler.AddTechType(NameID, FriendlyName, Description, false);
            }
            else // Full patching
            {
                this.TechType = TechTypeHandler.AddTechType(
                                                internalName: NameID,
                                                displayName: FriendlyName,
                                                tooltip: Description,
                                                sprite: ImageUtils.LoadSpriteFromFile($"./QMods/UpgradedVehicles/Assets/{NameID}.png"),
                                                unlockAtStart: false);

                CraftTreeHandler.AddCraftingNode(FabricatorType, this.TechType, FabricatorTab);
                CraftDataHandler.SetTechData(this.TechType, GetRecipe());

                PrefabHandler.RegisterPrefab(this);

                if (Config.ForceUnlockAtStart)
                    KnownTechHandler.UnlockOnStart(this.TechType);
                else
                    KnownTechHandler.SetAnalysisTechEntry(RequiredForUnlock, new TechType[1] { this.TechType }, $"{FriendlyName} blueprint discovered!");

                CraftDataHandler.AddToGroup(GroupForPDA, CategoryForPDA, this.TechType);
            }

            PostPatch(); // Run any postpatch overrides

            IsPatched = true;
        }

        protected abstract void PostPatch();

        protected abstract TechData GetRecipe();

        public override GameObject GetGameObject()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(PrefabTemplate);
            return GameObject.Instantiate(prefab);
        }
    }
}
