namespace CustomCraft2SML.PublicAPI
{
    using System.Text;

    public static class PathHelper
    {
        public static string GeneratePaths()
        {
            var builder = new StringBuilder();

            builder.AppendLine();
            builder.AppendLine("# Mobile Vehicle Bay #");
            builder.AppendLine(MobileVehicleBay.ConstructorScheme.GetCraftingPath.ToString());
            builder.AppendLine(MobileVehicleBay.Vehicles.VehiclesTab.GetCraftingPath.ToString());
            builder.AppendLine(MobileVehicleBay.NeptuneRocket.RocketTab.GetCraftingPath.ToString());
            builder.AppendLine();
            builder.AppendLine("# Cyclops Fabricator #");
            builder.AppendLine(CyclopsFabricator.CyclopsFabricatorScheme.GetCraftingPath.ToString());
            builder.AppendLine();
            builder.AppendLine("# Fabricator #");
            builder.AppendLine(Fabricator.FabricatorScheme.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Resources.ResourcesTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Resources.BasicMaterials.BasicMaterialsTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Resources.AdvancedMaterials.AdvancedMaterialsTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Resources.Electronics.ElectronicsTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Sustenance.SurvivalTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Sustenance.Water.WaterTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Sustenance.CookedFood.CookedFoodTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Sustenance.CuredFood.CuredFoodTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Personal.PersonalTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Personal.Equipment.EquipmentTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Personal.Tools.ToolsTab.GetCraftingPath.ToString());
            builder.AppendLine(Fabricator.Deployables.MachinesTab.GetCraftingPath.ToString());
            builder.AppendLine();
            builder.AppendLine("# Scanner Room #");
            builder.AppendLine(ScannerRoom.MapRoomSheme.GetCraftingPath.ToString());
            builder.AppendLine();
            builder.AppendLine("# Vehicle Upgrade Console #");
            builder.AppendLine(VehicleUpgradeConsole.SeamothUpgradesScheme.GetCraftingPath.ToString());
            builder.AppendLine(VehicleUpgradeConsole.CommonModules.CommonModulesTab.GetCraftingPath.ToString());
            builder.AppendLine(VehicleUpgradeConsole.SeamothModules.SeamothModulesTab.GetCraftingPath.ToString());
            builder.AppendLine(VehicleUpgradeConsole.PrawnSuitModules.ExosuitModulesTab.GetCraftingPath.ToString());
            builder.AppendLine(VehicleUpgradeConsole.Torpedoes.TorpedoesTab.GetCraftingPath.ToString());
            builder.AppendLine();
            builder.AppendLine("# Modification Station #");
            builder.AppendLine(ModificationStation.WorkbenchScheme.GetCraftingPath.ToString());
            builder.AppendLine(ModificationStation.SurvivalKnifeUpgrades.KnifeMenuTab.GetCraftingPath.ToString());
            builder.AppendLine(ModificationStation.AirTankUpgrades.TankMenuTab.GetCraftingPath.ToString());
            builder.AppendLine(ModificationStation.FinUpgrades.FinsMenuTab.GetCraftingPath.ToString());
            builder.AppendLine(ModificationStation.PropulsionCannonUpgrades.PropulsionCannonMenuTab.GetCraftingPath.ToString());
            builder.AppendLine(ModificationStation.CyclopsUpgrades.CyclopsMenuTab.GetCraftingPath.ToString());
            builder.AppendLine(ModificationStation.SeamothUpgrades.SeamothMenuTab.GetCraftingPath.ToString());
            builder.AppendLine(ModificationStation.PrawnSuitUpgrades.ExosuitMenuTab.GetCraftingPath.ToString());
            builder.AppendLine();

            return builder.ToString();
        }

        public static class MobileVehicleBay
        {
            public static readonly CraftingRoot ConstructorScheme = new CraftingRoot(CraftTree.Type.Constructor);

            public static class Vehicles
            {
                public static readonly CraftingTab VehiclesTab = new CraftingTab(ConstructorScheme, "Vehicles");
            }

            public static class NeptuneRocket
            {
                public static readonly CraftingTab RocketTab = new CraftingTab(ConstructorScheme, "Rocket");
            }
        }

        public static class CyclopsFabricator
        {
            public static readonly CraftingRoot CyclopsFabricatorScheme = new CraftingRoot(CraftTree.Type.CyclopsFabricator);
        }

