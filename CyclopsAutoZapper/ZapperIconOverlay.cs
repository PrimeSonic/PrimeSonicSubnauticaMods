namespace CyclopsAutoZapper
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class ZapperIconOverlay : IconOverlay
    {
        private readonly Zapper zapper;

        public ZapperIconOverlay(TechType zapperTechType, uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
            zapper = MCUServices.Find.AuxCyclopsManager<Zapper>(base.Cyclops);
        }

        public override void UpdateText()
        {
            base.UpperText.FontSize = 12;
            base.LowerText.FontSize = 12;

            if (zapper.SeamothInBay)
            {
                base.UpperText.TextString = "Seamoth\n[Connected]";
                base.UpperText.TextColor = Color.green;

                if (zapper.HasElectricalDefense)
                {
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
                    base.LowerText.TextString = "Defense System\n[Missing]";
                    base.LowerText.TextColor = Color.red;
                }
            }
            else
            {
                base.UpperText.TextString = "Seamoth\n[Not Connected]";
                base.UpperText.TextColor = Color.red;

                base.MiddleText.TextString = string.Empty;
                base.LowerText.TextString = string.Empty;
            }
        }
    }
}
