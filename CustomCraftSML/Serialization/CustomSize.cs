namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using UnityEngine.Assertions;

    internal class CustomSize : EmPropertyCollection, ICustomSize
    {
        public const short Max = 6;
        public const short Min = 1;

        private readonly EmProperty<string> emTechType;
        private readonly EmProperty<short> emWidth;
        private readonly EmProperty<short> emHeight;

        public string ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public short Width
        {
            get
            {
                Assert.IsTrue(emWidth.Value <= Max, $"Custom size value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(emWidth.Value >= Min, $"Custom size value for {ItemID} must be greater than {Min}.");
                return emWidth.Value;
            }
            set
            {
                Assert.IsTrue(value <= Max, $"Custom size value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(value >= Min, $"Custom size value for {ItemID} must be greater than {Min}.");
                emWidth.Value = value;
            }
        }

        public short Height
        {
            get
            {
                Assert.IsTrue(emHeight.Value <= Max, $"Custom size value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(emHeight.Value >= Min, $"Custom size value for {ItemID} must be greater than {Min}.");
                return emHeight.Value;
            }
            set
            {
                Assert.IsTrue(value <= Max, $"Custom size value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(value >= Min, $"Custom size value for {ItemID} must be greater than {Min}.");
                emHeight.Value = value;
            }
        }

        protected static List<EmProperty> SizeProperties => new List<EmProperty>(3)
        {
            new EmProperty<string>("ItemID"),
            new EmProperty<short>("Width", 1),
            new EmProperty<short>("Height", 1)
        };

        public CustomSize() : base("CustomSize", SizeProperties)
        {
            emTechType = (EmProperty<string>)Properties["ItemID"];
            emWidth = (EmProperty<short>)Properties["Width"];
            emHeight = (EmProperty<short>)Properties["Height"];
        }

        internal override EmProperty Copy() => new CustomSize();
    }
}
