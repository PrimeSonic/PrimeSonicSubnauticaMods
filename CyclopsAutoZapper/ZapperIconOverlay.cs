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
            base.MiddleText.FontSize = 12;
            base.LowerText.FontSize = 12;

            if (zapper.SeamothInBay)
            {
                base.UpperText.TextString = "Seamoth Ready";
                base.UpperText.TextColor = Color.white;

                if (zapper.HasElectricalDefense)
                {
                    base.MiddleText.TextString = "Defense System Ready";
                    base.MiddleText.TextColor = Color.white;

                    if (zapper.IsOnCooldown)
                    {
                        base.LowerText.TextString = "On cooldown";
                        base.LowerText.TextColor = Color.yellow;
                    }
                    else
                    {
                        base.LowerText.TextString = "Charged and ready";
                        base.LowerText.TextColor = Color.white;
                    }
                }
                else
                {
                    base.MiddleText.TextString = "Defense System Missing";
                    base.MiddleText.TextColor = Color.red;
                }
            }
            else
            {
                base.UpperText.TextString = "No Seamoth in bay";
                base.UpperText.TextColor = Color.red;

                base.MiddleText.TextString = string.Empty;
                base.LowerText.TextString = string.Empty;
            }
        }
    }
}