        public static class Fabricator
        {
            public static readonly CraftingRoot FabricatorScheme = new CraftingRoot(CraftTree.Type.Fabricator);

            public static class Resources
            {
                public static readonly CraftingTab ResourcesTab = new CraftingTab(FabricatorScheme, "Resources");

                public static class BasicMaterials
                {
                    public static readonly CraftingTab BasicMaterialsTab = new CraftingTab(ResourcesTab, "BasicMaterials");
                }

                public static class AdvancedMaterials
                {
                    public static readonly CraftingTab AdvancedMaterialsTab = new CraftingTab(ResourcesTab, "AdvancedMaterials");
                }

                public static class Electronics
                {
                    public static readonly CraftingTab ElectronicsTab = new CraftingTab(ResourcesTab, "Electronics");
                }
            }

            public static class Sustenance
            {
                public static readonly CraftingTab SurvivalTab = new CraftingTab(FabricatorScheme, "Survival");

                public static class Water
                {
                    public static readonly CraftingTab WaterTab = new CraftingTab(SurvivalTab, "Water");
                }

                public static class CookedFood
                {
                    public static readonly CraftingTab CookedFoodTab = new CraftingTab(SurvivalTab, "CookedFood");
                }

                public static class CuredFood
                {
                    public static readonly CraftingTab CuredFoodTab = new CraftingTab(SurvivalTab, "CuredFood");
                }
            }

            public static class Personal
            {
                public static readonly CraftingTab PersonalTab = new CraftingTab(FabricatorScheme, "Personal");

                public static class Equipment
                {
                    public static readonly CraftingTab EquipmentTab = new CraftingTab(PersonalTab, "Equipment");
                }

                public static class Tools
                {
                    public static readonly CraftingTab ToolsTab = new CraftingTab(PersonalTab, "Tools");
                }
            }

            public static class Deployables
            {
                public static readonly CraftingTab MachinesTab = new CraftingTab(FabricatorScheme, "Machines");
            }
        }

        public static class ScannerRoom
        {
            public static readonly CraftingRoot MapRoomSheme = new CraftingRoot(CraftTree.Type.MapRoom);
        }

        public static class VehicleUpgradeConsole
        {
            public static readonly CraftingRoot SeamothUpgradesScheme = new CraftingRoot(CraftTree.Type.SeamothUpgrades);

            public static class CommonModules
            {
                public static readonly CraftingTab CommonModulesTab = new CraftingTab(SeamothUpgradesScheme, "CommonModules");
            }

            public static class SeamothModules
            {
                public static readonly CraftingTab SeamothModulesTab = new CraftingTab(SeamothUpgradesScheme, "SeamothModules");
            }

            public static class PrawnSuitModules
            {
                public static readonly CraftingTab ExosuitModulesTab = new CraftingTab(SeamothUpgradesScheme, "ExosuitModules");
            }

            public static class Torpedoes
            {
                public static readonly CraftingTab TorpedoesTab = new CraftingTab(SeamothUpgradesScheme, "CommonModules");
            }
        }

        public static class ModificationStation
        {
            public static readonly CraftingRoot WorkbenchScheme = new CraftingRoot(CraftTree.Type.Workbench);

            public static class SurvivalKnifeUpgrades
            {
                public static readonly CraftingTab KnifeMenuTab = new CraftingTab(WorkbenchScheme, "KnifeMenu");
            }

            public static class AirTankUpgrades
            {
                public static readonly CraftingTab TankMenuTab = new CraftingTab(WorkbenchScheme, "TankMenu");
            }

            public static class FinUpgrades
            {
                public static readonly CraftingTab FinsMenuTab = new CraftingTab(WorkbenchScheme, "FinsMenu");
            }

            public static class PropulsionCannonUpgrades
            {
                public static readonly CraftingTab PropulsionCannonMenuTab = new CraftingTab(WorkbenchScheme, "PropulsionCannonMenu");
            }

            public static class CyclopsUpgrades
            {
                public static readonly CraftingTab CyclopsMenuTab = new CraftingTab(WorkbenchScheme, "CyclopsMenu");
            }

            public static class SeamothUpgrades
            {
                public static readonly CraftingTab SeamothMenuTab = new CraftingTab(WorkbenchScheme, "SeamothMenu");
            }

            public static class PrawnSuitUpgrades
            {
                public static readonly CraftingTab ExosuitMenuTab = new CraftingTab(WorkbenchScheme, "ExosuitMenu");
            }
        }
    }
}
