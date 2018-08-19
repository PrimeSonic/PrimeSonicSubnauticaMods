namespace MoreCyclopsUpgrades.SaveData
{
    using System.Collections.Generic;
    using Common.EasyMarkup;

    public class EmModuleSaveData : EmPropertyCollection
    {
        private const string ItemIDKey = "ID";
        private const string BatteryChargeKey = "B";
        private const string KeyName = "MDS";

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
                new EmProperty<int>(ItemIDKey, 0),
                new EmProperty<float>(BatteryChargeKey, -1f)
            };
        }

        public EmModuleSaveData() : this(KeyName)
        {
        }

        public EmModuleSaveData(string keyName) : base(keyName, GetDefinitions)
        {
            _itemID = (EmProperty<int>)Properties[ItemIDKey];
            _batteryCharge = (EmProperty<float>)Properties[BatteryChargeKey];
        }

        internal override EmProperty Copy() => new EmModuleSaveData();
    }
}
