namespace CyclopsAutoZapper.Managers
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class AutoDefenseMk2IconOverlay : IconOverlay
    {
        private readonly AutoDefenserMk2 zapper;

        public AutoDefenseMk2IconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
            zapper = MCUServices.Find.AuxCyclopsManager<AutoDefenserMk2>(base.Cyclops);
        }

        public override void UpdateText()
        {
            if (GameModeUtils.RequiresPower() && base.Cyclops.powerRelay.GetPower() < Zapper.EnergyRequiredToZap)
            {
                base.MiddleText.FontSize = 20;
                base.MiddleText.TextString = DisplayTexts.Main.CyclopsPowerLow;
                base.MiddleText.TextColor = Color.red;

                base.UpperText.TextString = string.Empty;
                base.LowerText.TextString = string.Empty;
            }
            else
            {
                base.UpperText.FontSize = 12;
                base.LowerText.FontSize = 12;

                if (zapper.IsOnCooldown)
                {
                    base.LowerText.TextString = DisplayTexts.Main.DefenseCooldown;
                    base.LowerText.TextColor = Color.yellow;
                }
                else
                {
                    base.LowerText.TextString = DisplayTexts.Main.DefenseCharged;
                    base.LowerText.TextColor = Color.white;
                }
            }
        }
    }
}
