namespace CustomCraft.PublicAPI
{
    public sealed class PathHelper
    {
        public sealed class MobileVehicleBay
        {
            internal static CraftingRoot ConstructorScheme = new CraftingRoot(CraftTree.Type.Constructor);
            public static CraftingPath GetThisNode => ConstructorScheme.GetCraftingPath;

            public sealed class Vehicles
            {
                internal static CraftingTab VehiclesTab = new CraftingTab(ConstructorScheme, "Vehicles");
                public static CraftingPath GetThisNode => VehiclesTab.GetCraftingPath;
            }

            public sealed class NeptuneRocket
            {
                internal static CraftingTab RocketTab = new CraftingTab(ConstructorScheme, "Rocket");
                public static CraftingPath GetThisNode => RocketTab.GetCraftingPath;
            }
        }

        public sealed class CyclopsFabricator
        {
            internal static CraftingRoot CyclopsFabricatorScheme = new CraftingRoot(CraftTree.Type.CyclopsFabricator);
            public static CraftingPath GetThisNode => CyclopsFabricatorScheme.GetCraftingPath;
        }

        public sealed class Fabricator
        {
            internal static CraftingRoot FabricatorScheme = new CraftingRoot(CraftTree.Type.Fabricator);
            public static CraftingPath GetThisNode => FabricatorScheme.GetCraftingPath;

            public sealed class Resources
            {
                internal static CraftingTab ResourcesTab = new CraftingTab(FabricatorScheme, "Resources");
                public static CraftingPath GetThisNode => ResourcesTab.GetCraftingPath;

                public sealed class BasicMaterials
                {
                    internal static CraftingTab BasicMaterialsTab = new CraftingTab(ResourcesTab, "BasicMaterials");
                    public static CraftingPath GetThisNode => BasicMaterialsTab.GetCraftingPath;
                }

                public sealed class AdvancedMaterials
                {
                    internal static CraftingTab AdvancedMaterialsTab = new CraftingTab(ResourcesTab, "AdvancedMaterials");
                    public static CraftingPath GetThisNode => AdvancedMaterialsTab.GetCraftingPath;
                }

                public sealed class Electronics
                {
                    internal static CraftingTab ElectronicsTab = new CraftingTab(ResourcesTab, "Electronics");
                    public static CraftingPath GetThisNode => ElectronicsTab.GetCraftingPath;
                }
            }

            public sealed class Sustenance
            {
                internal static CraftingTab SurvivalTab = new CraftingTab(FabricatorScheme, "Survival");
                public static CraftingPath GetThisNode => SurvivalTab.GetCraftingPath;

                public sealed class Water
                {
                    internal static CraftingTab WaterTab = new CraftingTab(SurvivalTab, "Water");
                    public static CraftingPath GetThisNode => WaterTab.GetCraftingPath;
                }

                public sealed class CookedFood
                {
                    internal static CraftingTab CookedFoodTab = new CraftingTab(SurvivalTab, "CookedFood");
                    public static CraftingPath GetThisNode => CookedFoodTab.GetCraftingPath;
                }

                public sealed class CuredFood
                {
                    internal static CraftingTab CuredFoodTab = new CraftingTab(SurvivalTab, "CuredFood");
                    public static CraftingPath GetThisNode => CuredFoodTab.GetCraftingPath;
                }
            }

            public sealed class Personal
            {
                internal static CraftingTab PersonalTab = new CraftingTab(FabricatorScheme, "Personal");
                public static CraftingPath GetThisNode => PersonalTab.GetCraftingPath;

                public sealed class Equipment
                {
                    internal static CraftingTab EquipmentTab = new CraftingTab(PersonalTab, "Equipment");
                    public static CraftingPath GetThisNode => EquipmentTab.GetCraftingPath;
                }

                public sealed class Tools
                {
                    internal static CraftingTab ToolsTab = new CraftingTab(PersonalTab, "Tools");
                    public static CraftingPath GetThisNode => ToolsTab.GetCraftingPath;
                }
            }

