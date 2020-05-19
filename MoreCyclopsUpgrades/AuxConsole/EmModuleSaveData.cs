namespace MoreCyclopsUpgrades.AuxConsole
{
    using System.Collections.Generic;
    using EasyMarkup;

    internal class EmModuleSaveData : EmPropertyCollection
    {
        private const string ItemIDKey = "ID";
        private const string RemainingChargeKey = "B";
        private const string KeyName = "MDS";

        private EmProperty<int> _itemID;
        private EmProperty<float> _remainingCharge;

        public int ItemID
        {
            get => _itemID.Value;
            set => _itemID.Value = value;
        }

        public float RemainingCharge
        {
            get => _remainingCharge.Value;
            set => _remainingCharge.Value = value;
        }

        private static ICollection<EmProperty> GetDefinitions => new List<EmProperty>()
        {
            new EmProperty<int>(ItemIDKey, 0),
            new EmProperty<float>(RemainingChargeKey, -1f)
        };

        public EmModuleSaveData(string keyName) : this(keyName, GetDefinitions)
        {
        }

        public EmModuleSaveData() : this(KeyName, GetDefinitions)
        {
        }

        public EmModuleSaveData(string keyName, ICollection<EmProperty> definitions) : base(keyName, definitions)
        {
            _itemID = (EmProperty<int>)Properties[ItemIDKey];
            _remainingCharge = (EmProperty<float>)Properties[RemainingChargeKey];
        }

        internal override EmProperty Copy()
        {
            return new EmModuleSaveData(this.Key, this.CopyDefinitions);
        }
    }
}
