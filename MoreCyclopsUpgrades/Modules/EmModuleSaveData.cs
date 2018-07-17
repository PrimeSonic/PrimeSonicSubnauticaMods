namespace MoreCyclopsUpgrades
{
    using System;
    using System.Collections.Generic;
    using ProtoBuf;
    using UnityEngine;
    using Common.EasyMarkup;
    using SMLHelper.V2.Utility;
    using System.IO;

    public class EmModuleSaveData : EmPropertyCollection
    {
        private EmPropertyTechType _itemID;
        private EmProperty<bool> _hasBattery;
        private EmProperty<float> _batteryCharge;

        public TechType ItemID
        {
            get => _itemID.Value;
            set => _itemID.Value = value;
        }

        public bool HasBattery
        {
            get => _hasBattery.Value;
            set => _hasBattery.Value = value;
        }

        public float BatteryCharge
        {
            get => _batteryCharge.Value;
            set => _batteryCharge.Value = value;
        }

        private static ICollection<EmProperty> GetDefinitions
        {
            get => new List<EmProperty>()
            {
                new EmPropertyTechType("ItemID"),
                new EmProperty<bool>("HasBattery"),
                new EmProperty<float>("BatteryCharge")
            };
        }

        public EmModuleSaveData() : base("ModuleSaveData", GetDefinitions)
        {
            _itemID = (EmPropertyTechType)Properties["ItemID"];
            _hasBattery = (EmProperty<bool>)Properties["HasBattery"];
            _batteryCharge = (EmProperty<float>)Properties["BatteryCharge"];
        }

        internal override EmProperty Copy() => new EmModuleSaveData();
    }
}
