namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Lists;
    using SMLHelper.V2.Handlers;

    internal class CustomSize : EmTechTyped, ICustomSize, ICustomCraft
    {
        internal static readonly string[] TutorialText = new[]
        {
           $"{CustomSizeList.ListKey}: Customize the space occupied by an inventory item.",
           $"    Width: Must be a value between {Min} and {Max}",
           $"    Height: Must be a value between {Min} and {Max}",
        };

        public const short Max = 6;
        public const short Min = 1;

        private const string WidthKey = "Width";
        private const string HeightKey = "Height";

        protected readonly EmProperty<short> emWidth;
        protected readonly EmProperty<short> emHeight;

        public string ID => this.ItemID;

        public short Width
        {
            get => emWidth.Value;
            set
            {
                if (value > Max || value < Min)
                    value = emWidth.DefaultValue;

                emWidth.Value = value;
            }
        }

        public short Height
        {
            get => emHeight.Value;
            set
            {
                if (value > Max || value < Min)
                    value = emHeight.DefaultValue;

                emHeight.Value = value;
            }
        }

        protected static List<EmProperty> SizeProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<short>(WidthKey, 1),
            new EmProperty<short>(HeightKey, 1)
        };

        public CustomSize() : this("CustomSize", SizeProperties)
        {
        }

        protected CustomSize(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emWidth = (EmProperty<short>)Properties[WidthKey];
            emHeight = (EmProperty<short>)Properties[HeightKey];
        }

        internal override EmProperty Copy() => new CustomSize(this.Key, this.CopyDefinitions);

        public override bool PassesPreValidation() => base.PassesPreValidation() && ValidateSizes();

        private bool ValidateSizes()
        {
            if (this.Width < Min || this.Height < Min || this.Width > Max || this.Height > Max)
            {
                QuickLogger.Error($"Error in {this.Key} for '{this.ItemID}'. Size values must be between between {Min} and {Max}.");
                return false;
            }

            return true;
        }

        public bool SendToSMLHelper()
        {
            try
            {
                CraftDataHandler.SetItemSize(this.TechType, this.Width, this.Height);
                QuickLogger.Message($"'{this.ItemID}' was resized to {this.Width}x{this.Height}");
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }
    }
}
