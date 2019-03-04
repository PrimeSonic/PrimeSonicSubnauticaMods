using System.Collections.Generic;
using Common.EasyMarkup;
using CustomCraft2SML.Serialization.Components;

namespace CustomCraft2SML.Interfaces
{
    interface ICustomFood : ITechTyped
    {
        short AmountCrafted { get; }
        bool ForceUnlockAtStart { get; }

        IList<EmIngredient> Ingredients { get; }
        IList<string> LinkedItemIDs { get; }
        IList<string> Unlocks { get; }
        IList<string> UnlockedBy { get; }

        string DisplayName { get; }
        string Tooltip { get; }
        TechType SpriteItemID { get; }

        string Path { get; }
        TechGroup PdaGroup { get; }
        TechCategory PdaCategory { get; }

        //FOOD SPECIFIC VALUES

        short FoodValue { get; }
        short WaterValue { get; }
        bool Decomposes{ get; }
        short DecayRate { get; }
    }
}
