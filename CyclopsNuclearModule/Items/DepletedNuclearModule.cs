namespace CyclopsNuclearUpgrades
{
    using CyclopsNuclearUpgrades.Management;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class DepletedNuclearModule : Spawnable
    {
        private readonly CyclopsNuclearModule nuclearModule;

        private const string DepletedEventKey = "CyNukeDepleted";
        private static string DepletedEventMsg => Language.main.Get(DepletedEventKey);

        public override string AssetsFolder { get; } = "CyclopsNuclearUpgrades/Assets";

        public DepletedNuclearModule(CyclopsNuclearModule module)
            : base("DepletedCyclopsNuclearModule",
                   "Depleted Cyclops Nuclear Reactor Module",
                   "Nuclear waste.")
        {
            nuclearModule = module;

            OnStartedPatching += () =>
            {
                if (!nuclearModule.IsPatched)
                    nuclearModule.Patch();
            };

            OnFinishedPatching += () =>
            {
                LanguageHandler.SetLanguageLine(DepletedEventKey, "Nuclear Reactor Module depleted");
                MCUServices.Client.RegisterUpgradeCreator((SubRoot cyclops) => 
                {
                    return new NuclearUpgradeHandler(nuclearModule.TechType, DepleteNuclearModule, cyclops);
                });
                MCUServices.Client.RegisterChargerCreator((SubRoot cyclops) =>
                {
                    return new NuclearChargeHandler(cyclops, nuclearModule.TechType);
                });
            };
        }

        public override GameObject GetGameObject()
        {
            return GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.DepletedReactorRod));
        }

        internal void DepleteNuclearModule(Equipment modules, string slotName)
        {
            InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
            GameObject.Destroy(inventoryItem.item.gameObject);
            modules.AddItem(slotName, CyclopsUpgrade.SpawnCyclopsModule(this.TechType), true);
            ErrorMessage.AddMessage(DepletedEventMsg);
        }
    }
}
