namespace CustomCraft2SML.Serialization.Entries
{
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.PublicAPI;
    using CustomCraft2SML.Serialization.Components;
    using CustomCraft2SML.SMLHelperItems;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using IOPath = System.IO.Path;

    public enum FoodModel
    {
        None = TechType.None,
        BigFilteredWater = TechType.BigFilteredWater,
        DisinfectedWater = TechType.DisinfectedWater,
        FilteredWater = TechType.FilteredWater,
        StillsuitWater = TechType.StillsuitWater,
        BulboTreePiece = TechType.BulboTreePiece,
        PurpleVegetable = TechType.PurpleVegetable,
        CreepvinePiece = TechType.CreepvinePiece,
        JellyPlant = TechType.JellyPlant,
        KooshChunk = TechType.KooshChunk,
        HangingFruit = TechType.HangingFruit,
        Melon = TechType.Melon,
        NutrientBlock = TechType.NutrientBlock,
        CookedPeeper = TechType.CookedPeeper,
        CookedHoleFish = TechType.CookedHoleFish,
        CookedGarryFish = TechType.CookedGarryFish,
        CookedReginald = TechType.CookedReginald,
        CookedBladderfish = TechType.CookedBladderfish,
        CookedHoverfish = TechType.CookedHoverfish,
        CookedSpadefish = TechType.CookedSpadefish,
        CookedBoomerang = TechType.CookedBoomerang,
        CookedEyeye = TechType.CookedEyeye,
        CookedOculus = TechType.CookedOculus,
        CookedHoopfish = TechType.CookedHoopfish,
        CookedSpinefish = TechType.CookedSpinefish,
        CookedLavaEyeye = TechType.CookedLavaEyeye,
        CookedLavaBoomerang = TechType.CookedLavaBoomerang,
        CuredPeeper = TechType.CuredPeeper,
        CuredHoleFish = TechType.CuredHoleFish,
        CuredGarryFish = TechType.CuredGarryFish,
        CuredReginald = TechType.CuredReginald,
        CuredBladderfish = TechType.CuredBladderfish,
        CuredHoverfish = TechType.CuredHoverfish,
        CuredSpadefish = TechType.CuredSpadefish,
        CuredBoomerang = TechType.CuredBoomerang,
        CuredEyeye = TechType.CuredEyeye,
        CuredOculus = TechType.CuredOculus,
        CuredHoopfish = TechType.CuredHoopfish,
        CuredSpinefish = TechType.CuredSpinefish,
        CuredLavaEyeye = TechType.CuredLavaEyeye,
        CuredLavaBoomerang = TechType.CuredLavaBoomerang,
    }

    internal class CustomFood : EmTechTyped, ICustomFood, ICustomCraft
    {
        // TODO - Make this cleanly inherit from Alias recipe again. Reduce the code duplication.

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

        public const string TypeName = "CustomFood";
        protected const string AmountCraftedKey = "AmountCrafted";
        protected const string ForceUnlockKey = "ForceUnlockAtStart";
        protected const string IngredientsKey = "Ingredients";
        protected const string LinkedKey = "LinkedItemIDs";
        protected const string UnlocksKey = "Unlocks";
        protected const string UnlockedbyKey = "UnlockedBy";
        protected const string DisplayNameKey = "DisplayName";
        protected const string TooltipKey = "Tooltip";
        protected const string PathKey = "Path";
        protected const string PdacategoryKey = "PdaCategory";
        protected const string FoodModelKey = "FoodType";
        protected const string FoodKey = "FoodValue";
        protected const string SpriteItemIdKey = "SpriteItemID";
        protected const string WaterKey = "WaterValue";
        protected const string DecayrateKey = "DecayRate";
        protected const string OverfillKey = "Overfill";

