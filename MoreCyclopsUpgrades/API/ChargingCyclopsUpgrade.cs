namespace MoreCyclopsUpgrades.API
{
    using MoreCyclopsUpgrades.Managers;

    public abstract class ChargingCyclopsUpgrade : CyclopsUpgrade
    {
        protected ChargingCyclopsUpgrade(string classId, string friendlyName, string description) 
            : base(classId, friendlyName, description)
        {
            base.OnFinishedPatching += RegisterWithPowerManager;
        }

        protected abstract ICyclopsCharger CreateCharger(SubRoot cyclops);

        private void RegisterWithPowerManager()
        {
            PowerManager.RegisterChargerCreator(CreateCharger);
        }
    }
}
