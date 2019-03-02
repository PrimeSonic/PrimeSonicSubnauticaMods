

namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Serialization.Lists;
    using CustomCraft2SML.SMLHelperItems;
    using SMLHelper.V2.Handlers;
    using CustomCraft2SML.Interfaces;

    internal class CustomFood : AliasRecipe, ICustomFood
    {
        internal new static readonly string[] TutorialText = new[]
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
        protected readonly EmProperty<bool> emDecomp;
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

        public bool? Decomposes
        {
            get
            {
                if (emDecomp.HasValue)
                    return emDecomp.Value;

                return null;
            }
            set
            {
                if (emDecomp.HasValue = value.HasValue)
                    emDecomp.Value = value.Value;
            }
        }

        public short? DecayRate
        {
            get
            {
                if (emDecayR.HasValue)
                    return emDecayR.Value;

                return null;
            }
            set
            {
                if (emDecayR.HasValue = value.HasValue)
                    emDecayR.Value = value.Value;
            }
        }

        protected static List<EmProperty> FoodProperties => new List<EmProperty>(AliasRecipeProperties)
        {
            new EmProperty<short>(FoodValueKey, 0),
            new EmProperty<short>(WaterValueKey, 0),
            new EmProperty<bool>(DecomposesKey, true),
            new EmProperty<short>(DecayRateKey, 1)
        };

        public CustomFood() : this("CustomFood", FoodProperties)
        {
        }

        protected CustomFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emFood = (EmProperty<short>) Properties[FoodValueKey];
            emWater = (EmProperty<short>) Properties[WaterValueKey];
            emDecomp = (EmProperty<bool>) Properties[DecomposesKey];
            emDecayR = (EmProperty<short>) Properties[DecayRateKey];
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
                HandleCustomFood();

                //  See if there is an asset in the asset folder that has the same name
                HandleCustomSprite();

                // Alias recipes should default to not producing the custom item unless explicitly configured
                HandleAddedRecipe(0);

                HandleCraftTreeAddition();

                HandleUnlocks();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }

        protected void HandleCustomFood()
        {
            var food = new CustomFoodCraftable(this, TechType.CookedPeeper);
            food.Patch();

            QuickLogger.Debug(
                $"{this.Key} '{this.ItemID}' will be a custom item cloned of '{this.FunctionalID}' - Entry from {this.Origin}");
        }
    }
}