        protected readonly EmProperty<short> emAmount;
        protected readonly EmYesNo emForceAtStart;
        protected readonly EmPropertyCollectionList<EmIngredient> emIngredient;
        protected readonly EmPropertyList<string> emLinked;
        protected readonly EmPropertyList<string> emUnlocks;
        protected readonly EmPropertyList<string> emUnlockedby;
        protected readonly EmProperty<string> emDisplayname;
        protected readonly EmProperty<string> emTooltip;
        protected readonly EmProperty<string> emPath;
        protected readonly EmProperty<TechCategory> emPdacategory;
        protected readonly EmProperty<FoodModel> emFoodModel;
        protected readonly EmProperty<TechType> emSpriteItemId;
        protected readonly EmProperty<short> emFood;
        protected readonly EmProperty<short> emWater;
        protected readonly EmProperty<float> emDecayrate;
        protected readonly EmYesNo emOverfill;

        public short AmountCrafted
        {
            get => emAmount.Value;
            set => emAmount.Value = value;
        }

        public bool ForceUnlockAtStart
        {
            get => emForceAtStart.Value;
            set => emForceAtStart.Value = value;
        }

        public IList<EmIngredient> Ingredients => emIngredient.Values;
        protected List<Ingredient> IngredientsItems { get; } = new List<Ingredient>();

        public IList<string> LinkedItemIDs => emLinked.Values;
        protected List<TechType> LinkedItems { get; } = new List<TechType>();

        public IList<string> Unlocks => emUnlocks.Values;
        protected List<TechType> UnlocksItems { get; } = new List<TechType>();

        public IList<string> UnlockedBy => emUnlockedby.Values;
        public TechType UnlockedByItem
        {
            get
            {
                if (!this.ForceUnlockAtStart && this.UnlockedBy.Count > 0)
                {
                    TechType item = GetTechType(this.UnlockedBy[0]);
                    return item;
                }
                else
                {
                    return TechType.None;
                }
            }
        }

        protected List<TechType> UnlockedByItems { get; } = new List<TechType>();

        public string DisplayName
        {
            get => emDisplayname.Value;
            set => emDisplayname.Value = value;
        }

        public string Tooltip
        {
            get => emTooltip.Value;
            set => emTooltip.Value = value;
        }

        public string Path
        {
            get => emPath.Value;
            set => emPath.Value = value;
        }

        public TechCategory PdaCategory
        {
            get => emPdacategory.Value;
            set => emPdacategory.Value = value;
        }

        public FoodModel FoodType
        {
            get => emFoodModel.Value;
            set => emFoodModel.Value = value;
        }

        public TechType SpriteItemID
        {
            get => emSpriteItemId.Value;
            set => emSpriteItemId.Value = value;
        }

        public short FoodValue
        {
            get => emFood.Value;
            set => emFood.Value = value;
        }

        public short WaterValue
        {
            get => emWater.Value;
            set => emWater.Value = value;
        }

        public float DecayRate
        {
            get => emDecayrate.Value;
            set => emDecayrate.Value = value;
        }

        public bool Overfill
        {
            get => emOverfill.Value;
            set => emOverfill.Value = value;
        }

        internal bool Decomposes => this.DecayRate > 0f;

        public string ID => this.ItemID;

        protected static List<EmProperty> CustomFoodProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<short>(AmountCraftedKey, 1) { Optional = true },
            new EmYesNo(ForceUnlockKey, true) { Optional = true },
            new EmPropertyCollectionList<EmIngredient>(IngredientsKey) { Optional = true },
            new EmPropertyList<string>(LinkedKey) { Optional = true },
            new EmPropertyList<string>(UnlocksKey) { Optional = true },
            new EmPropertyList<string>(UnlockedbyKey) { Optional = true },
            new EmProperty<string>(DisplayNameKey),
            new EmProperty<string>(TooltipKey),
            new EmProperty<string>(PathKey) { Optional = false },
            new EmProperty<TechCategory>(PdacategoryKey, TechCategory.CookedFood) { Optional = true },
            new EmProperty<FoodModel>(FoodModelKey, FoodModel.None) { Optional = true },
            new EmProperty<TechType>(SpriteItemIdKey, TechType.None) { Optional = true },
            new EmProperty<short>(FoodKey, 0) { Optional = false },
            new EmProperty<short>(WaterKey, 0) { Optional = false },
            new EmProperty<short>(DecayrateKey, 0) { Optional = true },
            new EmYesNo(OverfillKey, true) { Optional = true },
        };

