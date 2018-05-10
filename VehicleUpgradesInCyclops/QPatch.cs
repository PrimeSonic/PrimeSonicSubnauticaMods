namespace VehicleUpgradeInCyclops
{    
    using SMLHelper; // https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;

#if DEBUG
    using System;
    using Logger = Utilites.Logger.Logger;
    // TODO Custom sprites for an look and feel more similar to the actual Vehicle Upgrade Console
#endif
    public class QPatch
    {
        public static void Patch()
        {
#if DEBUG
            try
            {
#endif
                // Small Vehicle Upgrades (root tab to everything below)
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("VehicleUpgrades", "Small Vehicle Upgrades", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.BaseUpgradeConsole)));

                // Common Modules
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("VehicleUpgrades/CommonModules", "Common Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.VehicleStorageModule)));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.VehicleArmorPlating, CraftScheme.CyclopsFabricator, "VehicleUpgrades/CommonModules/VehicleArmorPlating"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.VehiclePowerUpgradeModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/CommonModules/VehiclePowerUpgradeModule"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.VehicleStorageModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/CommonModules/VehicleStorageModule"));

                // Seamoth Modules
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("VehicleUpgrades/SeamothModules", "Seamoth Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.Seamoth)));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.VehicleHullModule1, CraftScheme.CyclopsFabricator, "VehicleUpgrades/SeamothModules/VehicleHullModule1"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.SeamothSolarCharge, CraftScheme.CyclopsFabricator, "VehicleUpgrades/SeamothModules/SeamothSolarCharge"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.SeamothElectricalDefense, CraftScheme.CyclopsFabricator, "VehicleUpgrades/SeamothModules/SeamothElectricalDefense"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.SeamothTorpedoModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/SeamothModules/SeamothTorpedoModule"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.SeamothSonarModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/SeamothModules/SeamothSonarModule"));

                // Prawn Suit Modules
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("VehicleUpgrades/PrawnModules", "Prawn Suit Modules", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.Exosuit)));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.ExoHullModule1, CraftScheme.CyclopsFabricator, "VehicleUpgrades/PrawnModules/ExoHullModule1"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.ExosuitThermalReactorModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/PrawnModules/ExosuitThermalReactorModule"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.ExosuitJetUpgradeModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/PrawnModules/ExosuitJetUpgradeModule"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.ExosuitPropulsionArmModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/PrawnModules/ExosuitPropulsionArmModule"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.ExosuitGrapplingArmModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/PrawnModules/ExosuitGrapplingArmModule"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.ExosuitDrillArmModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/PrawnModules/ExosuitDrillArmModule"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.ExosuitTorpedoArmModule, CraftScheme.CyclopsFabricator, "VehicleUpgrades/PrawnModules/ExosuitTorpedoArmModule"));

                // Torpedoes
                CraftTreePatcher.customTabs.Add(new CustomCraftTab("VehicleUpgrades/Torpedoes", "Torpedoes", CraftScheme.CyclopsFabricator, SpriteManager.Get(TechType.SeamothTorpedoModule)));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.WhirlpoolTorpedo, CraftScheme.CyclopsFabricator, "VehicleUpgrades/Torpedoes/WhirlpoolTorpedo"));
                CraftTreePatcher.customNodes.Add(new CustomCraftNode(TechType.GasTorpedo, CraftScheme.CyclopsFabricator, "VehicleUpgrades/Torpedoes/GasTorpedo"));
#if DEBUG
            }
            catch (Exception ex)
            {
                Logger.Error("Error on load" + ex.ToString());
            }
#endif
        }

    }
}
