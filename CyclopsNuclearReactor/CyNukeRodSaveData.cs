namespace CyclopsNuclearReactor
{
    using Common.EasyMarkup;
    using System.Collections.Generic;

    internal class CyNukeRodSaveData : EmPropertyCollection
    {
        private const string ItemIDKey = "ID";
        private const string RemainingChargeKey = "C";
        private const string KeyName = "R";

        private EmProperty<int> _itemID;
        private EmProperty<float> _remainingCharge;

        public TechType TechTypeID
        {
            get => (TechType)this.ItemID;
            set => this.ItemID = (int)value;
        }

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

        public CyNukeRodSaveData(string keyName) : this(keyName, GetDefinitions)
        {
        }

        public CyNukeRodSaveData() : this(KeyName, GetDefinitions)
        {
        }

        public CyNukeRodSaveData(string keyName, ICollection<EmProperty> definitions) : base(keyName, definitions)
        {
            _itemID = (EmProperty<int>)Properties[ItemIDKey];
            _remainingCharge = (EmProperty<float>)Properties[RemainingChargeKey];
        }

        internal override EmProperty Copy()
        {
            return new CyNukeRodSaveData(this.Key, this.CopyDefinitions);
        }
    }
}
