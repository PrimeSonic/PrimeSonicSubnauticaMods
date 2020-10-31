namespace CustomCraft2SML.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.Serialization.Lists;
    using CustomCraft2SML.SMLHelperItems;
    using EasyMarkup;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using IOPath = System.IO.Path;

    internal class CustomFood : AliasRecipe, ICustomFood, ICustomCraft
    {
        public override string[] TutorialText => CustomFoodTutorialText;

        internal static readonly string[] CustomFoodTutorialText = new[]
        {
            $"{CustomFoodList.ListKey}: A powerful tool to satisfy the insatiable.",
            "    Custom foods allow you to create custom eatables, for example Hamburgers or Sodas.",
            $"    {CustomFoodList.ListKey} have all the same properties of {AliasRecipeList.ListKey}, but when creating your own items, you will want to include these new properties:",
            $"        {FoodKey}: Defines how much food the user will gain on consumption.",
            $"            Must be between {MinValue} and {MaxValue}.",
            $"        {WaterKey}: Defines how much water the user will gain on consumption.",
            $"            Must be between {MinValue} and {MaxValue}.",
            $"        {DecayRateKey}: An optional property that defines the speed at which food will decompose.",
            "            If set to 1 it will decompose as fast as any cooked fish." +
            "            If set to 2 it will decay twice as fast.",
            "            If set to 0.5 it will decay half as fast.",
            "            And if set to 0 it will not decay.",
            "            Without this property, the food item will not decay.",
            $"        {FoodModelKey}: Sets the model the food should use in the fabricator and upon being dropped. This needs to be an already existing food or drink.",
            "            Leaving out this property results in an automatic definition based on the food and water values.",
            $"        {UseDrinkSoundKey}: An optional property that sets whether the drinking sound effect should be used when consuming this item.",
            $"            Set to 'NO' to force the eating sound to be used, regardless of the {WaterKey}.",
            $"            Set to 'YES' to force the drinking sound to be used, regardless of the {FoodKey}.",
            $"            If not set, then the drinking sound will be used if {WaterKey} is 5 times greater than {FoodKey}",
        };

        // We may need this later.
        internal static bool IsMappedFoodType(TechType techType)
        {
            switch ((int)techType)
            {
                case (int)FoodModel.None:
                case (int)FoodModel.BigFilteredWater:
                case (int)FoodModel.DisinfectedWater:
                case (int)FoodModel.FilteredWater:
                case (int)FoodModel.StillsuitWater:
                case (int)FoodModel.BulboTreePiece:
                case (int)FoodModel.PurpleVegetable:
                case (int)FoodModel.CreepvinePiece:
                case (int)FoodModel.JellyPlant:
                case (int)FoodModel.KooshChunk:
                case (int)FoodModel.HangingFruit:
                case (int)FoodModel.Melon:
                case (int)FoodModel.NutrientBlock:
                case (int)FoodModel.CookedPeeper:
                case (int)FoodModel.CookedHoleFish:
                case (int)FoodModel.CookedGarryFish:
                case (int)FoodModel.CookedReginald:
                case (int)FoodModel.CookedBladderfish:
                case (int)FoodModel.CookedHoverfish:
                case (int)FoodModel.CookedSpadefish:
                case (int)FoodModel.CookedBoomerang:
                case (int)FoodModel.CookedEyeye:
                case (int)FoodModel.CookedOculus:
                case (int)FoodModel.CookedHoopfish:
                case (int)FoodModel.CookedSpinefish:
                case (int)FoodModel.CookedLavaEyeye:
                case (int)FoodModel.CookedLavaBoomerang:
                case (int)FoodModel.CuredPeeper:
                case (int)FoodModel.CuredHoleFish:
                case (int)FoodModel.CuredGarryFish:
                case (int)FoodModel.CuredReginald:
                case (int)FoodModel.CuredBladderfish:
                case (int)FoodModel.CuredHoverfish:
                case (int)FoodModel.CuredSpadefish:
                case (int)FoodModel.CuredBoomerang:
                case (int)FoodModel.CuredEyeye:
                case (int)FoodModel.CuredOculus:
                case (int)FoodModel.CuredHoopfish:
                case (int)FoodModel.CuredSpinefish:
                case (int)FoodModel.CuredLavaEyeye:
                case (int)FoodModel.CuredLavaBoomerang:
                    return true;
                default:
                    return false;
            }
        }

        internal const short MaxValue = 100;
        internal const short MinValue = -99;

        public new const string TypeName = "CustomFood";

        protected const string FoodModelKey = "FoodType";
        protected const string FoodKey = "FoodValue";
        protected const string WaterKey = "WaterValue";
        protected const string DecayRateKey = "DecayRateMod";
        protected const string UseDrinkSoundKey = "UseDrinkSound";

        protected readonly EmProperty<FoodModel> foodModel;
        protected readonly EmProperty<short> foodValue;
        protected readonly EmProperty<short> waterValue;
        protected readonly EmProperty<float> decayrate;
        protected readonly EmYesNo allowOverfill;
        protected readonly EmYesNo useDrinkSound;

        public FoodModel FoodType
        {
            get => foodModel.Value;
            set => foodModel.Value = value;
        }

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

        public float DecayRateMod
        {
            get => decayrate.Value;
            set => decayrate.Value = value;
        }

        public bool UseDrinkSound
        {
            get
            {
                if (useDrinkSound.HasValue)
                    return useDrinkSound.Value;
                else
                    return this.FoodValue * 5f <= this.WaterValue;
            }
            set => useDrinkSound.Value = value;
        }

        internal bool Decomposes => this.DecayRateMod > 0f;

        protected static List<EmProperty> CustomFoodProperties => new List<EmProperty>(AliasRecipeProperties)
        {
            new EmProperty<FoodModel>(FoodModelKey, FoodModel.None) { Optional = true },
            new EmProperty<TechType>(SpriteItemIdKey, TechType.None) { Optional = true },
            new EmProperty<short>(FoodKey, 0) { Optional = false },
            new EmProperty<short>(WaterKey, 0) { Optional = false },
            new EmProperty<float>(DecayRateKey, 0) { Optional = true },
            new EmYesNo(UseDrinkSoundKey, false) { Optional = true },
        };

        internal TechType FoodPrefab
        {
            get
            {
                if (this.FoodType == FoodModel.None)
                {
                    if (this.FoodValue >= this.WaterValue)
                    {
                        return TechType.NutrientBlock;
                    }
                    else
                    {
                        return TechType.FilteredWater;
                    }
                }

                if (IsMappedFoodType((TechType)this.FoodType))
                {
                    return (TechType)this.FoodType;
                }
                else
                {
                    return TechType.NutrientBlock;
                }
            }
        }

        internal string DefaultIconFileName
        {
            get
            {
                int index = (this.ItemID.GetHashCode() % 8) + 1;

                if (this.FoodValue >= this.WaterValue)
                    return $"cake{index}.png";
                else
                    return $"juice{index}.png";
            }
        }

        public CustomFood() : this(TypeName, CustomFoodProperties)
        {
        }

        protected CustomFood(string key) : this(key, CustomFoodProperties)
        {
        }

        protected CustomFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            foodValue = (EmProperty<short>)Properties[FoodKey];
            waterValue = (EmProperty<short>)Properties[WaterKey];
            decayrate = (EmProperty<float>)Properties[DecayRateKey];
            useDrinkSound = (EmYesNo)Properties[UseDrinkSoundKey];

            techGroup.Value = TechGroup.Survival;
#if SUBNAUTICA
            techCategory.DefaultValue = TechCategory.CookedFood;
#elif BELOWZERO
            techCategory.DefaultValue = TechCategory.FoodAndDrinks;
#endif
            foodModel = (EmProperty<FoodModel>)Properties[FoodModelKey];

            amountCrafted.DefaultValue = 1;
        }

        internal override EmProperty Copy()
        {
            return new CustomFood(this.Key, this.CopyDefinitions);
        }

        public override bool PassesPreValidation(OriginFile originFile)
        {
            return base.PassesPreValidation(originFile) & ValidateCustomFoodValues();
        }

        private bool ValidateCustomFoodValues()
        {
            if (this.FoodValue < MinValue || this.FoodValue > MaxValue)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} has {FoodKey} values out of range. Must be between {MinValue} and {MaxValue}. Entry will be discarded.");
                return false;
            }

            if (this.WaterValue < MinValue || this.FoodValue > MaxValue)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} has {WaterKey} values out of range. Must be between {MinValue} and {MaxValue}. Entry will be discarded.");
                return false;
            }

            if (this.WaterValue == 0 & this.FoodValue == 0)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' must have at least one non-zero value for either {FoodKey} or {WaterKey}. Entry will be discarded.");
                return false;
            }

            return true;
        }

        protected override void HandleCustomSprite()
        {
            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for the custom sprite of {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, ImageUtils.LoadSpriteFromFile(imagePath));
                return;
            }

            if (this.SpriteItemID > TechType.None && this.SpriteItemID < TechType.Databox)
            {
                QuickLogger.Debug($"{SpriteItemIdKey} '{this.SpriteItemID}' used for the custom sprite of {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(this.SpriteItemID));
                return;
            }

            if (this.FoodType != FoodModel.None)
            {
                QuickLogger.Debug($"{FoodModelKey} '{this.FoodType}' used for the custom sprite of {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get((TechType)this.FoodType));
                return;
            }

            // More defaulting behavior
            imagePath = IOPath.Combine(FileLocations.AssetsFolder, this.DefaultIconFileName);

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Default sprite used for {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get((TechType)this.FoodType));
                return;
            }

            QuickLogger.Warning($"Missing all custom sprites for  {this.Key} '{this.ItemID}' from {this.Origin}. Using last fallback.");
            TechType fallbackIcon = this.FoodValue >= this.WaterValue
                ? TechType.NutrientBlock
                : TechType.FilteredWater;

            SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(fallbackIcon));
        }

        protected override void HandleCustomPrefab()
        {
            if (this.TechType == TechType.None)
                throw new InvalidOperationException("TechTypeHandler.AddTechType must be called before PrefabHandler.RegisterPrefab.");

            PrefabHandler.RegisterPrefab(new CustomFoodPrefab(this));
#if SUBNAUTICA
            if (this.UseDrinkSound)
            {
                CraftDataHandler.SetEatingSound(this.TechType, "event:/player/drink");
            }
#endif
        }
    }
}