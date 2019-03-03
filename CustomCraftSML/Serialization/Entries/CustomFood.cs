namespace CustomCraft2SML.Serialization.Entries
{
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Lists;
    using CustomCraft2SML.SMLHelperItems;
    using System;
    using System.Collections.Generic;

    internal class CustomFood : AliasRecipe, ICustomFood
    {
        internal static new readonly string[] TutorialText = new[]
        {
            $"{CustomFoodList.ListKey}: Create a custom Eatable.",
            $"    FoodValue: Must be a value between {Min} and {Max}",
            $"    WaterValue: Must be a value between {Min} and {Max}",
            $"    Decomposes: Must be either YES or NO",
            $"    DecayRate: Must be a value between X and X",
        };

        public const short Max = 100;
        public const short Min = 0;

        private const string FoodValueKey = "FoodValue";
        private const string WaterValueKey = "WaterValue";
        private const string DecomposesKey = "Decomposes";
        private const string DecayRateKey = "DecayRate";

        protected readonly EmProperty<short> emFood;
        protected readonly EmProperty<short> emWater;
        protected readonly EmYesNo emDecomp;
        protected readonly EmProperty<short> emDecayR;

        public short FoodValue
        {
            get => emFood.Value;
            set
            {
                if (value > Max || value < Min)
                    value = emFood.DefaultValue;

                emFood.Value = value;
            }
        }

        public short WaterValue
        {
            get => emWater.Value;
            set
            {
                if (value > Max || value < Min)
                    value = emWater.DefaultValue;

                emWater.Value = value;
            }
        }

        public bool Decomposes
        {
            get
            {
                if (emDecomp.HasValue)
                    return emDecomp.Value;

                return emDecomp.DefaultValue;
            }
            set => emDecomp.Value = value;
        }

        public short DecayRate
        {
            get
            {
                if (emDecayR.HasValue)
                    return emDecayR.Value;

                return emDecayR.DefaultValue;
            }
            set => emDecayR.Value = value;
        }

        protected static List<EmProperty> FoodProperties => new List<EmProperty>(AliasRecipeProperties)
        {
            new EmProperty<short>(FoodValueKey, 0),
            new EmProperty<short>(WaterValueKey, 0),
            new EmYesNo(DecomposesKey, false) { Optional = true },
            new EmProperty<short>(DecayRateKey, 1) { Optional = true } // TODO find out what this actually is
        };

        public CustomFood() : this("CustomFood", FoodProperties)
        {
        }

        protected CustomFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emFood = (EmProperty<short>)Properties[FoodValueKey];
            emWater = (EmProperty<short>)Properties[WaterValueKey];
            emDecomp = (EmYesNo)Properties[DecomposesKey];
            emDecayR = (EmProperty<short>)Properties[DecayRateKey];
        }

        internal override EmProperty Copy() => new CustomFood(this.Key, this.CopyDefinitions);

        public override bool PassesPreValidation() => InnerItemsAreValid() && FunctionalItemIsValid() & ValidateFoods();

        private bool ValidateFoods()
        {
            if (this.FoodValue < Min || this.WaterValue < Min || this.FoodValue > Max || this.WaterValue > Max)
            {
                QuickLogger.Error(
                    $"Error in {this.Key} for '{this.ItemID}' from {this.Origin}. Eatable values must be between between {Min} and {Max}.");
                return false;
            }

            return true;
        }

        public override bool SendToSMLHelper()
        {
            try
            {
                TechType baseType = this.Decomposes ? TechType.CookedPeeper : TechType.CuredPeeper;
                var craftPath = new CraftingPath(this.Path, this.ItemID);

                var food = new CustomFoodCraftable(this, craftPath, baseType);
                food.Patch();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }

    }
}