        internal TechType FoodPrefab
        {
            get
            {
                if (this.FoodType == FoodModel.None)
                {
                    if (this.FoodValue >= this.WaterValue)
                        return TechType.NutrientBlock;
                    else
                        return TechType.FilteredWater;
                }

                return (TechType)this.FoodType;
            }
        }

        internal string IconName
        {
            get
            {
                string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");

                if (File.Exists(imagePath))
                    return imagePath;

                if (this.FoodValue >= this.WaterValue)
                    return "cake.png";
                else
                    return "juice.png";
            }
        }

        public OriginFile Origin { get; set; }

        public CustomFood() : this(TypeName, CustomFoodProperties)
        {
        }

        protected CustomFood(string key) : this(key, CustomFoodProperties)
        {
        }

        protected CustomFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emAmount = (EmProperty<short>)Properties[AmountCraftedKey];
            emForceAtStart = (EmYesNo)Properties[ForceUnlockKey];
            emIngredient = (EmPropertyCollectionList<EmIngredient>)Properties[IngredientsKey];
            emLinked = (EmPropertyList<string>)Properties[LinkedKey];
            emUnlocks = (EmPropertyList<string>)Properties[UnlocksKey];
            emUnlockedby = (EmPropertyList<string>)Properties[UnlockedbyKey];
            emDisplayname = (EmProperty<string>)Properties[DisplayNameKey];
            emTooltip = (EmProperty<string>)Properties[TooltipKey];
            emPath = (EmProperty<string>)Properties[PathKey];
            emPdacategory = (EmProperty<TechCategory>)Properties[PdacategoryKey];
            emSpriteItemId = (EmProperty<TechType>)Properties[SpriteItemIdKey];
            emFood = (EmProperty<short>)Properties[FoodKey];
            emWater = (EmProperty<short>)Properties[WaterKey];
            emDecayrate = (EmProperty<float>)Properties[DecayrateKey];
            emOverfill = (EmYesNo)Properties[OverfillKey];

