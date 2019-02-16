namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Serialization.Components;
    using SMLHelper.V2.Handlers;

    internal class CustomSize : EmTechTyped, ICustomSize, ICustomCraft
    {
        internal static readonly string[] TutorialText = new[]
        {
            "CustomSize: Customize the space occupied by an inventory item.",
            "    Width: Must be a value between 1 and 6",
            "    Height: Must be a value between 1 and 6",
        };
        public const short Max = 6;
        public const short Min = 1;

        private readonly EmProperty<short> emWidth;
        private readonly EmProperty<short> emHeight;

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
            new EmProperty<short>("Width", 1),
            new EmProperty<short>("Height", 1)
        };

        public CustomSize() : this("CustomSize", SizeProperties)
        {
        }

        protected CustomSize(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emWidth = (EmProperty<short>)Properties["Width"];
            emHeight = (EmProperty<short>)Properties["Height"];
        }

        internal override EmProperty Copy() => new CustomSize(this.Key, this.CopyDefinitions);

        public override bool PassesPreValidation() => base.PassesPreValidation() && ValidateSizes();

        private bool ValidateSizes()
        {
            if (this.Width <= 0 || this.Height <= 0 || this.Width > 6 || this.Height > 6)
            {
                QuickLogger.Error($"Error in custom size for '{this.ItemID}'. Size values must be between 1 and 6.");
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
                QuickLogger.Error($"Exception thrown while handling Custom Item Size '{this.ItemID}'{Environment.NewLine}{ex}");
                return false;
            }
        }
    }
}
