namespace CyclopsNuclearReactor
{
    using System.Collections.Generic;
    using EasyMarkup;

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
            new EmProperty<int>(ItemIDKey, (int)TechType.None),
            new EmProperty<float>(RemainingChargeKey, SlotData.EmptySlotCharge)
        };

        public CyNukeRodSaveData(string keyName) : this(keyName, GetDefinitions)
        {
        }

        public CyNukeRodSaveData() : this(KeyName, GetDefinitions)
        {
        }

        public CyNukeRodSaveData(SlotData slotData) : this()
        {
            this.TechTypeID = slotData.TechTypeID;
            this.RemainingCharge = slotData.Charge;
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
