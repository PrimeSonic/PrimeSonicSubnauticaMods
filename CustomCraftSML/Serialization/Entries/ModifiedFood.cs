namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.Serialization.Lists;
    using EasyMarkup;
    using SMLHelper.V2.Handlers;

    internal class ModifiedFood : EmTechTyped, IModifiedFood, ICustomCraft
    {
        public string[] TutorialText => ModifiedFoodTutorial;

        internal static readonly string[] ModifiedFoodTutorial = new[]
        {
            $"{ModifiedFoodList.ListKey}: Modify the food and water values of an existing food item.",
            $"    For use with existing food items or ones added by other mods.",
            $"        {FoodKey}: Defines how much food the user will gain on consumption.",
            $"            Must be between {MinValue} and {MaxValue}.",
            $"        {WaterKey}: Defines how much water the user will gain on consumption.",
            $"            Must be between {MinValue} and {MaxValue}.",
        };

        internal const short MaxValue = 100;
        internal const short MinValue = -99;

        public const string TypeName = "ModifiedFood";
        protected const string FoodKey = "FoodValue";
        protected const string WaterKey = "WaterValue";

        protected readonly EmProperty<short> foodValue;
        protected readonly EmProperty<short> waterValue;

        public string ID => this.ItemID;

        public short FoodValue
        {
            get => foodValue.Value;
            set => foodValue.Value = value;
        }
        public short WaterValue
        {
            get => waterValue.Value;
            set => waterValue.Value = value;
        }

        protected static List<EmProperty> ModifiedFoodProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<short>(FoodKey, 0) { Optional = true },
            new EmProperty<short>(WaterKey, 0) { Optional = true },
        };

        public ModifiedFood() : this(TypeName, ModifiedFoodProperties)
        {
        }

        protected ModifiedFood(string key) : this(key, ModifiedFoodProperties)
        {
        }

        protected ModifiedFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            foodValue = (EmProperty<short>)Properties[FoodKey];
            waterValue = (EmProperty<short>)Properties[WaterKey];
        }

        private bool ValidateModifiedFoodValues()
        {
            if (this.FoodValue < MinValue || this.FoodValue > MaxValue)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} has {FoodKey} values out of range. Must be between {MinValue} and {MaxValue}. Entry will be discarded.");
                return false;
            }

            if (this.WaterValue < MinValue || this.WaterValue > MaxValue)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} has {WaterKey} values out of range. Must be between {MinValue} and {MaxValue}. Entry will be discarded.");
                return false;
            }

            if (this.WaterValue == 0 & this.FoodValue == 0)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin}  must have at least one non-zero value for either {FoodKey} or {WaterKey}. Entry will be discarded.");
                return false;
            }

            return true;
        }

        public override bool PassesPreValidation(OriginFile originFile)
        {
            return base.PassesPreValidation(originFile) & ValidateModifiedFoodValues();
        }

        public bool PassedSecondValidation
        {
            get
            {
                if (!CustomFood.IsMappedFoodType(this.TechType))
                {
                    QuickLogger.Warning($"ModifiedFood entry '{this.ItemID}' from {this.Origin} is modifying a non-vanilla food item.");
                }
                return true;
            }
        }
        public OriginFile Origin { get; set; }

        public bool SendToSMLHelper()
        {
            try
            {
                EatableHandler.ModifyEatable(this.TechType, this.FoodValue, this.WaterValue);
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} entry '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }

        internal override EmProperty Copy()
        {
            return new ModifiedFood(this.Key, this.CopyDefinitions);
        }
    }
}
