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
        private EmProperty<int> _itemID;
        private EmProperty<float> _batteryCharge;

        public int ItemID
        {
            get => _itemID.Value;
            set => _itemID.Value = value;
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
                new EmProperty<int>("ItemID", 0),
                new EmProperty<float>("BatteryCharge", -1f)
            };
        }

        public EmModuleSaveData() : this("ModuleSaveData")
        {
        }

        public EmModuleSaveData(string keyName) : base(keyName, GetDefinitions)
        {
            _itemID = (EmProperty<int>)Properties["ItemID"];
            _batteryCharge = (EmProperty<float>)Properties["BatteryCharge"];
        }

        internal override EmProperty Copy() => new EmModuleSaveData();
    }
}