            OnValueExtractedEvent += ValueExtracted;
        }

        private void ValueExtracted()
        {
            foreach (EmIngredient ingredient in emIngredient)
            {
                string itemID = (ingredient["ItemID"] as EmProperty<string>).Value;
                short required = (ingredient["Required"] as EmProperty<short>).Value;
            }
        }

        internal override EmProperty Copy()
        {
            return new CustomFood(this.Key, this.CopyDefinitions);
        }

        public override bool PassesPreValidation()
        {
            return (GetTechType(this.ItemID) == TechType.None) & // Confirm that no other item is currently using this ID.
                                                                 // TODO = Log when the above check fails
                    ValidateIngredients() &
                    ValidateLinkedItems() &
                    ValidateUnlocks() &
                    ValidateUnlockedBy();
        }

        private bool ValidateUnlockedBy()
        {
            bool unlockedByValid = true;

            foreach (string unlockedBy in this.UnlockedBy)
            {
                TechType unlockByItemID = GetTechType(unlockedBy);

                if (unlockByItemID == TechType.None)
                {
                    QuickLogger.Warning($"{this.Key} entry with ID of '{this.ItemID}' contained an unknown {UnlockedbyKey} '{unlockedBy}'. Entry will be discarded.");
                    unlockedByValid = false;
                    continue;
                }

                this.UnlockedByItems.Add(unlockByItemID);
            }

            return unlockedByValid;
        }

        private bool ValidateUnlocks()
        {
            bool unlocksValid = true;

            foreach (string unlockingItem in this.Unlocks)
            {
                TechType unlockingItemID = GetTechType(unlockingItem);

                if (unlockingItemID == TechType.None)
                {
                    QuickLogger.Warning($"{this.Key} entry with ID of '{this.ItemID}' contained an unknown {UnlocksKey} '{unlockingItem}'. Entry will be discarded.");
                    unlocksValid = false;
                    continue;
                }

                this.UnlocksItems.Add(unlockingItemID);
            }

            return unlocksValid;
        }

        private bool ValidateLinkedItems()
        {
            bool linkedItemsValid = true;

            foreach (string linkedItem in this.LinkedItemIDs)
            {
                TechType linkedItemID = GetTechType(linkedItem);

                if (linkedItemID == TechType.None)
                {
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} contained an unknown {LinkedKey} '{linkedItem}'. Entry will be discarded.");
                    linkedItemsValid = false;
                    continue;
                }

                this.LinkedItems.Add(linkedItemID);
            }

            return linkedItemsValid;
        }

        private bool ValidateIngredients()
        {
            bool ingredientsValid = true;

            foreach (EmIngredient ingredient in this.Ingredients)
            {
                if (ingredient.PassesPreValidation())
                    this.IngredientsItems.Add(ingredient.ToSMLHelperIngredient());
                else
                {
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} contained an unknown {IngredientsKey} '{ingredient.ItemID}'. Entry will be discarded.");
                    ingredientsValid = false;
                }
            }

            return ingredientsValid;
        }

        private bool ValidateCustomFoodValues()
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

            return true;
        }

        public bool SendToSMLHelper()
        {
            try
            {
                this.TechType = TechTypeHandler.AddTechType(this.ItemID, this.DisplayName, this.Tooltip, this.ForceUnlockAtStart);

                HandleCustomSprite();

                HandleAddedRecipe();

                HandleUnlocks();

                HandleCraftTreeAddition();

                RegisterPreFab();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} entry '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }

        protected void HandleCustomSprite()
        {
            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, ImageUtils.LoadSpriteFromFile(imagePath));
                return;
            }

            if (this.SpriteItemID > TechType.None && this.SpriteItemID < TechType.Databox)
            {
                QuickLogger.Debug($"{SpriteItemIdKey} '{this.SpriteItemID}' used for {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(this.SpriteItemID));
                return;
            }

            if (this.FoodValue >= this.WaterValue)
            {
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.NutrientBlock));
                return;
            }
            else
            {
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.FilteredWater));
                return;
            }

            //QuickLogger.Warning($"No sprite loaded for {this.Key} '{this.ItemID}' from {this.Origin}");
        }

        internal TechData CreateRecipeTechData(short defaultCraftAmount = 1)
        {
            var replacement = new TechData
            {
                craftAmount = this.AmountCrafted
            };

            foreach (EmIngredient ingredient in this.Ingredients)
                replacement.Ingredients.Add(new Ingredient(ingredient.TechType, ingredient.Required));

            foreach (TechType linkedItem in this.LinkedItems)
                replacement.LinkedItems.Add(linkedItem);
            return replacement;
        }

        protected virtual void HandleCraftTreeAddition()
        {
            var craftPath = new CraftingPath(this.Path, this.ItemID);

            AddCraftNode(craftPath, this.TechType);
        }

        private void RegisterPreFab()
        {
            PrefabHandler.RegisterPrefab(new CustomFoodPrefab(this));
        }

        protected bool HandleUnlocks()
        {
            if (this.ForceUnlockAtStart)
            {
                KnownTechHandler.UnlockOnStart(this.TechType);
                QuickLogger.Debug($"{this.Key} for '{this.ItemID}' from {this.Origin} will be a unlocked at the start of the game");
            }

            if (this.UnlockingItems.Count > 0)
            {
                KnownTechHandler.SetAnalysisTechEntry(this.TechType, this.UnlockingItems);
            }

            return true;
        }


        protected void HandleAddedRecipe(short defaultCraftAmount = 1)
        {
            TechData replacement = CreateRecipeTechData(defaultCraftAmount);

            CraftDataHandler.SetTechData(this.TechType, replacement);
            QuickLogger.Debug($"Adding new recipe for '{this.ItemID}'");

            CraftDataHandler.AddToGroup(TechGroup.Survival, this.PdaCategory, this.TechType);
        }
    }
}