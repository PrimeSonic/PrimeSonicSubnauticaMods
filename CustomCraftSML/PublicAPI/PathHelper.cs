namespace CustomCraft.PublicAPI
{
    public sealed class PathHelper
    {
        public sealed class MobileVehicleBay
        {
            internal static CraftingRoot ConstructorScheme = new CraftingRoot(CraftTree.Type.Constructor);
            public static CraftingPath GetPathToHere => ConstructorScheme.GetCraftingPath;

            public sealed class Vehicles
            {
                internal static CraftingTab VehiclesTab = new CraftingTab(ConstructorScheme, "Vehicles");
                public static CraftingPath GetPathToHere => VehiclesTab.GetCraftingPath;
            }

            public sealed class NeptuneRocket
            {
                internal static CraftingTab RocketTab = new CraftingTab(ConstructorScheme, "Rocket");
                public static CraftingPath GetPathToHere => RocketTab.GetCraftingPath;
            }
        }

        public sealed class CyclopsFabricator
        {
            internal static CraftingRoot CyclopsFabricatorScheme = new CraftingRoot(CraftTree.Type.CyclopsFabricator);
            public static CraftingPath GetPathToHere => CyclopsFabricatorScheme.GetCraftingPath;
        }

        public sealed class Fabricator
        {
            internal static CraftingRoot FabricatorScheme = new CraftingRoot(CraftTree.Type.Fabricator);
            public static CraftingPath GetPathToHere => FabricatorScheme.GetCraftingPath;

            public sealed class Resources
            {
                internal static CraftingTab ResourcesTab = new CraftingTab(FabricatorScheme, "Resources");
                public static CraftingPath GetPathToHere => ResourcesTab.GetCraftingPath;

                public sealed class BasicMaterials
                {
                    internal static CraftingTab BasicMaterialsTab = new CraftingTab(ResourcesTab, "BasicMaterials");
                    public static CraftingPath GetPathToHere => BasicMaterialsTab.GetCraftingPath;
                }

                public sealed class AdvancedMaterials
                {
                    internal static CraftingTab AdvancedMaterialsTab = new CraftingTab(ResourcesTab, "AdvancedMaterials");
                    public static CraftingPath GetPathToHere => AdvancedMaterialsTab.GetCraftingPath;
                }

                public sealed class Electronics
                {
                    internal static CraftingTab ElectronicsTab = new CraftingTab(ResourcesTab, "Electronics");
                    public static CraftingPath GetPathToHere => ElectronicsTab.GetCraftingPath;
                }
            }

            public sealed class Sustenance
            {
                internal static CraftingTab SurvivalTab = new CraftingTab(FabricatorScheme, "Survival");
                public static CraftingPath GetPathToHere => SurvivalTab.GetCraftingPath;

                public sealed class Water
                {
                    internal static CraftingTab WaterTab = new CraftingTab(SurvivalTab, "Water");
                    public static CraftingPath GetPathToHere => WaterTab.GetCraftingPath;
                }

                public sealed class CookedFood
                {
                    internal static CraftingTab CookedFoodTab = new CraftingTab(SurvivalTab, "CookedFood");
                    public static CraftingPath GetPathToHere => CookedFoodTab.GetCraftingPath;
                }

                public sealed class CuredFood
                {
                    internal static CraftingTab CuredFoodTab = new CraftingTab(SurvivalTab, "CuredFood");
                    public static CraftingPath GetPathToHere => CuredFoodTab.GetCraftingPath;
                }
            }

            public sealed class Personal
            {
                internal static CraftingTab PersonalTab = new CraftingTab(FabricatorScheme, "Personal");
                public static CraftingPath GetPathToHere => PersonalTab.GetCraftingPath;

                public sealed class Equipment
                {
                    internal static CraftingTab EquipmentTab = new CraftingTab(PersonalTab, "Equipment");
                    public static CraftingPath GetPathToHere => EquipmentTab.GetCraftingPath;
                }

                public sealed class Tools
                {
                    internal static CraftingTab ToolsTab = new CraftingTab(PersonalTab, "Tools");
                    public static CraftingPath GetPathToHere => ToolsTab.GetCraftingPath;
                }
            }

            public sealed class Deployables
            {
                internal static CraftingTab MachinesTab = new CraftingTab(FabricatorScheme, "Machines");
                public static CraftingPath GetPathToHere => MachinesTab.GetCraftingPath;
            }
        }

        public sealed class ScannerRoom
        {
            internal static CraftingRoot MapRoomSheme = new CraftingRoot(CraftTree.Type.MapRoom);
            public static CraftingPath GetPathToHere => MapRoomSheme.GetCraftingPath;
        }

        public sealed class VehicleUpgradeConsole
        {
            internal static CraftingRoot SeamothUpgradesScheme = new CraftingRoot(CraftTree.Type.SeamothUpgrades);
            public static CraftingPath GetPathToHere => SeamothUpgradesScheme.GetCraftingPath;

            public sealed class CommonModules
            {
                internal static CraftingTab CommonModulesTab = new CraftingTab(SeamothUpgradesScheme, "CommonModules");
                public static CraftingPath GetPathToHere => CommonModulesTab.GetCraftingPath;
            }

            public sealed class SeamothModules
            {
                internal static CraftingTab SeamothModulesTab = new CraftingTab(SeamothUpgradesScheme, "SeamothModules");
                public static CraftingPath GetPathToHere => SeamothModulesTab.GetCraftingPath;
            }

            public sealed class PrawnSuitModules
            {
                internal static CraftingTab ExosuitModulesTab = new CraftingTab(SeamothUpgradesScheme, "ExosuitModules");
                public static CraftingPath GetPathToHere => ExosuitModulesTab.GetCraftingPath;
            }

            public sealed class Torpedoes
            {
                internal static CraftingTab TorpedoesTab = new CraftingTab(SeamothUpgradesScheme, "CommonModules");
                public static CraftingPath GetPathToHere => TorpedoesTab.GetCraftingPath;
            }
        }

        public sealed class ModificationStation
        {
            internal static CraftingRoot WorkbenchScheme = new CraftingRoot(CraftTree.Type.Workbench);
            public static CraftingPath GetPathToHere => WorkbenchScheme.GetCraftingPath;

            public sealed class SurvivalKnifeUpgrades
            {
                internal static CraftingTab KnifeMenuTab = new CraftingTab(WorkbenchScheme, "KnifeMenu");
                public static CraftingPath GetPathToHere => KnifeMenuTab.GetCraftingPath;
            }

            public sealed class AirTankUpgrades
            {
                internal static CraftingTab TankMenuTab = new CraftingTab(WorkbenchScheme, "TankMenu");
                public static CraftingPath GetPathToHere => TankMenuTab.GetCraftingPath;
            }

            public sealed class FinUpgrades
            {
                internal static CraftingTab FinsMenuTab = new CraftingTab(WorkbenchScheme, "FinsMenu");
                public static CraftingPath GetPathToHere => FinsMenuTab.GetCraftingPath;
            }

            public sealed class PropulsionCannonUpgrades
            {
                internal static CraftingTab PropulsionCannonMenuTab = new CraftingTab(WorkbenchScheme, "PropulsionCannonMenu");
                public static CraftingPath GetPathToHere => PropulsionCannonMenuTab.GetCraftingPath;
            }

            public sealed class CyclopsUpgrades
            {
                internal static CraftingTab CyclopsMenuTab = new CraftingTab(WorkbenchScheme, "CyclopsMenu");
                public static CraftingPath GetPathToHere => CyclopsMenuTab.GetCraftingPath;
            }

            public sealed class SeamothUpgrades
            {
                internal static CraftingTab SeamothMenuTab = new CraftingTab(WorkbenchScheme, "SeamothMenu");
                public static CraftingPath GetPathToHere => SeamothMenuTab.GetCraftingPath;
            }

            public sealed class PrawnSuitUpgrades
            {
                internal static CraftingTab ExosuitMenuTab = new CraftingTab(WorkbenchScheme, "ExosuitMenu");
                public static CraftingPath GetPathToHere => ExosuitMenuTab.GetCraftingPath;
            }
        }
    }
}
