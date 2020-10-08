namespace CyclopsAutoZapper.Managers
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class AntiParasiteIconOverlay : IconOverlay
    {
        private readonly ShieldPulser shieldPulser;

        public AntiParasiteIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
            shieldPulser = MCUServices.Find.AuxCyclopsManager<ShieldPulser>(base.Cyclops);
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
                base.MiddleText.FontSize = 12;
                base.LowerText.FontSize = 12;

                if (shieldPulser.HasShieldModule)
                {
                    base.UpperText.TextString = DisplayTexts.Main.ShieldConnected;
                    base.UpperText.TextColor = Color.green;

                    if (shieldPulser.IsOnCooldown)
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
                else
                {
                    base.UpperText.TextString = DisplayTexts.Main.ShieldNotConnected;
                    base.UpperText.TextColor = Color.red;

                    base.MiddleText.TextString = string.Empty;
                    base.LowerText.TextString = string.Empty;
                }
            }
        }
    }
}
