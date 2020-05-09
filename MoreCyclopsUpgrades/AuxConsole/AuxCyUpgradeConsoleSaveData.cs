namespace MoreCyclopsUpgrades.AuxConsole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using EasyMarkup;

    internal class AuxCyUpgradeConsoleSaveData : EmPropertyCollection
    {
        private readonly string ID;

        private readonly EmModuleSaveData Module1;
        private readonly EmModuleSaveData Module2;
        private readonly EmModuleSaveData Module3;
        private readonly EmModuleSaveData Module4;
        private readonly EmModuleSaveData Module5;
        private readonly EmModuleSaveData Module6;

        private static ICollection<EmProperty> AucUpConsoleDefs => new List<EmProperty>(6)
        {
            new EmModuleSaveData("M1"),
            new EmModuleSaveData("M2"),
            new EmModuleSaveData("M3"),
            new EmModuleSaveData("M4"),
            new EmModuleSaveData("M5"),
            new EmModuleSaveData("M6"),
        };

        public EmModuleSaveData GetModuleInSlot(string slot)
        {
            switch (slot)
            {
                case "Module1":
                    return Module1;
                case "Module2":
                    return Module2;
                case "Module3":
                    return Module3;
                case "Module4":
                    return Module4;
                case "Module5":
                    return Module5;
                case "Module6":
                    return Module6;
                default:
                    return null;
            }
        }

        public AuxCyUpgradeConsoleSaveData(string preFabID) : base("AuxUpgradeConsoleSaveData", AucUpConsoleDefs)
        {
            ID = preFabID;

            Module1 = (EmModuleSaveData)base.Properties["M1"];
            Module2 = (EmModuleSaveData)base.Properties["M2"];
            Module3 = (EmModuleSaveData)base.Properties["M3"];
            Module4 = (EmModuleSaveData)base.Properties["M4"];
            Module5 = (EmModuleSaveData)base.Properties["M5"];
            Module6 = (EmModuleSaveData)base.Properties["M6"];
        }

        public AuxCyUpgradeConsoleSaveData(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
        }

        private string SaveDirectory => Path.Combine(SaveLoadManager.GetTemporarySavePath(), "AuxUpgradeConsole");
        private string SaveFile => Path.Combine(this.SaveDirectory, ID + ".txt");

        public void Save()
        {
            this.Save(this.SaveDirectory, this.SaveFile);
        }

        public bool Load()
        {
            try
            {
                return this.Load(this.SaveDirectory, this.SaveFile);
            }
            catch (Exception ex)
            {
                QuickLogger.Error("Error when attempting to load AuxCyUpgradeConsoleSaveData", ex);
                return false;
            }
        }

        internal override EmProperty Copy()
        {
            return new AuxCyUpgradeConsoleSaveData(ID, this.CopyDefinitions);
        }
    }
}