            public sealed class Deployables
            {
                internal static CraftingTab MachinesTab = new CraftingTab(FabricatorScheme, "Machines");
                public static CraftingPath GetThisNode => MachinesTab.GetCraftingPath;
            }
        }

        public sealed class ScannerRoom
        {
            internal static CraftingRoot MapRoomSheme = new CraftingRoot(CraftTree.Type.MapRoom);
            public static CraftingPath GetThisNode => MapRoomSheme.GetCraftingPath;
        }

        public sealed class VehicleUpgradeConsole
        {
            internal static CraftingRoot SeamothUpgradesScheme = new CraftingRoot(CraftTree.Type.SeamothUpgrades);
            public static CraftingPath GetThisNode => SeamothUpgradesScheme.GetCraftingPath;

            public sealed class CommonModules
            {
                internal static CraftingTab CommonModulesTab = new CraftingTab(SeamothUpgradesScheme, "CommonModules");
                public static CraftingPath GetThisNode => CommonModulesTab.GetCraftingPath;
            }

            public sealed class SeamothModules
            {
                internal static CraftingTab SeamothModulesTab = new CraftingTab(SeamothUpgradesScheme, "SeamothModules");
                public static CraftingPath GetThisNode => SeamothModulesTab.GetCraftingPath;
            }

            public sealed class PrawnSuitModules
            {
                internal static CraftingTab ExosuitModulesTab = new CraftingTab(SeamothUpgradesScheme, "ExosuitModules");
                public static CraftingPath GetThisNode => ExosuitModulesTab.GetCraftingPath;
            }

            public sealed class Torpedoes
            {
                internal static CraftingTab TorpedoesTab = new CraftingTab(SeamothUpgradesScheme, "CommonModules");
                public static CraftingPath GetThisNode => TorpedoesTab.GetCraftingPath;
            }
        }

        public sealed class ModificationStation
        {
            internal static CraftingRoot WorkbenchScheme = new CraftingRoot(CraftTree.Type.Workbench);
            public static CraftingPath GetThisNode => WorkbenchScheme.GetCraftingPath;

            public sealed class SurvivalKnifeUpgrades
            {
                internal static CraftingTab KnifeMenuTab = new CraftingTab(WorkbenchScheme, "KnifeMenu");
                public static CraftingPath GetThisNode => KnifeMenuTab.GetCraftingPath;
            }

            public sealed class AirTankUpgrades
            {
                internal static CraftingTab TankMenuTab = new CraftingTab(WorkbenchScheme, "TankMenu");
                public static CraftingPath GetThisNode => TankMenuTab.GetCraftingPath;
            }

            public sealed class FinUpgrades
            {
                internal static CraftingTab FinsMenuTab = new CraftingTab(WorkbenchScheme, "FinsMenu");
                public static CraftingPath GetThisNode => FinsMenuTab.GetCraftingPath;
            }

            public sealed class PropulsionCannonUpgrades
            {
                internal static CraftingTab PropulsionCannonMenuTab = new CraftingTab(WorkbenchScheme, "PropulsionCannonMenu");
                public static CraftingPath GetThisNode => PropulsionCannonMenuTab.GetCraftingPath;
            }

            public sealed class CyclopsUpgrades
            {
                internal static CraftingTab CyclopsMenuTab = new CraftingTab(WorkbenchScheme, "CyclopsMenu");
                public static CraftingPath GetThisNode => CyclopsMenuTab.GetCraftingPath;
            }

            public sealed class SeamothUpgrades
            {
                internal static CraftingTab SeamothMenuTab = new CraftingTab(WorkbenchScheme, "SeamothMenu");
                public static CraftingPath GetThisNode => SeamothMenuTab.GetCraftingPath;
            }

            public sealed class PrawnSuitUpgrades
            {
                internal static CraftingTab ExosuitMenuTab = new CraftingTab(WorkbenchScheme, "ExosuitMenu");
                public static CraftingPath GetThisNode => ExosuitMenuTab.GetCraftingPath;
            }
        }
    }
}
