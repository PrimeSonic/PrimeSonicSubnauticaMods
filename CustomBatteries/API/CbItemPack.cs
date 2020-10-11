namespace CustomBatteries.API
{
    using Common;
    using CustomBatteries.Items;
    using SMLHelper.V2.Assets;

    /// <summary>
    /// A container class that a holds the modded prefab objects for your modded item.
    /// </summary>
    public class CbItemPack
    {
        public string Name { get; }

        internal CbCore CbCoreItem { get; }

        public CbItem OriginalItemData { get; internal set; }

        public ModPrefab ItemPrefab => CbCoreItem;

        public bool IsPatched => CbCoreItem.IsPatched == true;

        internal CbItemPack(string pluginPackName, CbItem originalItemData, ItemTypes itemType)
        {
            this.Name = pluginPackName;
            this.CbCoreItem = new CustomItem(originalItemData, itemType)
            {
                PluginPackName = pluginPackName,
                FriendlyName = originalItemData.Name,
                Description = originalItemData.FlavorText,
                PowerCapacity = originalItemData.EnergyCapacity,
                RequiredForUnlock = originalItemData.UnlocksWith,
                Parts = originalItemData.CraftingMaterials
            };
            this.OriginalItemData = originalItemData;
        }

        internal void Patch()
        {
            QuickLogger.Info($"Patching '{OriginalItemData.ID}' from '{Name}'");

            CbCoreItem.Patch();
        }
    }
}
