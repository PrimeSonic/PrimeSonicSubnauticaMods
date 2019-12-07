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
                base.UpperText.FontSize = 20;
                base.MiddleText.FontSize = 20;
                base.LowerText.FontSize = 20;

                base.UpperText.TextString = "CYCLOPS";
                base.MiddleText.TextString = "POWER";
                base.LowerText.TextString = "LOW";

                base.UpperText.TextColor = Color.red;
                base.MiddleText.TextColor = Color.red;
                base.LowerText.TextColor = Color.red;
            }
            else
            {
                base.UpperText.FontSize = 12;
                base.LowerText.FontSize = 12;

                if (zapper.IsOnCooldown)
                {
                    base.LowerText.TextString = "Defense System\n[Cooldown]";
                    base.LowerText.TextColor = Color.yellow;
                }
                else
                {
                    base.LowerText.TextString = "Defense System\n[Charged]";
                    base.LowerText.TextColor = Color.white;
                }
            }
        }
    }
}
