namespace VModFabricator
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using Common.EasyMarkup;
    using SMLHelper.V2.Crafting;

    internal class ModdedItemsConfig : EmPropertyCollection
    {
        private const string ConfigVersion = "2.1.x";
        private const string ConfigFile = "./QMods/VModFabricator/ModdedItemsConfig.txt";
        private const string ConfigTopLine = "Vehicle Module Fabricator Config for Version " + ConfigVersion;

        internal bool IsInitialized { get; private set; } = false;

        internal readonly EmPropertyList<string> CyclopsAbilityModules;
        internal readonly EmPropertyList<string> CyclopsPowerModules;
        internal readonly EmPropertyList<string> CyclopsRechargeTab;
        internal readonly EmPropertyList<string> ExosuitModules;
        internal readonly EmPropertyList<string> SeamothModules;
        internal readonly EmPropertyList<string> SeamothDepthModules;
        internal readonly EmPropertyList<string> SeamothAbilityModules;
        internal readonly EmPropertyList<string> CommonModules;
        internal readonly EmPropertyList<string> TorpedoesModules;

        private static List<EmProperty> ModdedItemsProperties => new List<EmProperty>(8)
        {
            new EmPropertyList<string>("CyclopsAbilityModules"),
            new EmPropertyList<string>("CyclopsPowerModules"),
            new EmPropertyList<string>("CyclopsRechargeTab"),
            new EmPropertyList<string>("ExosuitModules"),
            new EmPropertyList<string>("SeamothModules"),
            new EmPropertyList<string>("SeamothDepthModules"),
            new EmPropertyList<string>("SeamothAbilityModules"),
            new EmPropertyList<string>("CommonModules"),
            new EmPropertyList<string>("TorpedoesModules"),
        };

        public ModdedItemsConfig() : base("ModdedItems", ModdedItemsProperties)
        {
            CyclopsAbilityModules = (EmPropertyList<string>)Properties["CyclopsAbilityModules"];
            CyclopsPowerModules = (EmPropertyList<string>)Properties["CyclopsPowerModules"];
            CyclopsRechargeTab = (EmPropertyList<string>)Properties["CyclopsRechargeTab"];
            ExosuitModules = (EmPropertyList<string>)Properties["ExosuitModules"];
            SeamothModules = (EmPropertyList<string>)Properties["SeamothModules"];
            SeamothDepthModules = (EmPropertyList<string>)Properties["SeamothDepthModules"];
            SeamothAbilityModules = (EmPropertyList<string>)Properties["SeamothAbilityModules"];
            CommonModules = (EmPropertyList<string>)Properties["CommonModules"];
            TorpedoesModules = (EmPropertyList<string>)Properties["TorpedoesModules"];
        }

        internal override EmProperty Copy() => new ModdedItemsConfig();

        internal void AddModdedModules(ModCraftTreeTab craftTreeTab)
        {
            if (!this.Properties.TryGetValue(craftTreeTab.Name, out EmProperty property))
                return;

            var modulesList = (EmPropertyList<string>)property;

            if (modulesList == null || !modulesList.HasValue)
                return;

            foreach (string module in modulesList.Values)
                craftTreeTab.AddModdedCraftingNode(module);
        }

        internal void Initialize()
        {
            try
            {
                LoadFromFile();
            }
            catch
            {
                QuickLogger.Message("Error loading Config file for VModFabricator. Overwriting with default file.");
                SetDefaults();
                WriteConfigFile();
            }
            finally
            {
                IsInitialized = true;
            }
        }

        internal void SetDefaults()
        {
            foreach (EmProperty tab in Properties.Values)
                ((EmPropertyList<string>)tab).Clear();

            CyclopsPowerModules.Add("PowerUpgradeModuleMk2");
            CyclopsPowerModules.Add("PowerUpgradeModuleMk3");

            CyclopsRechargeTab.Add("CyclopsThermalChargerMk2");
            CyclopsRechargeTab.Add("CyclopsSolarCharger");
            CyclopsRechargeTab.Add("CyclopsSolarChargerMk2");
            CyclopsRechargeTab.Add("CyclopsNuclearModule");

            SeamothDepthModules.Add("SeamothHullModule4");
            SeamothDepthModules.Add("SeamothHullModule5");

            SeamothAbilityModules.Add("SeamothDrillModule");
            SeamothAbilityModules.Add("SeamothClawModule");

            SeamothModules.Add("SeamothThermalModule");

            CommonModules.Add("SpeedModule");
            CommonModules.Add("ScannerModule");
            CommonModules.Add("RepairModule");
            CommonModules.Add("LaserCannon");
        }

        private void LoadFromFile()
        {
            if (!File.Exists(ConfigFile))
            {
                QuickLogger.Message("Config file for VModFabricator not found. Writing default file.");
                SetDefaults();
                WriteConfigFile();
                return;
            }

            string text = File.ReadAllText(ConfigFile, Encoding.UTF8);

            if (!text.TrimStart('#').Trim().StartsWith(ConfigTopLine))
            {
                QuickLogger.Message("Config file for VModFabricator was outdated. Overwriting with default file.");
                SetDefaults();
                WriteConfigFile();
                return;
            }

            bool readCorrectly = FromString(text);

            if (!readCorrectly)
            {
                QuickLogger.Message("Config file for VModFabricator contained errors. Overwriting with default file.");
                SetDefaults();
                WriteConfigFile();
                return;
            }
        }

        private void WriteConfigFile()
        {
            const string horzontalLine =
                "---------------------------------------------------------------------------------------------------";

            string[] descriptionLines = EmUtils.CommentTextLinesCentered(new[]
            {
                ConfigTopLine,
                horzontalLine,
                "Changes to this config file will only take effect next time you boot the game",
                "This config file was built using EasyMarkup",
                horzontalLine,
                "Each entry here corresponds to one of the crafting tabs of the VMod Fabricator",
                "Add or remove modded items from these lists to update or customize the items in the VModFabricator",
                "You must use the modded item's internal 'TechType' name for this",
                horzontalLine,
            });

            string[] writingLines = new string[descriptionLines.Length + 1];

            descriptionLines.CopyTo(writingLines, 0);
            writingLines[descriptionLines.Length] = this.PrettyPrint();

            File.WriteAllLines(ConfigFile, writingLines, Encoding.UTF8);
        }
    }
}
