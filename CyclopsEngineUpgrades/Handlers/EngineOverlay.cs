namespace CyclopsEngineUpgrades.Handlers
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class EngineOverlay : IconOverlay
    {
        private readonly EngineHandler engineHandler;
        private readonly TechType techTypeInSlot;
        private readonly string tierString;
        private readonly string tierRating;

        public EngineOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
            engineHandler = MCUServices.Find.CyclopsGroupUpgradeHandler<EngineHandler>(base.Cyclops, TechType.PowerUpgradeModule);

            techTypeInSlot = upgradeModule.item.GetTechType();
            tierString = $"MK{engineHandler.TierValue(techTypeInSlot)}";
            tierRating = $"[Bonus Efficiency]\n{Mathf.RoundToInt(engineHandler.EngineRating(techTypeInSlot) * 100f)}%";
        }

        public override void UpdateText()
        {
            base.UpperText.FontSize = 12;
            base.UpperText.TextString = tierRating;

            base.MiddleText.FontSize = 14;
            base.MiddleText.TextString = tierString;

            float currPowerRating = base.Cyclops.currPowerRating;

            base.LowerText.FontSize = 13;
            base.LowerText.TextString = "[Total Efficiency]\n" +
                                       $"{Mathf.RoundToInt(currPowerRating * 100f)}%";

            if (currPowerRating >= 1f)
            {
                base.LowerText.TextColor = Color.green;
            }
            else
            {
                base.LowerText.TextColor = Color.yellow;
            }
        }
    }
}
