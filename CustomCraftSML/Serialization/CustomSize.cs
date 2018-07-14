namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using UnityEngine.Assertions;

    public class CustomSize : EmPropertyCollection, ICustomSize
    {
        public const short Max = 6;
        public const short Min = 1;

        private readonly EmPropertyTechType emTechType;
        private readonly EmProperty<short> width;
        private readonly EmProperty<short> height;

        public TechType ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public short Width
        {
            get
            {
                Assert.IsTrue(width.Value <= Max, $"Custom size value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(width.Value >= Min, $"Custom size value for {ItemID} must be greater than {Min}.");
                return width.Value;
            }
            set
            {
                Assert.IsTrue(value <= Max, $"Custom size value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(value >= Min, $"Custom size value for {ItemID} must be greater than {Min}.");
                width.Value = value;
            }
        }

        public short Height
        {
            get
            {
                Assert.IsTrue(height.Value <= Max, $"Custom size value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(height.Value >= Min, $"Custom size value for {ItemID} must be greater than {Min}.");
                return height.Value;
            }
            set
            {
                Assert.IsTrue(value <= Max, $"Custom size value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(value >= Min, $"Custom size value for {ItemID} must be greater than {Min}.");
                height.Value = value;
            }
        }

        public static List<EmProperty> SizeProperties => new List<EmProperty>(3)
        {
            new EmPropertyTechType("ItemID"),
            new EmProperty<short>("Width", 1),
            new EmProperty<short>("Height", 1)
        };

        public CustomSize() : this("CustomSize")
        {

        }

        public CustomSize(string key) : base(key, SizeProperties)
        {
            emTechType = (EmPropertyTechType)Properties["ItemID"];
            width = (EmProperty<short>)Properties["Width"];
            height = (EmProperty<short>)Properties["Height"];
        }

        internal override EmProperty Copy() => new CustomSize(Key);
    }
}
