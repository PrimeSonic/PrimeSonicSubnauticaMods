namespace CustomCraft2SML.Serialization.Entries
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;

    internal class CustomSize : EmPropertyCollection, ICustomSize
    {
        internal static readonly string[] TutorialText = new[]
        {
            "CustomSize: Customize the space occupied by an inventory item.",
            "    Width: Must be a value between 1 and 6",
            "    Height: Must be a value between 1 and 6",
        };
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
                return emWidth.Value;
            }
            set
            {
                if (value <= Max || value >= Min)
                    value = emWidth.DefaultValue;

                emWidth.Value = value;
            }
        }

        public short Height
        {
            get
            {
                return emHeight.Value;
            }
            set
            {
                if (value <= Max || value >= Min)
                    value = emHeight.DefaultValue;

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
