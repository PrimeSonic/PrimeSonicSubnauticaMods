namespace CyclopsAutoZapper
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;


    internal class AntiParasiteIconOverlay : IconOverlay
    {
        private readonly Zapper zapper;

        public AntiParasiteIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
            zapper = MCUServices.Find.AuxCyclopsManager<Zapper>(base.Cyclops);
        }

        public override void UpdateText()
        {
            base.UpperText.FontSize = 16;
            base.MiddleText.FontSize = 16;
            base.LowerText.FontSize = 16;

            if (zapper.HasShieldModule)
            {
                base.UpperText.TextString = "Shield\n[Connected]";
                base.UpperText.TextColor = Color.green;

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
            else
            {
                base.UpperText.TextString = "Shield\n[Not Connected]";
                base.UpperText.TextColor = Color.red;

                base.MiddleText.TextString = string.Empty;
                base.LowerText.TextString = string.Empty;
            }
        }
    }
}
