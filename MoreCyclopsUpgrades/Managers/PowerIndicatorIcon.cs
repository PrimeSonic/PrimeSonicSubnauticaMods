namespace MoreCyclopsUpgrades.Managers
{
    using UnityEngine.UI;

    internal class PowerIndicatorIcon
    {
        internal uGUI_Icon Icon;
        internal Text Text;

        internal PowerIndicatorIcon(uGUI_Icon icon, Text text)
        {
            Icon = icon;
            Text = text;
            SetEnabled(false);
        }

        internal void SetEnabled(bool value)
        {
            Icon.enabled = value;
            Text.enabled = value;
        }
    }
}
