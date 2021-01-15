namespace MoreCyclopsUpgrades.Managers
{
    using UnityEngine.UI;

    internal class IndicatorIcon
    {
        internal uGUI_Icon Icon;
        internal Text Text;

        internal IndicatorIcon(uGUI_Icon icon, Text text)
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
