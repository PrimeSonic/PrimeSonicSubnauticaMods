using System.IO;
using IOPath = System.IO.Path;
using CustomCraft2SML.Interfaces.InternalUse;
using CustomCraft2SML.Serialization.Components;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;

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

    internal class CustomFood : EmTechTyped, ICustomFood, ICustomCraft
    {

        public OriginFile Origin { get; set; }

        public short AmountCrafted
        {
            get => emAmount.Value;
            set => emAmount.Value = value;
        }
        protected readonly EmProperty<short> emAmount;
        protected const string AmountCraftedKey = "AmountCrafted";

        public bool ForceUnlockAtStart
        {
            get => emForceAtStart.Value;
            set => emForceAtStart.Value = value;
        }
        protected readonly EmYesNo emForceAtStart;
        protected const string ForceUnlockKey = "ForceUnlockAtStart";

        public IList<EmIngredient> Ingredients => emIngredient.Values;
        protected readonly EmPropertyCollectionList<EmIngredient> emIngredient;
        protected List<Ingredient> IngredientsItems { get; } = new List<Ingredient>();
        protected const string IngredientsKey = "Ingredients";

        public IList<string> LinkedItemIDs => emLinked.Values;
        protected readonly EmPropertyList<string> emLinked;
        protected List<TechType> LinkedItems { get; } = new List<TechType>();
        protected const string LinkedKey = "LinkedItemIDs";

        public IList<string> Unlocks => emUnlocks.Values;
        protected readonly EmPropertyList<string> emUnlocks;
        protected List<TechType> UnlocksItems { get; } = new List<TechType>();
        protected const string UnlocksKey = "Unlocks";

        public IList<string> UnlockedBy => emUnlockedby.Values;

        public TechType UnlockedBy_
        {
            get
            {
                if (!ForceUnlockAtStart && UnlockedBy.Count > 0)
                {
                    TechType item = GetTechType(UnlockedBy[0]);
                    return item;
                }
                else
                {
                    return TechType.None;
                }
            }
        }
        protected readonly EmPropertyList<string> emUnlockedby;
        protected List<TechType> UnlockedByItems { get; } = new List<TechType>();
        protected const string UnlockedbyKey = "UnlockedBy";

        public string DisplayName
        {
            get => emDisplayname.Value;
            set => emDisplayname.Value = value;
        }
        protected readonly EmProperty<string> emDisplayname;
        protected const string DisplayNameKey = "DisplayName";

        public string Tooltip
        {
            get => emTooltip.Value;
            set => emTooltip.Value = value;
        }
        protected readonly EmProperty<string> emTooltip;
        protected const string TooltipKey = "Tooltip";

        public TechType SpriteItemID
        {
            get => emSpriteID.Value;
            set => emSpriteID.Value = value;
        }
        protected readonly EmProperty<TechType> emSpriteID;
        protected const string SpriteKey = "SpriteItemID";

        public string Path
        {
            get => emPath.Value;
            set => emPath.Value = value;
        }
        protected readonly EmProperty<string> emPath;
        protected const string PathKey = "Path";

        public TechGroup PdaGroup
        {
            get => emPdagroup.Value;
            set => emPdagroup.Value = value;
        }
        protected readonly EmProperty<TechGroup> emPdagroup;
        protected const string PdagroupKey = "PdaGroup";

        public TechCategory PdaCategory
        {
            get => emPdacategory.Value;
            set => emPdacategory.Value = value;
        }
        protected readonly EmProperty<TechCategory> emPdacategory;
        protected const string PdacategoryKey = "PdaCategory";

        public short FoodValue
        {
            get => emFood.Value;
            set => emFood.Value = value;
        }
        protected readonly EmProperty<short> emFood;
        protected const string FoodKey = "FoodValue";

        public short WaterValue
        {
            get => emWater.Value;
            set => emWater.Value = value;
        }
        protected readonly EmProperty<short> emWater;
        protected const string WaterKey = "WaterValue";

        public bool Decomposes
        {
            //get => emDecomposes.Value;
            //set => emDecomposes.Value = value;
            get { return emDecayrate.Value > 0; }
        }
        //protected readonly EmYesNo emDecomposes;
        //protected const string DecomposesKey = "Decomposes";

        public short DecayRate
        {
            get => emDecayrate.Value;
            set => emDecayrate.Value = value;
        }
        protected readonly EmProperty<short> emDecayrate;
        protected const string DecayrateKey = "DecayRate";

        public bool Overfill
        {
            get => emOverfill.Value;
            set => emOverfill.Value = value;
        }
        protected readonly EmYesNo emOverfill;
        protected const string OverfillKey = "Overfill";

        public string ID => this.ItemID;
        //protected readonly EmProperty<string> emId;
        //protected const string IdKey = "ID";


        public const string TypeName = "CustomFood";


        protected static List<EmProperty> CustomFoodProperties => new List<EmProperty>(TechTypedProperties)
        {
            new EmProperty<short>(AmountCraftedKey,1) { Optional = true },
            new EmYesNo(ForceUnlockKey,true) { Optional = true },
            new EmPropertyCollectionList<EmIngredient>(IngredientsKey) { Optional = true },
            new EmPropertyList<string>(LinkedKey) { Optional = true },
            new EmPropertyList<string>(UnlocksKey) { Optional = true },
            new EmPropertyList<string>(UnlockedbyKey) { Optional = true },
            new EmProperty<string>(DisplayNameKey) { Optional = true },
            new EmProperty<string>(TooltipKey) { Optional = true },
            new EmProperty<TechType>(SpriteKey) { Optional = true },
            new EmProperty<string>(PathKey) { Optional = true },
            new EmProperty<TechGroup>(PdagroupKey,TechGroup.Survival) { Optional = true },
            new EmProperty<TechCategory>(PdacategoryKey,TechCategory.CookedFood) { Optional = true },
            new EmProperty<short>(FoodKey,0) { Optional = false },
            new EmProperty<short>(WaterKey,0) { Optional = false },
            //new EmYesNo(DecomposesKey,true) { Optional = true },
            new EmProperty<short>(DecayrateKey,150) { Optional = true },
            new EmYesNo(OverfillKey,true) { Optional = true },
        };


        internal CustomFood(TechType origTechType) : this()
        {
            ITechData origRecipe = CraftData.Get(origTechType);
            this.ItemID = origTechType.ToString();
            this.AmountCrafted = (short)origRecipe.craftAmount;

            for (int i = 0; i < origRecipe.ingredientCount; i++)
            {
                IIngredient origIngredient = origRecipe.GetIngredient(i);
                this.Ingredients.Add(new EmIngredient(origIngredient.techType, (short)origIngredient.amount));
            }

            for (int i = 0; i < origRecipe.linkedItemCount; i++)
                emLinked.Add(origRecipe.GetLinkedItem(i).ToString());
        }

        public CustomFood() : this(TypeName, CustomFoodProperties)
        {
        }

        protected CustomFood(string key) : this(key, CustomFoodProperties)
        {
        }

        protected CustomFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emAmount = (EmProperty<short>) Properties[AmountCraftedKey];
            emForceAtStart = (EmYesNo) Properties[ForceUnlockKey];
            emIngredient = (EmPropertyCollectionList<EmIngredient>) Properties[IngredientsKey];
            emLinked = (EmPropertyList<string>) Properties[LinkedKey];
            emUnlocks = (EmPropertyList<string>) Properties[UnlocksKey];
            emUnlockedby = (EmPropertyList<string>) Properties[UnlockedbyKey];
            emDisplayname = (EmProperty<string>) Properties[DisplayNameKey];
            emTooltip = (EmProperty<string>) Properties[TooltipKey];
            emSpriteID = (EmProperty<TechType>) Properties[SpriteKey];
            emPath = (EmProperty<string>) Properties[PathKey];
            emPdagroup = (EmProperty<TechGroup>) Properties[PdagroupKey];
            emPdacategory = (EmProperty<TechCategory>) Properties[PdacategoryKey];
            emFood = (EmProperty<short>) Properties[FoodKey];
            emWater = (EmProperty<short>) Properties[WaterKey];
            //emDecomposes = (EmYesNo) Properties[DecomposesKey];
            emDecayrate = (EmProperty<short>) Properties[DecayrateKey];
            emOverfill = (EmYesNo) Properties[OverfillKey];

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

        internal override EmProperty Copy() => new CustomFood(this.Key, this.CopyDefinitions);



        public bool SendToSMLHelper()
        {
            try
            {
                ValidateIngredients();
                ValidateLinkedItems();
                ValidateUnlockedBy();
                ValidateUnlocks();
                //HandleCraftTreeAddition();

                TechType baseType = /*this.Decomposes ? TechType.CookedPeeper : TechType.CuredPeeper*/TechType.Seaglide;
                var craftPath = new CraftingPath(this.Path, this.ItemID);

                var food = new CustomFoodCraftable(this, craftPath, baseType);
                food.Patch();
                HandleCustomSprite();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} entry '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
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
                    ingredientsValid = false;
            }

            return ingredientsValid;
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

        protected void HandleCustomSprite()
        {
            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for {this.Key} '{this.ItemID}' from {this.Origin}");
                Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            if (this.SpriteItemID > TechType.None && this.SpriteItemID < TechType.Databox)
            {
                QuickLogger.Debug($"{SpriteKey} '{this.SpriteItemID}' used for {this.Key} '{this.ItemID}' from {this.Origin}");
                Atlas.Sprite sprite = SpriteManager.Get(this.SpriteItemID);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            if (this.LinkedItems.Count > 0)
            {
                QuickLogger.Debug($"First entry in {LinkedKey} used for icon of {this.Key} '{this.ItemID}' from {this.Origin}");
                Atlas.Sprite sprite = SpriteManager.Get(this.LinkedItems[0]);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            QuickLogger.Warning($"No sprite loaded for {this.Key} '{this.ItemID}' from {this.Origin}");
        }

        protected virtual void HandleCraftTreeAddition()
        {
            var craftPath = new CraftingPath(this.Path, this.ItemID);

            AddCraftNode(craftPath, this.TechType);
        }
    }
}